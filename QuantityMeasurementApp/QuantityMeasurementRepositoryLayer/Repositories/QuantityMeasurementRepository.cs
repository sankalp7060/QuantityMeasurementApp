// Import required namespaces for Entity Framework Core, logging, DTO enums, entities, database context, and repository interfaces
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModelLayer.DTOs.Enums;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Data;
using QuantityMeasurementRepositoryLayer.Interface;

// Define the namespace for repository implementations
namespace QuantityMeasurementRepositoryLayer.Repositories
{
    /// <summary>
    /// EF Core implementation of quantity measurement repository
    /// This class handles all database operations using Entity Framework Core
    /// </summary>
    public class QuantityMeasurementRepository : IQuantityMeasurementRepository
    {
        // Private fields for dependencies
        private readonly ApplicationDbContext _context; // Database context for EF Core operations
        private readonly ILogger<QuantityMeasurementRepository> _logger; // Logger for tracking operations

        /// <summary>
        /// Constructor for QuantityMeasurementRepository
        /// Receives dependencies through Dependency Injection
        /// </summary>
        /// <param name="context">Database context for EF Core</param>
        /// <param name="logger">Logger for this repository</param>
        public QuantityMeasurementRepository(
            ApplicationDbContext context,
            ILogger<QuantityMeasurementRepository> logger
        )
        {
            // Assign dependencies with null checking (defensive programming)
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Basic CRUD Operations
        // CRUD = Create, Read, Update, Delete operations

        /// <inheritdoc/>
        /// Retrieves a single entity by its numeric ID
        public async Task<QuantityMeasurementEntity?> GetByIdAsync(long id)
        {
            try
            {
                // Use AsNoTracking() for read-only operations (better performance)
                // FirstOrDefaultAsync returns first match or null if not found
                return await _context
                    .QuantityMeasurements.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                // Log error and rethrow (let higher layers handle it)
                _logger.LogError(ex, "Error getting entity by ID {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        /// Retrieves a single entity by its unique measurement ID (string identifier)
        public async Task<QuantityMeasurementEntity?> GetByMeasurementIdAsync(string measurementId)
        {
            try
            {
                // Find entity by the business key (MeasurementId) instead of database ID
                return await _context
                    .QuantityMeasurements.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.MeasurementId == measurementId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting entity by MeasurementId {MeasurementId}",
                    measurementId
                );
                throw;
            }
        }

        /// <inheritdoc/>
        /// Retrieves all entities, ordered by creation date (newest first)
        public async Task<IEnumerable<QuantityMeasurementEntity>> GetAllAsync()
        {
            try
            {
                // Get all records, ordered by CreatedAt descending (most recent first)
                // ToListAsync executes the query and returns results as a List
                return await _context
                    .QuantityMeasurements.AsNoTracking()
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all entities");
                throw;
            }
        }

        /// <inheritdoc/>
        /// Adds a new entity to the database
        public async Task<QuantityMeasurementEntity> AddAsync(QuantityMeasurementEntity entity)
        {
            try
            {
                // Validate input parameter
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                // Set the creation timestamp to current UTC time
                entity.CreatedAt = DateTime.UtcNow;

                // Add entity to DbSet (tracking begins)
                await _context.QuantityMeasurements.AddAsync(entity);
                // Save changes to database (INSERT statement executed)
                await _context.SaveChangesAsync();

                // Log success at Debug level (not as important as Info/Error)
                _logger.LogDebug("Added entity with ID {Id}", entity.Id);
                return entity; // Return the entity with generated ID
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding entity");
                throw;
            }
        }

        /// <inheritdoc/>
        /// Updates an existing entity in the database
        public async Task UpdateAsync(QuantityMeasurementEntity entity)
        {
            try
            {
                // Validate input parameter
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                // Mark entity as modified (UPDATE statement will be generated)
                _context.QuantityMeasurements.Update(entity);
                // Save changes to database
                await _context.SaveChangesAsync();

                _logger.LogDebug("Updated entity with ID {Id}", entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity with ID {Id}", entity?.Id);
                throw;
            }
        }

        /// <inheritdoc/>
        /// Deletes an entity by its ID
        public async Task DeleteAsync(long id)
        {
            try
            {
                // Find the entity first (needs to be tracked for deletion)
                var entity = await _context.QuantityMeasurements.FindAsync(id);
                if (entity != null)
                {
                    // Mark entity for deletion (DELETE statement will be generated)
                    _context.QuantityMeasurements.Remove(entity);
                    // Save changes to database
                    await _context.SaveChangesAsync();
                    _logger.LogDebug("Deleted entity with ID {Id}", id);
                }
                // If entity not found, do nothing (idempotent operation)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity with ID {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        /// Checks if an entity with the given ID exists
        public async Task<bool> ExistsAsync(long id)
        {
            // AnyAsync returns true if at least one record matches the condition
            return await _context.QuantityMeasurements.AnyAsync(e => e.Id == id);
        }

        #endregion

        #region Query Operations
        // Specialized query methods for filtering data

        /// <inheritdoc/>
        /// Gets all entities of a specific operation type (Compare, Convert, Add, etc.)
        public async Task<IEnumerable<QuantityMeasurementEntity>> GetByOperationAsync(
            OperationType operation
        )
        {
            try
            {
                // Filter by OperationType enum, order by newest first
                return await _context
                    .QuantityMeasurements.AsNoTracking()
                    .Where(e => e.OperationType == operation)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entities by operation {Operation}", operation);
                throw;
            }
        }

        /// <inheritdoc/>
        /// Gets all entities for a specific category (Length, Weight, Volume, Temperature)
        public async Task<IEnumerable<QuantityMeasurementEntity>> GetByCategoryAsync(
            string category
        )
        {
            try
            {
                // Check both FirstOperandCategory and SourceOperandCategory (for different operation types)
                return await _context
                    .QuantityMeasurements.AsNoTracking()
                    .Where(e =>
                        e.FirstOperandCategory == category || e.SourceOperandCategory == category
                    )
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entities by category {Category}", category);
                throw;
            }
        }

        /// <inheritdoc/>
        /// Gets all entities created within a specific date range
        public async Task<IEnumerable<QuantityMeasurementEntity>> GetByDateRangeAsync(
            DateTime start,
            DateTime end
        )
        {
            try
            {
                // Filter by CreatedAt between start and end dates (inclusive)
                return await _context
                    .QuantityMeasurements.AsNoTracking()
                    .Where(e => e.CreatedAt >= start && e.CreatedAt <= end)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entities by date range");
                throw;
            }
        }

        /// <inheritdoc/>
        /// Gets all successful operations (IsSuccessful = true)
        public async Task<IEnumerable<QuantityMeasurementEntity>> GetSuccessfulOperationsAsync()
        {
            try
            {
                return await _context
                    .QuantityMeasurements.AsNoTracking()
                    .Where(e => e.IsSuccessful)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting successful operations");
                throw;
            }
        }

        /// <inheritdoc/>
        /// Gets all failed operations (IsSuccessful = false)
        public async Task<IEnumerable<QuantityMeasurementEntity>> GetFailedOperationsAsync()
        {
            try
            {
                return await _context
                    .QuantityMeasurements.AsNoTracking()
                    .Where(e => !e.IsSuccessful)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting failed operations");
                throw;
            }
        }

        #endregion

        #region Aggregation Operations
        // Methods that return statistical data (counts, sums, etc.)

        /// <inheritdoc/>
        /// Gets total count of all records in the table
        public async Task<int> GetTotalCountAsync()
        {
            // CountAsync executes SELECT COUNT(*) query
            return await _context.QuantityMeasurements.CountAsync();
        }

        /// <inheritdoc/>
        /// Gets count of records for a specific operation type
        public async Task<int> GetCountByOperationAsync(OperationType operation)
        {
            // Count with WHERE clause
            return await _context.QuantityMeasurements.CountAsync(e =>
                e.OperationType == operation
            );
        }

        /// <inheritdoc/>
        /// Gets counts for all operation types in a dictionary
        public async Task<Dictionary<OperationType, int>> GetOperationCountsAsync()
        {
            // Group by OperationType and count each group
            var counts = await _context
                .QuantityMeasurements.GroupBy(e => e.OperationType)
                .Select(g => new { Operation = g.Key, Count = g.Count() })
                .ToListAsync();

            // Convert to Dictionary<OperationType, int> for easy lookup
            return counts.ToDictionary(x => x.Operation, x => x.Count);
        }

        #endregion

        #region Pagination Support
        // Methods for paginated results (useful for large datasets)

        /// <inheritdoc/>
        /// Gets a page of results with specified page number and page size
        public async Task<IEnumerable<QuantityMeasurementEntity>> GetPagedAsync(
            int page,
            int pageSize
        )
        {
            // Input validation and default values
            if (page < 1)
                page = 1; // Default to first page
            if (pageSize < 1)
                pageSize = 10; // Default page size

            // Skip: how many records to skip based on page number
            // Take: how many records to return (page size)
            return await _context
                .QuantityMeasurements.AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize) // Calculate offset: (page-1)*pageSize
                .Take(pageSize) // Limit results
                .ToListAsync();
        }

        #endregion

        #region Save Changes
        // Explicit save method (useful for batch operations)

        /// <inheritdoc/>
        /// Explicitly saves changes to the database
        public async Task<int> SaveChangesAsync()
        {
            // Returns number of records affected
            return await _context.SaveChangesAsync();
        }

        #endregion
    }
}
