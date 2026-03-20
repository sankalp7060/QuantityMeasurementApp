using System;

namespace QuantityMeasurementApp.Core.Abstractions
{
    /// <summary>
    /// Functional interface to indicate whether a measurable unit supports arithmetic operations.
    /// UC14: Used to determine if addition, subtraction, or division is valid for a given unit type.
    /// </summary>
    public interface ISupportsArithmetic
    {
        /// <summary>
        /// Determines whether this unit supports arithmetic operations.
        /// </summary>
        /// <returns>True if arithmetic operations are supported, false otherwise.</returns>
        bool IsSupported();
    }

    /// <summary>
    /// Interface defining the contract for all measurement units.
    /// UC10: Standardizes unit behavior across all measurement categories.
    /// UC14: Refactored to make arithmetic operations optional through default methods.
    /// </summary>
    public interface IMeasurable
    {
        /// <summary>
        /// Default lambda expression indicating that all measurable units support arithmetic operations by default.
        /// Units like Temperature can override this to return false.
        /// </summary>
        ISupportsArithmetic SupportsArithmetic { get; }

        /// <summary>
        /// Checks if this unit supports arithmetic operations.
        /// </summary>
        /// <returns>True if arithmetic operations are supported, false otherwise.</returns>
        bool SupportsArithmeticOperation()
        {
            return SupportsArithmetic.IsSupported();
        }

        /// <summary>
        /// Validates whether a specific arithmetic operation is supported.
        /// Default implementation does nothing, allowing all operations by default.
        /// Subclasses like TemperatureUnit can override to throw exceptions for unsupported operations.
        /// </summary>
        /// <param name="operation">The arithmetic operation to validate.</param>
        /// <exception cref="NotSupportedException">Thrown when the operation is not supported.</exception>
        void ValidateOperationSupport(string operation)
        {
            // Default implementation allows all operations
        }

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
        /// <returns>The unit symbol (e.g., "ft", "kg", "lb", "°C").</returns>
        string GetSymbol();

        /// <summary>
        /// Gets the full name of the unit.
        /// </summary>
        /// <returns>The full unit name (e.g., "feet", "kilograms", "pounds", "Celsius").</returns>
        string GetName();
    }
}
