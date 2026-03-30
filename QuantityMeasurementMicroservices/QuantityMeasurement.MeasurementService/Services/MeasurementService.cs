using QuantityMeasurement.MeasurementService.Models;
using QuantityMeasurement.Shared.DTOs;

namespace QuantityMeasurement.MeasurementService.Services;

public class MeasurementService : IMeasurementService
{
    private readonly IMeasurementRepository _repository;
    private readonly ILogger<MeasurementService> _logger;

    public MeasurementService(IMeasurementRepository repository, ILogger<MeasurementService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<QuantityResponseDto> ConvertAsync(ConversionRequestDto request)
    {
        // Placeholder - implement actual conversion logic
        return new QuantityResponseDto
        {
            Success = true,
            Message = "Conversion successful",
            Result = request.Source.Value * 12,
            ResultUnit = request.TargetUnit,
            FormattedResult =
                $"{request.Source.Value} {request.Source.Unit} = {request.Source.Value * 12} {request.TargetUnit}",
            Operation = "Convert",
            Timestamp = DateTime.UtcNow,
        };
    }

    public async Task<QuantityResponseDto> CompareAsync(BinaryOperationRequestDto request)
    {
        return new QuantityResponseDto
        {
            Success = true,
            Message = "Comparison successful",
            Result = request.FirstQuantity.Value == request.SecondQuantity.Value ? 1 : 0,
            FormattedResult =
                $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit} = {request.SecondQuantity.Value} {request.SecondQuantity.Unit}",
            Operation = "Compare",
            Timestamp = DateTime.UtcNow,
        };
    }

    public async Task<QuantityResponseDto> AddAsync(BinaryOperationRequestDto request)
    {
        return new QuantityResponseDto
        {
            Success = true,
            Message = "Addition successful",
            Result = request.FirstQuantity.Value + request.SecondQuantity.Value,
            ResultUnit = request.FirstQuantity.Unit,
            FormattedResult =
                $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit} + {request.SecondQuantity.Value} {request.SecondQuantity.Unit} = {request.FirstQuantity.Value + request.SecondQuantity.Value} {request.FirstQuantity.Unit}",
            Operation = "Add",
            Timestamp = DateTime.UtcNow,
        };
    }

    public async Task<QuantityResponseDto> SubtractAsync(BinaryOperationRequestDto request)
    {
        return new QuantityResponseDto
        {
            Success = true,
            Message = "Subtraction successful",
            Result = request.FirstQuantity.Value - request.SecondQuantity.Value,
            ResultUnit = request.FirstQuantity.Unit,
            FormattedResult =
                $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit} - {request.SecondQuantity.Value} {request.SecondQuantity.Unit} = {request.FirstQuantity.Value - request.SecondQuantity.Value} {request.FirstQuantity.Unit}",
            Operation = "Subtract",
            Timestamp = DateTime.UtcNow,
        };
    }

    public async Task<DivisionResponseDto> DivideAsync(BinaryOperationRequestDto request)
    {
        var ratio = request.FirstQuantity.Value / request.SecondQuantity.Value;
        return new DivisionResponseDto
        {
            Success = true,
            Message = "Division successful",
            Ratio = ratio,
            Interpretation =
                $"{request.FirstQuantity.Value} {request.FirstQuantity.Unit} is {ratio:F2} times {request.SecondQuantity.Value} {request.SecondQuantity.Unit}",
            Operation = "Divide",
            Timestamp = DateTime.UtcNow,
        };
    }

    public async Task<IEnumerable<object>> GetHistoryAsync(string? operation = null)
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(e => new
        {
            e.Id,
            e.MeasurementId,
            e.CreatedAt,
            e.OperationType,
            e.FormattedResult,
            e.IsSuccessful,
            e.ErrorDetails,
        });
    }

    public async Task<IEnumerable<object>> GetCategoryHistoryAsync(string category)
    {
        var entities = await _repository.GetByCategoryAsync(category);
        return entities.Select(e => new
        {
            e.Id,
            e.MeasurementId,
            e.CreatedAt,
            e.OperationType,
            e.FormattedResult,
            e.IsSuccessful,
        });
    }

    public async Task<IEnumerable<object>> GetDateRangeHistoryAsync(DateTime start, DateTime end)
    {
        var entities = await _repository.GetByDateRangeAsync(start, end);
        return entities.Select(e => new
        {
            e.Id,
            e.MeasurementId,
            e.CreatedAt,
            e.OperationType,
            e.FormattedResult,
            e.IsSuccessful,
        });
    }

    public async Task<IEnumerable<object>> GetErrorHistoryAsync()
    {
        var entities = await _repository.GetFailedOperationsAsync();
        return entities.Select(e => new
        {
            e.Id,
            e.MeasurementId,
            e.CreatedAt,
            e.OperationType,
            e.ErrorDetails,
        });
    }

    public async Task<Dictionary<string, object>> GetStatisticsAsync()
    {
        var total = await _repository.GetTotalCountAsync();
        var operationCounts = await _repository.GetOperationCountsAsync();
        var successCount = (await _repository.GetSuccessfulOperationsAsync()).Count();

        return new Dictionary<string, object>
        {
            ["TotalOperations"] = total,
            ["SuccessfulOperations"] = successCount,
            ["FailedOperations"] = total - successCount,
            ["OperationCounts"] = operationCounts,
            ["LastUpdated"] = DateTime.UtcNow,
        };
    }

    public async Task<int> GetOperationCountAsync(string operation)
    {
        var operationType = operation.ToUpper() switch
        {
            "CONVERT" => 1,
            "COMPARE" => 0,
            "ADD" => 2,
            "SUBTRACT" => 3,
            "DIVIDE" => 4,
            _ => 0,
        };
        return await _repository.GetCountByOperationAsync(operationType);
    }
}
