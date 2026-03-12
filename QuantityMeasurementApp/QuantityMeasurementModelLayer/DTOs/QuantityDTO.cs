using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModelLayer.DTOs
{
    /// <summary>
    /// Data Transfer Object for quantity input.
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
    /// Request DTO for binary operations (compare, add, subtract, divide).
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
}
