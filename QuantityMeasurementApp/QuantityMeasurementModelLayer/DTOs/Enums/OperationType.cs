using System.Text.Json.Serialization;

namespace QuantityMeasurementModelLayer.DTOs.Enums
{
    /// <summary>
    /// Enum representing the type of measurement operation
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OperationType
    {
        /// <summary>Compare two quantities</summary>
        Compare = 0,

        /// <summary>Convert a quantity to another unit</summary>
        Convert = 1,

        /// <summary>Add two quantities</summary>
        Add = 2,

        /// <summary>Subtract one quantity from another</summary>
        Subtract = 3,

        /// <summary>Divide one quantity by another</summary>
        Divide = 4,
    }
}
