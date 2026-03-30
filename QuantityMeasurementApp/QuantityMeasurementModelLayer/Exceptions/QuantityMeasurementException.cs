// Import System namespace for base Exception class
using System;

// Define the namespace for custom exceptions
namespace QuantityMeasurementModelLayer.Exceptions
{
    /// <summary>
    /// Custom exception for quantity measurement operations
    /// Base exception class for all measurement-related errors in the application
    /// Inherits from standard Exception class to add measurement-specific properties
    /// </summary>
    public class QuantityMeasurementException : Exception
    {
        /// <summary>
        /// Default constructor - creates empty QuantityMeasurementException
        /// </summary>
        public QuantityMeasurementException()
            : base() { }

        /// <summary>
        /// Constructor with error message only
        /// </summary>
        /// <param name="message">Human-readable error description</param>
        public QuantityMeasurementException(string message)
            : base(message) { }

        /// <summary>
        /// Constructor with message and inner exception (for exception chaining)
        /// Used when catching a lower-level exception and wrapping it
        /// </summary>
        /// <param name="message">Human-readable error description</param>
        /// <param name="innerException">The original exception that caused this error</param>
        public QuantityMeasurementException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// The type of operation being performed when the error occurred
        /// (Compare, Convert, Add, Subtract, Divide)
        /// Helps with debugging and logging specific operation failures
        /// </summary>
        public string? OperationType { get; set; }

        /// <summary>
        /// The measurement category involved in the error
        /// (LENGTH, WEIGHT, VOLUME, TEMPERATURE)
        /// Helps identify which part of the system failed
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// When the exception occurred (UTC)
        /// Automatically set to current UTC time
        /// Useful for tracking error timing without relying on log timestamps
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
