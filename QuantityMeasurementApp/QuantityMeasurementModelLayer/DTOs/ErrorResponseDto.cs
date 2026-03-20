// Define the namespace for Data Transfer Objects
namespace QuantityMeasurementModelLayer.DTOs
{
    /// <summary>
    /// Standard error response DTO for API
    /// Provides a consistent error format across all API endpoints
    /// Follows RFC 7807 (Problem Details for HTTP APIs) pattern
    /// </summary>
    public class ErrorResponseDto
    {
        /// <summary>
        /// When the error occurred
        /// UTC timestamp for tracking when the error happened
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// HTTP status code
        /// e.g., 400 for Bad Request, 404 for Not Found, 500 for Internal Server Error
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Error type/title
        /// Short, human-readable summary of the error type
        /// </summary>
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// Detailed error message
        /// Specific details about what went wrong
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Path where error occurred
        /// The API endpoint that was called when the error happened
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Request ID for tracking
        /// Correlates the error with server logs for debugging
        /// </summary>
        public string? TraceId { get; set; }

        /// <summary>
        /// Factory method to create an error response
        /// Convenience method to create a fully populated ErrorResponseDto
        /// </summary>
        /// <param name="status">HTTP status code</param>
        /// <param name="error">Error type/title</param>
        /// <param name="message">Detailed error message</param>
        /// <param name="path">Request path</param>
        /// <param name="traceId">Request trace ID</param>
        /// <returns>Populated ErrorResponseDto</returns>
        public static ErrorResponseDto Create(
            int status,
            string error,
            string message,
            string path,
            string? traceId = null
        )
        {
            return new ErrorResponseDto
            {
                Timestamp = DateTime.UtcNow, // Set current UTC time
                Status = status,
                Error = error,
                Message = message,
                Path = path,
                TraceId = traceId,
            };
        }
    }
}
