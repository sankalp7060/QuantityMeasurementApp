// Import required DTOs and enums from the model layer
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.DTOs.Enums;

// Define the namespace for business layer interfaces
namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>
    /// Service interface for quantity measurement operations
    /// This interface defines the contract for all business logic operations
    /// It separates the contract from implementation, allowing for:
    /// - Dependency injection
    /// - Unit testing with mocks
    /// - Multiple implementations if needed
    /// </summary>
    public interface IQuantityMeasurementService
    {
        // ==================== CORE OPERATIONS ====================
        // These methods perform the main measurement calculations

        /// <summary>
        /// Compares two quantities for equality
        /// </summary>
        /// <param name="request">DTO containing two quantities to compare</param>
        /// <returns>Response with comparison result and formatted message</returns>
        Task<QuantityResponseDto> CompareQuantitiesAsync(BinaryOperationRequestDto request);

        /// <summary>
        /// Converts a quantity from one unit to another
        /// </summary>
        /// <param name="request">DTO containing source quantity and target unit</param>
        /// <returns>Response with converted value and formatted result</returns>
        Task<QuantityResponseDto> ConvertQuantityAsync(ConversionRequestDto request);

        /// <summary>
        /// Adds two quantities of the same category
        /// </summary>
        /// <param name="request">DTO containing two quantities to add</param>
        /// <returns>Response with sum and formatted result</returns>
        Task<QuantityResponseDto> AddQuantitiesAsync(BinaryOperationRequestDto request);

        /// <summary>
        /// Subtracts second quantity from first quantity
        /// </summary>
        /// <param name="request">DTO containing quantities to subtract (first - second)</param>
        /// <returns>Response with difference and formatted result</returns>
        Task<QuantityResponseDto> SubtractQuantitiesAsync(BinaryOperationRequestDto request);

        /// <summary>
        /// Divides first quantity by second quantity (returns dimensionless ratio)
        /// </summary>
        /// <param name="request">DTO containing quantities to divide (first ÷ second)</param>
        /// <returns>Response with ratio and human-readable interpretation</returns>
        Task<DivisionResponseDto> DivideQuantitiesAsync(BinaryOperationRequestDto request);

        // ==================== HISTORY AND QUERY OPERATIONS ====================
        // These methods retrieve past operation records

        /// <summary>
        /// Gets operation history, optionally filtered by operation type
        /// </summary>
        /// <param name="operation">Optional filter for specific operation type</param>
        /// <returns>Collection of history records</returns>
        Task<IEnumerable<MeasurementHistoryDto>> GetOperationHistoryAsync(
            OperationType? operation = null
        );

        /// <summary>
        /// Gets history records for a specific category
        /// </summary>
        /// <param name="category">Category to filter by (LENGTH, WEIGHT, VOLUME, TEMPERATURE)</param>
        /// <returns>Collection of history records for the category</returns>
        Task<IEnumerable<MeasurementHistoryDto>> GetCategoryHistoryAsync(string category);

        /// <summary>
        /// Gets history records within a specific date range
        /// </summary>
        /// <param name="start">Start date (inclusive)</param>
        /// <param name="end">End date (inclusive)</param>
        /// <returns>Collection of history records in the date range</returns>
        Task<IEnumerable<MeasurementHistoryDto>> GetDateRangeHistoryAsync(
            DateTime start,
            DateTime end
        );

        /// <summary>
        /// Gets history of failed operations only
        /// </summary>
        /// <returns>Collection of failed operation records</returns>
        Task<IEnumerable<MeasurementHistoryDto>> GetErrorHistoryAsync();

        // ==================== STATISTICS ====================
        // These methods provide statistical insights

        /// <summary>
        /// Gets comprehensive statistics about all operations
        /// </summary>
        /// <returns>Dictionary containing various statistics (counts, success rate, etc.)</returns>
        Task<Dictionary<string, object>> GetStatisticsAsync();

        /// <summary>
        /// Gets count of operations for a specific operation type
        /// </summary>
        /// <param name="operation">Operation type to count</param>
        /// <returns>Number of operations of that type</returns>
        Task<int> GetOperationCountAsync(OperationType operation);
    }
}
