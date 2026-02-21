using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Core.Exceptions;

namespace QuantityMeasurementApp.Domain.Quantities
{
    /// <summary>
    /// Generic quantity class that works with any measurement unit implementing IMeasurable.
    /// UC10: Single class replaces both Quantity and WeightQuantity, eliminating code duplication.
    /// UC12: Added Subtraction and Division operations.
    /// </summary>
    /// <typeparam name="T">The unit type (must implement IMeasurable).</typeparam>
    public class GenericQuantity<T>
        where T : class, IMeasurable
    {
        private readonly double _value;
        private readonly T _unit;

        /// <summary>
        /// Initializes a new instance of the GenericQuantity class.
        /// </summary>
        /// <param name="value">The numeric value of the measurement.</param>
        /// <param name="unit">The unit of measurement.</param>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when unit is null.</exception>
        public GenericQuantity(double value, T unit)
        {
            ValidateValue(value);
            _unit = unit ?? throw new ArgumentNullException(nameof(unit));
            _value = value;
        }

        /// <summary>
        /// Gets the measurement value.
        /// </summary>
        public double Value => _value;

        /// <summary>
        /// Gets the measurement unit.
        /// </summary>
        public T Unit => _unit;

        /// <summary>
        /// Converts this quantity to a target unit.
        /// UC5: Unit conversion feature for any unit type.
        /// </summary>
        /// <param name="targetUnit">The unit to convert to.</param>
        /// <returns>A new GenericQuantity in the target unit.</returns>
        public GenericQuantity<T> ConvertTo(T targetUnit)
        {
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit));

            double valueInBase = _unit.ToBaseUnit(_value);
            double convertedValue = targetUnit.FromBaseUnit(valueInBase);

            return new GenericQuantity<T>(convertedValue, targetUnit);
        }

        /// <summary>
        /// Converts this quantity to a double value in the target unit.
        /// </summary>
        /// <param name="targetUnit">The unit to convert to.</param>
        /// <returns>The converted value as double.</returns>
        public double ConvertToDouble(T targetUnit)
        {
            return ConvertTo(targetUnit).Value;
        }

        #region Addition Operations

        /// <summary>
        /// Adds another quantity to this quantity.
        /// UC6: Addition with result in this quantity's unit.
        /// </summary>
        /// <param name="other">The other quantity to add.</param>
        /// <returns>A new GenericQuantity representing the sum.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        public GenericQuantity<T> Add(GenericQuantity<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return Add(other, _unit);
        }

        /// <summary>
        /// Adds another quantity to this quantity with result in specified unit.
        /// UC7: Addition with explicit target unit.
        /// </summary>
        /// <param name="other">The other quantity to add.</param>
        /// <param name="targetUnit">The unit for the result.</param>
        /// <returns>A new GenericQuantity representing the sum in the target unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        public GenericQuantity<T> Add(GenericQuantity<T> other, T targetUnit)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit));

            double thisInBase = _unit.ToBaseUnit(_value);
            double otherInBase = other._unit.ToBaseUnit(other._value);
            double sumInBase = thisInBase + otherInBase;
            double sumInTarget = targetUnit.FromBaseUnit(sumInBase);

            return new GenericQuantity<T>(sumInTarget, targetUnit);
        }

        /// <summary>
        /// Static method to add two quantities.
        /// </summary>
        /// <param name="first">First quantity.</param>
        /// <param name="second">Second quantity.</param>
        /// <param name="targetUnit">Target unit for result.</param>
        /// <returns>The sum in target unit.</returns>
        public static GenericQuantity<T> Add(
            GenericQuantity<T> first,
            GenericQuantity<T> second,
            T targetUnit
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
        public static GenericQuantity<T> Add(
            double firstValue,
            T firstUnit,
            double secondValue,
            T secondUnit,
            T targetUnit
        )
        {
            var firstQuantity = new GenericQuantity<T>(firstValue, firstUnit);
            var secondQuantity = new GenericQuantity<T>(secondValue, secondUnit);
            return firstQuantity.Add(secondQuantity, targetUnit);
        }

        #endregion

        #region Subtraction Operations (UC12)

        /// <summary>
        /// Subtracts another quantity from this quantity.
        /// UC12: Subtraction with result in this quantity's unit.
        /// </summary>
        /// <param name="other">The other quantity to subtract.</param>
        /// <returns>A new GenericQuantity representing the difference.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        /// <exception cref="ArgumentException">Thrown when categories don't match.</exception>
        public GenericQuantity<T> Subtract(GenericQuantity<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return Subtract(other, _unit);
        }

        /// <summary>
        /// Subtracts another quantity from this quantity with result in specified unit.
        /// UC12: Subtraction with explicit target unit.
        /// </summary>
        /// <param name="other">The other quantity to subtract.</param>
        /// <param name="targetUnit">The unit for the result.</param>
        /// <returns>A new GenericQuantity representing the difference in the target unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other or targetUnit is null.</exception>
        /// <exception cref="ArgumentException">Thrown when categories don't match.</exception>
        public GenericQuantity<T> Subtract(GenericQuantity<T> other, T targetUnit)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit));

            double thisInBase = _unit.ToBaseUnit(_value);
            double otherInBase = other._unit.ToBaseUnit(other._value);
            double differenceInBase = thisInBase - otherInBase;
            double differenceInTarget = targetUnit.FromBaseUnit(differenceInBase);

            return new GenericQuantity<T>(differenceInTarget, targetUnit);
        }

        /// <summary>
        /// Static method to subtract two quantities.
        /// </summary>
        /// <param name="first">First quantity (minuend).</param>
        /// <param name="second">Second quantity (subtrahend).</param>
        /// <param name="targetUnit">Target unit for result.</param>
        /// <returns>The difference in target unit.</returns>
        public static GenericQuantity<T> Subtract(
            GenericQuantity<T> first,
            GenericQuantity<T> second,
            T targetUnit
        )
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            return first.Subtract(second, targetUnit);
        }

        /// <summary>
        /// Static method to subtract two values with units.
        /// </summary>
        /// <param name="firstValue">First value (minuend).</param>
        /// <param name="firstUnit">Unit of first value.</param>
        /// <param name="secondValue">Second value (subtrahend).</param>
        /// <param name="secondUnit">Unit of second value.</param>
        /// <param name="targetUnit">Target unit for result.</param>
        /// <returns>The difference in target unit.</returns>
        public static GenericQuantity<T> Subtract(
            double firstValue,
            T firstUnit,
            double secondValue,
            T secondUnit,
            T targetUnit
        )
        {
            var firstQuantity = new GenericQuantity<T>(firstValue, firstUnit);
            var secondQuantity = new GenericQuantity<T>(secondValue, secondUnit);
            return firstQuantity.Subtract(secondQuantity, targetUnit);
        }

        #endregion

        #region Division Operations (UC12)

        /// <summary>
        /// Divides this quantity by another quantity.
        /// UC12: Division returning a dimensionless scalar ratio.
        /// </summary>
        /// <param name="other">The other quantity (divisor).</param>
        /// <returns>The ratio as a double (dimensionless).</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        /// <exception cref="ArgumentException">Thrown when categories don't match.</exception>
        /// <exception cref="DivideByZeroException">Thrown when divisor is zero.</exception>
        public double Divide(GenericQuantity<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            double thisInBase = _unit.ToBaseUnit(_value);
            double otherInBase = other._unit.ToBaseUnit(other._value);

            if (Math.Abs(otherInBase) < 0.000000001)
                throw new DivideByZeroException("Cannot divide by zero quantity");

            return thisInBase / otherInBase;
        }

        /// <summary>
        /// Static method to divide two quantities.
        /// </summary>
        /// <param name="first">First quantity (dividend).</param>
        /// <param name="second">Second quantity (divisor).</param>
        /// <returns>The ratio as a double (dimensionless).</returns>
        public static double Divide(GenericQuantity<T> first, GenericQuantity<T> second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            return first.Divide(second);
        }

        /// <summary>
        /// Static method to divide two values with units.
        /// </summary>
        /// <param name="firstValue">First value (dividend).</param>
        /// <param name="firstUnit">Unit of first value.</param>
        /// <param name="secondValue">Second value (divisor).</param>
        /// <param name="secondUnit">Unit of second value.</param>
        /// <returns>The ratio as a double (dimensionless).</returns>
        public static double Divide(
            double firstValue,
            T firstUnit,
            double secondValue,
            T secondUnit
        )
        {
            var firstQuantity = new GenericQuantity<T>(firstValue, firstUnit);
            var secondQuantity = new GenericQuantity<T>(secondValue, secondUnit);
            return firstQuantity.Divide(secondQuantity);
        }

        #endregion

        /// <summary>
        /// Determines whether the specified object is equal to the current quantity.
        /// UC1-UC4, UC9: Value-based equality across all units of same category.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object? other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null)
                return false;

            if (other.GetType() != GetType())
                return false;

            var otherQuantity = (GenericQuantity<T>)other;

            double thisInBase = _unit.ToBaseUnit(_value);
            double otherInBase = otherQuantity._unit.ToBaseUnit(otherQuantity._value);

            return AreApproximatelyEqual(thisInBase, otherInBase);
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

        private static void ValidateValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new InvalidValueException(value);
            }
        }

        private static bool AreApproximatelyEqual(double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.000001;
        }
    }
}
