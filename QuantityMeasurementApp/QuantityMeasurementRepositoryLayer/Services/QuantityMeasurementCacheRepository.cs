using System.Collections.Concurrent;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementRepositoryLayer.Services
{
    /// <summary>
    /// In-memory cache repository implementation.
    /// </summary>
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        private static readonly Lazy<QuantityMeasurementCacheRepository> _instance =
            new Lazy<QuantityMeasurementCacheRepository>(() =>
                new QuantityMeasurementCacheRepository()
            );

        private readonly ConcurrentDictionary<string, QuantityMeasurementEntity> _storage;
        private readonly string _storagePath;

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

        public void Save(QuantityMeasurementEntity entity)
        {
            _storage[entity.Id] = entity;
            SaveToDisk();
        }

        public List<QuantityMeasurementEntity> GetAll()
        {
            return _storage.Values.ToList();
        }

        public QuantityMeasurementEntity? GetById(string id)
        {
            _storage.TryGetValue(id, out var entity);
            return entity;
        }

        public void Clear()
        {
            _storage.Clear();
            SaveToDisk();
        }

        private void SaveToDisk()
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(_storage.Values.ToList());
                File.WriteAllText(_storagePath, json);
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
                    var json = File.ReadAllText(_storagePath);
                    var entities = System.Text.Json.JsonSerializer.Deserialize<
                        List<QuantityMeasurementEntity>
                    >(json);
                    if (entities != null)
                    {
                        foreach (var entity in entities)
                        {
                            _storage[entity.Id] = entity;
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
