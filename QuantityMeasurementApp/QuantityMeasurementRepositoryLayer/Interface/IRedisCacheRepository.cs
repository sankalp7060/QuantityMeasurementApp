using QuantityMeasurementModelLayer.Entities;

namespace QuantityMeasurementRepositoryLayer.Interface
{
    /// <summary>
    /// Interface for Redis cache repository operations.
    /// </summary>
    public interface IRedisCacheRepository
    {
        /// <summary>
        /// Gets an entity by ID from Redis cache.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>The entity if found, null otherwise.</returns>
        Task<QuantityMeasurementEntity?> GetByIdAsync(string id);

        /// <summary>
        /// Saves an entity to Redis cache.
        /// </summary>
        /// <param name="entity">The entity to save.</param>
        /// <param name="expirationMinutes">Optional expiration time in minutes.</param>
        Task SaveAsync(QuantityMeasurementEntity entity, int? expirationMinutes = null);

        /// <summary>
        /// Gets all entities from Redis cache.
        /// </summary>
        /// <returns>List of all entities.</returns>
        Task<List<QuantityMeasurementEntity>> GetAllAsync();

        /// <summary>
        /// Removes an entity from Redis cache.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        Task RemoveAsync(string id);

        /// <summary>
        /// Clears all entities from Redis cache.
        /// </summary>
        Task ClearAllAsync();

        /// <summary>
        /// Checks if Redis is connected.
        /// </summary>
        /// <returns>True if connected, false otherwise.</returns>
        Task<bool> IsConnectedAsync();
    }
}
