using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Services;
using QuantityMeasurementRepositoryLayer.Interface;
using QuantityMeasurementRepositoryLayer.Services;

namespace QuantityMeasurementConsole.Factory
{
    public class ServiceFactory
    {
        private readonly IQuantityMeasurementRepository _repository;
        private readonly IQuantityMeasurementService _service;
        private readonly ILoggerFactory _loggerFactory;
        private string? _connectionString;

        public ServiceFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            var logger = _loggerFactory.CreateLogger<ServiceFactory>();

            try
            {
                // Create AutoHybrid repository - it will handle errors silently
                var repoLogger = _loggerFactory.CreateLogger<AutoHybridRepository>();
                _repository = new AutoHybridRepository(repoLogger);

                var serviceLogger = _loggerFactory.CreateLogger<QuantityMeasurementService>();
                _service = new QuantityMeasurementService(serviceLogger, _repository);

                // Quick silent check - don't show errors
                try
                {
                    var stats = Task.Run(async () =>
                        await _repository.GetRepositoryStatisticsAsync()
                    ).Result;
                }
                catch
                {
                    // Silently ignore - errors will be shown in stats page if needed
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("Starting in cache-only mode: {Message}", ex.Message);

                // Fallback to cache-only mode
                _repository = QuantityMeasurementCacheRepository.Instance;
                var serviceLogger = _loggerFactory.CreateLogger<QuantityMeasurementService>();
                _service = new QuantityMeasurementService(serviceLogger, _repository);
            }
        }

        private void LoadConfiguration()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(basePath, "appsettings.json");

            if (!File.Exists(configPath))
            {
                // Try parent directory (project root)
                basePath = Directory.GetParent(basePath)?.FullName ?? basePath;
                configPath = Path.Combine(basePath, "appsettings.json");
            }

            if (File.Exists(configPath))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                var configuration = builder.Build();
                var configConnection = configuration.GetConnectionString("DefaultConnection");

                if (!string.IsNullOrEmpty(configConnection))
                {
                    _connectionString = configConnection;
                }
            }
        }

        public IQuantityMeasurementRepository GetRepository() => _repository;

        public IQuantityMeasurementService GetService() => _service;
    }
}
