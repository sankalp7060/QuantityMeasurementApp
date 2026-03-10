namespace QuantityMeasurementBusinessLayer.Configuration
{
    /// <summary>
    /// Configuration settings for Redis cache.
    /// </summary>
    public class RedisConfiguration
    {
        /// <summary>
        /// Redis connection string.
        /// </summary>
        public string ConnectionString { get; set; } = "localhost:6379";

        /// <summary>
        /// Default cache expiration time in minutes.
        /// </summary>
        public int DefaultExpirationMinutes { get; set; } = 60;

        /// <summary>
        /// Instance name prefix for Redis keys.
        /// </summary>
        public string InstanceName { get; set; } = "QuantityMeasurement:";

        /// <summary>
        /// Whether to enable Redis caching.
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}
