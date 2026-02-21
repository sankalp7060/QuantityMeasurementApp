using System;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Represents a quantity with a value and unit of measurement.
    /// This class provides functionality for equality comparison and unit conversion.
    /// </summary>
    public class Quantity
    {
        // Private fields for value and unit
        private readonly double _value;
        private readonly LengthUnit _unit;

        /// <summary>
        /// Initializes a new instance of the Quantity class.
        /// </summary>
        /// <param name="value">The numeric value of the measurement.</param>
        /// <param name="unit">The unit of measurement.</param>
        public Quantity(double value, LengthUnit unit)
        {
            _value = value;
            _unit = unit;
        }

        /// <summary>
        /// Gets the measurement value.
        /// </summary>
        public double Value => _value;

        /// <summary>
        /// Gets the measurement unit.
        /// </summary>
        public LengthUnit Unit => _unit;

        /// <summary>
        /// Converts the current quantity to a target unit.
        /// </summary>
        /// <param name="targetUnit">The unit to convert to.</param>
        /// <returns>A new Quantity object with the converted value and target unit.</returns>
        /// <exception cref="ArgumentException">Thrown when target unit is invalid.</exception>
        public Quantity ConvertTo(LengthUnit targetUnit)
        {
            ValidateUnit(targetUnit);

            double valueInBaseUnit = _value * _unit.GetConversionFactorToFeet();
            double convertedValue = valueInBaseUnit / targetUnit.GetConversionFactorToFeet();

            return new Quantity(convertedValue, targetUnit);
        }

        /// <summary>
        /// Static method to convert a value from one unit to another.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="sourceUnit">The source unit.</param>
        /// <param name="targetUnit">The target unit.</param>
        /// <returns>The converted value as a double.</returns>
        /// <exception cref="ArgumentException">Thrown when value is invalid or units are invalid.</exception>
        public static double Convert(double value, LengthUnit sourceUnit, LengthUnit targetUnit)
        {
            ValidateValue(value);
            ValidateUnit(sourceUnit);
            ValidateUnit(targetUnit);

            Quantity quantity = new Quantity(value, sourceUnit);
            Quantity converted = quantity.ConvertTo(targetUnit);

            return converted.Value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current Quantity object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is null || GetType() != obj.GetType())
                return false;

            Quantity other = (Quantity)obj;

            double thisInFeet = this.ConvertTo(LengthUnit.FEET).Value;
            double otherInFeet = other.ConvertTo(LengthUnit.FEET).Value;

            return LengthUnitExtensions.AreApproximatelyEqual(thisInFeet, otherInFeet);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            double valueInFeet = ConvertTo(LengthUnit.FEET).Value;
            return Math.Round(valueInFeet, 6).GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the Quantity object.
        /// </summary>
        /// <returns>A string in the format "{value} {unitSymbol}".</returns>
        public override string ToString()
        {
            return $"{_value} {_unit.GetUnitSymbol()}";
        }

        /// <summary>
        /// Validates that a unit is not null.
        /// </summary>
        /// <param name="unit">The unit to validate.</param>
        /// <exception cref="ArgumentException">Thrown when unit is invalid.</exception>
        private static void ValidateUnit(LengthUnit unit)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), unit))
            {
                throw new ArgumentException($"Invalid unit: {unit}");
            }
        }

        /// <summary>
        /// Validates that a value is finite.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <exception cref="ArgumentException">Thrown when value is NaN or Infinity.</exception>
        private static void ValidateValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException(
                    $"Invalid value: {value}. Value must be a finite number."
                );
            }
        }
    }
}
