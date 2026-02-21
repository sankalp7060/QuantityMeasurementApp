using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Utils.Validators;

namespace QuantityMeasurementApp.Services
{
    /// <summary>
    /// Service class providing weight measurement operations.
    /// UC9: Dedicated service for weight measurements.
    /// </summary>
    public class WeightMeasurementService
    {
        /// <summary>
        /// Compares two weights for equality.
        /// </summary>
        /// <param name="firstWeight">First weight.</param>
        /// <param name="secondWeight">Second weight.</param>
        /// <returns>True if equal.</returns>
        public bool AreWeightsEqual(WeightQuantity firstWeight, WeightQuantity secondWeight)
        {
            if (firstWeight == null || secondWeight == null)
                return false;
            return firstWeight.Equals(secondWeight);
        }

        /// <summary>
        /// Converts a weight value from one unit to another.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="sourceUnit">Source unit.</param>
        /// <param name="targetUnit">Target unit.</param>
        /// <returns>The converted value.</returns>
        public double ConvertWeightValue(double value, WeightUnit sourceUnit, WeightUnit targetUnit)
        {
            var weight = new WeightQuantity(value, sourceUnit);
            return weight.ConvertToDouble(targetUnit);
        }

        /// <summary>
        /// Adds two weights with result in first weight's unit.
        /// </summary>
        /// <param name="firstWeight">First weight.</param>
        /// <param name="secondWeight">Second weight.</param>
        /// <returns>The sum in first weight's unit.</returns>
        public WeightQuantity AddWeights(WeightQuantity firstWeight, WeightQuantity secondWeight)
        {
            if (firstWeight == null || secondWeight == null)
                throw new ArgumentNullException("Weights cannot be null");

            return firstWeight.Add(secondWeight);
        }

        /// <summary>
        /// Adds two weights with result in specified unit.
        /// </summary>
        /// <param name="firstWeight">First weight.</param>
        /// <param name="secondWeight">Second weight.</param>
        /// <param name="targetUnit">Target unit for result.</param>
        /// <returns>The sum in target unit.</returns>
        public WeightQuantity AddWeightsWithTarget(
            WeightQuantity firstWeight,
            WeightQuantity secondWeight,
            WeightUnit targetUnit
        )
        {
            if (firstWeight == null || secondWeight == null)
                throw new ArgumentNullException("Weights cannot be null");

            return firstWeight.Add(secondWeight, targetUnit);
        }

        /// <summary>
        /// Creates a weight from string input.
        /// </summary>
        /// <param name="inputValue">The input string.</param>
        /// <param name="unitOfMeasure">The unit of measurement.</param>
        /// <returns>A WeightQuantity if parsing succeeded, null otherwise.</returns>
        public WeightQuantity? CreateWeightFromString(string? inputValue, WeightUnit unitOfMeasure)
        {
            if (!InputValidator.TryParseDouble(inputValue, out double parsedValue))
                return null;

            try
            {
                return new WeightQuantity(parsedValue, unitOfMeasure);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Validates that a weight and length cannot be compared.
        /// This method always returns false as weight and length are incompatible.
        /// </summary>
        /// <param name="weight">The weight quantity.</param>
        /// <param name="length">The length quantity.</param>
        /// <returns>Always false.</returns>
        public bool AreWeightAndLengthEqual(WeightQuantity weight, Quantity length)
        {
            // Weight and length are different categories and cannot be equal
            return false;
        }
    }
}
