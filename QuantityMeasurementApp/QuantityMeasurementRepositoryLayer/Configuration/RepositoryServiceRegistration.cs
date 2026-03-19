using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuantityMeasurementRepositoryLayer.Data;
using QuantityMeasurementRepositoryLayer.Interface; // Note: singular
using QuantityMeasurementRepositoryLayer.Repositories;

namespace QuantityMeasurementRepositoryLayer.Configuration
{
    /// <summary>
    /// Repository layer dependency injection registration
    /// </summary>
    public static class RepositoryServiceRegistration
    {
        /// <summary>
        /// Registers repository layer services with DI container
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddRepositoryLayer(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Register DbContext with SQL Server - Scoped lifetime
            services.AddDbContext<ApplicationDbContext>(
                (serviceProvider, options) =>
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection");

                    options.UseSqlServer(
                        connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly("QuantityMeasurementRepositoryLayer");
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null
                            );
                            sqlOptions.CommandTimeout(30);
                        }
                    );

                    // Add sensitive data logging in development
                    if (
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                        == "Development"
                    )
                    {
                        options.EnableSensitiveDataLogging();
                        options.EnableDetailedErrors();
                    }
                },
                ServiceLifetime.Scoped
            );

            // Register repository - Scoped lifetime (one per HTTP request)
            services.AddScoped<IQuantityMeasurementRepository, QuantityMeasurementRepository>();

            // Register Unit of Work pattern if needed
            // services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
