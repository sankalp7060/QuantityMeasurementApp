using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Utils.Validators;

namespace QuantityMeasurementApp.Services
{
    /// <summary>
    /// Generic service class providing measurement operations for any unit type.
    /// UC10: Single service for all measurement categories.
    /// </summary>
    public class GenericMeasurementService
    {
        /// <summary>
        /// Compares two quantities for equality.
        /// </summary>
        /// <typeparam name="T">The unit type (must implement IMeasurable).</typeparam>
        /// <param name="firstQuantity">First quantity.</param>
        /// <param name="secondQuantity">Second quantity.</param>
        /// <returns>True if equal.</returns>
        public bool AreQuantitiesEqual<T>(
            GenericQuantity<T> firstQuantity,
            GenericQuantity<T> secondQuantity
        )
            where T : class, IMeasurable
        {
            if (firstQuantity == null || secondQuantity == null)
                return false;
            return firstQuantity.Equals(secondQuantity);
        }

        /// <summary>
        /// Converts a value from one unit to another.
        /// </summary>
        /// <typeparam name="T">The unit type (must implement IMeasurable).</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="sourceUnit">Source unit.</param>
        /// <param name="targetUnit">Target unit.</param>
        /// <returns>The converted value.</returns>
        public double ConvertValue<T>(double value, T sourceUnit, T targetUnit)
            where T : class, IMeasurable
        {
            var quantity = new GenericQuantity<T>(value, sourceUnit);
            return quantity.ConvertToDouble(targetUnit);
        }

        /// <summary>
        /// Adds two quantities with result in first quantity's unit.
        /// </summary>
        /// <typeparam name="T">The unit type (must implement IMeasurable).</typeparam>
        /// <param name="firstQuantity">First quantity.</param>
        /// <param name="secondQuantity">Second quantity.</param>
        /// <returns>The sum in first quantity's unit.</returns>
        public GenericQuantity<T> AddQuantities<T>(
            GenericQuantity<T> firstQuantity,
            GenericQuantity<T> secondQuantity
        )
            where T : class, IMeasurable
        {
            if (firstQuantity == null || secondQuantity == null)
                throw new ArgumentNullException("Quantities cannot be null");

            return firstQuantity.Add(secondQuantity);
        }

        /// <summary>
        /// Adds two quantities with result in specified unit.
        /// </summary>
        /// <typeparam name="T">The unit type (must implement IMeasurable).</typeparam>
        /// <param name="firstQuantity">First quantity.</param>
        /// <param name="secondQuantity">Second quantity.</param>
        /// <param name="targetUnit">Target unit for result.</param>
        /// <returns>The sum in target unit.</returns>
        public GenericQuantity<T> AddQuantitiesWithTarget<T>(
            GenericQuantity<T> firstQuantity,
            GenericQuantity<T> secondQuantity,
            T targetUnit
        )
            where T : class, IMeasurable
        {
            if (firstQuantity == null || secondQuantity == null)
                throw new ArgumentNullException("Quantities cannot be null");

            return firstQuantity.Add(secondQuantity, targetUnit);
        }

        /// <summary>
        /// Creates a quantity from string input.
        /// </summary>
        /// <typeparam name="T">The unit type (must implement IMeasurable).</typeparam>
        /// <param name="inputValue">The input string.</param>
        /// <param name="unitOfMeasure">The unit of measurement.</param>
        /// <returns>A GenericQuantity if parsing succeeded, null otherwise.</returns>
        public GenericQuantity<T>? CreateQuantityFromString<T>(string? inputValue, T unitOfMeasure)
            where T : class, IMeasurable
        {
            if (!InputValidator.TryParseDouble(inputValue, out double parsedValue))
                return null;

            try
            {
                return new GenericQuantity<T>(parsedValue, unitOfMeasure);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Validates that two quantities from different categories cannot be compared.
        /// This method always returns false as different categories are incompatible.
        /// </summary>
        public bool AreQuantitiesFromDifferentCategoriesEqual<T1, T2>(
            GenericQuantity<T1> first,
            GenericQuantity<T2> second
        )
            where T1 : class, IMeasurable
            where T2 : class, IMeasurable
        {
            // Different measurement categories can never be equal
            return false;
        }
    }
}
