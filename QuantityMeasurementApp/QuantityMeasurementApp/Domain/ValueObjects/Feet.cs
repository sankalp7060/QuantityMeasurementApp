using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Domain.ValueObjects
{
    /// <summary>
    /// Legacy Feet class for backward compatibility (UC1).
    /// Maintained to ensure existing code continues to work.
    /// </summary>
    public class Feet
    {
        private readonly double _value;

        /// <summary>
        /// Initializes a new instance of the Feet class.
        /// </summary>
        /// <param name="value">The value in feet.</param>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public Feet(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new InvalidValueException(value);

            _value = value;
        }

        /// <summary>
        /// Gets the value in feet.
        /// </summary>
        public double Value => _value;

        /// <summary>
        /// Determines whether the specified object is equal to the current Feet.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is null || GetType() != obj.GetType())
                return false;

            Feet other = (Feet)obj;
            return Math.Abs(_value - other._value) < 0.000001;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode() => _value.GetHashCode();

        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>String in format "{value} ft".</returns>
        public override string ToString() => $"{_value} ft";

        /// <summary>
        /// Converts this Feet to a Quantity.
        /// </summary>
        /// <returns>A Quantity representing the same value in feet.</returns>
        public Quantity ToQuantity() => new Quantity(_value, LengthUnit.FEET);
    }
}
