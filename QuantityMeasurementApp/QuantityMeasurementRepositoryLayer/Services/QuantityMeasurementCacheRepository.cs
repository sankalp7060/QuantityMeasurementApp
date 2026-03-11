using System.Collections.Concurrent;
using System.Text.Json;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementRepositoryLayer.Services
{
    /// <summary>
    /// In-memory cache repository implementation (Singleton Pattern).
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

        /// <summary>
        /// Private constructor for Singleton pattern.
        /// </summary>
        private QuantityMeasurementCacheRepository()
        {
            _storage = new ConcurrentDictionary<string, QuantityMeasurementEntity>();
            _storagePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "quantity_data.json"
            );
            LoadFromDisk();
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static QuantityMeasurementCacheRepository Instance => _instance.Value;

        /// <summary>
        /// Saves an entity to the repository.
        /// </summary>
        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _storage[entity.Id] = entity;
            SaveToDisk();
        }

        /// <summary>
        /// Gets all entities from the repository.
        /// </summary>
        public List<QuantityMeasurementEntity> GetAll()
        {
            return _storage.Values.OrderByDescending(e => e.Timestamp).ToList();
        }

        /// <summary>
        /// Gets an entity by ID.
        /// </summary>
        public QuantityMeasurementEntity? GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            _storage.TryGetValue(id, out var entity);
            return entity;
        }

        /// <summary>
        /// Clears all entities from the repository.
        /// </summary>
        public void Clear()
        {
            _storage.Clear();
            SaveToDisk();
        }

        /// <summary>
        /// Saves data to disk with thread safety.
        /// </summary>
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
                // Log but don't throw - this is a non-critical operation
                Console.WriteLine($"Error saving to disk: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads data from disk.
        /// </summary>
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
                                if (entity != null && !string.IsNullOrEmpty(entity.Id))
                                {
                                    _storage[entity.Id] = entity;
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
