using QuantityMeasurementModelLayer.Entities;

namespace QuantityMeasurementRepositoryLayer.Interface
{
    /// <summary>
    /// Interface for quantity measurement repository operations.
    /// </summary>
    public interface IQuantityMeasurementRepository
    {
        /// <summary>
        /// Saves a measurement entity to the repository.
        /// </summary>
        /// <param name="entity">The measurement entity to save.</param>
        void Save(QuantityMeasurementEntity entity);

        /// <summary>
        /// Gets all measurement entities from the repository.
        /// </summary>
        /// <returns>List of all measurement entities.</returns>
        List<QuantityMeasurementEntity> GetAll();

        /// <summary>
        /// Gets a measurement entity by ID.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>The measurement entity if found, null otherwise.</returns>
        QuantityMeasurementEntity? GetById(string id);

        /// <summary>
        /// Clears all entities from the repository.
        /// </summary>
        void Clear();
    }
}
