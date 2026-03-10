using Microsoft.OpenApi.Models;
using QuantityMeasurementRepositoryLayer.Interface;
using QuantityMeasurementWebApp.Extensions;
using Serilog;
using Serilog.Formatting.Json;

namespace QuantityMeasurementWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(
                    new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile(
                            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                            true
                        )
                        .Build()
                )
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.File(
                    new JsonFormatter(),
                    path: "logs/quantityapi-.json",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30
                )
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Logging.ClearProviders();
                builder.Host.UseSerilog();

                // Register repositories (pass configuration)
                builder.Services.AddRepositoryServices(builder.Configuration);

                // Register business logic services
                builder.Services.AddBusinessLogicServices();

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "Quantity Measurement API",
                            Version = "v1",
                            Description =
                                "API for performing quantity measurements across Length, Weight, Volume, and Temperature units with Redis caching",
                            Contact = new OpenApiContact
                            {
                                Name = "Quantity Measurement Team",
                                Email = "api@quantitymeasurement.com",
                            },
                        }
                    );
                });

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(
                        "AllowAll",
                        builder =>
                        {
                            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                        }
                    );
                });

                var app = builder.Build();

                using (var scope = app.Services.CreateScope())
                {
                    var redisCache =
                        scope.ServiceProvider.GetRequiredService<IRedisCacheRepository>();
                    var isConnected = redisCache.IsConnectedAsync().Result;
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    if (isConnected)
                    {
                        logger.LogInformation("✅ Redis cache is connected and operational");
                    }
                    else
                    {
                        logger.LogWarning(
                            "⚠️ Redis cache is not connected. Using file storage only."
                        );
                    }
                }

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint(
                            "/swagger/v1/swagger.json",
                            "Quantity Measurement API v1"
                        );
                        c.RoutePrefix = string.Empty;
                    });
                }

                app.UseHttpsRedirection();
                app.UseCors("AllowAll");
                app.UseAuthorization();
                app.MapControllers();

                Log.Information("Quantity Measurement API with Redis started");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
