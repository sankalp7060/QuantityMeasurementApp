using QuantityMeasurementModelLayer.DTOs.Enums;

namespace QuantityMeasurementModelLayer.DTOs
{
    /// <summary>
    /// Response DTO for quantity operations.
    /// </summary>
    public class QuantityResponse
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public double? Result { get; set; }
        public string? ResultUnit { get; set; }
        public string? FormattedResult { get; set; }
        public OperationType Operation { get; set; }
        public DateTime Timestamp { get; set; }

        public static QuantityResponse SuccessResponse(
            double result,
            string unit,
            OperationType operation,
            string formattedResult
        )
        {
            return new QuantityResponse
            {
                StatusCode = 200,
                Success = true,
                Message = "Operation completed successfully",
                Result = result,
                ResultUnit = unit,
                FormattedResult = formattedResult,
                Operation = operation,
                Timestamp = DateTime.UtcNow,
            };
        }

        public static QuantityResponse ErrorResponse(
            string errorMessage,
            OperationType operation,
            int statusCode = 400
        )
        {
            return new QuantityResponse
            {
                StatusCode = statusCode,
                Success = false,
                Message = errorMessage,
                Operation = operation,
                Timestamp = DateTime.UtcNow,
            };
        }
    }

    /// <summary>
    /// Response DTO for division operations.
    /// </summary>
    public class DivisionResponse
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public double? Ratio { get; set; }
        public string? Interpretation { get; set; }
        public OperationType Operation { get; set; }
        public DateTime Timestamp { get; set; }

        public static DivisionResponse SuccessResponse(double ratio, string interpretation)
        {
            return new DivisionResponse
            {
                StatusCode = 200,
                Success = true,
                Message = "Division completed successfully",
                Ratio = ratio,
                Interpretation = interpretation,
                Operation = OperationType.Divide,
                Timestamp = DateTime.UtcNow,
            };
        }

        public static DivisionResponse ErrorResponse(string errorMessage, int statusCode = 400)
        {
            return new DivisionResponse
            {
                StatusCode = statusCode,
                Success = false,
                Message = errorMessage,
                Operation = OperationType.Divide,
                Timestamp = DateTime.UtcNow,
            };
        }
    }

    /// <summary>
    /// Batch response DTO containing multiple operation results.
    /// </summary>
    public class BatchResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<T> Results { get; set; } = new();
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Cache statistics response.
    /// </summary>
    public class CacheStatsResponse
    {
        public bool IsRedisConnected { get; set; }
        public int CacheItemCount { get; set; }
        public double? HitRate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
