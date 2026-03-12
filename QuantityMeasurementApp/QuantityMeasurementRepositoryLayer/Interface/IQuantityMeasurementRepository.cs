using QuantityMeasurementModelLayer.DTOs.Enums;
using QuantityMeasurementModelLayer.Entities;

namespace QuantityMeasurementRepositoryLayer.Interface
{
    /// <summary>
    /// Interface for quantity measurement repository operations.
    /// All methods are async with proper naming conventions.
    /// </summary>
    public interface IQuantityMeasurementRepository
    {
        // Core CRUD operations - Async with proper naming
        Task SaveQuantityAsync(QuantityMeasurementEntity entity);
        Task<List<QuantityMeasurementEntity>> GetAllQuantitiesAsync();
        Task<QuantityMeasurementEntity?> GetQuantityByIdAsync(string id);
        Task ClearAllQuantitiesAsync();

        // Query methods - Async with proper naming
        Task<List<QuantityMeasurementEntity>> GetQuantitiesByOperationAsync(
            OperationType operationType
        );
        Task<List<QuantityMeasurementEntity>> GetQuantitiesByCategoryAsync(string category);
        Task<List<QuantityMeasurementEntity>> GetQuantitiesByDateRangeAsync(
            DateTime startDate,
            DateTime endDate
        );
        Task<int> GetTotalQuantityCountAsync();

        // Statistics and monitoring
        Task<Dictionary<string, object>> GetRepositoryStatisticsAsync();

        // Resource management
        Task ReleaseResourcesAsync();
    }
}
