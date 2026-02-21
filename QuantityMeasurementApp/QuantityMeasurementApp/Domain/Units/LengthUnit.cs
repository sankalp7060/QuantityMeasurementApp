using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Core.Exceptions;

namespace QuantityMeasurementApp.Domain.Units
{
    /// <summary>
    /// Class representing length units.
    /// UC10: Implements IMeasurable interface for standardized unit behavior.
    /// </summary>
    public class LengthUnit : IMeasurable
    {
        // Private constructor to prevent direct instantiation
        private LengthUnit(string name, string symbol, double conversionFactor)
        {
            Name = name;
            Symbol = symbol;
            ConversionFactor = conversionFactor;
        }

        /// <summary>
        /// Gets the name of the unit.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the symbol of the unit.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Gets the conversion factor to the base unit (feet).
        /// </summary>
        public double ConversionFactor { get; }

        // Static instances for each length unit
        public static readonly LengthUnit FEET = new LengthUnit("feet", "ft", 1.0);
        public static readonly LengthUnit INCH = new LengthUnit("inches", "in", 1.0 / 12.0);
        public static readonly LengthUnit YARD = new LengthUnit("yards", "yd", 3.0);
        public static readonly LengthUnit CENTIMETER = new LengthUnit(
            "centimeters",
            "cm",
            1.0 / (2.54 * 12.0)
        );

        /// <summary>
        /// Gets all available length units.
        /// </summary>
        public static LengthUnit[] GetAllUnits() => new[] { FEET, INCH, YARD, CENTIMETER };

        /// <summary>
        /// Gets the conversion factor for this unit to the base unit (feet).
        /// Implements IMeasurable.GetConversionFactor.
        /// </summary>
        public double GetConversionFactor() => ConversionFactor;

        /// <summary>
        /// Converts a value from this unit to the base unit (feet).
        /// Implements IMeasurable.ToBaseUnit.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value converted to feet.</returns>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public double ToBaseUnit(double value)
        {
            ValidateValue(value);
            return value * ConversionFactor;
        }

        /// <summary>
        /// Converts a value from the base unit (feet) to this unit.
        /// Implements IMeasurable.FromBaseUnit.
        /// </summary>
        /// <param name="valueInBaseUnit">The value in feet to convert.</param>
        /// <returns>The value converted from feet to this unit.</returns>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public double FromBaseUnit(double valueInBaseUnit)
        {
            ValidateValue(valueInBaseUnit);
            return valueInBaseUnit / ConversionFactor;
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
            if (obj is not LengthUnit other)
                return false;
            return Name == other.Name
                && Symbol == other.Symbol
                && Math.Abs(ConversionFactor - other.ConversionFactor) < 0.000001;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(Name, Symbol, ConversionFactor);
    }
}
