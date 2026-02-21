using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Domain.Quantities
{
    /// <summary>
    /// Represents a quantity with a value and unit of measurement.
    /// This is the core domain class that handles all measurement operations.
    /// All methods are instance methods to ensure thread safety.
    /// </summary>
    public class Quantity
    {
        private readonly double _value;
        private readonly LengthUnit _unit;

        /// <summary>
        /// Initializes a new instance of the Quantity class.
        /// </summary>
        /// <param name="value">The numeric value of the measurement.</param>
        /// <param name="unit">The unit of measurement.</param>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        /// <exception cref="InvalidUnitException">Thrown when unit is invalid.</exception>
        public Quantity(double value, LengthUnit unit)
        {
            ValidateValue(value);
            ValidateUnit(unit);

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
        /// Converts this quantity to a target unit.
        /// UC5: Unit conversion feature.
        /// </summary>
        /// <param name="targetUnit">The unit to convert to.</param>
        /// <returns>A new Quantity in the target unit.</returns>
        public Quantity ConvertTo(LengthUnit targetUnit)
        {
            ValidateUnit(targetUnit);
            double valueInBase = _unit.ToBaseUnit(_value);
            double convertedValue = targetUnit.FromBaseUnit(valueInBase);
            return new Quantity(convertedValue, targetUnit);
        }

        /// <summary>
        /// Converts this quantity to a double value in the target unit.
        /// Thread-safe instance method for conversion.
        /// </summary>
        /// <param name="targetUnit">The unit to convert to.</param>
        /// <returns>The converted value as double.</returns>
        public double ConvertToDouble(LengthUnit targetUnit)
        {
            return ConvertTo(targetUnit).Value;
        }

        /// <summary>
        /// Adds another quantity to this quantity.
        /// UC6: Addition with result in this quantity's unit.
        /// </summary>
        /// <param name="other">The other quantity to add.</param>
        /// <returns>A new Quantity representing the sum.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        public Quantity Add(Quantity other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return Add(other, this._unit);
        }

        /// <summary>
        /// Adds another quantity to this quantity with result in specified unit.
        /// UC7: Addition with explicit target unit.
        /// </summary>
        /// <param name="other">The other quantity to add.</param>
        /// <param name="targetUnit">The unit for the result.</param>
        /// <returns>A new Quantity representing the sum in the target unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        public Quantity Add(Quantity other, LengthUnit targetUnit)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            ValidateUnit(targetUnit);

            double thisQuantityInBase = _unit.ToBaseUnit(_value);
            double otherQuantityInBase = other._unit.ToBaseUnit(other._value);
            double sumInBase = thisQuantityInBase + otherQuantityInBase;
            double sumInTarget = targetUnit.FromBaseUnit(sumInBase);

            return new Quantity(sumInTarget, targetUnit);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current quantity.
        /// UC1-UC4: Value-based equality across all units.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object? other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null || GetType() != other.GetType())
                return false;

            Quantity otherQuantity = (Quantity)other;
            double thisQuantityInBase = _unit.ToBaseUnit(_value);
            double otherQuantityInBase = otherQuantity._unit.ToBaseUnit(otherQuantity._value);

            return LengthUnitExtensions.AreApproximatelyEqual(
                thisQuantityInBase,
                otherQuantityInBase
            );
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
        /// Returns a string representation of the quantity.
        /// </summary>
        /// <returns>String in format "{value} {symbol}".</returns>
        public override string ToString()
        {
            return $"{_value} {_unit.GetSymbol()}";
        }

        private static void ValidateUnit(LengthUnit unit)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), unit))
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
