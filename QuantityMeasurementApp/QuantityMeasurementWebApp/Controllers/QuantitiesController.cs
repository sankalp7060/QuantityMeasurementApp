using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.DTOs.Enums;

namespace QuantityMeasurementWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class QuantitiesController : ControllerBase
    {
        private readonly IQuantityMeasurementService _quantityService;
        private readonly ILogger<QuantitiesController> _logger;

        public QuantitiesController(
            IQuantityMeasurementService quantityService,
            ILogger<QuantitiesController> logger
        )
        {
            _quantityService = quantityService;
            _logger = logger;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(
                new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    SupportedCategories = new[] { "Length", "Weight", "Volume", "Temperature" },
                    Version = "1.0.0",
                }
            );
        }

        [HttpPost("compare")]
        public async Task<IActionResult> CompareQuantities([FromBody] BinaryQuantityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(
                    QuantityResponse.ErrorResponse("Invalid request format", OperationType.Compare)
                );

            var result = await _quantityService.CompareQuantitiesAsync(request);
            return result.StatusCode == 200 ? Ok(result) : StatusCode(result.StatusCode, result);
        }

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertQuantity([FromBody] ConversionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(
                    QuantityResponse.ErrorResponse("Invalid request format", OperationType.Convert)
                );

            var result = await _quantityService.ConvertQuantityAsync(request);
            return result.StatusCode == 200 ? Ok(result) : StatusCode(result.StatusCode, result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddQuantities([FromBody] BinaryQuantityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(
                    QuantityResponse.ErrorResponse("Invalid request format", OperationType.Add)
                );

            var result = await _quantityService.AddQuantitiesAsync(request);
            return result.StatusCode == 200 ? Ok(result) : StatusCode(result.StatusCode, result);
        }

        [HttpPost("subtract")]
        public async Task<IActionResult> SubtractQuantities(
            [FromBody] BinaryQuantityRequest request
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(
                    QuantityResponse.ErrorResponse("Invalid request format", OperationType.Subtract)
                );

            var result = await _quantityService.SubtractQuantitiesAsync(request);
            return result.StatusCode == 200 ? Ok(result) : StatusCode(result.StatusCode, result);
        }

        [HttpPost("divide")]
        public async Task<IActionResult> DivideQuantities([FromBody] BinaryQuantityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(DivisionResponse.ErrorResponse("Invalid request format"));

            var result = await _quantityService.DivideQuantitiesAsync(request);
            return result.StatusCode == 200 ? Ok(result) : StatusCode(result.StatusCode, result);
        }

        [HttpPost("compare/batch")]
        public async Task<IActionResult> CompareQuantitiesBatch(
            [FromBody] BatchBinaryRequest request
        )
        {
            if (!ModelState.IsValid || request.Requests == null || request.Requests.Count == 0)
                return BadRequest(
                    new BatchResponse<QuantityResponse>
                    {
                        Success = false,
                        Message = "Invalid batch request format or empty list",
                        Timestamp = DateTime.UtcNow,
                    }
                );

            var result = await _quantityService.CompareQuantitiesBatchAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("convert/batch")]
        public async Task<IActionResult> ConvertQuantitiesBatch(
            [FromBody] BatchConversionRequest request
        )
        {
            if (!ModelState.IsValid || request.Requests == null || request.Requests.Count == 0)
                return BadRequest(
                    new BatchResponse<QuantityResponse>
                    {
                        Success = false,
                        Message = "Invalid batch request format or empty list",
                        Timestamp = DateTime.UtcNow,
                    }
                );

            var result = await _quantityService.ConvertQuantitiesBatchAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("add/batch")]
        public async Task<IActionResult> AddQuantitiesBatch([FromBody] BatchBinaryRequest request)
        {
            if (!ModelState.IsValid || request.Requests == null || request.Requests.Count == 0)
                return BadRequest(
                    new BatchResponse<QuantityResponse>
                    {
                        Success = false,
                        Message = "Invalid batch request format or empty list",
                        Timestamp = DateTime.UtcNow,
                    }
                );

            var result = await _quantityService.AddQuantitiesBatchAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("subtract/batch")]
        public async Task<IActionResult> SubtractQuantitiesBatch(
            [FromBody] BatchBinaryRequest request
        )
        {
            if (!ModelState.IsValid || request.Requests == null || request.Requests.Count == 0)
                return BadRequest(
                    new BatchResponse<QuantityResponse>
                    {
                        Success = false,
                        Message = "Invalid batch request format or empty list",
                        Timestamp = DateTime.UtcNow,
                    }
                );

            var result = await _quantityService.SubtractQuantitiesBatchAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("divide/batch")]
        public async Task<IActionResult> DivideQuantitiesBatch(
            [FromBody] BatchBinaryRequest request
        )
        {
            if (!ModelState.IsValid || request.Requests == null || request.Requests.Count == 0)
                return BadRequest(
                    new BatchResponse<DivisionResponse>
                    {
                        Success = false,
                        Message = "Invalid batch request format or empty list",
                        Timestamp = DateTime.UtcNow,
                    }
                );

            var result = await _quantityService.DivideQuantitiesBatchAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("cache/statistics")]
        public async Task<IActionResult> GetCacheStatistics()
        {
            try
            {
                var stats = await _quantityService.GetCacheStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache stats");
                return StatusCode(
                    500,
                    new { Success = false, Message = $"Error getting cache stats: {ex.Message}" }
                );
            }
        }

        [HttpPost("cache/clear")]
        public async Task<IActionResult> ClearCache()
        {
            try
            {
                var result = await _quantityService.ClearCacheAsync();
                return Ok(
                    new
                    {
                        Success = result,
                        Message = result ? "Cache cleared successfully" : "Failed to clear cache",
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cache");
                return StatusCode(
                    500,
                    new { Success = false, Message = $"Error clearing cache: {ex.Message}" }
                );
            }
        }
    }
}
