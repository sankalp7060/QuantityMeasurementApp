// Import required namespaces for network operations, JSON serialization, and custom exceptions
using System.Net;
using System.Text.Json;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.Exceptions;

// Namespace for custom middleware components
namespace QuantityMeasurementWebAPI.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// This middleware catches all unhandled exceptions and returns consistent error responses
    /// </summary>
    public class GlobalExceptionHandler
    {
        // Private fields for dependencies
        private readonly RequestDelegate _next; // Next middleware in the pipeline
        private readonly ILogger<GlobalExceptionHandler> _logger; // Logger for recording errors

        /// <summary>
        /// Constructor for GlobalExceptionHandler
        /// </summary>
        /// <param name="next">Next middleware delegate (injected by framework)</param>
        /// <param name="logger">Logger instance (injected by DI)</param>
        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            // Assign dependencies with null checking
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Invoke method called by the middleware pipeline for each HTTP request
        /// </summary>
        /// <param name="context">Current HTTP context</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Try to execute the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // If any exception occurs, handle it
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exceptions by creating appropriate HTTP responses
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="exception">The exception that was thrown</param>
        /// <returns>Task representing the response writing operation</returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Get the response object from context
            var response = context.Response;
            response.ContentType = "application/json"; // Set response type to JSON

            // Create base error response DTO with common information
            var errorResponse = new ErrorResponseDto
            {
                Timestamp = DateTime.UtcNow, // When the error occurred (UTC)
                Path = context.Request.Path, // Which URL path caused the error
                TraceId = context.TraceIdentifier, // Unique ID for tracing the request
            };

            // ==================== EXCEPTION TYPE HANDLING ====================
            // Handle different exception types with appropriate status codes and messages

            // Case 1: Database Exception
            if (exception is DatabaseException dbEx)
            {
                // Log database error (as Error level)
                _logger.LogError(dbEx, "Database error: {Message}", dbEx.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
                errorResponse.Status = response.StatusCode;
                errorResponse.Error = "Database Error";
                errorResponse.Message = "A database error occurred. Please try again later.";

                // In development environment, include detailed error message
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    errorResponse.Message = dbEx.Message;
                }
            }
            // Case 2: Quantity Measurement Exception (business logic errors)
            else if (exception is QuantityMeasurementException qmEx)
            {
                // Log business logic error (as Warning level)
                _logger.LogWarning(qmEx, "Quantity measurement error: {Message}", qmEx.Message);
                response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                errorResponse.Status = response.StatusCode;
                errorResponse.Error = "Quantity Measurement Error";
                errorResponse.Message = qmEx.Message; // Show the actual error message
            }
            // Case 3: Argument Exception (invalid input)
            else if (exception is ArgumentException argEx)
            {
                // Log argument error (as Warning level)
                _logger.LogWarning(argEx, "Argument error: {Message}", argEx.Message);
                response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                errorResponse.Status = response.StatusCode;
                errorResponse.Error = "Invalid Argument";
                errorResponse.Message = argEx.Message;
            }
            // Case 4: Unauthorized Access
            else if (exception is UnauthorizedAccessException unauthEx)
            {
                // Log unauthorized access (as Warning level)
                _logger.LogWarning(unauthEx, "Unauthorized access: {Message}", unauthEx.Message);
                response.StatusCode = (int)HttpStatusCode.Unauthorized; // 401
                errorResponse.Status = response.StatusCode;
                errorResponse.Error = "Unauthorized";
                errorResponse.Message = "You are not authorized to perform this action.";
            }
            // Case 5: Any other unhandled exception
            else
            {
                // Log unexpected error (as Error level)
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
                errorResponse.Status = response.StatusCode;
                errorResponse.Error = "Internal Server Error";
                errorResponse.Message = "An unexpected error occurred. Please try again later.";

                // In development environment, include detailed error message
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    errorResponse.Message = exception.Message;
                }
            }

            // ==================== SERIALIZE AND SEND RESPONSE ====================
            // Convert error response object to JSON
            var jsonResponse = JsonSerializer.Serialize(
                errorResponse,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use camelCase for JSON properties
                }
            );

            // Write the JSON response to the HTTP response body
            await response.WriteAsync(jsonResponse);
        }
    }
}
