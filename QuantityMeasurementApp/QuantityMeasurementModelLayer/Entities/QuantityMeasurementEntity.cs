// Import required namespaces for data annotations (attributes that control database mapping)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuantityMeasurementModelLayer.DTOs.Enums;

// Define the namespace for entity classes (database models)
namespace QuantityMeasurementModelLayer.Entities
{
    /// <summary>
    /// Entity class for quantity measurement operations
    /// Maps to database table using EF Core
    /// This class represents a record in the QuantityMeasurements database table
    /// It stores all details about a measurement operation for history and auditing
    /// </summary>
    [Table("QuantityMeasurements")] // Specifies the database table name
    public class QuantityMeasurementEntity
    {
        /// <summary>
        /// Unique identifier for the measurement (Primary Key)
        /// Maps to Id column in database (IDENTITY column)
        /// </summary>
        [Key] // Marks this property as the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Database generates value on insert (auto-increment)
        [Column("Id")] // Explicitly map to Id column in database
        public long Id { get; set; } // Using long (Int64) for large number of records

        /// <summary>
        /// Unique identifier for tracking (Business key)
        /// Maps to MeasurementId column in database (NVARCHAR(50))
        /// </summary>
        [Required] // NOT NULL in database
        [MaxLength(50)] // Maximum 50 characters in database
        [Column("MeasurementId")] // Explicitly map to MeasurementId column
        public string MeasurementId { get; set; } = Guid.NewGuid().ToString(); // Generate new GUID by default

        /// <summary>
        /// When the measurement was created
        /// Timestamp for when the operation was performed
        /// </summary>
        [Required] // NOT NULL in database
        [Column("CreatedAt")] // Explicitly map to CreatedAt column
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current UTC time

        /// <summary>
        /// Type of operation performed
        /// Enum stored as int in database (Compare=0, Convert=1, Add=2, Subtract=3, Divide=4)
        /// </summary>
        [Required] // NOT NULL in database
        [Column("OperationType")] // Explicitly map to OperationType column
        public OperationType OperationType { get; set; }

        // ==================== BINARY OPERATION FIELDS ====================
        // These fields are used for operations with two operands (Compare, Add, Subtract, Divide)

        /// <summary>
        /// First operand value
        /// </summary>
        [Column("FirstOperandValue")] // Explicitly map to FirstOperandValue column
        public double? FirstOperandValue { get; set; } // double? allows null values

        /// <summary>
        /// First operand unit
        /// </summary>
        [MaxLength(20)] // Limit string length in database
        [Column("FirstOperandUnit")] // Explicitly map to FirstOperandUnit column
        public string? FirstOperandUnit { get; set; } // string? allows null

        /// <summary>
        /// First operand category
        /// </summary>
        [MaxLength(20)]
        [Column("FirstOperandCategory")] // Explicitly map to FirstOperandCategory column
        public string? FirstOperandCategory { get; set; }

        /// <summary>
        /// Second operand value
        /// </summary>
        [Column("SecondOperandValue")] // Explicitly map to SecondOperandValue column
        public double? SecondOperandValue { get; set; }

        /// <summary>
        /// Second operand unit
        /// </summary>
        [MaxLength(20)]
        [Column("SecondOperandUnit")] // Explicitly map to SecondOperandUnit column
        public string? SecondOperandUnit { get; set; }

        /// <summary>
        /// Second operand category
        /// </summary>
        [MaxLength(20)]
        [Column("SecondOperandCategory")] // Explicitly map to SecondOperandCategory column
        public string? SecondOperandCategory { get; set; }

        /// <summary>
        /// Target unit for result
        /// Used when client requests result in specific unit
        /// </summary>
        [MaxLength(20)]
        [Column("TargetUnit")] // Explicitly map to TargetUnit column
        public string? TargetUnit { get; set; }

        // ==================== CONVERSION OPERATION FIELDS ====================
        // These fields are used specifically for conversion operations

        /// <summary>
        /// Source operand value (for conversion)
        /// The value to convert from
        /// </summary>
        [Column("SourceOperandValue")] // Explicitly map to SourceOperandValue column
        public double? SourceOperandValue { get; set; }

        /// <summary>
        /// Source operand unit
        /// The unit we're converting from
        /// </summary>
        [MaxLength(20)]
        [Column("SourceOperandUnit")] // Explicitly map to SourceOperandUnit column
        public string? SourceOperandUnit { get; set; }

        /// <summary>
        /// Source operand category
        /// The category of the source unit
        /// </summary>
        [MaxLength(20)]
        [Column("SourceOperandCategory")] // Explicitly map to SourceOperandCategory column
        public string? SourceOperandCategory { get; set; }

        // ==================== RESULT FIELDS ====================
        // Common fields for all operation types

        /// <summary>
        /// Result value
        /// The numeric result of the operation
        /// </summary>
        [Column("ResultValue")] // Explicitly map to ResultValue column
        public double? ResultValue { get; set; }

        /// <summary>
        /// Result unit
        /// The unit of the result
        /// </summary>
        [MaxLength(20)]
        [Column("ResultUnit")] // Explicitly map to ResultUnit column
        public string? ResultUnit { get; set; }

        /// <summary>
        /// Formatted result for display
        /// Human-readable string representation (e.g., "5 feet = 60 inches")
        /// </summary>
        [MaxLength(200)]
        [Column("FormattedResult")] // Explicitly map to FormattedResult column
        public string? FormattedResult { get; set; }

        /// <summary>
        /// Whether operation was successful
        /// True for successful operations, False for failed ones
        /// </summary>
        [Required] // NOT NULL in database
        [Column("IsSuccessful")] // Explicitly map to IsSuccessful column
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Error details if operation failed
        /// Stores exception message or validation error for failed operations
        /// </summary>
        [Column("ErrorDetails")] // Explicitly map to ErrorDetails column
        public string? ErrorDetails { get; set; }

        /// <summary>
        /// Factory method to create a binary operation entity
        /// </summary>
        public static QuantityMeasurementEntity CreateBinaryOperation(
            OperationType operation,
            double firstValue,
            string firstUnit,
            string firstCategory,
            double secondValue,
            string secondUnit,
            string secondCategory,
            string? targetUnit,
            double? resultValue,
            string? resultUnit,
            string? formattedResult,
            bool isSuccessful,
            string? errorDetails = null
        )
        {
            return new QuantityMeasurementEntity
            {
                MeasurementId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                OperationType = operation,
                FirstOperandValue = firstValue,
                FirstOperandUnit = firstUnit,
                FirstOperandCategory = firstCategory,
                SecondOperandValue = secondValue,
                SecondOperandUnit = secondUnit,
                SecondOperandCategory = secondCategory,
                TargetUnit = targetUnit,
                ResultValue = resultValue,
                ResultUnit = resultUnit,
                FormattedResult = formattedResult,
                IsSuccessful = isSuccessful,
                ErrorDetails = errorDetails,
            };
        }

        /// <summary>
        /// Factory method to create a conversion operation entity
        /// </summary>
        public static QuantityMeasurementEntity CreateConversion(
            double sourceValue,
            string sourceUnit,
            string sourceCategory,
            string targetUnit,
            double? resultValue,
            string? resultUnit,
            string? formattedResult,
            bool isSuccessful,
            string? errorDetails = null
        )
        {
            return new QuantityMeasurementEntity
            {
                MeasurementId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                OperationType = OperationType.Convert,
                SourceOperandValue = sourceValue,
                SourceOperandUnit = sourceUnit,
                SourceOperandCategory = sourceCategory,
                TargetUnit = targetUnit,
                ResultValue = resultValue,
                ResultUnit = resultUnit,
                FormattedResult = formattedResult,
                IsSuccessful = isSuccessful,
                ErrorDetails = errorDetails,
            };
        }
    }
}
