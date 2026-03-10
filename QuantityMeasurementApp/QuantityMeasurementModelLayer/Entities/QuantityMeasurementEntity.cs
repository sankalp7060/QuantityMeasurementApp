using QuantityMeasurementModelLayer.DTOs.Enums;

namespace QuantityMeasurementModelLayer.Entities
{
    /// <summary>
    /// Entity class for storing quantity measurement operations.
    /// </summary>
    [Serializable]
    public class QuantityMeasurementEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; }
        public OperationType OperationType { get; set; }

        // Binary operation fields
        public double? Quantity1Value { get; set; }
        public string? Quantity1Unit { get; set; }
        public string? Quantity1Category { get; set; }
        public double? Quantity2Value { get; set; }
        public string? Quantity2Unit { get; set; }
        public string? Quantity2Category { get; set; }
        public string? TargetUnit { get; set; }

        // Conversion operation fields
        public double? SourceValue { get; set; }
        public string? SourceUnit { get; set; }
        public string? SourceCategory { get; set; }

        // Result fields
        public double? ResultValue { get; set; }
        public string? ResultUnit { get; set; }
        public string? FormattedResult { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public static QuantityMeasurementEntity CreateBinaryOperation(
            OperationType operation,
            double q1Value,
            string q1Unit,
            string q1Category,
            double q2Value,
            string q2Unit,
            string q2Category,
            string? targetUnit,
            double? resultValue,
            string? resultUnit,
            string? formattedResult,
            bool isSuccess,
            string? errorMessage = null
        )
        {
            return new QuantityMeasurementEntity
            {
                Timestamp = DateTime.UtcNow,
                OperationType = operation,
                Quantity1Value = q1Value,
                Quantity1Unit = q1Unit,
                Quantity1Category = q1Category,
                Quantity2Value = q2Value,
                Quantity2Unit = q2Unit,
                Quantity2Category = q2Category,
                TargetUnit = targetUnit,
                ResultValue = resultValue,
                ResultUnit = resultUnit,
                FormattedResult = formattedResult,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage,
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
            bool isSuccess,
            string? errorMessage = null
        )
        {
            return new QuantityMeasurementEntity
            {
                Timestamp = DateTime.UtcNow,
                OperationType = OperationType.Convert,
                SourceValue = sourceValue,
                SourceUnit = sourceUnit,
                SourceCategory = sourceCategory,
                TargetUnit = targetUnit,
                ResultValue = resultValue,
                ResultUnit = resultUnit,
                FormattedResult = formattedResult,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage,
            };
        }
    }
}
