// Import required namespaces for Entity Framework, business layer, repository layer, WebAPI extensions, and logging
using System.Security.Claims;
using System.Text;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Services;
using QuantityMeasurementRepositoryLayer.Data;
using QuantityMeasurementRepositoryLayer.Interface;
using QuantityMeasurementRepositoryLayer.Repositories;
using QuantityMeasurementWebAPI.Extensions;
using QuantityMeasurementWebAPI.Middleware;
using Serilog;

// ==================== SERILOG INITIALIZATION ====================
// Configure Serilog first - this happens before the application starts
// Serilog provides structured logging with enrichment
Log.Logger = new LoggerConfiguration()
    // Write logs to console with custom format
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    // Write logs to daily rolling files in logs folder
    .WriteTo.File("logs/quantityapi-.txt", rollingInterval: RollingInterval.Day)
    // Enrich logs with additional context
    .Enrich.FromLogContext() // Add custom properties from logging context
    .Enrich.WithMachineName() // Add machine name to logs
    .Enrich.WithThreadId() // Add thread ID to logs
    .CreateLogger(); // Build the logger instance

try
{
    // Log application start
    Log.Information("Starting Quantity Measurement API");

    // ==================== APPLICATION BUILDER SETUP ====================
    // Create the web application builder with default configuration
    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog as the logging provider (replaces default .NET logging)
    builder.Host.UseSerilog();

    // ==================== LOAD JWT KEY FROM ENVIRONMENT VARIABLE ====================
    // NEVER store JWT keys in appsettings.json - always use environment variables
    var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");

    // For development only - you can use a fallback with a warning
    if (string.IsNullOrEmpty(jwtKey))
    {
        // In production, this should throw an exception
        // For development, we'll use a warning and a fallback
        if (builder.Environment.IsDevelopment())
        {
            Log.Warning(
                "JWT_KEY environment variable not set. Using development fallback key. THIS IS INSECURE FOR PRODUCTION!"
            );
            jwtKey = "DevelopmentOnlyInsecureKeyDoNotUseInProduction12345";
        }
        else
        {
            throw new InvalidOperationException(
                "JWT_KEY environment variable is not set. This is a security requirement for production."
            );
        }
    }

    // Validate JWT key length (should be at least 32 characters for HMAC-SHA256)
    if (jwtKey.Length < 32)
    {
        Log.Warning(
            "JWT key is less than 32 characters. This reduces security. Key length: {Length}",
            jwtKey.Length
        );
    }

    // ==================== SERVICE REGISTRATION (DI Container) ====================

    // Add MVC controller services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Add Data Protection
    var dataProtectionBuilder = builder
        .Services.AddDataProtection()
        .SetApplicationName("QuantityMeasurementApp");

    // DPAPI is supported only on Windows
    if (OperatingSystem.IsWindows())
    {
        dataProtectionBuilder.ProtectKeysWithDpapi();
    }

    // ==================== DATABASE CONTEXT REGISTRATION ====================
    // Configure Database Context with DI (Scoped lifetime)
    builder.Services.AddDbContext<ApplicationDbContext>(
        options =>
            options.UseSqlServer(
                // Get connection string from appsettings.json
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    // Enable retry on failure for transient faults (network issues, timeouts)
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5, // Try up to 5 times
                        maxRetryDelay: TimeSpan.FromSeconds(30), // Wait up to 30 seconds between retries
                        errorNumbersToAdd: null // Use default SQL error numbers
                    );
                    // Set which assembly contains the migrations
                    sqlOptions.MigrationsAssembly("QuantityMeasurementRepositoryLayer");
                    // Set command timeout to 30 seconds
                    sqlOptions.CommandTimeout(30);
                }
            ),
        ServiceLifetime.Scoped // Scoped = created once per HTTP request
    );

    // ==================== REPOSITORY REGISTRATION ====================
    // Register repository with DI container
    // Maps interface IQuantityMeasurementRepository to concrete class QuantityMeasurementRepository
    builder.Services.AddScoped<IQuantityMeasurementRepository, QuantityMeasurementRepository>();

    // ==================== BUSINESS SERVICE REGISTRATION ====================
    // Register business service with DI container
    builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementService>();

    // ==================== AUTHENTICATION & AUTHORIZATION REGISTRATION ====================
    // Register Auth repository and service
    builder.Services.AddScoped<IAuthRepository, AuthRepository>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    // ==================== ADDITIONAL SERVICE REGISTRATIONS ====================
    builder.Services.AddScoped<IAuditLogService, AuditLogService>();
    builder.Services.AddScoped<IEncryptionService, EncryptionService>();

    // Add rate limiting configuration
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(
        builder.Configuration.GetSection("IpRateLimiting")
    );
    builder.Services.AddInMemoryRateLimiting();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

    // ==================== AUTHENTICATION CONFIGURATION ====================
    builder
        .Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                SaveSigninToken = true,
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<
                        ILogger<Program>
                    >();
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        logger.LogWarning("Authentication failed - Token expired");
                    }
                    else if (context.Exception is SecurityTokenInvalidSignatureException)
                    {
                        logger.LogWarning("Authentication failed - Invalid token signature");
                    }
                    else
                    {
                        logger.LogError(context.Exception, "Authentication failed");
                    }
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<
                        ILogger<Program>
                    >();
                    var userId = context.Principal?.FindFirst("sub")?.Value;
                    var username = context.Principal?.Identity?.Name;
                    logger.LogInformation(
                        "Token validated successfully for user: {Username} (ID: {UserId})",
                        username,
                        userId
                    );
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<
                        ILogger<Program>
                    >();
                    if (context.Response.HasStarted)
                        return Task.CompletedTask;
                    logger.LogWarning(
                        "Authorization challenge for path: {Path}, Error: {Error}",
                        context.Request.Path,
                        context.Error
                    );
                    return Task.CompletedTask;
                },
                OnMessageReceived = context => Task.CompletedTask,
            };
        })
        .AddCookie(
            CookieAuthenticationDefaults.AuthenticationScheme,
            options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Change to None for HTTP
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.Name = "AuthCookie";
                options.Cookie.Path = "/";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            }
        );

    // Add authorization policies
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"));
    });

    // ==================== CORS CONFIGURATION ====================
    // Add this BEFORE app.Build() in the services section
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "AllowReactApp",
            policy =>
            {
                policy
                    .WithOrigins("http://localhost:3001", "http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
        );
    });

    // ==================== WEB API SERVICES REGISTRATION ====================
    // Add custom services from extensions (Swagger, CORS, Health Checks, etc.)
    builder.Services.AddWebApiServices(builder.Configuration);

    // ==================== APPLICATION BUILDING ====================
    // Build the app (IoC container is now ready and locked for modifications)
    var app = builder.Build();

    // ==================== DATABASE MIGRATION ON STARTUP ====================
    // Ensure database is created and migrations are applied automatically
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
            dbContext.Database.EnsureCreated();
            logger.LogInformation("Database created using EnsureCreated");
        }
    }

    // ==================== HTTP REQUEST PIPELINE CONFIGURATION ====================
    // ORDER IS CRITICAL - DO NOT REARRANGE

    // 1. Global exception handling middleware (must be first)
    app.UseMiddleware<GlobalExceptionHandler>();

    // Add security headers middleware after exception handling
    app.UseMiddleware<SecurityHeadersMiddleware>();

    // 2. Add rate limiting middleware to prevent brute force attacks
    app.UseMiddleware<RateLimitingMiddleware>();

    // 3. Swagger in Development (before auth)
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    // 4. HTTPS Redirection - enforce HTTPS
    app.UseHttpsRedirection();

    // 5. Add HSTS for production (HTTP Strict Transport Security)
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    // 6. CORS (should be before Authentication)
    app.UseCors("AllowReactApp");

    // 7. Authentication MUST come before Authorization
    app.UseAuthentication();

    // 8. Authorization checks the [Authorize] attributes
    app.UseAuthorization();

    // 9. Map controller endpoints to routes
    app.MapControllers();

    // 10. Add health check endpoint at /health for monitoring
    app.MapHealthChecks("/health");

    // Log successful startup
    Log.Information("Quantity Measurement API started successfully");
    Log.Information(
        "JWT Authentication configured with issuer: {Issuer}",
        builder.Configuration["Jwt:Issuer"]
    );

    // ==================== START APPLICATION ====================
    // Run the application and start listening for requests
    app.Run();
}
catch (Exception ex)
{
    // If application fails to start, log fatal error
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    // Ensure all logs are written before application exits
    Log.CloseAndFlush();
}
