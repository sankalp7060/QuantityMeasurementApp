using QuantityMeasurementApp.Core.Exceptions;

namespace QuantityMeasurementApp.Utils.Validators
{
    /// <summary>
    /// Utility class for input validation.
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Tries to parse a string to a double.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The parsed value.</param>
        /// <returns>True if parsing succeeded.</returns>
        public static bool TryParseDouble(string? input, out double result)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                result = 0;
                return false;
            }

            return double.TryParse(input, out result);
        }

        /// <summary>
        /// Validates that a value is finite.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value is finite.</returns>
        public static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// Validates a value and throws exception if invalid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public static void ValidateValue(double value)
        {
            if (!IsFinite(value))
            {
                throw new InvalidValueException(value);
            }
        }
    }
}
