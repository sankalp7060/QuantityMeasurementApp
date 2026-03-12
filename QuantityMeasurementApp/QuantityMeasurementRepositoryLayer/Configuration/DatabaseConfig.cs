using System;

namespace QuantityMeasurementRepositoryLayer.Configuration
{
    public class DatabaseConfig
    {
        private readonly string? _connectionString;

        public DatabaseConfig()
        {
            try
            {
                // Try environment variable first
                _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            }
            catch
            {
                // Ignore errors
            }

            // Default to your SQL Express connection
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString =
                    @"Server=localhost\SQLEXPRESS;Database=QuantityMeasurementDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            }
        }

        public string? ConnectionString => _connectionString;

        public int CommandTimeout => 30;
        public int MaxRetryCount => 3;
        public bool EnableDetailedErrors => false; // Set to false to hide detailed errors
    }
}
