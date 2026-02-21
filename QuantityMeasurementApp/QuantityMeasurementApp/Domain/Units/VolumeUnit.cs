using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Core.Exceptions;

namespace QuantityMeasurementApp.Domain.Units
{
    /// <summary>
    /// Class representing volume units.
    /// UC11: Implements IMeasurable interface for standardized unit behavior.
    /// Litre (L) is the base unit for volume measurements.
    /// </summary>
    public class VolumeUnit : IMeasurable
    {
        // Private constructor to prevent direct instantiation
        private VolumeUnit(string name, string symbol, double conversionFactor)
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
        /// Gets the conversion factor to the base unit (litre).
        /// </summary>
        public double ConversionFactor { get; }

        // Static instances for each volume unit
        public static readonly VolumeUnit LITRE = new VolumeUnit("litres", "L", 1.0);
        public static readonly VolumeUnit MILLILITRE = new VolumeUnit("millilitres", "mL", 0.001);
        public static readonly VolumeUnit GALLON = new VolumeUnit("gallons", "gal", 3.78541);

        /// <summary>
        /// Gets all available volume units.
        /// </summary>
        public static VolumeUnit[] GetAllUnits() => new[] { LITRE, MILLILITRE, GALLON };

        /// <summary>
        /// Gets the conversion factor for this unit to the base unit (litre).
        /// Implements IMeasurable.GetConversionFactor.
        /// </summary>
        public double GetConversionFactor() => ConversionFactor;

        /// <summary>
        /// Converts a value from this unit to the base unit (litre).
        /// Implements IMeasurable.ToBaseUnit.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value converted to litres.</returns>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public double ToBaseUnit(double value)
        {
            ValidateValue(value);
            return value * ConversionFactor;
        }

        /// <summary>
        /// Converts a value from the base unit (litre) to this unit.
        /// Implements IMeasurable.FromBaseUnit.
        /// </summary>
        /// <param name="valueInBaseUnit">The value in litres to convert.</param>
        /// <returns>The value converted from litres to this unit.</returns>
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
            if (obj is not VolumeUnit other)
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
