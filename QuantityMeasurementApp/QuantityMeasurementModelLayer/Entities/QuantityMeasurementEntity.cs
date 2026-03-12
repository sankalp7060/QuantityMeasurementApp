using System.Text.Json.Serialization;
using QuantityMeasurementModelLayer.DTOs.Enums;

namespace QuantityMeasurementModelLayer.Entities
{
    /// <summary>
    /// Entity class for storing quantity measurement operations.
    /// Properties follow proper naming conventions (no numbers).
    /// </summary>
    [Serializable]
    public class QuantityMeasurementEntity
    {
        public string MeasurementId { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public OperationType OperationType { get; set; }

        // Binary operation fields - proper names (no numbers)
        public double? FirstOperandValue { get; set; }
        public string? FirstOperandUnit { get; set; }
        public string? FirstOperandCategory { get; set; }
        public double? SecondOperandValue { get; set; }
        public string? SecondOperandUnit { get; set; }
        public string? SecondOperandCategory { get; set; }
        public string? TargetUnit { get; set; }

        // Conversion operation fields
        public double? SourceOperandValue { get; set; }
        public string? SourceOperandUnit { get; set; }
        public string? SourceOperandCategory { get; set; }

        // Result fields
        public double? ResultValue { get; set; }
        public string? ResultUnit { get; set; }
        public string? FormattedResult { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorDetails { get; set; }

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
                CreatedAt = DateTime.Now,
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
                CreatedAt = DateTime.Now,
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

        // Helper method to map from database reader
        public static QuantityMeasurementEntity FromDataReader(Dictionary<string, object> row)
        {
            return new QuantityMeasurementEntity
            {
                MeasurementId = row["MeasurementId"].ToString() ?? Guid.NewGuid().ToString(),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                OperationType = (OperationType)Convert.ToInt32(row["OperationType"]),
                FirstOperandValue = row["FirstOperandValue"] as double?,
                FirstOperandUnit = row["FirstOperandUnit"]?.ToString(),
                FirstOperandCategory = row["FirstOperandCategory"]?.ToString(),
                SecondOperandValue = row["SecondOperandValue"] as double?,
                SecondOperandUnit = row["SecondOperandUnit"]?.ToString(),
                SecondOperandCategory = row["SecondOperandCategory"]?.ToString(),
                TargetUnit = row["TargetUnit"]?.ToString(),
                SourceOperandValue = row["SourceOperandValue"] as double?,
                SourceOperandUnit = row["SourceOperandUnit"]?.ToString(),
                SourceOperandCategory = row["SourceOperandCategory"]?.ToString(),
                ResultValue = row["ResultValue"] as double?,
                ResultUnit = row["ResultUnit"]?.ToString(),
                FormattedResult = row["FormattedResult"]?.ToString(),
                IsSuccessful = Convert.ToBoolean(row["IsSuccessful"]),
                ErrorDetails = row["ErrorDetails"]?.ToString(),
            };
        }
    }
}
