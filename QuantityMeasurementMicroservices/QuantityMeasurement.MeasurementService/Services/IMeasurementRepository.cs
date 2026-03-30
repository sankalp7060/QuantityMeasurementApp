using QuantityMeasurement.MeasurementService.Models;

namespace QuantityMeasurement.MeasurementService.Services;

public interface IMeasurementRepository
{
    Task<Measurement?> GetByIdAsync(long id);
    Task<Measurement?> GetByMeasurementIdAsync(string measurementId);
    Task<IEnumerable<Measurement>> GetAllAsync();
    Task<Measurement> AddAsync(Measurement entity);
    Task UpdateAsync(Measurement entity);
    Task DeleteAsync(long id);
    Task<IEnumerable<Measurement>> GetByOperationAsync(int operationType);
    Task<IEnumerable<Measurement>> GetByCategoryAsync(string category);
    Task<IEnumerable<Measurement>> GetByDateRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<Measurement>> GetSuccessfulOperationsAsync();
    Task<IEnumerable<Measurement>> GetFailedOperationsAsync();
    Task<int> GetTotalCountAsync();
    Task<int> GetCountByOperationAsync(int operationType);
    Task<Dictionary<int, int>> GetOperationCountsAsync();
}
