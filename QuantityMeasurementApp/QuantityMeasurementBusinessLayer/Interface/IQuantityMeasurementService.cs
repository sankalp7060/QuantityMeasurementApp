using QuantityMeasurementModelLayer.DTOs;

namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>
    /// Interface for quantity measurement business logic.
    /// </summary>
    public interface IQuantityMeasurementService
    {
        // Single operation methods
        Task<QuantityResponse> CompareQuantitiesAsync(BinaryQuantityRequest request);
        Task<QuantityResponse> ConvertQuantityAsync(ConversionRequest request);
        Task<QuantityResponse> AddQuantitiesAsync(BinaryQuantityRequest request);
        Task<QuantityResponse> SubtractQuantitiesAsync(BinaryQuantityRequest request);
        Task<DivisionResponse> DivideQuantitiesAsync(BinaryQuantityRequest request);

        // Batch operation methods
        Task<BatchResponse<QuantityResponse>> CompareQuantitiesBatchAsync(
            BatchBinaryRequest request
        );
        Task<BatchResponse<QuantityResponse>> ConvertQuantitiesBatchAsync(
            BatchConversionRequest request
        );
        Task<BatchResponse<QuantityResponse>> AddQuantitiesBatchAsync(BatchBinaryRequest request);
        Task<BatchResponse<QuantityResponse>> SubtractQuantitiesBatchAsync(
            BatchBinaryRequest request
        );
        Task<BatchResponse<DivisionResponse>> DivideQuantitiesBatchAsync(
            BatchBinaryRequest request
        );

        // Cache management methods
        Task<bool> ClearCacheAsync();
        Task<CacheStatsResponse> GetCacheStatisticsAsync();
    }
}
