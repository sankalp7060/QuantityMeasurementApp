namespace QuantityMeasurementApp.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when an invalid unit is provided.
    /// </summary>
    public class InvalidUnitException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InvalidUnitException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidUnitException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the InvalidUnitException class with a specific unit.
        /// </summary>
        /// <param name="unit">The invalid unit that caused the exception.</param>
        public InvalidUnitException(object unit)
            : base($"Invalid unit: {unit}. Unit must be a valid measurement unit.") { }
    }
}
