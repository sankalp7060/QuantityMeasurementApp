using System.Text.Json;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Configuration;
using QuantityMeasurementRepositoryLayer.Interface;
using StackExchange.Redis;

namespace QuantityMeasurementRepositoryLayer.Services
{
    /// <summary>
    /// Redis implementation of the cache repository.
    /// </summary>
    public class RedisCacheRepository : IRedisCacheRepository
    {
        private readonly IConnectionMultiplexer? _redisConnection;
        private readonly IDatabase? _redisDatabase;
        private readonly RedisConfiguration _configuration;
        private readonly ILogger<RedisCacheRepository> _logger;
        private readonly string _entityKeyPrefix = "entity:";

        public RedisCacheRepository(
            RedisConfiguration configuration,
            ILogger<RedisCacheRepository> logger
        )
        {
            _configuration = configuration;
            _logger = logger;

            try
            {
                if (_configuration.Enabled)
                {
                    _redisConnection = ConnectionMultiplexer.Connect(
                        _configuration.ConnectionString
                    );
                    _redisDatabase = _redisConnection.GetDatabase();
                    _logger.LogInformation("Redis cache initialized successfully");
                }
                else
                {
                    _redisConnection = null;
                    _redisDatabase = null;
                    _logger.LogWarning("Redis cache is disabled");
                }
            }
            catch (Exception ex)
            {
                _redisConnection = null;
                _redisDatabase = null;
                _logger.LogError(ex, "Failed to initialize Redis cache");
            }
        }

        private string GetEntityKey(string id) =>
            $"{_configuration.InstanceName}{_entityKeyPrefix}{id}";

        private string SerializeEntity(QuantityMeasurementEntity entity)
        {
            return JsonSerializer.Serialize(
                entity,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                }
            );
        }

        private QuantityMeasurementEntity? DeserializeEntity(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<QuantityMeasurementEntity>(
                    json,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize entity");
                return null;
            }
        }

        private IServer? GetServer()
        {
            if (_redisConnection == null)
                return null;
            var endpoints = _redisConnection.GetEndPoints();
            return endpoints.Length > 0 ? _redisConnection.GetServer(endpoints.First()) : null;
        }

        public async Task<QuantityMeasurementEntity?> GetByIdAsync(string id)
        {
            try
            {
                if (!_configuration.Enabled || _redisDatabase == null)
                {
                    return null;
                }

                var key = GetEntityKey(id);
                var value = await _redisDatabase.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                {
                    _logger.LogDebug("Entity with ID {Id} not found in Redis cache", id);
                    return null;
                }

                var entity = DeserializeEntity(value.ToString());
                _logger.LogDebug("Retrieved entity with ID {Id} from Redis cache", id);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity with ID {Id} from Redis cache", id);
                return null;
            }
        }

        public async Task SaveAsync(QuantityMeasurementEntity entity, int? expirationMinutes = null)
        {
            try
            {
                if (!_configuration.Enabled || _redisDatabase == null)
                {
                    _logger.LogWarning("Redis cache is disabled or not initialized");
                    return;
                }

                var key = GetEntityKey(entity.Id);
                var serialized = SerializeEntity(entity);
                var expiry = expirationMinutes.HasValue
                    ? TimeSpan.FromMinutes(expirationMinutes.Value)
                    : TimeSpan.FromMinutes(_configuration.DefaultExpirationMinutes);

                await _redisDatabase.StringSetAsync(key, serialized, expiry);

                _logger.LogDebug(
                    "Saved entity with ID {Id} to Redis cache with expiry {Expiry} minutes",
                    entity.Id,
                    expiry.TotalMinutes
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving entity with ID {Id} to Redis cache", entity.Id);
            }
        }

        public async Task<List<QuantityMeasurementEntity>> GetAllAsync()
        {
            var entities = new List<QuantityMeasurementEntity>();

            try
            {
                if (!_configuration.Enabled || _redisDatabase == null || _redisConnection == null)
                {
                    _logger.LogWarning("Redis cache is disabled or not initialized");
                    return entities;
                }

                var server = GetServer();
                if (server == null)
                {
                    _logger.LogWarning("Could not get Redis server");
                    return entities;
                }

                var pattern = $"{_configuration.InstanceName}{_entityKeyPrefix}*";
                var keys = server.Keys(pattern: pattern).ToArray();

                _logger.LogDebug(
                    "Found {Count} keys in Redis matching pattern {Pattern}",
                    keys.Length,
                    pattern
                );

                foreach (var key in keys)
                {
                    var value = await _redisDatabase.StringGetAsync(key);
                    if (!value.IsNullOrEmpty)
                    {
                        var entity = DeserializeEntity(value.ToString());
                        if (entity != null)
                        {
                            entities.Add(entity);
                        }
                    }
                }

                _logger.LogDebug("Retrieved {Count} entities from Redis cache", entities.Count);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all entities from Redis cache");
                return entities;
            }
        }

        public async Task RemoveAsync(string id)
        {
            try
            {
                if (!_configuration.Enabled || _redisDatabase == null)
                {
                    _logger.LogWarning("Redis cache is disabled or not initialized");
                    return;
                }

                var key = GetEntityKey(id);
                await _redisDatabase.KeyDeleteAsync(key);

                _logger.LogDebug("Removed entity with ID {Id} from Redis cache", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing entity with ID {Id} from Redis cache", id);
            }
        }

        public async Task ClearAllAsync()
        {
            try
            {
                if (!_configuration.Enabled || _redisDatabase == null || _redisConnection == null)
                {
                    _logger.LogWarning("Redis cache is disabled or not initialized");
                    return;
                }

                var server = GetServer();
                if (server == null)
                {
                    _logger.LogWarning("Could not get Redis server");
                    return;
                }

                var pattern = $"{_configuration.InstanceName}{_entityKeyPrefix}*";
                var keys = server.Keys(pattern: pattern).ToArray();

                foreach (var key in keys)
                {
                    await _redisDatabase.KeyDeleteAsync(key);
                }

                _logger.LogInformation("Cleared all entities from Redis cache");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing Redis cache");
            }
        }

        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                if (!_configuration.Enabled || _redisConnection == null)
                    return false;

                return await Task.Run(() => _redisConnection.IsConnected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Redis connection");
                return false;
            }
        }
    }
}
