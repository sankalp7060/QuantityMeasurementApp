using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurement.MeasurementService.Services;
using QuantityMeasurement.Shared.DTOs;

namespace QuantityMeasurement.MeasurementService.Controllers;

[ApiController]
[Route("api/v1/Quantities")]
[Authorize] // Add this to require authentication
public class QuantitiesController : ControllerBase
{
    private readonly IMeasurementService _measurementService;
    private readonly ILogger<QuantitiesController> _logger;

    public QuantitiesController(
        IMeasurementService measurementService,
        ILogger<QuantitiesController> logger
    )
    {
        _measurementService = measurementService;
        _logger = logger;
    }

    [HttpPost("convert")]
    public async Task<IActionResult> Convert([FromBody] ConversionRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _measurementService.ConvertAsync(request);
        return Ok(result);
    }

    [HttpPost("compare")]
    public async Task<IActionResult> Compare([FromBody] BinaryOperationRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _measurementService.CompareAsync(request);
        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] BinaryOperationRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _measurementService.AddAsync(request);
        return Ok(result);
    }

    [HttpPost("subtract")]
    public async Task<IActionResult> Subtract([FromBody] BinaryOperationRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _measurementService.SubtractAsync(request);
        return Ok(result);
    }

    [HttpPost("divide")]
    public async Task<IActionResult> Divide([FromBody] BinaryOperationRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _measurementService.DivideAsync(request);
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] string? operation = null)
    {
        var history = await _measurementService.GetHistoryAsync(operation);
        return Ok(history);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = await _measurementService.GetStatisticsAsync();
        return Ok(stats);
    }

    [HttpGet("count/{operation}")]
    public async Task<IActionResult> GetOperationCount(string operation)
    {
        var count = await _measurementService.GetOperationCountAsync(operation);
        return Ok(new { Operation = operation, Count = count });
    }
}
