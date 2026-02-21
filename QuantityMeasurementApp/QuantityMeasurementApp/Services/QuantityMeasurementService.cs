using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Domain.ValueObjects;
using QuantityMeasurementApp.Utils.Validators;

namespace QuantityMeasurementApp.Services
{
    /// <summary>
    /// Service class providing measurement operations.
    /// Acts as a facade between UI and domain layer.
    /// All methods are instance methods for thread safety.
    /// </summary>
    public class QuantityMeasurementService
    {
        /// <summary>
        /// Compares two quantities for equality.
        /// </summary>
        /// <param name="firstQuantity">First quantity.</param>
        /// <param name="secondQuantity">Second quantity.</param>
        /// <returns>True if equal.</returns>
        public bool AreQuantitiesEqual(Quantity firstQuantity, Quantity secondQuantity)
        {
            if (firstQuantity == null || secondQuantity == null)
                return false;
            return firstQuantity.Equals(secondQuantity);
        }

        /// <summary>
        /// Converts a value from one unit to another.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="sourceUnit">Source unit.</param>
        /// <param name="targetUnit">Target unit.</param>
        /// <returns>The converted value.</returns>
        public double ConvertValue(double value, LengthUnit sourceUnit, LengthUnit targetUnit)
        {
            var quantity = new Quantity(value, sourceUnit);
            return quantity.ConvertToDouble(targetUnit);
        }

        /// <summary>
        /// Adds two quantities with result in first quantity's unit.
        /// </summary>
        /// <param name="firstQuantity">First quantity.</param>
        /// <param name="secondQuantity">Second quantity.</param>
        /// <returns>The sum in first quantity's unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either quantity is null.</exception>
        public Quantity AddQuantities(Quantity firstQuantity, Quantity secondQuantity)
        {
            if (firstQuantity == null || secondQuantity == null)
                throw new ArgumentNullException("Quantities cannot be null");

            return firstQuantity.Add(secondQuantity);
        }

        /// <summary>
        /// Adds two quantities with result in specified unit.
        /// </summary>
        /// <param name="firstQuantity">First quantity.</param>
        /// <param name="secondQuantity">Second quantity.</param>
        /// <param name="targetUnit">Target unit for result.</param>
        /// <returns>The sum in target unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either quantity is null.</exception>
        public Quantity AddQuantitiesWithTarget(
            Quantity firstQuantity,
            Quantity secondQuantity,
            LengthUnit targetUnit
        )
        {
            if (firstQuantity == null || secondQuantity == null)
                throw new ArgumentNullException("Quantities cannot be null");

            return firstQuantity.Add(secondQuantity, targetUnit);
        }

        /// <summary>
        /// Creates a quantity from string input.
        /// </summary>
        /// <param name="inputValue">The input string.</param>
        /// <param name="unitOfMeasure">The unit of measurement.</param>
        /// <returns>A Quantity if parsing succeeded, null otherwise.</returns>
        public Quantity? CreateQuantityFromString(string? inputValue, LengthUnit unitOfMeasure)
        {
            if (!InputValidator.TryParseDouble(inputValue, out double parsedValue))
                return null;

            try
            {
                return new Quantity(parsedValue, unitOfMeasure);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Legacy method for Feet parsing.
        /// </summary>
        /// <param name="inputValue">The input string.</param>
        /// <returns>A Feet object if parsing succeeded, null otherwise.</returns>
        public Feet? CreateFeetFromString(string? inputValue)
        {
            if (!InputValidator.TryParseDouble(inputValue, out double parsedValue))
                return null;

            try
            {
                return new Feet(parsedValue);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Legacy method for Inch parsing.
        /// </summary>
        /// <param name="inputValue">The input string.</param>
        /// <returns>An Inch object if parsing succeeded, null otherwise.</returns>
        public Inch? CreateInchFromString(string? inputValue)
        {
            if (!InputValidator.TryParseDouble(inputValue, out double parsedValue))
                return null;

            try
            {
                return new Inch(parsedValue);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Legacy method for Feet comparison.
        /// </summary>
        /// <param name="firstFeet">First Feet object.</param>
        /// <param name="secondFeet">Second Feet object.</param>
        /// <returns>True if equal.</returns>
        public bool AreFeetEqual(Feet? firstFeet, Feet? secondFeet)
        {
            if (firstFeet == null || secondFeet == null)
                return false;
            return firstFeet.Equals(secondFeet);
        }

        /// <summary>
        /// Legacy method for Inch comparison.
        /// </summary>
        /// <param name="firstInch">First Inch object.</param>
        /// <param name="secondInch">Second Inch object.</param>
        /// <returns>True if equal.</returns>
        public bool AreInchesEqual(Inch? firstInch, Inch? secondInch)
        {
            if (firstInch == null || secondInch == null)
                return false;
            return firstInch.Equals(secondInch);
        }
    }
}
