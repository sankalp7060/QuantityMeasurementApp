namespace QuantityMeasurementApp.Core.Abstractions
{
    /// <summary>
    /// Interface defining the contract for all measurement units.
    /// Any unit enum (LengthUnit, WeightUnit, etc.) must implement this interface.
    /// UC10: Standardizes unit behavior across all measurement categories.
    /// </summary>
    public interface IMeasurable
    {
        /// <summary>
        /// Gets the conversion factor to the base unit for this measurement category.
        /// </summary>
        /// <returns>The conversion factor to convert this unit to the base unit.</returns>
        double GetConversionFactor();

        /// <summary>
        /// Converts a value from this unit to the base unit.
        /// </summary>
        /// <param name="value">The value in this unit.</param>
        /// <returns>The value converted to the base unit.</returns>
        double ToBaseUnit(double value);

        /// <summary>
        /// Converts a value from the base unit to this unit.
        /// </summary>
        /// <param name="valueInBaseUnit">The value in the base unit.</param>
        /// <returns>The value converted from base unit to this unit.</returns>
        double FromBaseUnit(double valueInBaseUnit);

        /// <summary>
        /// Gets the symbol representing the unit.
        /// </summary>
        /// <returns>The unit symbol (e.g., "ft", "kg", "lb").</returns>
        string GetSymbol();

        /// <summary>
        /// Gets the full name of the unit.
        /// </summary>
        /// <returns>The full unit name (e.g., "feet", "kilograms", "pounds").</returns>
        string GetName();
    }
}
