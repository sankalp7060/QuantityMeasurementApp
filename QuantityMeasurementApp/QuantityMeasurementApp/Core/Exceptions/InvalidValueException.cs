namespace QuantityMeasurementApp.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when an invalid numeric value is provided.
    /// </summary>
    public class InvalidValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InvalidValueException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidValueException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the InvalidValueException class with the invalid value.
        /// </summary>
        /// <param name="value">The invalid value that caused the exception.</param>
        public InvalidValueException(double value)
            : base($"Invalid value: {value}. Value must be a finite number.") { }
    }
}
