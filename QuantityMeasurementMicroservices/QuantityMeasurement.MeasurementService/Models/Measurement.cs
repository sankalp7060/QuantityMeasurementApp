namespace QuantityMeasurement.MeasurementService.Models;

public class Measurement
{
    public long Id { get; set; }
    public string MeasurementId { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int OperationType { get; set; }

    public double? FirstOperandValue { get; set; }
    public string? FirstOperandUnit { get; set; }
    public string? FirstOperandCategory { get; set; }

    public double? SecondOperandValue { get; set; }
    public string? SecondOperandUnit { get; set; }
    public string? SecondOperandCategory { get; set; }

    public string? TargetUnit { get; set; }

    public double? SourceOperandValue { get; set; }
    public string? SourceOperandUnit { get; set; }
    public string? SourceOperandCategory { get; set; }

    public double? ResultValue { get; set; }
    public string? ResultUnit { get; set; }
    public string? FormattedResult { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ErrorDetails { get; set; }
}
