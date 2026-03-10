using System.ComponentModel.DataAnnotations;
using QuantityMeasurementModelLayer.DTOs.Enums;

namespace QuantityMeasurementModelLayer.DTOs
{
    /// <summary>
    /// Request DTO for quantity operations.
    /// Represents a single quantity with value and unit.
    /// </summary>
    public class QuantityDTO
    {
        [Required(ErrorMessage = "Value is required")]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Value must be a valid number")]
        public double Value { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for binary operations.
    /// </summary>
    public class BinaryQuantityRequest
    {
        [Required]
        public QuantityDTO Quantity1 { get; set; } = new();

        [Required]
        public QuantityDTO Quantity2 { get; set; } = new();

        public string? TargetUnit { get; set; }
    }

    /// <summary>
    /// Request DTO for conversion operations.
    /// </summary>
    public class ConversionRequest
    {
        [Required]
        public QuantityDTO Source { get; set; } = new();

        [Required]
        public string TargetUnit { get; set; } = string.Empty;
    }

    /// <summary>
    /// Batch request DTO for multiple binary operations.
    /// </summary>
    public class BatchBinaryRequest
    {
        [Required]
        public List<BinaryQuantityRequest> Requests { get; set; } = new();
    }

    /// <summary>
    /// Batch request DTO for multiple conversion operations.
    /// </summary>
    public class BatchConversionRequest
    {
        [Required]
        public List<ConversionRequest> Requests { get; set; } = new();
    }
}
