using QuantityMeasurementModelLayer.DTOs;

namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>
    /// Interface for quantity measurement business logic.
    /// Follows Interface Segregation Principle.
    /// </summary>
    public interface IQuantityMeasurementService
    {
        /// <summary>
        /// Compares two quantities for equality.
        /// </summary>
        Task<QuantityResponse> CompareQuantitiesAsync(BinaryQuantityRequest request);

        /// <summary>
        /// Converts a quantity to a different unit.
        /// </summary>
        Task<QuantityResponse> ConvertQuantityAsync(ConversionRequest request);

        /// <summary>
        /// Adds two quantities.
        /// </summary>
        Task<QuantityResponse> AddQuantitiesAsync(BinaryQuantityRequest request);

        /// <summary>
        /// Subtracts one quantity from another.
        /// </summary>
        Task<QuantityResponse> SubtractQuantitiesAsync(BinaryQuantityRequest request);

        /// <summary>
        /// Divides one quantity by another.
        /// </summary>
        Task<DivisionResponse> DivideQuantitiesAsync(BinaryQuantityRequest request);
    }
}
