using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Services;
using QuantityMeasurementRepositoryLayer.Configuration;
using QuantityMeasurementRepositoryLayer.Interface;
using QuantityMeasurementRepositoryLayer.Services;

namespace QuantityMeasurementWebApp.Extensions
{
    /// <summary>
    /// Extension methods for registering services.
    /// </summary>
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRepositoryServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Register Redis configuration
            var redisConfig = new RedisConfiguration();
            configuration.GetSection("Redis").Bind(redisConfig);
            services.AddSingleton(redisConfig);

            services.AddSingleton<IQuantityMeasurementRepository>(sp =>
            {
                var fileRepo = QuantityMeasurementCacheRepository.Instance;
                var cacheRepo = sp.GetRequiredService<IRedisCacheRepository>();

                return new HybridRepository(
                    fileRepo,
                    cacheRepo,
                    sp.GetRequiredService<ILogger<HybridRepository>>()
                );
            });

            services.AddSingleton<IRedisCacheRepository>(sp =>
            {
                var config = sp.GetRequiredService<RedisConfiguration>();
                var logger = sp.GetRequiredService<ILogger<RedisCacheRepository>>();
                return new RedisCacheRepository(config, logger);
            });

            return services;
        }

        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<IQuantityMeasurementService, QuantityMeasurementService>();
            return services;
        }
    }
}
