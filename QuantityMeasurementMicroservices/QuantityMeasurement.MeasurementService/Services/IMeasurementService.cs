using QuantityMeasurement.Shared.DTOs;

namespace QuantityMeasurement.MeasurementService.Services;

public interface IMeasurementService
{
    Task<QuantityResponseDto> ConvertAsync(ConversionRequestDto request);
    Task<QuantityResponseDto> CompareAsync(BinaryOperationRequestDto request);
    Task<QuantityResponseDto> AddAsync(BinaryOperationRequestDto request);
    Task<QuantityResponseDto> SubtractAsync(BinaryOperationRequestDto request);
    Task<DivisionResponseDto> DivideAsync(BinaryOperationRequestDto request);
    Task<IEnumerable<object>> GetHistoryAsync(string? operation = null);
    Task<IEnumerable<object>> GetCategoryHistoryAsync(string category);
    Task<IEnumerable<object>> GetDateRangeHistoryAsync(DateTime start, DateTime end);
    Task<IEnumerable<object>> GetErrorHistoryAsync();
    Task<Dictionary<string, object>> GetStatisticsAsync();
    Task<int> GetOperationCountAsync(string operation);
}
