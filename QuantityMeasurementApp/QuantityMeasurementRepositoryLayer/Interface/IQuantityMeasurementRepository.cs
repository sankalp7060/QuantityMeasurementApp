// Import required namespaces for DTO enums and entities
using QuantityMeasurementModelLayer.DTOs.Enums;
using QuantityMeasurementModelLayer.Entities;

// Define the namespace for repository interfaces
namespace QuantityMeasurementRepositoryLayer.Interface
{
    /// <summary>
    /// Repository interface for quantity measurement data access
    /// This interface defines the contract that any repository implementation must follow
    /// It abstracts the data access layer, making the application database-agnostic
    /// </summary>
    public interface IQuantityMeasurementRepository
    {
        // ==================== BASIC CRUD OPERATIONS ====================
        // These methods provide fundamental database operations

        /// <summary>
        /// Gets a single entity by its numeric ID
        /// </summary>
        /// <param name="id">The database ID of the entity</param>
        /// <returns>The entity if found, null otherwise</returns>
        Task<QuantityMeasurementEntity?> GetByIdAsync(long id);

        /// <summary>
        /// Gets a single entity by its business key (MeasurementId)
        /// </summary>
        /// <param name="measurementId">The unique measurement identifier string</param>
        /// <returns>The entity if found, null otherwise</returns>
        Task<QuantityMeasurementEntity?> GetByMeasurementIdAsync(string measurementId);

        /// <summary>
        /// Gets all entities from the database
        /// </summary>
        /// <returns>Collection of all entities, ordered by creation date (newest first)</returns>
        Task<IEnumerable<QuantityMeasurementEntity>> GetAllAsync();

        /// <summary>
        /// Adds a new entity to the database
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The added entity with generated ID and timestamps</returns>
        Task<QuantityMeasurementEntity> AddAsync(QuantityMeasurementEntity entity);

        /// <summary>
        /// Updates an existing entity in the database
        /// </summary>
        /// <param name="entity">The entity with updated values</param>
        /// <returns>Task representing the async operation</returns>
        Task UpdateAsync(QuantityMeasurementEntity entity);

        /// <summary>
        /// Deletes an entity by its ID
        /// </summary>
        /// <param name="id">The ID of the entity to delete</param>
        /// <returns>Task representing the async operation</returns>
        Task DeleteAsync(long id);

        /// <summary>
        /// Checks if an entity with the given ID exists
        /// </summary>
        /// <param name="id">The ID to check</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsAsync(long id);

        // ==================== QUERY OPERATIONS ====================
        // Specialized query methods for filtering data

        /// <summary>
        /// Gets all entities of a specific operation type
        /// </summary>
        /// <param name="operation">The operation type to filter by (Compare, Convert, Add, etc.)</param>
        /// <returns>Collection of entities matching the operation type</returns>
        Task<IEnumerable<QuantityMeasurementEntity>> GetByOperationAsync(OperationType operation);

        /// <summary>
        /// Gets all entities for a specific measurement category
        /// </summary>
        /// <param name="category">Category to filter by (LENGTH, WEIGHT, VOLUME, TEMPERATURE)</param>
        /// <returns>Collection of entities in the specified category</returns>
        Task<IEnumerable<QuantityMeasurementEntity>> GetByCategoryAsync(string category);

        /// <summary>
        /// Gets all entities created within a specific date range
        /// </summary>
        /// <param name="start">Start date (inclusive)</param>
        /// <param name="end">End date (inclusive)</param>
        /// <returns>Collection of entities created in the date range</returns>
        Task<IEnumerable<QuantityMeasurementEntity>> GetByDateRangeAsync(
            DateTime start,
            DateTime end
        );

        /// <summary>
        /// Gets all successful operations
        /// </summary>
        /// <returns>Collection of entities where IsSuccessful = true</returns>
        Task<IEnumerable<QuantityMeasurementEntity>> GetSuccessfulOperationsAsync();

        /// <summary>
        /// Gets all failed operations
        /// </summary>
        /// <returns>Collection of entities where IsSuccessful = false</returns>
        Task<IEnumerable<QuantityMeasurementEntity>> GetFailedOperationsAsync();

        // ==================== AGGREGATION OPERATIONS ====================
        // Methods that return statistical data

        /// <summary>
        /// Gets total count of all records in the database
        /// </summary>
        /// <returns>Total number of records</returns>
        Task<int> GetTotalCountAsync();

        /// <summary>
        /// Gets count of records for a specific operation type
        /// </summary>
        /// <param name="operation">The operation type to count</param>
        /// <returns>Number of records with this operation type</returns>
        Task<int> GetCountByOperationAsync(OperationType operation);

        /// <summary>
        /// Gets counts for all operation types
        /// </summary>
        /// <returns>Dictionary mapping operation types to their counts</returns>
        Task<Dictionary<OperationType, int>> GetOperationCountsAsync();

        // ==================== PAGINATION SUPPORT ====================
        /// <summary>
        /// Gets a page of results for large datasets
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>A page of entities</returns>
        Task<IEnumerable<QuantityMeasurementEntity>> GetPagedAsync(int page, int pageSize);

        // ==================== SAVE CHANGES ====================
        /// <summary>
        /// Explicitly saves all pending changes to the database
        /// Useful for batch operations where multiple changes need to be saved together
        /// </summary>
        /// <returns>Number of records affected</returns>
        Task<int> SaveChangesAsync();
    }
}
