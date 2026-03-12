using System;

namespace QuantityMeasurementModelLayer.Exceptions
{
    /// <summary>
    /// Custom exception for database-related errors in UC16.
    /// Located in Model Layer for sharing across all layers.
    /// </summary>
    public class DatabaseException : Exception
    {
        public DatabaseException()
            : base() { }

        public DatabaseException(string message)
            : base(message) { }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException) { }

        public string? SqlState { get; set; }
        public int? ErrorCode { get; set; }
        public string? ConstraintName { get; set; }
        public string? DatabaseName { get; set; }
        public string? ServerName { get; set; }

        public override string ToString()
        {
            var details = $"Database Error: {Message}";
            if (ErrorCode.HasValue)
                details += $"\nError Code: {ErrorCode}";
            if (!string.IsNullOrEmpty(SqlState))
                details += $"\nSQL State: {SqlState}";
            if (!string.IsNullOrEmpty(ConstraintName))
                details += $"\nConstraint: {ConstraintName}";
            if (!string.IsNullOrEmpty(DatabaseName))
                details += $"\nDatabase: {DatabaseName}";
            if (!string.IsNullOrEmpty(ServerName))
                details += $"\nServer: {ServerName}";
            return details;
        }
    }
}
