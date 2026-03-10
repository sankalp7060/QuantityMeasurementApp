using Microsoft.Extensions.Logging;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementRepositoryLayer.Services
{
    /// <summary>
    /// Hybrid repository that uses both Redis cache and file storage.
    /// </summary>
    public class HybridRepository : IQuantityMeasurementRepository
    {
        private readonly IQuantityMeasurementRepository _fileRepository;
        private readonly IRedisCacheRepository _cacheRepository;
        private readonly ILogger<HybridRepository> _logger;

        public HybridRepository(
            IQuantityMeasurementRepository fileRepository,
            IRedisCacheRepository cacheRepository,
            ILogger<HybridRepository> logger
        )
        {
            _fileRepository = fileRepository;
            _cacheRepository = cacheRepository;
            _logger = logger;
        }

        public void Save(QuantityMeasurementEntity entity)
        {
            try
            {
                _fileRepository.Save(entity);
                Task.Run(async () => await _cacheRepository.SaveAsync(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving entity with ID {Id}", entity.Id);
                throw;
            }
        }

        public List<QuantityMeasurementEntity> GetAll()
        {
            try
            {
                var cachedEntities = Task.Run(async () =>
                    await _cacheRepository.GetAllAsync()
                ).Result;

                if (cachedEntities != null && cachedEntities.Any())
                {
                    _logger.LogDebug("Retrieved {Count} entities from cache", cachedEntities.Count);
                    return cachedEntities;
                }

                _logger.LogDebug("Cache miss, retrieving from file storage");
                var fileEntities = _fileRepository.GetAll();

                if (fileEntities.Any())
                {
                    Task.Run(async () =>
                    {
                        foreach (var entity in fileEntities)
                        {
                            await _cacheRepository.SaveAsync(entity);
                        }
                    });
                }

                return fileEntities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all entities, falling back to file storage");
                return _fileRepository.GetAll();
            }
        }

        public QuantityMeasurementEntity? GetById(string id)
        {
            try
            {
                var cachedEntity = Task.Run(async () =>
                    await _cacheRepository.GetByIdAsync(id)
                ).Result;

                if (cachedEntity != null)
                {
                    _logger.LogDebug("Retrieved entity with ID {Id} from cache", id);
                    return cachedEntity;
                }

                _logger.LogDebug("Cache miss for ID {Id}, retrieving from file storage", id);
                var fileEntity = _fileRepository.GetById(id);

                if (fileEntity != null)
                {
                    Task.Run(async () => await _cacheRepository.SaveAsync(fileEntity));
                }

                return fileEntity;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting entity with ID {Id}, falling back to file storage",
                    id
                );
                return _fileRepository.GetById(id);
            }
        }

        public void Clear()
        {
            try
            {
                _fileRepository.Clear();
                Task.Run(async () => await _cacheRepository.ClearAllAsync());
                _logger.LogInformation("Cleared all entities from both file and cache storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing repositories");
                throw;
            }
        }
    }
}
