using QuantityMeasurementModelLayer.Entities;

namespace QuantityMeasurementRepositoryLayer.Interface
{
    /// <summary>
    /// Interface for quantity measurement repository operations.
    /// Follows Interface Segregation Principle.
    /// </summary>
    public interface IQuantityMeasurementRepository
    {
        /// <summary>
        /// Saves a measurement entity to the repository.
        /// </summary>
        void Save(QuantityMeasurementEntity entity);

        /// <summary>
        /// Gets all measurement entities from the repository.
        /// </summary>
        List<QuantityMeasurementEntity> GetAll();

        /// <summary>
        /// Gets a measurement entity by ID.
        /// </summary>
        QuantityMeasurementEntity? GetById(string id);

        /// <summary>
        /// Clears all entities from the repository.
        /// </summary>
        void Clear();
    }
}
