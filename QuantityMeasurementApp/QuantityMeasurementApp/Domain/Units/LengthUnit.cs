using QuantityMeasurementApp.Core.Exceptions;

namespace QuantityMeasurementApp.Domain.Units
{
    /// <summary>
    /// Enum representing different length units with their properties.
    /// UC3, UC4: Extended unit support with yards and centimeters.
    /// UC8: Standalone unit enum with conversion responsibility.
    /// </summary>
    public enum LengthUnit
    {
        /// <summary>Feet - base unit for length measurements.</summary>
        FEET,

        /// <summary>Inches - 1 foot = 12 inches.</summary>
        INCH,

        /// <summary>Yards - 1 yard = 3 feet.</summary>
        YARD,

        /// <summary>Centimeters - 1 cm = 0.393700787 inches.</summary>
        CENTIMETER,
    }

    /// <summary>
    /// Extension methods for LengthUnit providing conversion functionality.
    /// UC8: All conversion logic is centralized here.
    /// </summary>
    public static class LengthUnitExtensions
    {
        // Tolerance for floating point comparisons
        private const double EPSILON = 0.000001;

        // Conversion factors to feet (base unit)
        private static readonly double[] ToFeetConversionFactors = new double[]
        {
            1.0, // FEET to FEET
            1.0 / 12.0, // INCH to FEET
            3.0, // YARD to FEET
            1.0 / (2.54 * 12.0), // CENTIMETER to FEET
        };

        /// <summary>
        /// Gets the conversion factor for this unit to the base unit (feet).
        /// </summary>
        /// <param name="unit">The length unit.</param>
        /// <returns>The conversion factor to feet.</returns>
        /// <exception cref="InvalidUnitException">Thrown when unit is invalid.</exception>
        public static double GetConversionFactor(this LengthUnit unit)
        {
            int index = (int)unit;
            if (index >= 0 && index < ToFeetConversionFactors.Length)
            {
                return ToFeetConversionFactors[index];
            }
            throw new InvalidUnitException(unit);
        }

        /// <summary>
        /// Converts a value from this unit to the base unit (feet).
        /// </summary>
        /// <param name="unit">The source unit.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value converted to feet.</returns>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public static double ToBaseUnit(this LengthUnit unit, double value)
        {
            ValidateValue(value);
            return value * unit.GetConversionFactor();
        }

        /// <summary>
        /// Converts a value from the base unit (feet) to this unit.
        /// </summary>
        /// <param name="unit">The target unit.</param>
        /// <param name="valueInBaseUnit">The value in feet to convert.</param>
        /// <returns>The value converted from feet to this unit.</returns>
        /// <exception cref="InvalidValueException">Thrown when value is invalid.</exception>
        public static double FromBaseUnit(this LengthUnit unit, double valueInBaseUnit)
        {
            ValidateValue(valueInBaseUnit);
            return valueInBaseUnit / unit.GetConversionFactor();
        }

        /// <summary>
        /// Directly converts a value from one unit to another.
        /// UC5: Unit-to-unit conversion.
        /// </summary>
        /// <param name="sourceUnit">The source unit.</param>
        /// <param name="targetUnit">The target unit.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double ConvertTo(
            this LengthUnit sourceUnit,
            LengthUnit targetUnit,
            double value
        )
        {
            double valueInBase = sourceUnit.ToBaseUnit(value);
            return targetUnit.FromBaseUnit(valueInBase);
        }

        /// <summary>
        /// Gets the symbol for the unit.
        /// </summary>
        /// <param name="unit">The length unit.</param>
        /// <returns>The unit symbol.</returns>
        public static string GetSymbol(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.FEET => "ft",
                LengthUnit.INCH => "in",
                LengthUnit.YARD => "yd",
                LengthUnit.CENTIMETER => "cm",
                _ => unit.ToString().ToLower(),
            };
        }

        /// <summary>
        /// Gets the full name of the unit.
        /// </summary>
        /// <param name="unit">The length unit.</param>
        /// <returns>The full unit name.</returns>
        public static string GetName(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.FEET => "feet",
                LengthUnit.INCH => "inches",
                LengthUnit.YARD => "yards",
                LengthUnit.CENTIMETER => "centimeters",
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
