using System;
using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Core.Exceptions;

namespace QuantityMeasurementApp.Domain.Units
{
    /// <summary>
    /// Class representing temperature units.
    /// UC14: Implements IMeasurable interface with selective arithmetic support.
    /// Temperature does not support addition, subtraction, or division of absolute values.
    /// Conversions use formulas: °F = (°C × 9/5) + 32, °C = (°F - 32) × 5/9
    /// </summary>
    public class TemperatureUnit : IMeasurable
    {
        // Private constructor to prevent direct instantiation
        private TemperatureUnit(
            string name,
            string symbol,
            Func<double, double> toBaseUnitFunc,
            Func<double, double> fromBaseUnitFunc
        )
        {
            Name = name;
            Symbol = symbol;
            _toBaseUnitFunc = toBaseUnitFunc;
            _fromBaseUnitFunc = fromBaseUnitFunc;
        }

        /// <summary>
        /// Lambda expression indicating that TemperatureUnit does NOT support arithmetic operations.
        /// UC14: Temperature cannot perform addition, subtraction, or division of absolute values.
        /// </summary>
        public ISupportsArithmetic SupportsArithmetic { get; } =
            new SupportsArithmeticImpl(() => false);

        /// <summary>
        /// Gets the name of the unit.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the symbol of the unit.
        /// </summary>
        public string Symbol { get; }

        // Private fields for conversion functions
        private readonly Func<double, double> _toBaseUnitFunc;
        private readonly Func<double, double> _fromBaseUnitFunc;

        // Static instances for each temperature unit
        public static readonly TemperatureUnit CELSIUS = new TemperatureUnit(
            "Celsius",
            "°C",
            toBaseUnitFunc: (celsius) => celsius, // Celsius is the base unit
            fromBaseUnitFunc: (celsius) => celsius
        );

        public static readonly TemperatureUnit FAHRENHEIT = new TemperatureUnit(
            "Fahrenheit",
            "°F",
            toBaseUnitFunc: (fahrenheit) => (fahrenheit - 32) * 5 / 9,
            fromBaseUnitFunc: (celsius) => (celsius * 9 / 5) + 32
        );

        public static readonly TemperatureUnit KELVIN = new TemperatureUnit(
            "Kelvin",
            "K",
            toBaseUnitFunc: (kelvin) => kelvin - 273.15,
            fromBaseUnitFunc: (celsius) => celsius + 273.15
        );

        /// <summary>
        /// Gets all available temperature units.
        /// </summary>
        public static TemperatureUnit[] GetAllUnits() => new[] { CELSIUS, FAHRENHEIT, KELVIN };

        /// <summary>
        /// Gets the conversion factor for this unit to the base unit (Celsius).
        /// Note: Temperature conversions are non-linear, so this method throws an exception.
        /// Use ToBaseUnit and FromBaseUnit methods instead.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown for temperature units.</exception>
        public double GetConversionFactor()
        {
            throw new NotSupportedException(
                "Temperature conversions are non-linear. Use ToBaseUnit/FromBaseUnit methods instead."
            );
        }

        /// <summary>
        /// Converts a value from this unit to the base unit (Celsius).
        /// Implements IMeasurable.ToBaseUnit using the stored lambda function.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value converted to Celsius.</returns>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public double ToBaseUnit(double value)
        {
            ValidateValue(value);
            return _toBaseUnitFunc(value);
        }

        /// <summary>
        /// Converts a value from the base unit (Celsius) to this unit.
        /// Implements IMeasurable.FromBaseUnit using the stored lambda function.
        /// </summary>
        /// <param name="valueInBaseUnit">The value in Celsius to convert.</param>
        /// <returns>The value converted from Celsius to this unit.</returns>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public double FromBaseUnit(double valueInBaseUnit)
        {
            ValidateValue(valueInBaseUnit);
            return _fromBaseUnitFunc(valueInBaseUnit);
        }

        /// <summary>
        /// Gets the symbol for the unit.
        /// Implements IMeasurable.GetSymbol.
        /// </summary>
        public string GetSymbol() => Symbol;

        /// <summary>
        /// Gets the full name of the unit.
        /// Implements IMeasurable.GetName.
        /// </summary>
        public string GetName() => Name;

        /// <summary>
        /// Checks if this unit supports arithmetic operations.
        /// Implements IMeasurable.SupportsArithmeticOperation.
        /// </summary>
        public bool SupportsArithmeticOperation() => SupportsArithmetic.IsSupported();

        /// <summary>
        /// Validates whether a specific arithmetic operation is supported.
        /// Implements IMeasurable.ValidateOperationSupport.
        /// </summary>
        /// <param name="operation">The arithmetic operation to validate.</param>
        /// <exception cref="NotSupportedException">Always thrown for temperature units.</exception>
        public void ValidateOperationSupport(string operation)
        {
            throw new NotSupportedException(
                $"Temperature units do not support {operation} operations. "
                    + $"Adding, subtracting, or dividing absolute temperature values is not physically meaningful. "
                    + $"Only equality comparison and unit conversion are supported for temperatures."
            );
        }

        /// <summary>
        /// Returns a string representation of the unit.
        /// </summary>
        public override string ToString() => $"{Name} ({Symbol})";

        /// <summary>
        /// Validates that a value is finite.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        private static void ValidateValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new InvalidValueException(value);
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current unit.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not TemperatureUnit other)
                return false;
            return Name == other.Name && Symbol == other.Symbol;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(Name, Symbol);

        /// <summary>
        /// Private implementation of ISupportsArithmetic.
        /// </summary>
        private class SupportsArithmeticImpl : ISupportsArithmetic
        {
            private readonly Func<bool> _isSupported;

            public SupportsArithmeticImpl(Func<bool> isSupported)
            {
                _isSupported = isSupported;
            }

            public bool IsSupported() => _isSupported();
        }
    }
}
