using System;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Enum representing different length units with their conversion factors to feet (base unit)
    /// </summary>
    public enum LengthUnit
    {
        FEET, // Base unit - conversion factor 1.0
        INCH, // 1 foot = 12 inches, so 1 inch = 1/12 feet
        YARD, // 1 yard = 3 feet, so 1 yard = 3.0 feet
        CENTIMETER, // 1 cm = 0.393700787 inches, so 1 cm = (0.393700787 / 12) feet
        // More precise conversion: 1 inch = 2.54 cm exactly
    }

    /// <summary>
    /// Non-static class for unit conversion operations (instance-based)
    /// </summary>
    public class UnitConverter
    {
        // Conversion factors to feet (base unit)
        // All units are converted to feet for consistent comparison
        private readonly double[] ToFeetConversionFactors = new double[]
        {
            1.0, // FEET to FEET conversion factor
            1.0 / 12.0, // INCH to FEET conversion factor (1 inch = 1/12 feet)
            3.0, // YARD to FEET conversion factor (1 yard = 3 feet)
            1.0 / (2.54 * 12.0), // CENTIMETER to FEET conversion factor
            // 1 inch = 2.54 cm exactly
            // So 1 cm = 1/2.54 inches
            // Then 1 cm in feet = (1/2.54) / 12 = 1/(2.54 * 12)
        };

        // Tolerance for floating point comparisons
        public const double EPSILON = 0.000001;

        // Get the conversion factor to convert this unit to feet
        public double GetConversionFactorToFeet(LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.FEET:
                    return ToFeetConversionFactors[0];
                case LengthUnit.INCH:
                    return ToFeetConversionFactors[1];
                case LengthUnit.YARD:
                    return ToFeetConversionFactors[2];
                case LengthUnit.CENTIMETER:
                    return ToFeetConversionFactors[3];
                default:
                    throw new ArgumentException($"Invalid unit: {unit}");
            }
        }

        // Compare two double values with tolerance
        public bool AreApproximatelyEqual(double value1, double value2, double epsilon = EPSILON)
        {
            return Math.Abs(value1 - value2) < epsilon;
        }
    }

    /// <summary>
    /// Static class for UI helper methods (can be shared across users)
    /// </summary>
    public static class LengthUnitExtensions
    {
        // Get the string representation of the unit
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

        // Get the full name of the unit
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
    }
}
