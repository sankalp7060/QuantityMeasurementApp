using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Domain.Quantities
{
    /// <summary>
    /// Represents a weight quantity with a value and unit of measurement.
    /// UC9: Weight measurements with kilogram as base unit.
    /// This class follows the same pattern as Quantity for length.
    /// </summary>
    public class WeightQuantity
    {
        private readonly double _value;
        private readonly WeightUnit _unit;

        /// <summary>
        /// Initializes a new instance of the WeightQuantity class.
        /// </summary>
        /// <param name="value">The numeric value of the weight.</param>
        /// <param name="unit">The unit of measurement.</param>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        /// <exception cref="InvalidUnitException">Thrown when unit is invalid.</exception>
        public WeightQuantity(double value, WeightUnit unit)
        {
            ValidateValue(value);
            ValidateUnit(unit);

            _value = value;
            _unit = unit;
        }

        /// <summary>
        /// Gets the weight value.
        /// </summary>
        public double Value => _value;

        /// <summary>
        /// Gets the weight unit.
        /// </summary>
        public WeightUnit Unit => _unit;

        /// <summary>
        /// Converts this weight to a target unit.
        /// </summary>
        /// <param name="targetUnit">The unit to convert to.</param>
        /// <returns>A new WeightQuantity in the target unit.</returns>
        public WeightQuantity ConvertTo(WeightUnit targetUnit)
        {
            ValidateUnit(targetUnit);
            double valueInBase = _unit.ToBaseUnit(_value);
            double convertedValue = targetUnit.FromBaseUnit(valueInBase);
            return new WeightQuantity(convertedValue, targetUnit);
        }

        /// <summary>
        /// Converts this weight to a double value in the target unit.
        /// </summary>
        /// <param name="targetUnit">The unit to convert to.</param>
        /// <returns>The converted value as double.</returns>
        public double ConvertToDouble(WeightUnit targetUnit)
        {
            return ConvertTo(targetUnit).Value;
        }

        /// <summary>
        /// Adds another weight to this weight.
        /// UC6 pattern: Result in this weight's unit.
        /// </summary>
        /// <param name="other">The other weight to add.</param>
        /// <returns>A new WeightQuantity representing the sum.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        public WeightQuantity Add(WeightQuantity other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return Add(other, this._unit);
        }

        /// <summary>
        /// Adds another weight to this weight with result in specified unit.
        /// UC7 pattern: Addition with explicit target unit.
        /// </summary>
        /// <param name="other">The other weight to add.</param>
        /// <param name="targetUnit">The unit for the result.</param>
        /// <returns>A new WeightQuantity representing the sum in the target unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        public WeightQuantity Add(WeightQuantity other, WeightUnit targetUnit)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            ValidateUnit(targetUnit);

            double thisInBase = _unit.ToBaseUnit(_value);
            double otherInBase = other._unit.ToBaseUnit(other._value);
            double sumInBase = thisInBase + otherInBase;
            double sumInTarget = targetUnit.FromBaseUnit(sumInBase);

            return new WeightQuantity(sumInTarget, targetUnit);
        }

        /// <summary>
        /// Static method to add two weights.
        /// </summary>
        /// <param name="first">First weight.</param>
        /// <param name="second">Second weight.</param>
        /// <param name="targetUnit">Target unit for result.</param>
        /// <returns>The sum in target unit.</returns>
        public static WeightQuantity Add(
            WeightQuantity first,
            WeightQuantity second,
            WeightUnit targetUnit
        )
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            return first.Add(second, targetUnit);
        }

        /// <summary>
        /// Static method to add two values with units.
        /// </summary>
        /// <param name="firstValue">First value.</param>
        /// <param name="firstUnit">Unit of first value.</param>
        /// <param name="secondValue">Second value.</param>
        /// <param name="secondUnit">Unit of second value.</param>
        /// <param name="targetUnit">Target unit for result.</param>
        /// <returns>The sum in target unit.</returns>
        public static WeightQuantity Add(
            double firstValue,
            WeightUnit firstUnit,
            double secondValue,
            WeightUnit secondUnit,
            WeightUnit targetUnit
        )
        {
            var firstQuantity = new WeightQuantity(firstValue, firstUnit);
            var secondQuantity = new WeightQuantity(secondValue, secondUnit);
            return firstQuantity.Add(secondQuantity, targetUnit);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current weight.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object? other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null || GetType() != other.GetType())
                return false;

            WeightQuantity otherWeight = (WeightQuantity)other;
            double thisInBase = _unit.ToBaseUnit(_value);
            double otherInBase = otherWeight._unit.ToBaseUnit(otherWeight._value);

            return WeightUnitExtensions.AreApproximatelyEqual(thisInBase, otherInBase);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            double valueInBase = _unit.ToBaseUnit(_value);
            return Math.Round(valueInBase, 6).GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the weight.
        /// </summary>
        /// <returns>String in format "{value} {symbol}".</returns>
        public override string ToString()
        {
            return $"{_value} {_unit.GetSymbol()}";
        }

        private static void ValidateUnit(WeightUnit unit)
        {
            if (!Enum.IsDefined(typeof(WeightUnit), unit))
            {
                throw new InvalidUnitException(unit);
            }
        }

        private static void ValidateValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new InvalidValueException(value);
            }
        }
    }
}
