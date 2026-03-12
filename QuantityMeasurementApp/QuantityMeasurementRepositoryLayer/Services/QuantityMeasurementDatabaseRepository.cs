using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModelLayer.DTOs.Enums;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementModelLayer.Exceptions;
using QuantityMeasurementRepositoryLayer.Configuration;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementRepositoryLayer.Services
{
    /// <summary>
    /// Database repository implementation using ADO.NET and Dapper.
    /// UC16: All methods are async with proper naming conventions.
    /// Uses custom DatabaseException instead of SqlException.
    /// </summary>
    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        private readonly DatabaseConfig _databaseConfig;
        private readonly ILogger<QuantityMeasurementDatabaseRepository> _logger;
        private readonly string? _connectionString;
        private int _queryCount;
        private readonly object _statsLock = new object();

        public QuantityMeasurementDatabaseRepository(
            ILogger<QuantityMeasurementDatabaseRepository> logger
        )
        {
            _databaseConfig = new DatabaseConfig();
            _logger = logger;
            _connectionString = _databaseConfig.ConnectionString;
            _queryCount = 0;

            Task.Run(async () => await EnsureDatabaseCreatedAsync()).Wait();
            _logger.LogInformation(
                "Database repository initialized with connection: {ConnectionString}",
                _connectionString?.Replace("Password=", "Password=***") ?? "No connection string"
            );
        }

        private async Task EnsureDatabaseCreatedAsync()
        {
            try
            {
                var masterBuilder = new SqlConnectionStringBuilder(_connectionString);
                var databaseName = masterBuilder.InitialCatalog;
                masterBuilder.InitialCatalog = "master";

                using (var connection = new SqlConnection(masterBuilder.ConnectionString))
                {
                    await connection.OpenAsync();

                    // Check if database exists
                    var checkDbSql =
                        $"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'";
                    var dbExists = (int?)await connection.ExecuteScalarAsync(checkDbSql) > 0;

                    if (!dbExists)
                    {
                        _logger.LogInformation(
                            "Database {DatabaseName} does not exist. Creating...",
                            databaseName
                        );
                        var createDbSql = $"CREATE DATABASE [{databaseName}]";
                        await connection.ExecuteAsync(createDbSql);
                        _logger.LogInformation(
                            "Database {DatabaseName} created successfully.",
                            databaseName
                        );
                    }
                }

                // Create tables if they don't exist
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var createTableSql =
                        @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='QuantityMeasurements' AND xtype='U')
                        CREATE TABLE QuantityMeasurements (
                            MeasurementId NVARCHAR(50) PRIMARY KEY,
                            CreatedAt DATETIME2 NOT NULL,
                            OperationType INT NOT NULL,
                            FirstOperandValue FLOAT NULL,
                            FirstOperandUnit NVARCHAR(20) NULL,
                            FirstOperandCategory NVARCHAR(20) NULL,
                            SecondOperandValue FLOAT NULL,
                            SecondOperandUnit NVARCHAR(20) NULL,
                            SecondOperandCategory NVARCHAR(20) NULL,
                            TargetUnit NVARCHAR(20) NULL,
                            SourceOperandValue FLOAT NULL,
                            SourceOperandUnit NVARCHAR(20) NULL,
                            SourceOperandCategory NVARCHAR(20) NULL,
                            ResultValue FLOAT NULL,
                            ResultUnit NVARCHAR(20) NULL,
                            FormattedResult NVARCHAR(200) NULL,
                            IsSuccessful BIT NOT NULL,
                            ErrorDetails NVARCHAR(MAX) NULL
                        );

                        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_QuantityMeasurements_CreatedAt')
                        CREATE INDEX IX_QuantityMeasurements_CreatedAt ON QuantityMeasurements(CreatedAt);

                        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_QuantityMeasurements_OperationType')
                        CREATE INDEX IX_QuantityMeasurements_OperationType ON QuantityMeasurements(OperationType);

                        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_QuantityMeasurements_FirstCategory')
                        CREATE INDEX IX_QuantityMeasurements_FirstCategory ON QuantityMeasurements(FirstOperandCategory);";

                    await connection.ExecuteAsync(createTableSql);
                    _logger.LogInformation("Database schema ensured.");
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Failed to ensure database creation");
                throw new DatabaseException($"Database initialization failed: {ex.Message}", ex)
                {
                    ErrorCode = ex.Number,
                    SqlState = ex.State.ToString(),
                    ServerName = ex.Server,
                    DatabaseName = ex.Class.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during database initialization");
                throw new DatabaseException($"Unexpected database error: {ex.Message}", ex);
            }
        }

        public async Task SaveQuantityAsync(QuantityMeasurementEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                var sql =
                    @"
                    INSERT INTO QuantityMeasurements (
                        MeasurementId, CreatedAt, OperationType, 
                        FirstOperandValue, FirstOperandUnit, FirstOperandCategory,
                        SecondOperandValue, SecondOperandUnit, SecondOperandCategory,
                        TargetUnit, SourceOperandValue, SourceOperandUnit, SourceOperandCategory,
                        ResultValue, ResultUnit, FormattedResult, IsSuccessful, ErrorDetails
                    ) VALUES (
                        @MeasurementId, @CreatedAt, @OperationType,
                        @FirstOperandValue, @FirstOperandUnit, @FirstOperandCategory,
                        @SecondOperandValue, @SecondOperandUnit, @SecondOperandCategory,
                        @TargetUnit, @SourceOperandValue, @SourceOperandUnit, @SourceOperandCategory,
                        @ResultValue, @ResultUnit, @FormattedResult, @IsSuccessful, @ErrorDetails
                    )";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var parameters = new
                    {
                        entity.MeasurementId,
                        entity.CreatedAt,
                        OperationType = (int)entity.OperationType,
                        entity.FirstOperandValue,
                        entity.FirstOperandUnit,
                        entity.FirstOperandCategory,
                        entity.SecondOperandValue,
                        entity.SecondOperandUnit,
                        entity.SecondOperandCategory,
                        entity.TargetUnit,
                        entity.SourceOperandValue,
                        entity.SourceOperandUnit,
                        entity.SourceOperandCategory,
                        entity.ResultValue,
                        entity.ResultUnit,
                        entity.FormattedResult,
                        entity.IsSuccessful,
                        entity.ErrorDetails,
                    };

                    await connection.ExecuteAsync(sql, parameters);

                    lock (_statsLock)
                    {
                        _queryCount++;
                    }

                    _logger.LogDebug(
                        "Saved entity with ID {MeasurementId} to database",
                        entity.MeasurementId
                    );
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(
                    ex,
                    "SQL error saving entity with ID {MeasurementId}",
                    entity?.MeasurementId
                );
                throw new DatabaseException($"Database error saving measurement: {ex.Message}", ex)
                {
                    ErrorCode = ex.Number,
                    SqlState = ex.State.ToString(),
                    ConstraintName = ex.Class.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error saving entity with ID {MeasurementId} to database",
                    entity?.MeasurementId
                );
                throw new DatabaseException(
                    $"Unexpected error saving measurement: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<QuantityMeasurementEntity>> GetAllQuantitiesAsync()
        {
            try
            {
                var sql = "SELECT * FROM QuantityMeasurements ORDER BY CreatedAt DESC";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var rows = await connection.QueryAsync(sql);

                    var entities = new List<QuantityMeasurementEntity>();
                    foreach (var row in rows)
                    {
                        entities.Add(MapRowToEntity(row));
                    }

                    lock (_statsLock)
                    {
                        _queryCount++;
                    }

                    _logger.LogDebug("Retrieved {Count} entities from database", entities.Count);
                    return entities;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error retrieving all entities");
                throw new DatabaseException(
                    $"Database error retrieving measurements: {ex.Message}",
                    ex
                )
                {
                    ErrorCode = ex.Number,
                    SqlState = ex.State.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving all entities");
                throw new DatabaseException(
                    $"Unexpected error retrieving measurements: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<QuantityMeasurementEntity?> GetQuantityByIdAsync(string id)
        {
            try
            {
                var sql = "SELECT * FROM QuantityMeasurements WHERE MeasurementId = @MeasurementId";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var row = await connection.QueryFirstOrDefaultAsync(
                        sql,
                        new { MeasurementId = id }
                    );

                    lock (_statsLock)
                    {
                        _queryCount++;
                    }

                    if (row == null)
                    {
                        _logger.LogDebug(
                            "Entity with ID {MeasurementId} not found in database",
                            id
                        );
                        return null;
                    }

                    var entity = MapRowToEntity(row);
                    _logger.LogDebug("Retrieved entity with ID {MeasurementId} from database", id);
                    return entity;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error retrieving entity with ID {MeasurementId}", id);
                throw new DatabaseException(
                    $"Database error retrieving measurement: {ex.Message}",
                    ex
                )
                {
                    ErrorCode = ex.Number,
                    SqlState = ex.State.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error retrieving entity with ID {MeasurementId}",
                    id
                );
                throw new DatabaseException(
                    $"Unexpected error retrieving measurement: {ex.Message}",
                    ex
                );
            }
        }

        public async Task ClearAllQuantitiesAsync()
        {
            try
            {
                var sql = "DELETE FROM QuantityMeasurements";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var affected = await connection.ExecuteAsync(sql);

                    lock (_statsLock)
                    {
                        _queryCount++;
                    }

                    _logger.LogInformation("Cleared {Count} records from database", affected);
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error clearing database");
                throw new DatabaseException(
                    $"Database error clearing measurements: {ex.Message}",
                    ex
                )
                {
                    ErrorCode = ex.Number,
                    SqlState = ex.State.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error clearing database");
                throw new DatabaseException(
                    $"Unexpected error clearing measurements: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<QuantityMeasurementEntity>> GetQuantitiesByOperationAsync(
            OperationType operationType
        )
        {
            try
            {
                var sql =
                    "SELECT * FROM QuantityMeasurements WHERE OperationType = @OperationType ORDER BY CreatedAt DESC";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var rows = await connection.QueryAsync(
                        sql,
                        new { OperationType = (int)operationType }
                    );

                    var entities = new List<QuantityMeasurementEntity>();
                    foreach (var row in rows)
                    {
                        entities.Add(MapRowToEntity(row));
                    }

                    lock (_statsLock)
                    {
                        _queryCount++;
                    }

                    _logger.LogDebug(
                        "Retrieved {Count} entities for operation {OperationType}",
                        entities.Count,
                        operationType
                    );
                    return entities;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error retrieving measurements by operation");
                throw new DatabaseException(
                    $"Database error retrieving measurements by operation: {ex.Message}",
                    ex
                )
                {
                    ErrorCode = ex.Number,
                    SqlState = ex.State.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving measurements by operation");
                throw new DatabaseException(
                    $"Unexpected error retrieving measurements by operation: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<QuantityMeasurementEntity>> GetQuantitiesByCategoryAsync(
            string category
        )
        {
            try
            {
                var sql =
                    @"
                    SELECT * FROM QuantityMeasurements 
                    WHERE FirstOperandCategory = @Category OR SourceOperandCategory = @Category 
                    ORDER BY CreatedAt DESC";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var rows = await connection.QueryAsync(sql, new { Category = category });

                    var entities = new List<QuantityMeasurementEntity>();
                    foreach (var row in rows)
                    {
                        entities.Add(MapRowToEntity(row));
                    }

                    lock (_statsLock)
                    {
                        _queryCount++;
                    }

                    _logger.LogDebug(
                        "Retrieved {Count} entities for category {Category}",
                        entities.Count,
                        category
                    );
                    return entities;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error retrieving measurements by category");
                throw new DatabaseException(
                    $"Database error retrieving measurements by category: {ex.Message}",
                    ex
                )
                {
                    ErrorCode = ex.Number,
                    SqlState = ex.State.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving measurements by category");
                throw new DatabaseException(
                    $"Unexpected error retrieving measurements by category: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<QuantityMeasurementEntity>> GetQuantitiesByDateRangeAsync(
            DateTime startDate,
            DateTime endDate
        )
        {
            try
            {
                var sql =
                    "SELECT * FROM QuantityMeasurements WHERE CreatedAt BETWEEN @StartDate AND @EndDate ORDER BY CreatedAt DESC";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var rows = await connection.QueryAsync(
                        sql,
                        new { StartDate = startDate, EndDate = endDate }
                    );

                    var entities = new List<QuantityMeasurementEntity>();
                    foreach (var row in rows)
                    {
                        entities.Add(MapRowToEntity(row));
                    }

                    lock (_statsLock)
                    {
                        _queryCount++;
                    }

                    _logger.LogDebug(
                        "Retrieved {Count} entities between {StartDate} and {EndDate}",
                        entities.Count,
                        startDate,
                        endDate
                    );
                    return entities;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error retrieving measurements by date range");
                throw new DatabaseException(
                    $"Database error retrieving measurements by date range: {ex.Message}",
                    ex
                )
                {
                    ErrorCode = ex.Number,
                    SqlState = ex.State.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving measurements by date range");
                throw new DatabaseException(
                    $"Unexpected error retrieving measurements by date range: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<int> GetTotalQuantityCountAsync()
        {
            try
            {
                var sql = "SELECT COUNT(*) FROM QuantityMeasurements";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var count = await connection.ExecuteScalarAsync<int>(sql);

                    lock (_statsLock)
                    {
                        _queryCount++;
                    }

                    _logger.LogDebug("Total count in database: {Count}", count);
                    return count;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error getting total count");
                throw new DatabaseException($"Database error getting total count: {ex.Message}", ex)
                {
                    ErrorCode = ex.Number,
                    SqlState = ex.State.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting total count");
                throw new DatabaseException(
                    $"Unexpected error getting total count: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<Dictionary<string, object>> GetRepositoryStatisticsAsync()
        {
            var stats = new Dictionary<string, object>
            {
                ["RepositoryType"] = "Database",
                ["ConnectionString"] =
                    _connectionString?.Replace("Password=", "Password=***")
                    ?? "No connection string",
                ["TotalQueriesExecuted"] = _queryCount,
                ["TotalRecords"] = await GetTotalQuantityCountAsync(),
                ["DatabaseProvider"] = "SQL Server",
                ["LastUpdated"] = DateTime.Now,
            };

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Get database size
                    var sizeSql =
                        @"
                        SELECT 
                            SUM(size * 8 / 1024) as SizeMB
                        FROM sys.master_files 
                        WHERE DB_NAME(database_id) = DB_NAME()";

                    var size = await connection.ExecuteScalarAsync<double?>(sizeSql);
                    if (size.HasValue)
                    {
                        stats["DatabaseSizeMB"] = Math.Round(size.Value, 2);
                    }

                    // Get record counts by operation
                    var operationCounts = await connection.QueryAsync(
                        @"
                        SELECT OperationType, COUNT(*) as RecordCount 
                        FROM QuantityMeasurements 
                        GROUP BY OperationType"
                    );

                    var countsDict = new Dictionary<string, int>();
                    foreach (var row in operationCounts)
                    {
                        var opType = ((OperationType)Convert.ToInt32(row.OperationType)).ToString();
                        countsDict[opType] = (int)row.RecordCount;
                    }

                    stats["CountsByOperation"] = countsDict;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogWarning(ex, "Failed to get additional database statistics");
                stats["StatisticsError"] = "Partial statistics only";
            }

            return stats;
        }

        public async Task ReleaseResourcesAsync()
        {
            await Task.Run(() =>
            {
                // Connection pooling handled by ADO.NET
                SqlConnection.ClearAllPools();
                _logger.LogInformation("Database resources released");
            });
        }

        private QuantityMeasurementEntity MapRowToEntity(dynamic row)
        {
            return new QuantityMeasurementEntity
            {
                MeasurementId = row.MeasurementId,
                CreatedAt = row.CreatedAt,
                OperationType = (OperationType)row.OperationType,
                FirstOperandValue = row.FirstOperandValue,
                FirstOperandUnit = row.FirstOperandUnit,
                FirstOperandCategory = row.FirstOperandCategory,
                SecondOperandValue = row.SecondOperandValue,
                SecondOperandUnit = row.SecondOperandUnit,
                SecondOperandCategory = row.SecondOperandCategory,
                TargetUnit = row.TargetUnit,
                SourceOperandValue = row.SourceOperandValue,
                SourceOperandUnit = row.SourceOperandUnit,
                SourceOperandCategory = row.SourceOperandCategory,
                ResultValue = row.ResultValue,
                ResultUnit = row.ResultUnit,
                FormattedResult = row.FormattedResult,
                IsSuccessful = row.IsSuccessful,
                ErrorDetails = row.ErrorDetails,
            };
        }
    }
}
