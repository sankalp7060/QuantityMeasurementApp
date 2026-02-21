using System;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Standalone enum representing different length units with conversion responsibilities.
    /// This class handles all unit-specific conversion logic to and from the base unit (feet).
    /// Following Single Responsibility Principle, this class is responsible ONLY for unit conversions.
    /// </summary>
    public enum LengthUnit
    {
        FEET, // Base unit - conversion factor 1.0
        INCH, // 1 foot = 12 inches, so 1 inch = 1/12 feet
        YARD, // 1 yard = 3 feet, so 1 yard = 3.0 feet
        CENTIMETER, // 1 cm = 0.393700787 inches, so 1 cm = (0.393700787 / 12) feet
        // More precise: 1 inch = 2.54 cm exactly, so 1 cm = 1/(2.54 * 12) feet
    }

    /// <summary>
    /// Extension class providing conversion functionality to LengthUnit enum.
    /// This class encapsulates all unit conversion logic, making LengthUnit responsible
    /// for knowing how to convert values to and from the base unit (feet).
    /// </summary>
    public static class LengthUnitExtensions
    {
        // Tolerance for floating point comparisons
        public const double EPSILON = 0.000001;

        // Conversion factors to feet (base unit)
        // These are private to encapsulate the conversion factors
        private static readonly double[] ToFeetConversionFactors = new double[]
        {
            1.0, // FEET to FEET conversion factor
            1.0 / 12.0, // INCH to FEET conversion factor (1 inch = 1/12 feet)
            3.0, // YARD to FEET conversion factor (1 yard = 3 feet)
            1.0 / (2.54 * 12.0), // CENTIMETER to FEET conversion factor
            // 1 inch = 2.54 cm exactly
            // So 1 cm = 1/2.54 inches
            // Then 1 cm in feet = (1/2.54) / 12 = 1/(2.54 * 12)
        };

        /// <summary>
        /// Gets the conversion factor from this unit to the base unit (feet).
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>The conversion factor to convert this unit to feet.</returns>
        /// <exception cref="ArgumentException">Thrown when unit is invalid.</exception>
        private static double GetConversionFactorToFeet(this LengthUnit unit)
        {
            int index = (int)unit;
            if (index >= 0 && index < ToFeetConversionFactors.Length)
            {
                return ToFeetConversionFactors[index];
            }
            throw new ArgumentException($"Invalid unit: {unit}");
        }

        /// <summary>
        /// Converts a value from this unit to the base unit (feet).
        /// This method encapsulates the conversion logic for this unit to feet.
        /// </summary>
        /// <param name="unit">The unit of the input value.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value converted to feet.</returns>
        /// <exception cref="ArgumentException">Thrown when value is invalid.</exception>
        public static double ConvertToBaseUnit(this LengthUnit unit, double value)
        {
            ValidateValue(value);
            return value * unit.GetConversionFactorToFeet();
        }

        /// <summary>
        /// Converts a value from the base unit (feet) to this unit.
        /// This method encapsulates the conversion logic from feet to this unit.
        /// </summary>
        /// <param name="unit">The target unit.</param>
        /// <param name="valueInFeet">The value in feet to convert.</param>
        /// <returns>The value converted from feet to this unit.</returns>
        /// <exception cref="ArgumentException">Thrown when value is invalid.</exception>
        public static double ConvertFromBaseUnit(this LengthUnit unit, double valueInFeet)
        {
            ValidateValue(valueInFeet);
            return valueInFeet / unit.GetConversionFactorToFeet();
        }

        /// <summary>
        /// Converts a value from one unit to another using the base unit (feet) as intermediate.
        /// This provides a convenient method for direct unit-to-unit conversion.
        /// </summary>
        /// <param name="sourceUnit">The source unit.</param>
        /// <param name="targetUnit">The target unit.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value converted from source unit to target unit.</returns>
        public static double Convert(
            this LengthUnit sourceUnit,
            LengthUnit targetUnit,
            double value
        )
        {
            double valueInFeet = sourceUnit.ConvertToBaseUnit(value);
            return targetUnit.ConvertFromBaseUnit(valueInFeet);
        }

        /// <summary>
        /// Gets the string representation of the unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>The unit symbol.</returns>
        public static string GetUnitSymbol(this LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.FEET:
                    return "ft";
                case LengthUnit.INCH:
                    return "in";
                case LengthUnit.YARD:
                    return "yd";
                case LengthUnit.CENTIMETER:
                    return "cm";
                default:
                    return unit.ToString().ToLower();
            }
        }

        /// <summary>
        /// Gets the full name of the unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>The full unit name.</returns>
        public static string GetUnitName(this LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.FEET:
                    return "feet";
                case LengthUnit.INCH:
                    return "inches";
                case LengthUnit.YARD:
                    return "yards";
                case LengthUnit.CENTIMETER:
                    return "centimeters";
                default:
                    return unit.ToString().ToLower();
            }
        }

        /// <summary>
        /// Compares two double values with tolerance.
        /// </summary>
        /// <param name="value1">First value.</param>
        /// <param name="value2">Second value.</param>
        /// <param name="epsilon">Tolerance for comparison.</param>
        /// <returns>True if values are approximately equal.</returns>
        public static bool AreApproximatelyEqual(
            double value1,
            double value2,
            double epsilon = EPSILON
        )
        {
            return Math.Abs(value1 - value2) < epsilon;
        }

        /// <summary>
        /// Validates that a value is finite.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <exception cref="ArgumentException">Thrown when value is NaN or Infinity.</exception>
        private static void ValidateValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException(
                    $"Invalid value: {value}. Value must be a finite number."
                );
            }
        }
    }
}
