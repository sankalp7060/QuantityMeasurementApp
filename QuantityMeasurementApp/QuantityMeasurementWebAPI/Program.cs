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
    .CreateLogger();

try
{
    Log.Information("Starting Quantity Measurement API");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // JWT Key - must be set via environment variable in production
    var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
    if (string.IsNullOrEmpty(jwtKey))
    {
        jwtKey = "DevelopmentOnlyInsecureKeyDoNotUseInProduction12345";
        Log.Warning("Using development JWT key - set JWT_KEY env var in production");
    }

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Database - PostgreSQL via DATABASE_URL (Render sets this automatically)
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    string connectionString;

    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Render provides DATABASE_URL in postgres:// format - convert to Npgsql format
        connectionString = ConvertDatabaseUrl(databaseUrl);
        Log.Information("Using DATABASE_URL from environment");
    }
    else
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        Log.Information("Using connection string from appsettings.json");
    }

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly("QuantityMeasurementRepositoryLayer");
            npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
        })
    );

    // Repositories & Services
    builder.Services.AddScoped<IQuantityMeasurementRepository, QuantityMeasurementRepository>();
    builder.Services.AddScoped<IAuthRepository, AuthRepository>();
    builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementService>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    // JWT Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

    // CORS - add your Render frontend URL here
    var frontendUrls = builder.Configuration.GetSection("Frontend:Urls").Get<string[]>()
        ?? new[] { "http://localhost:3000" };

    // Also allow any RENDER_EXTERNAL_URL set at runtime
    var renderFrontend = Environment.GetEnvironmentVariable("FRONTEND_URL");
    if (!string.IsNullOrEmpty(renderFrontend))
        frontendUrls = frontendUrls.Append(renderFrontend).ToArray();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(frontendUrls)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    // Swagger - enabled in all environments so it works on Render
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Auto-migrate database on startup
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
        Log.Information("Database migration applied successfully");
    }

    // Always show Swagger (remove condition for Render hosting)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1");
        c.RoutePrefix = string.Empty;
    });

    // Render handles HTTPS, so we can disable this to avoid "Failed to determine https port" warnings
    // app.UseHttpsRedirection();

    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Add a root endpoint for Render health checks
    app.MapGet("/", () => "Quantity Measurement API is running!");

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

// Helper: convert postgres://user:pass@host:port/db to Npgsql connection string
static string ConvertDatabaseUrl(string databaseUrl)
{
    // If it's already a standard ADO.NET connection string, just return it
    if (!databaseUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) && 
        !databaseUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        return databaseUrl;
    }

    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    var username = userInfo[0];
    var password = userInfo.Length > 1 ? userInfo[1] : "";

    return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
}
