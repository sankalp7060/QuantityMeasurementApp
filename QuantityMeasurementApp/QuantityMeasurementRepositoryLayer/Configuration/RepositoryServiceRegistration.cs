using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuantityMeasurementRepositoryLayer.Data;
using QuantityMeasurementRepositoryLayer.Interface;
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
        public static IServiceCollection AddRepositoryLayer(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<ApplicationDbContext>(
                (serviceProvider, options) =>
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection");

                    options.UseNpgsql(
                        connectionString,
                        npgsqlOptions =>
                        {
                            npgsqlOptions.MigrationsAssembly("QuantityMeasurementRepositoryLayer");
                            npgsqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorCodesToAdd: null
                            );
                            npgsqlOptions.CommandTimeout(30);
                        }
                    );

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        options.EnableSensitiveDataLogging();
                        options.EnableDetailedErrors();
                    }
                },
                ServiceLifetime.Scoped
            );

            services.AddScoped<IQuantityMeasurementRepository, QuantityMeasurementRepository>();

            return services;
        }
    }
}
