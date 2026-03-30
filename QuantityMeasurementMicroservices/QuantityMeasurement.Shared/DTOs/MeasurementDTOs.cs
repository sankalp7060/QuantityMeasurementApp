using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurement.Shared.DTOs;

public class QuantityRequestDto
{
    [Required]
    [Range(double.Epsilon, double.MaxValue)]
    public double Value { get; set; }

    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(LENGTH|WEIGHT|VOLUME|TEMPERATURE)$")]
    public string Category { get; set; } = string.Empty;
}

public class BinaryOperationRequestDto
{
    [Required]
    public QuantityRequestDto FirstQuantity { get; set; } = new();

    [Required]
    public QuantityRequestDto SecondQuantity { get; set; } = new();

    [MaxLength(20)]
    public string? TargetUnit { get; set; }
}

public class ConversionRequestDto
{
    [Required]
    public QuantityRequestDto Source { get; set; } = new();

    [Required]
    [MaxLength(20)]
    public string TargetUnit { get; set; } = string.Empty;
}

public class QuantityResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public double? Result { get; set; }
    public string? ResultUnit { get; set; }
    public string? FormattedResult { get; set; }
    public string Operation { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class DivisionResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public double? Ratio { get; set; }
    public string? Interpretation { get; set; }
    public string Operation { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}