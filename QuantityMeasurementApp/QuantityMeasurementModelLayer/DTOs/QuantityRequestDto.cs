// Import DataAnnotations for validation attributes
using System.ComponentModel.DataAnnotations;

// Define the namespace for Data Transfer Objects
namespace QuantityMeasurementModelLayer.DTOs
{
    /// <summary>
    /// DTO for quantity input in API requests
    /// Represents a single quantity with its value, unit, and category
    /// Used as a building block for more complex request DTOs
    /// </summary>
    public class QuantityRequestDto
    {
        /// <summary>
        /// The numeric value of the measurement
        /// Must be greater than 0 (no negative or zero measurements)
        /// </summary>
        [Required(ErrorMessage = "Value is required")] // Validation: field must be present
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Value must be greater than 0")] // Validation: must be positive
        public double Value { get; set; }

        /// <summary>
        /// The unit of measurement (e.g., FEET, KILOGRAM, LITRE, CELSIUS)
        /// Must match predefined unit names in the system
        /// </summary>
        [Required(ErrorMessage = "Unit is required")]
        [MaxLength(20, ErrorMessage = "Unit cannot exceed 20 characters")] // Prevent excessively long strings
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// The measurement category (Length, Weight, Volume, Temperature)
        /// Must be one of the four allowed categories
        /// </summary>
        [Required(ErrorMessage = "Category is required")]
        [RegularExpression(
            "^(LENGTH|WEIGHT|VOLUME|TEMPERATURE)$", // Regular expression validation
            ErrorMessage = "Category must be LENGTH, WEIGHT, VOLUME, or TEMPERATURE"
        )]
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for binary operations (compare, add, subtract, divide)
    /// Contains two quantities to operate on and an optional target unit
    /// </summary>
    public class BinaryOperationRequestDto
    {
        /// <summary>
        /// First quantity
        /// Required - must be provided
        /// </summary>
        [Required] // Ensures this property is not null
        public QuantityRequestDto FirstQuantity { get; set; } = new(); // Initialize with new instance

        /// <summary>
        /// Second quantity
        /// Required - must be provided
        /// </summary>
        [Required]
        public QuantityRequestDto SecondQuantity { get; set; } = new();

        /// <summary>
        /// Optional target unit for result
        /// If provided, result will be converted to this unit
        /// If not provided, result uses appropriate default unit
        /// </summary>
        [MaxLength(20)]
        public string? TargetUnit { get; set; }
    }

    /// <summary>
    /// DTO for conversion operations
    /// Contains source quantity and target unit
    /// </summary>
    public class ConversionRequestDto
    {
        /// <summary>
        /// Source quantity to convert
        /// Contains value, unit, and category
        /// </summary>
        [Required]
        public QuantityRequestDto Source { get; set; } = new();

        /// <summary>
        /// Target unit for conversion
        /// The unit to convert the source value to
        /// </summary>
        [Required(ErrorMessage = "Target unit is required")]
        [MaxLength(20, ErrorMessage = "Target unit cannot exceed 20 characters")]
        public string TargetUnit { get; set; } = string.Empty;
    }
}
