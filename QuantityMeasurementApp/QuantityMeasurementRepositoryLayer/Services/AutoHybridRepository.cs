using System.Collections.Concurrent;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using QuantityMeasurementModelLayer.DTOs.Enums;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementModelLayer.Exceptions;
using QuantityMeasurementRepositoryLayer.Configuration;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementRepositoryLayer.Services
{
    /// <summary>
    /// Auto-detecting Hybrid Repository
    /// - Automatically uses cache + database when available
    /// - Falls back to cache-only when database is down
    /// - No configuration needed - it just works!
    /// </summary>
    public class AutoHybridRepository : IQuantityMeasurementRepository
    {
        private readonly IQuantityMeasurementRepository _cacheRepository;
        private IQuantityMeasurementRepository? _databaseRepository;
        private readonly ILogger<AutoHybridRepository> _logger;
        private readonly ConcurrentQueue<QuantityMeasurementEntity> _writeQueue;
        private Timer? _flushTimer;
        private readonly object _flushLock = new object();
        private bool _isFlushing;
        private bool _isDatabaseAvailable;
        private DateTime _lastDbCheck;
        private Timer? _statusCheckTimer;
        private readonly DatabaseConfig _databaseConfig;
        private bool _forceDatabaseOffline = false;
        private readonly object _statsLock = new object();
        private int _totalWritesAttempted = 0;
        private int _successfulWrites = 0;
        private int _failedWrites = 0;

        public AutoHybridRepository(ILogger<AutoHybridRepository> logger)
        {
            _logger = logger;
            _cacheRepository = QuantityMeasurementCacheRepository.Instance;
            _writeQueue = new ConcurrentQueue<QuantityMeasurementEntity>();
            _databaseConfig = new DatabaseConfig();
            _isDatabaseAvailable = false;

            // Try to initialize database connection (don't wait)
            Task.Run(async () => await InitializeDatabaseAsync());

            // Start a timer to check database status every 5 seconds (more frequent)
            _statusCheckTimer = new Timer(
                CheckDatabaseStatus,
                null,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5)
            );

            // Start flush timer (will run every 2 seconds if database is available)
            _flushTimer = new Timer(
                FlushToDatabase,
                null,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(2)
            );

            _logger.LogInformation("AutoHybrid repository initialized");
        }

        private void CheckDatabaseStatus(object? state)
        {
            // If we're already connected and queue is empty, no need to check frequently
            if (_isDatabaseAvailable && _databaseRepository != null && _writeQueue.IsEmpty)
                return;

            try
            {
                // Try to connect or reconnect
                if (_databaseRepository == null)
                {
                    var dbLogger = NullLogger<QuantityMeasurementDatabaseRepository>.Instance;
                    _databaseRepository = new QuantityMeasurementDatabaseRepository(dbLogger);
                }

                // Test connection with a simple query
                var task = Task.Run(async () => await TestDatabaseConnectionAsync());
                if (task.Wait(TimeSpan.FromSeconds(5)))
                {
                    if (task.Result)
                    {
                        bool wasOffline = !_isDatabaseAvailable;
                        _isDatabaseAvailable = true;
                        _lastDbCheck = DateTime.Now;

                        if (wasOffline)
                        {
                            _logger.LogInformation(
                                "Database connection RESTORED - will sync pending writes"
                            );

                            // Immediately try to flush any pending writes
                            if (!_writeQueue.IsEmpty)
                            {
                                _logger.LogInformation(
                                    "Found {Count} pending writes - starting sync...",
                                    _writeQueue.Count
                                );
                                Task.Run(async () => await FlushToDatabaseAsync());
                            }
                            else
                            {
                                // Even if queue is empty, we should load cache from DB to ensure sync
                                Task.Run(async () => await LoadCacheFromDatabaseAsync());
                            }
                        }
                    }
                    else
                    {
                        if (_isDatabaseAvailable)
                        {
                            _logger.LogWarning(
                                "Database connection LOST - switching to cache-only mode"
                            );
                            _isDatabaseAvailable = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_isDatabaseAvailable)
                {
                    _logger.LogDebug("Database connection lost: {Message}", ex.Message);
                    _isDatabaseAvailable = false;
                }
            }
        }

        public void SetDatabaseOffline(bool offline)
        {
            _forceDatabaseOffline = offline;
            if (offline)
            {
                _isDatabaseAvailable = false;
                _logger.LogWarning("Database manually set to OFFLINE mode");
            }
            else
            {
                // Try to reconnect
                Task.Run(async () => await InitializeDatabaseAsync());
            }
        }

        private async Task InitializeDatabaseAsync()
        {
            if (_forceDatabaseOffline)
            {
                _isDatabaseAvailable = false;
                return;
            }

            try
            {
                var dbLogger = NullLogger<QuantityMeasurementDatabaseRepository>.Instance;
                _databaseRepository = new QuantityMeasurementDatabaseRepository(dbLogger);

                // Test connection silently
                await _databaseRepository.GetTotalQuantityCountAsync();
                _isDatabaseAvailable = true;
                _lastDbCheck = DateTime.Now;

                _logger.LogDebug("Database connection successful");

                // Load existing database records into cache
                await LoadCacheFromDatabaseAsync();
            }
            catch (Exception)
            {
                _isDatabaseAvailable = false;
                _databaseRepository = null;
                _logger.LogDebug("Database not available - running in cache-only mode");

                if (!_forceDatabaseOffline)
                {
                    _ = Task.Delay(TimeSpan.FromSeconds(30))
                        .ContinueWith(async _ => await InitializeDatabaseAsync());
                }
            }
        }

        public async Task SaveQuantityAsync(QuantityMeasurementEntity entity)
        {
            try
            {
                // ALWAYS save to cache immediately
                await _cacheRepository.SaveQuantityAsync(entity);

                Interlocked.Increment(ref _totalWritesAttempted);

                bool isDbAvailable = false;
                if (_databaseRepository != null && !_forceDatabaseOffline)
                {
                    try
                    {
                        // Quick connection test
                        await _databaseRepository.GetTotalQuantityCountAsync();
                        isDbAvailable = true;
                    }
                    catch
                    {
                        isDbAvailable = false;
                    }
                }

                if (isDbAvailable && _databaseRepository != null)
                {
                    // Database is ONLINE - save immediately
                    try
                    {
                        // Check if record already exists in database
                        var existing = await _databaseRepository.GetQuantityByIdAsync(
                            entity.MeasurementId
                        );

                        if (existing != null)
                        {
                            // Record exists - compare timestamps
                            if (entity.CreatedAt > existing.CreatedAt)
                            {
                                // Newer - update database
                                await _databaseRepository.SaveQuantityAsync(entity);
                                _logger.LogDebug(
                                    "Updated existing record {MeasurementId} in database (newer)",
                                    entity.MeasurementId
                                );
                            }
                            else
                            {
                                // Older or same - skip
                                _logger.LogDebug(
                                    "Skipping database save for {MeasurementId} - existing record is newer",
                                    entity.MeasurementId
                                );
                            }
                        }
                        else
                        {
                            // New record - insert
                            await _databaseRepository.SaveQuantityAsync(entity);
                            _logger.LogDebug(
                                "Inserted new record {MeasurementId} into database",
                                entity.MeasurementId
                            );
                        }

                        Interlocked.Increment(ref _successfulWrites);
                    }
                    catch (Exception ex)
                    {
                        // If direct save fails, queue it
                        _logger.LogWarning(ex, "Direct database save failed, queuing instead");
                        _writeQueue.Enqueue(entity);
                        Interlocked.Increment(ref _failedWrites);
                        _isDatabaseAvailable = false;
                    }
                }
                else
                {
                    // Database is OFFLINE - queue for later
                    _writeQueue.Enqueue(entity);
                    Interlocked.Increment(ref _failedWrites);
                    _logger.LogDebug(
                        "Entity {MeasurementId} queued for database (offline mode)",
                        entity.MeasurementId
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving entity {MeasurementId}", entity.MeasurementId);
                throw new DatabaseException($"Failed to save: {ex.Message}", ex);
            }
        }

        public async Task ForceFlushToDatabaseAsync()
        {
            if (!_isDatabaseAvailable || _databaseRepository == null || _forceDatabaseOffline)
            {
                _logger.LogWarning("Cannot flush - database not available");
                return;
            }

            await FlushToDatabaseAsync();
        }

        public async Task<QuantityMeasurementEntity?> GetQuantityByIdAsync(string id)
        {
            // Always check cache first (fast path)
            var cached = await _cacheRepository.GetQuantityByIdAsync(id);
            if (cached != null)
            {
                return cached;
            }

            // Cache miss - try database if available
            if (_isDatabaseAvailable && _databaseRepository != null)
            {
                var fromDb = await _databaseRepository.GetQuantityByIdAsync(id);

                if (fromDb != null)
                {
                    // Update cache for future requests
                    await _cacheRepository.SaveQuantityAsync(fromDb);
                }

                return fromDb;
            }

            return null;
        }

        public async Task<List<QuantityMeasurementEntity>> GetAllQuantitiesAsync()
        {
            // Try cache first
            var cached = await _cacheRepository.GetAllQuantitiesAsync();
            if (cached.Any())
            {
                return cached;
            }

            // Cache empty - try database if available
            if (_isDatabaseAvailable && _databaseRepository != null)
            {
                var fromDb = await _databaseRepository.GetAllQuantitiesAsync();

                // Update cache
                foreach (var entity in fromDb)
                {
                    await _cacheRepository.SaveQuantityAsync(entity);
                }

                return fromDb;
            }

            return new List<QuantityMeasurementEntity>();
        }

        public async Task ClearAllQuantitiesAsync()
        {
            // Clear cache immediately
            await _cacheRepository.ClearAllQuantitiesAsync();

            // Clear database if available
            if (_isDatabaseAvailable && _databaseRepository != null)
            {
                await _databaseRepository.ClearAllQuantitiesAsync();
            }

            // Clear pending queue
            while (_writeQueue.TryDequeue(out _)) { }

            // Reset stats
            Interlocked.Exchange(ref _totalWritesAttempted, 0);
            Interlocked.Exchange(ref _successfulWrites, 0);
            Interlocked.Exchange(ref _failedWrites, 0);

            _logger.LogInformation("Cleared all quantities");
        }

        #region Query Methods

        public async Task<List<QuantityMeasurementEntity>> GetQuantitiesByOperationAsync(
            OperationType operationType
        )
        {
            var cached = await _cacheRepository.GetQuantitiesByOperationAsync(operationType);
            if (cached.Any())
                return cached;

            if (_isDatabaseAvailable && _databaseRepository != null)
            {
                var fromDb = await _databaseRepository.GetQuantitiesByOperationAsync(operationType);
                foreach (var entity in fromDb)
                {
                    await _cacheRepository.SaveQuantityAsync(entity);
                }
                return fromDb;
            }

            return new List<QuantityMeasurementEntity>();
        }

        public async Task<List<QuantityMeasurementEntity>> GetQuantitiesByCategoryAsync(
            string category
        )
        {
            var cached = await _cacheRepository.GetQuantitiesByCategoryAsync(category);
            if (cached.Any())
                return cached;

            if (_isDatabaseAvailable && _databaseRepository != null)
            {
                var fromDb = await _databaseRepository.GetQuantitiesByCategoryAsync(category);
                foreach (var entity in fromDb)
                {
                    await _cacheRepository.SaveQuantityAsync(entity);
                }
                return fromDb;
            }

            return new List<QuantityMeasurementEntity>();
        }

        public async Task<List<QuantityMeasurementEntity>> GetQuantitiesByDateRangeAsync(
            DateTime startDate,
            DateTime endDate
        )
        {
            var cached = await _cacheRepository.GetQuantitiesByDateRangeAsync(startDate, endDate);
            if (cached.Any())
                return cached;

            if (_isDatabaseAvailable && _databaseRepository != null)
            {
                var fromDb = await _databaseRepository.GetQuantitiesByDateRangeAsync(
                    startDate,
                    endDate
                );
                foreach (var entity in fromDb)
                {
                    await _cacheRepository.SaveQuantityAsync(entity);
                }
                return fromDb;
            }

            return new List<QuantityMeasurementEntity>();
        }

        public async Task<int> GetTotalQuantityCountAsync()
        {
            return await _cacheRepository.GetTotalQuantityCountAsync();
        }

        #endregion

        #region Statistics & Resource Management

        public async Task<Dictionary<string, object>> GetRepositoryStatisticsAsync()
        {
            // Test REAL database connection
            bool isDbReallyAvailable = false;
            int? dbRecordCount = null;
            string dbStatusMessage = "OFFLINE";
            string connectionStatus = "Unknown";

            if (_databaseRepository != null && !_forceDatabaseOffline)
            {
                try
                {
                    using (var connection = new SqlConnection(_databaseConfig.ConnectionString))
                    {
                        await connection.OpenAsync();
                        var cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM QuantityMeasurements",
                            connection
                        );
                        dbRecordCount = (int?)await cmd.ExecuteScalarAsync();
                        await connection.CloseAsync();

                        isDbReallyAvailable = true;
                        dbStatusMessage = "ONLINE";
                        connectionStatus = "Connected";

                        // Update the flag if it was false but now true
                        if (!_isDatabaseAvailable)
                        {
                            _isDatabaseAvailable = true;
                            _logger.LogInformation(
                                "Database connection restored (detected in stats)"
                            );

                            // If we have pending writes, try to flush them
                            if (!_writeQueue.IsEmpty)
                            {
                                _ = Task.Run(async () => await FlushToDatabaseAsync());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    isDbReallyAvailable = false;
                    dbRecordCount = null;
                    dbStatusMessage = "OFFLINE";
                    connectionStatus =
                        $"Failed: {ex.Message.Substring(0, Math.Min(30, ex.Message.Length))}...";

                    if (_isDatabaseAvailable)
                    {
                        _logger.LogWarning(
                            "Database connection lost - switching to cache-only mode"
                        );
                        _isDatabaseAvailable = false;
                    }
                }
            }

            // Get cache count
            int cacheCount = await _cacheRepository.GetTotalQuantityCountAsync();

            var stats = new Dictionary<string, object>
            {
                ["Mode"] = isDbReallyAvailable
                    ? "Auto Hybrid (Cache + Database)"
                    : "Cache Only (Database unavailable)",
                ["CacheRecords"] = cacheCount,
                ["PendingWrites"] = _writeQueue.Count,
                ["DatabaseAvailable"] = isDbReallyAvailable,
                ["DatabaseStatus"] = dbStatusMessage,
                ["ConnectionStatus"] = connectionStatus,
                ["LastDbCheck"] = DateTime.Now,
                ["Timestamp"] = DateTime.Now,
                ["TotalWritesAttempted"] = _totalWritesAttempted,
                ["SuccessfulWrites"] = _successfulWrites,
                ["FailedWrites"] = _failedWrites,
            };

            // Show database records if available
            if (isDbReallyAvailable && dbRecordCount.HasValue)
            {
                stats["DatabaseRecords"] = dbRecordCount.Value;

                // Check if cache and DB are in sync
                if (cacheCount == dbRecordCount.Value && _writeQueue.IsEmpty)
                {
                    stats["Note"] = "Database online - cache and DB in sync";
                    stats["SyncStatus"] = "IN SYNC";
                }
                else if (cacheCount == dbRecordCount.Value && !_writeQueue.IsEmpty)
                {
                    stats["Note"] = $"Database online but {_writeQueue.Count} writes pending sync";
                    stats["SyncStatus"] = $"PENDING ({_writeQueue.Count})";
                }
                else
                {
                    stats["Note"] =
                        $"Database online but OUT OF SYNC - Cache:{cacheCount} DB:{dbRecordCount.Value} Pending:{_writeQueue.Count}";
                    stats["SyncStatus"] = "OUT OF SYNC";
                }
            }
            else
            {
                // Database is offline
                stats["DatabaseRecords"] = "OFFLINE";
                stats["Note"] = "Database offline - running in cache-only mode";
                stats["SyncStatus"] = "OFFLINE";

                if (_writeQueue.Count > 0)
                {
                    stats["Note"] =
                        $"Database offline - {_writeQueue.Count} writes pending when connection restores";
                }
            }

            // Add operation counts (from cache)
            var allItems = await _cacheRepository.GetAllQuantitiesAsync();
            var counts = allItems
                .GroupBy(x => x.OperationType)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());
            stats["CountsByOperation"] = counts;

            return stats;
        }

        public async Task ReleaseResourcesAsync()
        {
            // Flush any pending writes before shutdown
            if (_isDatabaseAvailable && _databaseRepository != null && !_writeQueue.IsEmpty)
            {
                _logger.LogInformation(
                    "Shutdown - flushing {Count} pending writes...",
                    _writeQueue.Count
                );
                await FlushToDatabaseAsync();
                _flushTimer?.Dispose();
            }

            await _cacheRepository.ReleaseResourcesAsync();
            _statusCheckTimer?.Dispose();

            _logger.LogInformation("AutoHybrid repository resources released");
        }

        #endregion

        #region Background Flush

        private void FlushToDatabase(object? state)
        {
            if (_databaseRepository == null || !_isDatabaseAvailable || _writeQueue.IsEmpty)
                return;

            lock (_flushLock)
            {
                if (_isFlushing)
                    return;
                _isFlushing = true;
            }

            try
            {
                // Test connection first
                var isConnected = Task.Run(async () => await TestDatabaseConnectionAsync()).Result;
                if (!isConnected)
                {
                    _isDatabaseAvailable = false;
                    return;
                }

                Task.Run(async () => await FlushToDatabaseAsync()).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during background flush");
                _isDatabaseAvailable = false;
            }
            finally
            {
                lock (_flushLock)
                {
                    _isFlushing = false;
                }
            }
        }

        private async Task FlushToDatabaseAsync()
        {
            if (_databaseRepository == null || !_isDatabaseAvailable || _writeQueue.IsEmpty)
                return;

            int batchSize = 0;
            int totalFlushed = 0;
            int failedCount = 0;

            while (!_writeQueue.IsEmpty && batchSize < 50)
            {
                var batch = new List<QuantityMeasurementEntity>();

                // Take up to 20 items at a time
                while (batch.Count < 20 && _writeQueue.TryDequeue(out var entity))
                {
                    batch.Add(entity);
                }

                if (batch.Any())
                {
                    try
                    {
                        _logger.LogInformation(
                            "💾 Flushing {Count} pending writes to database...",
                            batch.Count
                        );

                        var tasks = batch.Select(e => _databaseRepository.SaveQuantityAsync(e));
                        await Task.WhenAll(tasks);

                        totalFlushed += batch.Count;
                        Interlocked.Add(ref _successfulWrites, batch.Count);

                        _logger.LogInformation(
                            "Successfully flushed {Count} records to database",
                            batch.Count
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Failed to flush batch: {Message}", ex.Message);
                        failedCount += batch.Count;

                        // Put failed items back in queue
                        foreach (var item in batch)
                        {
                            _writeQueue.Enqueue(item);
                        }

                        // If we get an error, stop trying for now
                        if (ex is SqlException)
                        {
                            _isDatabaseAvailable = false;
                            break;
                        }
                    }

                    batchSize += batch.Count;
                }
            }

            if (totalFlushed > 0)
            {
                _logger.LogInformation(
                    "Flush complete - Total: {TotalFlushed}, Failed: {FailedCount}, Remaining: {Remaining}",
                    totalFlushed,
                    failedCount,
                    _writeQueue.Count
                );
            }
        }

        private async Task LoadCacheFromDatabaseAsync()
        {
            if (!_isDatabaseAvailable || _databaseRepository == null)
                return;

            try
            {
                _logger.LogInformation("Loading existing database records into cache...");
                var allFromDb = await _databaseRepository.GetAllQuantitiesAsync();

                int loaded = 0;
                foreach (var entity in allFromDb)
                {
                    // Only add if not already in cache (to avoid overwriting newer cache entries)
                    var cached = await _cacheRepository.GetQuantityByIdAsync(entity.MeasurementId);
                    if (cached == null)
                    {
                        await _cacheRepository.SaveQuantityAsync(entity);
                        loaded++;
                    }
                }

                _logger.LogInformation("Loaded {Loaded} new records into cache", loaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load database records into cache");
            }
        }

        private async Task<bool> TestDatabaseConnectionAsync()
        {
            if (_databaseRepository == null)
                return false;

            try
            {
                await _databaseRepository.GetTotalQuantityCountAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Force a full sync between cache and database
        /// - Missing database records will be added to database
        /// - Missing cache records will be loaded from database
        /// - Conflicting records (same ID) will use the latest timestamp
        /// </summary>
        public async Task<bool> ForceFullSyncAsync()
        {
            _logger.LogInformation("Starting full cache-database sync...");

            try
            {
                // First, ensure database is available
                if (!_isDatabaseAvailable || _databaseRepository == null)
                {
                    _logger.LogWarning("Cannot sync - database not available");
                    return false;
                }

                // Get all records from both sources
                var cacheRecords = await _cacheRepository.GetAllQuantitiesAsync();
                var dbRecords = await _databaseRepository.GetAllQuantitiesAsync();

                _logger.LogInformation(
                    "Cache: {CacheCount} records, Database: {DbCount} records",
                    cacheRecords.Count,
                    dbRecords.Count
                );

                int addedToDb = 0;
                int addedToCache = 0;
                int updated = 0;
                int skipped = 0;

                // Create dictionaries for quick lookup
                var cacheDict = cacheRecords.ToDictionary(x => x.MeasurementId);
                var dbDict = dbRecords.ToDictionary(x => x.MeasurementId);

                // STEP 1: Handle records in cache but not in database (INSERT)
                var missingInDb = cacheRecords
                    .Where(c => !dbDict.ContainsKey(c.MeasurementId))
                    .ToList();
                if (missingInDb.Any())
                {
                    _logger.LogInformation(
                        "Found {Count} records in cache but not in database - inserting...",
                        missingInDb.Count
                    );

                    foreach (var record in missingInDb)
                    {
                        try
                        {
                            await _databaseRepository.SaveQuantityAsync(record);
                            addedToDb++;
                            _logger.LogDebug(
                                "Inserted record {Id} into database",
                                record.MeasurementId
                            );
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(
                                ex,
                                "Failed to insert record {Id} - may already exist",
                                record.MeasurementId
                            );
                            skipped++;
                        }
                    }
                }

                // STEP 2: Handle records in database but not in cache (LOAD into cache)
                var missingInCache = dbRecords
                    .Where(d => !cacheDict.ContainsKey(d.MeasurementId))
                    .ToList();
                if (missingInCache.Any())
                {
                    _logger.LogInformation(
                        "Found {Count} records in database but not in cache - loading...",
                        missingInCache.Count
                    );

                    foreach (var record in missingInCache)
                    {
                        await _cacheRepository.SaveQuantityAsync(record);
                        addedToCache++;
                        _logger.LogDebug("Loaded record {Id} into cache", record.MeasurementId);
                    }
                }

                // STEP 3: Handle conflicting records (same ID exists in both)
                var commonIds = cacheDict.Keys.Intersect(dbDict.Keys).ToList();
                _logger.LogInformation(
                    "Checking {Count} common records for conflicts...",
                    commonIds.Count
                );

                foreach (var id in commonIds)
                {
                    var cacheRecord = cacheDict[id];
                    var dbRecord = dbDict[id];

                    // Compare records - if they're identical, skip
                    bool areIdentical = AreRecordsIdentical(cacheRecord, dbRecord);

                    if (areIdentical)
                    {
                        _logger.LogDebug(
                            "Record {Id} is identical in both cache and database - skipping",
                            id
                        );
                        continue;
                    }

                    // If timestamps differ, keep the newer one
                    _logger.LogDebug(
                        "Conflict detected for ID {Id}: Cache={CacheTime}, DB={DbTime}",
                        id,
                        cacheRecord.CreatedAt,
                        dbRecord.CreatedAt
                    );

                    if (cacheRecord.CreatedAt > dbRecord.CreatedAt)
                    {
                        // Cache is newer - update database
                        _logger.LogInformation(
                            "Updating database with newer cache record for ID {Id}",
                            id
                        );
                        await _databaseRepository.SaveQuantityAsync(cacheRecord);
                        updated++;
                    }
                    else if (dbRecord.CreatedAt > cacheRecord.CreatedAt)
                    {
                        // Database is newer - update cache
                        _logger.LogInformation(
                            "Updating cache with newer database record for ID {Id}",
                            id
                        );
                        await _cacheRepository.SaveQuantityAsync(dbRecord);
                        updated++;
                    }
                    else
                    {
                        // Same timestamp but different data - keep cache version (arbitrary choice)
                        _logger.LogWarning(
                            "Same timestamp but different data for ID {Id} - keeping cache version",
                            id
                        );
                        await _databaseRepository.SaveQuantityAsync(cacheRecord);
                        updated++;
                    }
                }

                _logger.LogInformation(
                    "Sync complete - Added to DB: {AddedToDb}, Added to Cache: {AddedToCache}, Updated: {Updated}, Skipped: {Skipped}",
                    addedToDb,
                    addedToCache,
                    updated,
                    skipped
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sync failed: {Message}", ex.Message);
                return false;
            }
        }

        private bool AreRecordsIdentical(QuantityMeasurementEntity a, QuantityMeasurementEntity b)
        {
            if (a.MeasurementId != b.MeasurementId)
                return false;
            if (a.OperationType != b.OperationType)
                return false;
            if (a.FirstOperandValue != b.FirstOperandValue)
                return false;
            if (a.FirstOperandUnit != b.FirstOperandUnit)
                return false;
            if (a.FirstOperandCategory != b.FirstOperandCategory)
                return false;
            if (a.SecondOperandValue != b.SecondOperandValue)
                return false;
            if (a.SecondOperandUnit != b.SecondOperandUnit)
                return false;
            if (a.SecondOperandCategory != b.SecondOperandCategory)
                return false;
            if (a.ResultValue != b.ResultValue)
                return false;
            if (a.ResultUnit != b.ResultUnit)
                return false;
            if (a.FormattedResult != b.FormattedResult)
                return false;
            if (a.IsSuccessful != b.IsSuccessful)
                return false;
            if (a.ErrorDetails != b.ErrorDetails)
                return false;

            // Compare timestamps within a reasonable tolerance (1 second)
            if (Math.Abs((a.CreatedAt - b.CreatedAt).TotalSeconds) > 1)
                return false;

            return true;
        }

        #endregion
    }
}
