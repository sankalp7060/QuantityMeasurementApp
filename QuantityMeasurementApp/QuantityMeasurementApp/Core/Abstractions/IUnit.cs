namespace QuantityMeasurementApp.Core.Abstractions
{
    /// <summary>
    /// Interface defining the contract for measurement units.
    /// All unit types (Length, Weight, Volume, etc.) should implement this.
    /// </summary>
    public interface IUnit
    {
        /// <summary>
        /// Gets the conversion factor to the base unit.
        /// </summary>
        double ConversionFactor { get; }

        /// <summary>
        /// Gets the symbol representing the unit.
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Gets the full name of the unit.
        /// </summary>
        string Name { get; }
    }
}
