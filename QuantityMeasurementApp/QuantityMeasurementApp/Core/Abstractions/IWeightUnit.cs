namespace QuantityMeasurementApp.Core.Abstractions
{
    /// <summary>
    /// Interface defining the contract for weight measurement units.
    /// All weight unit types should implement this.
    /// </summary>
    public interface IWeightUnit
    {
        /// <summary>
        /// Gets the conversion factor to the base unit (kilogram).
        /// </summary>
        double ConversionFactor { get; }

        /// <summary>
        /// Gets the symbol representing the weight unit.
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Gets the full name of the weight unit.
        /// </summary>
        string Name { get; }
    }
}
