using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuantityMeasurementBusinessLayer.Interface; // Note: singular
using QuantityMeasurementBusinessLayer.Services;

namespace QuantityMeasurementBusinessLayer.Configuration
{
    /// <summary>
    /// Business layer dependency injection registration
    /// </summary>
    public static class BusinessServiceRegistration
    {
        /// <summary>
        /// Registers business layer services with DI container
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddBusinessLayer(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Register service - Scoped lifetime (one per HTTP request)
            services.AddScoped<IQuantityMeasurementService, QuantityMeasurementService>();

            // Register any additional business services here
            // services.AddScoped<IValidationService, ValidationService>();

            return services;
        }
    }
}
