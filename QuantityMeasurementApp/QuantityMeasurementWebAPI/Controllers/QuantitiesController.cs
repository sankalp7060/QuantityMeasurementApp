// Import necessary namespaces for HTTP handling, MVC pattern, business layer interfaces, DTOs, enums, exceptions, and Swagger documentation
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.DTOs.Enums;
using Swashbuckle.AspNetCore.Annotations;

// Define the namespace for the WebAPI controllers
namespace QuantityMeasurementWebAPI.Controllers
{
    /// <summary>
    /// Controller for quantity measurement operations
    /// This controller handles all HTTP requests related to quantity measurements
    /// </summary>
    [ApiController] // Indicates that this class is an API controller with built-in features like model validation
    [Route("api/v1/[controller]")] // Sets the base route as api/v1/quantities (controller name without "Controller" suffix)
    [Produces("application/json")] // Specifies that all actions return JSON responses
    [SwaggerTag("Quantity measurement operations (compare, convert, add, subtract, divide)")] // Adds a tag in Swagger documentation
    public class QuantitiesController : ControllerBase // Inherits from ControllerBase for MVC features without view support
    {
        // Private fields to store dependencies (readonly means they can only be set in constructor)
        private readonly IQuantityMeasurementService _quantityService; // Interface for business layer operations
        private readonly ILogger<QuantitiesController> _logger; // Logger for recording events and errors

        /// <summary>
        /// Initializes a new instance of the QuantitiesController
        /// Constructor that receives dependencies through Dependency Injection
        /// </summary>
        /// <param name="quantityService">Quantity measurement service</param>
        /// <param name="logger">Logger instance</param>
        public QuantitiesController(
            IQuantityMeasurementService quantityService,
            ILogger<QuantitiesController> logger
        )
        {
            // Assign injected dependencies to private fields, with null checking
            _quantityService =
                quantityService ?? throw new ArgumentNullException(nameof(quantityService)); // Throws exception if quantityService is null
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Throws exception if logger is null
        }

        #region Core Operations
        // This region groups all the main measurement operations

        /// <summary>
        /// Compares two quantities for equality
        /// HTTP POST endpoint that accepts JSON body with two quantities to compare
        /// </summary>
        /// <param name="request">The quantities to compare (contained in request DTO)</param>
        /// <returns>Comparison result indicating if quantities are equal</returns>
        /// <response code="200">Returns the comparison result</response>
        /// <response code="400">If the request is invalid or categories don't match</response>
        /// <response code="500">If an internal server error occurs</response>
        [HttpPost("compare")] // Maps HTTP POST requests to /api/v1/quantities/compare
        [SwaggerOperation(
            Summary = "Compare two quantities", // Short description for Swagger
            Description = "Compares two quantities of the same category to check if they are equal" // Detailed description
        )]
        [SwaggerResponse(200, "Comparison completed successfully", typeof(QuantityResponseDto))] // Success response documentation
        [SwaggerResponse(400, "Invalid request or category mismatch", typeof(ErrorResponseDto))] // Bad request response documentation
        [SwaggerResponse(500, "Internal server error", typeof(ErrorResponseDto))] // Server error response documentation
        public async Task<IActionResult> CompareQuantities(
            [FromBody] BinaryOperationRequestDto request // Gets request data from HTTP request body
        )
        {
            // Log that the endpoint was called (for debugging and monitoring)
            _logger.LogInformation("POST /api/v1/quantities/compare called");

            // Check if the request model passes validation rules (data annotations in DTO)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 Bad Request with validation errors
            }

            // Call the business layer service to perform the comparison asynchronously
            var result = await _quantityService.CompareQuantitiesAsync(request);
            return Ok(result); // Return 200 OK with the comparison result
        }

        /// <summary>
        /// Converts a quantity to a different unit
        /// </summary>
        /// <param name="request">The conversion request with source and target unit</param>
        /// <returns>Converted quantity in target unit</returns>
        [HttpPost("convert")]
        [SwaggerOperation(
            Summary = "Convert quantity units",
            Description = "Converts a quantity from one unit to another within the same category"
        )]
        [SwaggerResponse(200, "Conversion completed successfully", typeof(QuantityResponseDto))]
        [SwaggerResponse(400, "Invalid request or target unit", typeof(ErrorResponseDto))]
        [SwaggerResponse(500, "Internal server error", typeof(ErrorResponseDto))]
        public async Task<IActionResult> ConvertQuantity([FromBody] ConversionRequestDto request)
        {
            // Log the endpoint call
            _logger.LogInformation("POST /api/v1/quantities/convert called");

            // Validate the request model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Perform the conversion and return result
            var result = await _quantityService.ConvertQuantityAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Adds two quantities
        /// </summary>
        /// <param name="request">The quantities to add with optional target unit</param>
        /// <returns>Sum of the two quantities</returns>
        [HttpPost("add")]
        [SwaggerOperation(
            Summary = "Add two quantities",
            Description = "Adds two quantities of the same category with optional target unit"
        )]
        [SwaggerResponse(200, "Addition completed successfully", typeof(QuantityResponseDto))]
        [SwaggerResponse(
            400,
            "Invalid request or operation not supported",
            typeof(ErrorResponseDto)
        )]
        [SwaggerResponse(500, "Internal server error", typeof(ErrorResponseDto))]
        public async Task<IActionResult> AddQuantities([FromBody] BinaryOperationRequestDto request)
        {
            // Log the endpoint call
            _logger.LogInformation("POST /api/v1/quantities/add called");

            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Perform addition and return result
            var result = await _quantityService.AddQuantitiesAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Subtracts one quantity from another
        /// </summary>
        /// <param name="request">The quantities to subtract (first - second) with optional target unit</param>
        /// <returns>Difference between the two quantities</returns>
        [HttpPost("subtract")]
        [SwaggerOperation(
            Summary = "Subtract two quantities",
            Description = "Subtracts the second quantity from the first with optional target unit"
        )]
        [SwaggerResponse(200, "Subtraction completed successfully", typeof(QuantityResponseDto))]
        [SwaggerResponse(
            400,
            "Invalid request or operation not supported",
            typeof(ErrorResponseDto)
        )]
        [SwaggerResponse(500, "Internal server error", typeof(ErrorResponseDto))]
        public async Task<IActionResult> SubtractQuantities(
            [FromBody] BinaryOperationRequestDto request
        )
        {
            // Log the endpoint call
            _logger.LogInformation("POST /api/v1/quantities/subtract called");

            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Perform subtraction and return result
            var result = await _quantityService.SubtractQuantitiesAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Divides one quantity by another
        /// </summary>
        /// <param name="request">The quantities to divide (first ÷ second)</param>
        /// <returns>Dimensionless ratio with interpretation</returns>
        [HttpPost("divide")]
        [SwaggerOperation(
            Summary = "Divide two quantities",
            Description = "Divides the first quantity by the second, returning a dimensionless ratio"
        )]
        [SwaggerResponse(200, "Division completed successfully", typeof(DivisionResponseDto))]
        [SwaggerResponse(400, "Invalid request or division by zero", typeof(ErrorResponseDto))]
        [SwaggerResponse(500, "Internal server error", typeof(ErrorResponseDto))]
        public async Task<IActionResult> DivideQuantities(
            [FromBody] BinaryOperationRequestDto request
        )
        {
            // Log the endpoint call
            _logger.LogInformation("POST /api/v1/quantities/divide called");

            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Perform division and return result (special DTO for division)
            var result = await _quantityService.DivideQuantitiesAsync(request);
            return Ok(result);
        }

        #endregion

        #region History Operations
        // This region groups all operations related to retrieving operation history

        /// <summary>
        /// Gets operation history
        /// </summary>
        /// <param name="operation">Optional operation type filter (if null, returns all)</param>
        /// <returns>List of measurement history records</returns>
        [HttpGet("history")] // HTTP GET request to /api/v1/quantities/history
        [SwaggerOperation(
            Summary = "Get operation history",
            Description = "Retrieves all measurement operations, optionally filtered by operation type"
        )]
        [SwaggerResponse(
            200,
            "History retrieved successfully",
            typeof(IEnumerable<MeasurementHistoryDto>)
        )]
        public async Task<IActionResult> GetHistory([FromQuery] OperationType? operation = null) // Gets operation from query string
        {
            // Log the request including the optional operation filter
            _logger.LogInformation(
                "GET /api/v1/quantities/history called with operation={Operation}",
                operation
            );

            // Retrieve history from service and return
            var history = await _quantityService.GetOperationHistoryAsync(operation);
            return Ok(history);
        }

        /// <summary>
        /// Gets history by category
        /// </summary>
        /// <param name="category">Category to filter by (LENGTH, WEIGHT, VOLUME, TEMPERATURE)</param>
        /// <returns>List of measurement history records for the specified category</returns>
        [HttpGet("history/category/{category}")] // Route includes category as URL parameter
        [SwaggerOperation(
            Summary = "Get history by category",
            Description = "Retrieves measurement operations for a specific category"
        )]
        [SwaggerResponse(
            200,
            "Category history retrieved successfully",
            typeof(IEnumerable<MeasurementHistoryDto>)
        )]
        public async Task<IActionResult> GetHistoryByCategory(string category) // category comes from URL
        {
            // Log which category is being requested
            _logger.LogInformation(
                "GET /api/v1/quantities/history/category/{Category} called",
                category
            );

            // Retrieve category-specific history
            var history = await _quantityService.GetCategoryHistoryAsync(category);
            return Ok(history);
        }

        /// <summary>
        /// Gets history by date range
        /// </summary>
        /// <param name="startDate">Start date (yyyy-MM-dd)</param>
        /// <param name="endDate">End date (yyyy-MM-dd)</param>
        /// <returns>List of measurement history records within the date range</returns>
        [HttpGet("history/range")]
        [SwaggerOperation(
            Summary = "Get history by date range",
            Description = "Retrieves measurement operations within a specified date range"
        )]
        [SwaggerResponse(
            200,
            "Date range history retrieved successfully",
            typeof(IEnumerable<MeasurementHistoryDto>)
        )]
        public async Task<IActionResult> GetHistoryByDateRange(
            [FromQuery] DateTime startDate, // Gets startDate from query string
            [FromQuery] DateTime endDate // Gets endDate from query string
        )
        {
            // Log the date range being queried
            _logger.LogInformation(
                "GET /api/v1/quantities/history/range called from {Start} to {End}",
                startDate,
                endDate
            );

            // Retrieve history within date range
            var history = await _quantityService.GetDateRangeHistoryAsync(startDate, endDate);
            return Ok(history);
        }

        /// <summary>
        /// Gets error history
        /// </summary>
        /// <returns>List of failed operations</returns>
        [HttpGet("history/errors")]
        [SwaggerOperation(
            Summary = "Get error history",
            Description = "Retrieves all failed measurement operations"
        )]
        [SwaggerResponse(
            200,
            "Error history retrieved successfully",
            typeof(IEnumerable<MeasurementHistoryDto>)
        )]
        public async Task<IActionResult> GetErrorHistory()
        {
            // Log the error history request
            _logger.LogInformation("GET /api/v1/quantities/history/errors called");

            // Retrieve only failed operations
            var history = await _quantityService.GetErrorHistoryAsync();
            return Ok(history);
        }

        #endregion

        #region Statistics Operations
        // This region groups all statistical operations

        /// <summary>
        /// Gets operation statistics
        /// </summary>
        /// <returns>Statistics about operations (counts, success rates, etc.)</returns>
        [HttpGet("statistics")]
        [SwaggerOperation(
            Summary = "Get operation statistics",
            Description = "Retrieves statistics about all operations"
        )]
        [SwaggerResponse(
            200,
            "Statistics retrieved successfully",
            typeof(Dictionary<string, object>)
        )]
        public async Task<IActionResult> GetStatistics()
        {
            // Log statistics request
            _logger.LogInformation("GET /api/v1/quantities/statistics called");

            // Retrieve statistical data
            var statistics = await _quantityService.GetStatisticsAsync();
            return Ok(statistics);
        }

        /// <summary>
        /// Gets operation count for a specific operation type
        /// </summary>
        /// <param name="operation">Operation type (Compare, Convert, Add, etc.)</param>
        /// <returns>Count of operations of that type</returns>
        [HttpGet("count/{operation}")] // operation comes from URL
        [SwaggerOperation(
            Summary = "Get operation count",
            Description = "Retrieves the count of operations for a specific operation type"
        )]
        [SwaggerResponse(200, "Count retrieved successfully", typeof(int))]
        public async Task<IActionResult> GetOperationCount(OperationType operation)
        {
            // Log which operation count is being requested
            _logger.LogInformation("GET /api/v1/quantities/count/{Operation} called", operation);

            // Get the count from service
            var count = await _quantityService.GetOperationCountAsync(operation);
            // Return as anonymous object with operation name and count
            return Ok(new { Operation = operation.ToString(), Count = count });
        }

        #endregion
    }
}
