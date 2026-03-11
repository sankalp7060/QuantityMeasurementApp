using QuantityMeasurementModelLayer.DTOs.Enums;

namespace QuantityMeasurementModelLayer.DTOs
{
    /// <summary>
    /// Response DTO for quantity operations.
    /// </summary>
    public class QuantityResponse
    {
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
                Success = true,
                Message = "Operation completed successfully",
                Result = result,
                ResultUnit = unit,
                FormattedResult = formattedResult,
                Operation = operation,
                Timestamp = DateTime.Now,
            };
        }

        public static QuantityResponse ErrorResponse(string errorMessage, OperationType operation)
        {
            return new QuantityResponse
            {
                Success = false,
                Message = errorMessage,
                Operation = operation,
                Timestamp = DateTime.Now,
            };
        }
    }

    /// <summary>
    /// Response DTO for division operations.
    /// </summary>
    public class DivisionResponse
    {
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
                Success = true,
                Message = "Division completed successfully",
                Ratio = ratio,
                Interpretation = interpretation,
                Operation = OperationType.Divide,
                Timestamp = DateTime.Now,
            };
        }

        public static DivisionResponse ErrorResponse(string errorMessage)
        {
            return new DivisionResponse
            {
                Success = false,
                Message = errorMessage,
                Operation = OperationType.Divide,
                Timestamp = DateTime.Now,
            };
        }
    }
}
