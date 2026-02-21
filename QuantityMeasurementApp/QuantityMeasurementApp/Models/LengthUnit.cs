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
    }

    /// <summary>
    /// Extension class to add conversion factor functionality to LengthUnit enum
    /// </summary>
    public static class LengthUnitExtensions
    {
        // Conversion factors to feet (base unit)
        private static readonly double[] ToFeetConversionFactors = new double[]
        {
            1.0, // FEET to FEET conversion factor
            1.0 / 12.0, // INCH to FEET conversion factor (1 inch = 1/12 feet)
        };

        // Get the conversion factor to convert this unit to feet
        public static double GetConversionFactorToFeet(this LengthUnit unit)
        {
            int index = (int)unit;
            if (index >= 0 && index < ToFeetConversionFactors.Length)
            {
                return ToFeetConversionFactors[index];
            }
            throw new ArgumentException($"Invalid unit: {unit}");
        }

        // Get the string representation of the unit
        public static string GetUnitSymbol(this LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.FEET:
                    return "ft";
                case LengthUnit.INCH:
                    return "in";
                default:
                    return unit.ToString().ToLower();
            }
        }
    }
}
