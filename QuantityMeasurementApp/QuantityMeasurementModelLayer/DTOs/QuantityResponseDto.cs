// Import OperationType enum for operation identification
using QuantityMeasurementModelLayer.DTOs.Enums;

// Define the namespace for Data Transfer Objects
namespace QuantityMeasurementModelLayer.DTOs
{
    /// <summary>
    /// Standard response DTO for quantity operations
    /// Provides a consistent response format for most API endpoints
    /// </summary>
    public class QuantityResponseDto
    {
        /// <summary>
        /// Whether the operation was successful
        /// True for success, False for failure
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result
        /// Success message or error description
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Result value of the operation
        /// The numeric result (null for failed operations)
        /// </summary>
        public double? Result { get; set; }

        /// <summary>
        /// Unit of the result
        /// The unit in which the result is expressed
        /// </summary>
        public string? ResultUnit { get; set; }

        /// <summary>
        /// Formatted result string
        /// Human-readable representation (e.g., "5 feet = 60 inches")
        /// </summary>
        public string? FormattedResult { get; set; }

        /// <summary>
        /// Type of operation performed
        /// Helps client identify which operation produced this response
        /// </summary>
        public OperationType Operation { get; set; }

        /// <summary>
        /// When the operation was performed
        /// UTC timestamp of the operation
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Factory method to create a success response
        /// Convenience method for successful operations
        /// </summary>
        /// <param name="result">Numeric result</param>
        /// <param name="unit">Result unit</param>
        /// <param name="operation">Operation type</param>
        /// <param name="formattedResult">Formatted result string</param>
        /// <returns>Populated success response</returns>
        public static QuantityResponseDto SuccessResponse(
            double result,
            string unit,
            OperationType operation,
            string formattedResult
        )
        {
            return new QuantityResponseDto
            {
                Success = true,
                Message = "Operation completed successfully",
                Result = result,
                ResultUnit = unit,
                FormattedResult = formattedResult,
                Operation = operation,
                Timestamp = DateTime.UtcNow,
            };
        }

        /// <summary>
        /// Factory method to create an error response
        /// Convenience method for failed operations
        /// </summary>
        /// <param name="errorMessage">Error description</param>
        /// <param name="operation">Operation type that failed</param>
        /// <returns>Populated error response</returns>
        public static QuantityResponseDto ErrorResponse(
            string errorMessage,
            OperationType operation
        )
        {
            return new QuantityResponseDto
            {
                Success = false,
                Message = errorMessage,
                Operation = operation,
                Timestamp = DateTime.UtcNow,
            };
        }
    }

    /// <summary>
    /// Response DTO for division operations
    /// Division returns a ratio (dimensionless) with interpretation
    /// Separate DTO because division results are different from other operations
    /// </summary>
    public class DivisionResponseDto
    {
        /// <summary>
        /// Whether the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The division ratio (dimensionless)
        /// First quantity divided by second quantity (no units)
        /// </summary>
        public double? Ratio { get; set; }

        /// <summary>
        /// Human-readable interpretation of the ratio
        /// e.g., "First quantity is 2.5 times the second quantity"
        /// </summary>
        public string? Interpretation { get; set; }

        /// <summary>
        /// Type of operation performed
        /// Always OperationType.Divide for this DTO
        /// </summary>
        public OperationType Operation { get; set; }

        /// <summary>
        /// When the operation was performed
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Factory method to create a success response
        /// </summary>
        /// <param name="ratio">The division ratio</param>
        /// <param name="interpretation">Human-readable interpretation</param>
        /// <returns>Populated success response</returns>
        public static DivisionResponseDto SuccessResponse(double ratio, string interpretation)
        {
            return new DivisionResponseDto
            {
                Success = true,
                Message = "Division completed successfully",
                Ratio = ratio,
                Interpretation = interpretation,
                Operation = OperationType.Divide,
                Timestamp = DateTime.UtcNow,
            };
        }

        /// <summary>
        /// Factory method to create an error response
        /// </summary>
        /// <param name="errorMessage">Error description</param>
        /// <returns>Populated error response</returns>
        public static DivisionResponseDto ErrorResponse(string errorMessage)
        {
            return new DivisionResponseDto
            {
                Success = false,
                Message = errorMessage,
                Operation = OperationType.Divide,
                Timestamp = DateTime.UtcNow,
            };
        }
    }

    /// <summary>
    /// DTO for measurement history records
    /// Used when retrieving operation history from the database
    /// Contains a simplified view of measurement data for list displays
    /// </summary>
    public class MeasurementHistoryDto
    {
        /// <summary>
        /// Unique identifier (database ID)
        /// Used for referencing specific records
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// When the measurement was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Type of operation (as string for display)
        /// Converted from OperationType enum to string
        /// </summary>
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// Formatted result
        /// Ready-to-display result string
        /// </summary>
        public string? Result { get; set; }

        /// <summary>
        /// Whether operation was successful
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Error message if any
        /// Only populated for failed operations
        /// </summary>
        public string? Error { get; set; }
    }
}
