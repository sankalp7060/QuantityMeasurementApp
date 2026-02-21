using QuantityMeasurementApp.Core.Exceptions;

namespace QuantityMeasurementApp.Domain.Units
{
    /// <summary>
    /// Enum representing different weight units with their properties.
    /// UC9: Weight measurements with kilogram as base unit.
    /// </summary>
    public enum WeightUnit
    {
        /// <summary>Kilogram - base unit for weight measurements.</summary>
        KILOGRAM,

        /// <summary>Gram - 1 kilogram = 1000 grams.</summary>
        GRAM,

        /// <summary>Pound - 1 pound ≈ 0.453592 kilograms.</summary>
        POUND,
    }

    /// <summary>
    /// Extension methods for WeightUnit providing conversion functionality.
    /// UC9: All weight conversion logic is centralized here.
    /// </summary>
    public static class WeightUnitExtensions
    {
        // Tolerance for floating point comparisons
        private const double EPSILON = 0.000001;

        // Conversion factors to kilograms (base unit)
        private static readonly double[] ToKilogramConversionFactors = new double[]
        {
            1.0, // KILOGRAM to KILOGRAM
            0.001, // GRAM to KILOGRAM (1 g = 0.001 kg)
            0.453592, // POUND to KILOGRAM (1 lb ≈ 0.453592 kg)
        };

        /// <summary>
        /// Gets the conversion factor for this unit to the base unit (kilogram).
        /// </summary>
        /// <param name="unit">The weight unit.</param>
        /// <returns>The conversion factor to kilograms.</returns>
        /// <exception cref="InvalidUnitException">Thrown when unit is invalid.</exception>
        public static double GetConversionFactor(this WeightUnit unit)
        {
            int index = (int)unit;
            if (index >= 0 && index < ToKilogramConversionFactors.Length)
            {
                return ToKilogramConversionFactors[index];
            }
            throw new InvalidUnitException(unit);
        }

        /// <summary>
        /// Converts a value from this unit to the base unit (kilogram).
        /// </summary>
        /// <param name="unit">The source unit.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value converted to kilograms.</returns>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public static double ToBaseUnit(this WeightUnit unit, double value)
        {
            ValidateValue(value);
            return value * unit.GetConversionFactor();
        }

        /// <summary>
        /// Converts a value from the base unit (kilogram) to this unit.
        /// </summary>
        /// <param name="unit">The target unit.</param>
        /// <param name="valueInBaseUnit">The value in kilograms to convert.</param>
        /// <returns>The value converted from kilograms to this unit.</returns>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public static double FromBaseUnit(this WeightUnit unit, double valueInBaseUnit)
        {
            ValidateValue(valueInBaseUnit);
            return valueInBaseUnit / unit.GetConversionFactor();
        }

        /// <summary>
        /// Directly converts a value from one weight unit to another.
        /// </summary>
        /// <param name="sourceUnit">The source unit.</param>
        /// <param name="targetUnit">The target unit.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double ConvertTo(
            this WeightUnit sourceUnit,
            WeightUnit targetUnit,
            double value
        )
        {
            double valueInBase = sourceUnit.ToBaseUnit(value);
            return targetUnit.FromBaseUnit(valueInBase);
        }

        /// <summary>
        /// Gets the symbol for the weight unit.
        /// </summary>
        /// <param name="unit">The weight unit.</param>
        /// <returns>The unit symbol.</returns>
        public static string GetSymbol(this WeightUnit unit)
        {
            return unit switch
            {
                WeightUnit.KILOGRAM => "kg",
                WeightUnit.GRAM => "g",
                WeightUnit.POUND => "lb",
                _ => unit.ToString().ToLower(),
            };
        }

        /// <summary>
        /// Gets the full name of the weight unit.
        /// </summary>
        /// <param name="unit">The weight unit.</param>
        /// <returns>The full unit name.</returns>
        public static string GetName(this WeightUnit unit)
        {
            return unit switch
            {
                WeightUnit.KILOGRAM => "kilograms",
                WeightUnit.GRAM => "grams",
                WeightUnit.POUND => "pounds",
                _ => unit.ToString().ToLower(),
            };
        }

        /// <summary>
        /// Compares two double values with tolerance.
        /// </summary>
        /// <param name="value1">First value.</param>
        /// <param name="value2">Second value.</param>
        /// <returns>True if values are approximately equal.</returns>
        public static bool AreApproximatelyEqual(double value1, double value2)
        {
            return Math.Abs(value1 - value2) < EPSILON;
        }

        /// <summary>
        /// Validates that a value is finite.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        private static void ValidateValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new InvalidValueException(value);
            }
        }
    }
}
