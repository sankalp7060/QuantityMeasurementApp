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

    var jwtKey =
        Environment.GetEnvironmentVariable("JWT_KEY")
        ?? throw new InvalidOperationException("JWT_KEY environment variable is not set");

    // Get Database connection string
    var dbConnectionString =
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? builder.Configuration.GetConnectionString("DefaultConnection");

    // Get Google credentials
    var googleClientId =
        Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")
        ?? throw new InvalidOperationException("GOOGLE_CLIENT_ID not set");
    var googleClientSecret =
        Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")
        ?? throw new InvalidOperationException("GOOGLE_CLIENT_SECRET not set");

    // Get Frontend URL
    var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000";

    builder.Host.UseSerilog();

    // JWT Key
    var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
    if (string.IsNullOrEmpty(jwtKey))
    {
        jwtKey = "DevelopmentOnlyInsecureKeyDoNotUseInProduction12345";
        Log.Warning("Using development JWT key");
    }

    // Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Database
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

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

    // ==================== CORS CONFIGURATION ====================
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
                        "http://localhost:5500"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
        );
    });

    // Swagger
    builder.Services.AddSwaggerGen();

    // Build App
    var app = builder.Build();

    // Database
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
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
