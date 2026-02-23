using System;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Represents a quantity with a value and unit of measurement.
    /// This class provides functionality for equality comparison, unit conversion, and addition operations.
    /// </summary>
    public class Quantity
    {
        // Private fields for value and unit
        private readonly double _value;
        private readonly LengthUnit _unit;
        private readonly UnitConverter _unitConverter;

        /// <summary>
        /// Initializes a new instance of the Quantity class.
        /// </summary>
        /// <param name="value">The numeric value of the measurement.</param>
        /// <param name="unit">The unit of measurement.</param>
        public Quantity(double value, LengthUnit unit)
        {
            ValidateValue(value);
            ValidateUnit(unit);

            _value = value;
            _unit = unit;
            _unitConverter = new UnitConverter();
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

            double valueInBaseUnit = _value * _unitConverter.GetConversionFactorToFeet(_unit);
            double convertedValue =
                valueInBaseUnit / _unitConverter.GetConversionFactorToFeet(targetUnit);

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
        /// Adds this quantity to another quantity and returns the result in the specified target unit.
        /// </summary>
        /// <param name="other">The other quantity to add.</param>
        /// <param name="targetUnit">The unit for the result.</param>
        /// <returns>A new Quantity object representing the sum in the target unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other quantity is null.</exception>
        /// <exception cref="ArgumentException">Thrown when target unit is invalid.</exception>
        public Quantity Add(Quantity other, LengthUnit targetUnit)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Other quantity cannot be null");

            ValidateUnit(targetUnit);

            // Convert both quantities to base unit (feet)
            double thisInBase = this.ConvertTo(LengthUnit.FEET).Value;
            double otherInBase = other.ConvertTo(LengthUnit.FEET).Value;

            // Add in base unit
            double sumInBase = thisInBase + otherInBase;

            // Convert sum to target unit
            double sumInTarget = sumInBase / _unitConverter.GetConversionFactorToFeet(targetUnit);

            return new Quantity(sumInTarget, targetUnit);
        }

        /// <summary>
        /// Static method to add two quantities and return the result in the specified target unit.
        /// </summary>
        /// <param name="quantity1">First quantity.</param>
        /// <param name="quantity2">Second quantity.</param>
        /// <param name="targetUnit">The unit for the result.</param>
        /// <returns>A new Quantity object representing the sum in the target unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either quantity is null.</exception>
        /// <exception cref="ArgumentException">Thrown when target unit is invalid.</exception>
        public static Quantity Add(Quantity quantity1, Quantity quantity2, LengthUnit targetUnit)
        {
            if (quantity1 == null)
                throw new ArgumentNullException(nameof(quantity1), "First quantity cannot be null");
            if (quantity2 == null)
                throw new ArgumentNullException(
                    nameof(quantity2),
                    "Second quantity cannot be null"
                );

            return quantity1.Add(quantity2, targetUnit);
        }

        /// <summary>
        /// Static method to add two values with their units and return the result in the specified target unit.
        /// </summary>
        /// <param name="value1">First value.</param>
        /// <param name="unit1">Unit of first value.</param>
        /// <param name="value2">Second value.</param>
        /// <param name="unit2">Unit of second value.</param>
        /// <param name="targetUnit">The unit for the result.</param>
        /// <returns>A new Quantity object representing the sum in the target unit.</returns>
        /// <exception cref="ArgumentException">Thrown when values or units are invalid.</exception>
        public static Quantity Add(
            double value1,
            LengthUnit unit1,
            double value2,
            LengthUnit unit2,
            LengthUnit targetUnit
        )
        {
            Quantity quantity1 = new Quantity(value1, unit1);
            Quantity quantity2 = new Quantity(value2, unit2);

            return quantity1.Add(quantity2, targetUnit);
        }

        /// <summary>
        /// Adds another quantity to this quantity and returns the result in the unit of this quantity.
        /// </summary>
        /// <param name="other">The other quantity to add.</param>
        /// <returns>A new Quantity object representing the sum in this quantity's unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other quantity is null.</exception>
        public Quantity Add(Quantity other)
        {
            return Add(other, this._unit);
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

            return _unitConverter.AreApproximatelyEqual(thisInFeet, otherInFeet);
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
        /// Validates that a unit is valid.
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
