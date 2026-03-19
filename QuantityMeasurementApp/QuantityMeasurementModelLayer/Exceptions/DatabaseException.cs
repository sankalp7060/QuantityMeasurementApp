// Import System namespace for base Exception class
using System;

// Define the namespace for custom exceptions
namespace QuantityMeasurementModelLayer.Exceptions
{
    /// <summary>
    /// Custom exception for database-related errors
    /// Inherits from QuantityMeasurementException to maintain exception hierarchy
    /// This exception is thrown when database operations fail (connection issues, constraint violations, etc.)
    /// </summary>
    public class DatabaseException : QuantityMeasurementException
    {
        /// <summary>
        /// Default constructor - creates empty DatabaseException
        /// </summary>
        public DatabaseException()
            : base() { }

        /// <summary>
        /// Constructor with error message only
        /// </summary>
        /// <param name="message">Human-readable error description</param>
        public DatabaseException(string message)
            : base(message) { }

        /// <summary>
        /// Constructor with message and inner exception (for exception chaining)
        /// Used when catching a lower-level exception and wrapping it in a DatabaseException
        /// </summary>
        /// <param name="message">Human-readable error description</param>
        /// <param name="innerException">The original exception that caused this error</param>
        public DatabaseException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Database-specific error code (e.g., SQL Server error number)
        /// Nullable int because not all database errors have codes
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Name of the database constraint that was violated (if applicable)
        /// Useful for debugging foreign key, unique constraint, or check constraint violations
        /// </summary>
        public string? ConstraintName { get; set; }
    }
}
