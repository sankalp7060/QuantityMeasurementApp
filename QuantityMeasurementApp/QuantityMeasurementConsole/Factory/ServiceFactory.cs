using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Services;
using QuantityMeasurementRepositoryLayer.Interface;
using QuantityMeasurementRepositoryLayer.Services;

namespace QuantityMeasurementConsole.Factory
{
    /// <summary>
    /// Factory class for creating service instances (Factory Pattern)
    /// Only the menu layer is static, service instances are not static
    /// </summary>
    public class ServiceFactory
    {
        private readonly IQuantityMeasurementRepository _repository;
        private readonly IQuantityMeasurementService _service;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Constructor initializes all non-static service instances
        /// </summary>
        public ServiceFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            // Initialize repository (singleton instance from cache)
            _repository = QuantityMeasurementCacheRepository.Instance;

            // Initialize service with dependencies
            var logger = _loggerFactory.CreateLogger<QuantityMeasurementService>();
            _service = new QuantityMeasurementService(logger, _repository);
        }

        /// <summary>
        /// Gets the repository instance (non-static)
        /// </summary>
        public IQuantityMeasurementRepository GetRepository() => _repository;

        /// <summary>
        /// Gets the service instance (non-static)
        /// </summary>
        public IQuantityMeasurementService GetService() => _service;
    }
}
