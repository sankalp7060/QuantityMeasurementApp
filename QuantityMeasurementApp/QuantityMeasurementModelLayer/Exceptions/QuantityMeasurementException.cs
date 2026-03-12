namespace QuantityMeasurementBusinessLayer.Exceptions
{
    /// <summary>
    /// Custom exception for quantity measurement operations.
    /// </summary>
    public class QuantityMeasurementException : Exception
    {
        public QuantityMeasurementException()
            : base() { }

        public QuantityMeasurementException(string message)
            : base(message) { }

        public QuantityMeasurementException(string message, Exception innerException)
            : base(message, innerException) { }

        public string? OperationType { get; set; }
        public string? Category { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
