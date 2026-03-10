namespace QuantityMeasurementRepositoryLayer.Configuration
{
    /// <summary>
    /// Configuration settings for Redis cache.
    /// </summary>
    public class RedisConfiguration
    {
        public string ConnectionString { get; set; } = "localhost:6379";
        public int DefaultExpirationMinutes { get; set; } = 60;
        public string InstanceName { get; set; } = "QuantityMeasurement:";
        public bool Enabled { get; set; } = true;
    }
}
