using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Core.Exceptions;

namespace QuantityMeasurementApp.Domain.Quantities
{
    /// <summary>
    /// Generic quantity class that works with any measurement unit implementing IMeasurable.
    /// UC10: Single class replaces both Quantity and WeightQuantity, eliminating code duplication.
    /// UC12: Added Subtraction and Division operations.
    /// UC13: Centralized arithmetic logic to enforce DRY principle.
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

        #region Private Validation and Arithmetic Helpers (UC13)

        /// <summary>
        /// Enum representing arithmetic operations for centralized dispatch.
        /// UC13: Uses lambda expressions for clean operation definitions.
        /// </summary>
        private enum ArithmeticOperation
        {
            ADD,
            SUBTRACT,
            DIVIDE,
        }

        /// <summary>
        /// Validates arithmetic operands for null, and finiteness.
        /// Note: Category compatibility is enforced by the generic type parameter T.
        /// UC13: Centralized validation for all arithmetic operations.
        /// </summary>
        /// <param name="other">The other quantity.</param>
        /// <param name="targetUnit">The target unit (may be null for division).</param>
        /// <param name="isTargetUnitRequired">Whether target unit validation is required.</param>
        /// <exception cref="ArgumentNullException">Thrown when other is null or targetUnit required but null.</exception>
        /// <exception cref="InvalidValueException">Thrown when values are invalid.</exception>
        private void ValidateArithmeticOperands(
            GenericQuantity<T> other,
            T? targetUnit,
            bool isTargetUnitRequired
        )
        {
            // Validate other quantity
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Other quantity cannot be null");

            // Validate target unit if required
            if (isTargetUnitRequired && targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");

            // Note: We don't need to validate that targetUnit is a valid unit because
            // the type parameter T ensures it's the correct type.

            // Validate values are finite
            if (!IsFinite(_value) || !IsFinite(other._value))
                throw new InvalidValueException("Both quantities must have finite values");
        }

        /// <summary>
        /// Performs arithmetic operation in base unit and returns result in base unit.
        /// UC13: Centralized arithmetic execution using lambda dispatch.
        /// </summary>
        /// <param name="other">The other quantity.</param>
        /// <param name="operation">The arithmetic operation to perform.</param>
        /// <returns>The result in base unit.</returns>
        /// <exception cref="DivideByZeroException">Thrown when dividing by zero.</exception>
        private double PerformBaseArithmetic(
            GenericQuantity<T> other,
            ArithmeticOperation operation
        )
        {
            // Convert both to base unit
            double thisInBase = _unit.ToBaseUnit(_value);
            double otherInBase = other._unit.ToBaseUnit(other._value);

            // Perform operation using lambda dispatch
            return operation switch
            {
                ArithmeticOperation.ADD => thisInBase + otherInBase,
                ArithmeticOperation.SUBTRACT => thisInBase - otherInBase,
                ArithmeticOperation.DIVIDE when Math.Abs(otherInBase) < 0.000000001 =>
                    throw new DivideByZeroException("Cannot divide by zero quantity"),
                ArithmeticOperation.DIVIDE => thisInBase / otherInBase,
                _ => throw new InvalidOperationException($"Unknown operation: {operation}"),
            };
        }

        /// <summary>
        /// Rounds a value to two decimal places.
        /// UC13: Consistent rounding for add/subtract results.
        /// </summary>
        /// <param name="value">The value to round.</param>
        /// <returns>The rounded value.</returns>
        private static double RoundToTwoDecimals(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Checks if a value is finite.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if value is finite.</returns>
        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        #endregion

        #region Addition Operations (UC6, UC7) - Refactored for UC13

        /// <summary>
        /// Adds another quantity to this quantity.
        /// UC6: Addition with result in this quantity's unit.
        /// UC13: Delegates to centralized arithmetic helpers.
        /// </summary>
        /// <param name="other">The other quantity to add.</param>
        /// <returns>A new GenericQuantity representing the sum.</returns>
        public GenericQuantity<T> Add(GenericQuantity<T> other)
        {
            return Add(other, _unit);
        }

        /// <summary>
        /// Adds another quantity to this quantity with result in specified unit.
        /// UC7: Addition with explicit target unit.
        /// UC13: Delegates to centralized arithmetic helpers.
        /// </summary>
        /// <param name="other">The other quantity to add.</param>
        /// <param name="targetUnit">The unit for the result.</param>
        /// <returns>A new GenericQuantity representing the sum in the target unit.</returns>
        public GenericQuantity<T> Add(GenericQuantity<T> other, T targetUnit)
        {
            // Validate operands
            ValidateArithmeticOperands(other, targetUnit, true);

            // Perform addition in base unit
            double resultInBase = PerformBaseArithmetic(other, ArithmeticOperation.ADD);

            // Convert result to target unit and round
            double resultInTarget = targetUnit.FromBaseUnit(resultInBase);
            double roundedResult = RoundToTwoDecimals(resultInTarget);

            return new GenericQuantity<T>(roundedResult, targetUnit);
        }

        /// <summary>
        /// Static method to add two quantities.
        /// </summary>
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

        #region Subtraction Operations (UC12) - Refactored for UC13

        /// <summary>
        /// Subtracts another quantity from this quantity.
        /// UC12: Subtraction with result in this quantity's unit.
        /// UC13: Delegates to centralized arithmetic helpers.
        /// </summary>
        /// <param name="other">The other quantity to subtract.</param>
        /// <returns>A new GenericQuantity representing the difference.</returns>
        public GenericQuantity<T> Subtract(GenericQuantity<T> other)
        {
            return Subtract(other, _unit);
        }

        /// <summary>
        /// Subtracts another quantity from this quantity with result in specified unit.
        /// UC12: Subtraction with explicit target unit.
        /// UC13: Delegates to centralized arithmetic helpers.
        /// </summary>
        /// <param name="other">The other quantity to subtract.</param>
        /// <param name="targetUnit">The unit for the result.</param>
        /// <returns>A new GenericQuantity representing the difference in the target unit.</returns>
        public GenericQuantity<T> Subtract(GenericQuantity<T> other, T targetUnit)
        {
            // Validate operands
            ValidateArithmeticOperands(other, targetUnit, true);

            // Perform subtraction in base unit
            double resultInBase = PerformBaseArithmetic(other, ArithmeticOperation.SUBTRACT);

            // Convert result to target unit and round
            double resultInTarget = targetUnit.FromBaseUnit(resultInBase);
            double roundedResult = RoundToTwoDecimals(resultInTarget);

            return new GenericQuantity<T>(roundedResult, targetUnit);
        }

        /// <summary>
        /// Static method to subtract two quantities.
        /// </summary>
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

        #region Division Operations (UC12) - Refactored for UC13

        /// <summary>
        /// Divides this quantity by another quantity.
        /// UC12: Division returning a dimensionless scalar ratio.
        /// UC13: Delegates to centralized arithmetic helpers.
        /// </summary>
        /// <param name="other">The other quantity (divisor).</param>
        /// <returns>The ratio as a double (dimensionless).</returns>
        public double Divide(GenericQuantity<T> other)
        {
            // Validate operands (no target unit needed for division)
            ValidateArithmeticOperands(other, null, false);

            // Perform division in base unit
            double resultInBase = PerformBaseArithmetic(other, ArithmeticOperation.DIVIDE);

            // Return raw result (no rounding, no unit conversion)
            return resultInBase;
        }

        /// <summary>
        /// Static method to divide two quantities.
        /// </summary>
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
