using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Services;
using QuantityMeasurementRepositoryLayer.Data;
using QuantityMeasurementRepositoryLayer.Interface;
using QuantityMeasurementRepositoryLayer.Repositories;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/quantityapi-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Quantity Measurement API");

    var builder = WebApplication.CreateBuilder(args);

    // ==================== JWT KEY ====================
    var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
    if (string.IsNullOrEmpty(jwtKey))
    {
        jwtKey = "DevelopmentOnlyInsecureKeyDoNotUseInProduction12345";
        Log.Warning("Using development JWT key");
    }

    // ==================== DATABASE CONNECTION ====================
    string dbConnectionString = null;

    // Try environment variable first
    var envConnString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    Log.Information(
        $"DB_CONNECTION_STRING from env: {(string.IsNullOrEmpty(envConnString) ? "NOT SET" : "SET (length: " + envConnString.Length + ")")}"
    );

    if (!string.IsNullOrEmpty(envConnString))
    {
        dbConnectionString = envConnString;
        Log.Information("Using connection string from environment variable");
    }
    else
    {
        // Try appsettings.json
        dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Log.Information("Using connection string from appsettings.json");
    }

    if (string.IsNullOrEmpty(dbConnectionString))
    {
        Log.Error("NO CONNECTION STRING FOUND! Check environment variables and appsettings.json");
        throw new InvalidOperationException("Database connection string is not configured");
    }

    // Log full connection string for debugging
    Log.Information($"FULL CONNECTION STRING: '{dbConnectionString}'");

    // Detect PostgreSQL vs SQL Server
    bool isOnRender = Environment.GetEnvironmentVariable("RENDER") == "true";
    bool isPostgres =
        isOnRender
        || dbConnectionString.StartsWith("postgresql://")
        || dbConnectionString.StartsWith("postgres://")
        || dbConnectionString.Contains("Host=")
        || dbConnectionString.Contains("Server=")
        || dbConnectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase);

    Log.Information($"Database Type: {(isPostgres ? "PostgreSQL" : "SQL Server")}");
    Log.Information($"Running on Render: {isOnRender}");

    // ==================== GOOGLE OAUTH ====================
    var googleClientId =
        Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")
        ?? throw new InvalidOperationException("GOOGLE_CLIENT_ID not set");
    var googleClientSecret =
        Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")
        ?? throw new InvalidOperationException("GOOGLE_CLIENT_SECRET not set");

    // ==================== FRONTEND URL ====================
    var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000";

    builder.Host.UseSerilog();

    // ==================== SERVICES ====================
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Database configuration
    if (isPostgres)
    {
        Log.Information("Configuring PostgreSQL database...");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                dbConnectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly("QuantityMeasurementRepositoryLayer");
                    npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    npgsqlOptions.CommandTimeout(30);

                    if (isOnRender)
                    {
                        npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    }
                }
            )
        );

        // Optional: Add logging for SQL queries in development
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(dbConnectionString)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );
        }
    }
    else
    {
        Log.Information("Configuring SQL Server database...");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(dbConnectionString)
        );
    }

    // Repositories & Services
    builder.Services.AddScoped<IQuantityMeasurementRepository, QuantityMeasurementRepository>();
    builder.Services.AddScoped<IAuthRepository, AuthRepository>();
    builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementService>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    // JWT Authentication
    builder
        .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero,
            };
        });

    builder.Services.AddAuthorization();

    // ==================== CORS ====================
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "AllowFrontend",
            policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:3000",
                        "http://localhost:3001",
                        "http://127.0.0.1:5500",
                        "http://localhost:5500",
                        frontendUrl,
                        "https://quantitymeasurementapp-frontend.onrender.com"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
        );
    });

    // ==================== SWAGGER ====================
    builder.Services.AddSwaggerGen();

    // ==================== BUILD APP ====================
    var app = builder.Build();

    // Force port binding for Render
    if (Environment.GetEnvironmentVariable("RENDER") == "true")
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        app.Urls.Add($"http://0.0.0.0:{port}");
        Log.Information($"Binding to port: {port}");
    }

    // Database Migration/Creation
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Log.Information("Ensuring database is created...");
            dbContext.Database.EnsureCreated();
            Log.Information("Database check completed successfully");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to create/verify database");
        throw;
    }

    // Middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("API Started Successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
