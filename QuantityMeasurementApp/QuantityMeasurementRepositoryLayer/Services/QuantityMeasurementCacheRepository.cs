using System.Collections.Concurrent;
using System.Text.Json;
using QuantityMeasurementModelLayer.DTOs.Enums;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementRepositoryLayer.Services
{
    /// <summary>
    /// In-memory cache repository implementation (Singleton Pattern).
    /// All methods are async with proper naming conventions.
    /// </summary>
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        private static readonly Lazy<QuantityMeasurementCacheRepository> _instance =
            new Lazy<QuantityMeasurementCacheRepository>(() =>
                new QuantityMeasurementCacheRepository()
            );

        private readonly ConcurrentDictionary<string, QuantityMeasurementEntity> _storage;
        private readonly string _storagePath;
        private readonly object _fileLock = new object();

        private QuantityMeasurementCacheRepository()
        {
            _storage = new ConcurrentDictionary<string, QuantityMeasurementEntity>();
            _storagePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "quantity_data.json"
            );
            LoadFromDisk();
        }

        public static QuantityMeasurementCacheRepository Instance => _instance.Value;

        public async Task SaveQuantityAsync(QuantityMeasurementEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await Task.Run(() =>
            {
                _storage[entity.MeasurementId] = entity;
                SaveToDisk();
            });
        }

        public async Task<List<QuantityMeasurementEntity>> GetAllQuantitiesAsync()
        {
            return await Task.Run(() =>
                _storage.Values.OrderByDescending(e => e.CreatedAt).ToList()
            );
        }

        public async Task<QuantityMeasurementEntity?> GetQuantityByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return await Task.Run(() =>
            {
                _storage.TryGetValue(id, out var entity);
                return entity;
            });
        }

        public async Task ClearAllQuantitiesAsync()
        {
            await Task.Run(() =>
            {
                _storage.Clear();
                SaveToDisk();
            });
        }

        public async Task<List<QuantityMeasurementEntity>> GetQuantitiesByOperationAsync(
            OperationType operationType
        )
        {
            return await Task.Run(() =>
                _storage
                    .Values.Where(e => e.OperationType == operationType)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList()
            );
        }

        public async Task<List<QuantityMeasurementEntity>> GetQuantitiesByCategoryAsync(
            string category
        )
        {
            return await Task.Run(() =>
                _storage
                    .Values.Where(e =>
                        e.FirstOperandCategory == category || e.SourceOperandCategory == category
                    )
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList()
            );
        }

        public async Task<List<QuantityMeasurementEntity>> GetQuantitiesByDateRangeAsync(
            DateTime startDate,
            DateTime endDate
        )
        {
            return await Task.Run(() =>
                _storage
                    .Values.Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList()
            );
        }

        public async Task<int> GetTotalQuantityCountAsync()
        {
            return await Task.Run(() => _storage.Count);
        }

        public async Task<Dictionary<string, object>> GetRepositoryStatisticsAsync()
        {
            return await Task.Run(() =>
            {
                var stats = new Dictionary<string, object>
                {
                    ["RepositoryType"] = "Cache",
                    ["TotalRecords"] = _storage.Count,
                    ["StoragePath"] = _storagePath,
                    ["FileExists"] = File.Exists(_storagePath),
                    ["LastUpdated"] = DateTime.Now,
                };

                // Add counts by operation
                var operationCounts = new Dictionary<string, int>();
                foreach (OperationType op in Enum.GetValues(typeof(OperationType)))
                {
                    operationCounts[op.ToString()] = _storage.Values.Count(e =>
                        e.OperationType == op
                    );
                }
                stats["CountsByOperation"] = operationCounts;

                return stats;
            });
        }

        public async Task ReleaseResourcesAsync()
        {
            await Task.Run(() =>
            {
                SaveToDisk();
                _storage.Clear();
                Console.WriteLine("Cache repository resources released");
            });
        }

        private void SaveToDisk()
        {
            try
            {
                lock (_fileLock)
                {
                    var json = JsonSerializer.Serialize(
                        _storage.Values.ToList(),
                        new JsonSerializerOptions { WriteIndented = true }
                    );
                    File.WriteAllText(_storagePath, json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to disk: {ex.Message}");
            }
        }

        private void LoadFromDisk()
        {
            try
            {
                if (File.Exists(_storagePath))
                {
                    lock (_fileLock)
                    {
                        var json = File.ReadAllText(_storagePath);
                        var entities = JsonSerializer.Deserialize<List<QuantityMeasurementEntity>>(
                            json
                        );
                        if (entities != null)
                        {
                            foreach (var entity in entities)
                            {
                                if (entity != null && !string.IsNullOrEmpty(entity.MeasurementId))
                                {
                                    _storage[entity.MeasurementId] = entity;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from disk: {ex.Message}");
            }
        }
    }
}
