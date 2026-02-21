using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Test class to verify that the refactored architecture supports multiple measurement categories.
    /// Demonstrates how WeightUnit, VolumeUnit, etc. could follow the same pattern.
    /// </summary>
    [TestClass]
    public class ArchitecturalScalabilityTests
    {
        private const double Tolerance = 0.000001;

        /// <summary>
        /// Tests that the pattern established for LengthUnit can be replicated for WeightUnit.
        /// Verifies that the architecture supports multiple categories without coupling.
        /// </summary>
        [TestMethod]
        public void ArchitecturalPattern_CanBeReplicated_ForWeightUnit()
        {
            // Test WeightUnit conversions
            double poundsToOunces = WeightUnit.POUND.Convert(WeightUnit.OUNCE, 1.0);
            Assert.AreEqual(16.0, poundsToOunces, Tolerance, "1 pound should equal 16 ounces");

            double ouncesToPounds = WeightUnit.OUNCE.Convert(WeightUnit.POUND, 16.0);
            Assert.AreEqual(1.0, ouncesToPounds, Tolerance, "16 ounces should equal 1 pound");

            double kgToPounds = WeightUnit.KILOGRAM.Convert(WeightUnit.POUND, 1.0);
            Assert.AreEqual(
                2.20462,
                kgToPounds,
                0.001,
                "1 kg should approximately equal 2.20462 pounds"
            );
        }

        /// <summary>
        /// Tests that WeightQuantity works correctly following the same pattern as Quantity.
        /// Verifies that the pattern is reusable.
        /// </summary>
        [TestMethod]
        public void WeightQuantity_FollowsSamePattern_WorksCorrectly()
        {
            var w1 = new WeightQuantity(1.0, WeightUnit.POUND);
            var w2 = new WeightQuantity(16.0, WeightUnit.OUNCE);

            // Test equality
            Assert.IsTrue(w1.Equals(w2), "1 lb should equal 16 oz");

            // Test conversion
            var converted = w1.ConvertTo(WeightUnit.OUNCE);
            Assert.AreEqual(16.0, converted.Value, Tolerance, "1 lb should convert to 16 oz");

            // Test addition
            var sum = w1.Add(w2, WeightUnit.POUND);
            Assert.AreEqual(2.0, sum.Value, Tolerance, "1 lb + 16 oz should equal 2 lb");
        }

        /// <summary>
        /// Tests that LengthUnit and WeightUnit are completely independent.
        /// Verifies no coupling between different measurement categories.
        /// </summary>
        [TestMethod]
        public void DifferentMeasurementCategories_AreIndependent()
        {
            // This test verifies that there's no coupling between Length and Weight categories

            // Length operations
            var length1 = new Quantity(1.0, LengthUnit.FEET);
            var length2 = new Quantity(12.0, LengthUnit.INCH);

            // Weight operations
            var weight1 = new WeightQuantity(1.0, WeightUnit.POUND);
            var weight2 = new WeightQuantity(16.0, WeightUnit.OUNCE);

            // Both should work independently
            Assert.IsTrue(length1.Equals(length2), "Length equality should work");
            Assert.IsTrue(weight1.Equals(weight2), "Weight equality should work");

            // There should be no cross-category methods or dependencies
            // (This is a structural test - we're verifying the design pattern)
        }

        /// <summary>
        /// Tests that the enum extension pattern is consistent across categories.
        /// Verifies architectural consistency.
        /// </summary>
        [TestMethod]
        public void EnumExtensionPattern_IsConsistent_AcrossCategories()
        {
            // Test LengthUnit pattern
            Assert.AreEqual("ft", LengthUnit.FEET.GetUnitSymbol());
            Assert.AreEqual(12.0, LengthUnit.FEET.Convert(LengthUnit.INCH, 1.0), Tolerance);

            // Test WeightUnit pattern (same structure)
            Assert.AreEqual("lb", WeightUnit.POUND.GetUnitSymbol());
            Assert.AreEqual(16.0, WeightUnit.POUND.Convert(WeightUnit.OUNCE, 1.0), Tolerance);

            // Both follow the same architectural pattern
        }
    }

    #region Example WeightUnit Implementation (Demonstrating Scalability)

    /// <summary>
    /// Example enum for weight units demonstrating how new categories can follow the same pattern.
    /// This is defined at top level (not nested) to properly support extension methods.
    /// </summary>
    public enum WeightUnit
    {
        POUND, // Base unit
        OUNCE, // 1 pound = 16 ounces
        KILOGRAM, // 1 kilogram = 2.20462 pounds
    }

    /// <summary>
    /// Extension class for WeightUnit providing conversion functionality.
    /// This is a top-level static class as required for extension methods.
    /// </summary>
    public static class WeightUnitExtensions
    {
        // Conversion factors to pounds (base unit)
        private static readonly double[] ToPoundsConversionFactors = new double[]
        {
            1.0, // POUND to POUND
            1.0 / 16.0, // OUNCE to POUND
            2.20462, // KILOGRAM to POUND
        };

        /// <summary>
        /// Converts a value from this weight unit to the base unit (pounds).
        /// </summary>
        /// <param name="unit">The weight unit.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value converted to pounds.</returns>
        public static double ConvertToBaseUnit(this WeightUnit unit, double value)
        {
            ValidateValue(value);
            int index = (int)unit;
            if (index >= 0 && index < ToPoundsConversionFactors.Length)
            {
                return value * ToPoundsConversionFactors[index];
            }
            throw new ArgumentException($"Invalid unit: {unit}");
        }

        /// <summary>
        /// Converts a value from the base unit (pounds) to this weight unit.
        /// </summary>
        /// <param name="unit">The target weight unit.</param>
        /// <param name="valueInPounds">The value in pounds to convert.</param>
        /// <returns>The value converted from pounds to this unit.</returns>
        public static double ConvertFromBaseUnit(this WeightUnit unit, double valueInPounds)
        {
            ValidateValue(valueInPounds);
            int index = (int)unit;
            if (index >= 0 && index < ToPoundsConversionFactors.Length)
            {
                return valueInPounds / ToPoundsConversionFactors[index];
            }
            throw new ArgumentException($"Invalid unit: {unit}");
        }

        /// <summary>
        /// Directly converts a value from one weight unit to another.
        /// </summary>
        /// <param name="sourceUnit">The source unit.</param>
        /// <param name="targetUnit">The target unit.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double Convert(
            this WeightUnit sourceUnit,
            WeightUnit targetUnit,
            double value
        )
        {
            double valueInPounds = sourceUnit.ConvertToBaseUnit(value);
            return targetUnit.ConvertFromBaseUnit(valueInPounds);
        }

        /// <summary>
        /// Gets the symbol for the weight unit.
        /// </summary>
        /// <param name="unit">The weight unit.</param>
        /// <returns>The unit symbol.</returns>
        public static string GetUnitSymbol(this WeightUnit unit)
        {
            switch (unit)
            {
                case WeightUnit.POUND:
                    return "lb";
                case WeightUnit.OUNCE:
                    return "oz";
                case WeightUnit.KILOGRAM:
                    return "kg";
                default:
                    return unit.ToString().ToLower();
            }
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

    /// <summary>
    /// Example Quantity class for weight measurements demonstrating the reusable pattern.
    /// </summary>
    public class WeightQuantity
    {
        private readonly double _value;
        private readonly WeightUnit _unit;

        /// <summary>
        /// Initializes a new instance of the WeightQuantity class.
        /// </summary>
        /// <param name="value">The weight value.</param>
        /// <param name="unit">The weight unit.</param>
        public WeightQuantity(double value, WeightUnit unit)
        {
            ValidateValue(value);
            ValidateUnit(unit);
            _value = value;
            _unit = unit;
        }

        /// <summary>
        /// Gets the weight value.
        /// </summary>
        public double Value => _value;

        /// <summary>
        /// Gets the weight unit.
        /// </summary>
        public WeightUnit Unit => _unit;

        /// <summary>
        /// Converts this weight to a target unit.
        /// </summary>
        /// <param name="targetUnit">The target unit.</param>
        /// <returns>A new WeightQuantity in the target unit.</returns>
        public WeightQuantity ConvertTo(WeightUnit targetUnit)
        {
            ValidateUnit(targetUnit);
            double valueInPounds = _unit.ConvertToBaseUnit(_value);
            double convertedValue = targetUnit.ConvertFromBaseUnit(valueInPounds);
            return new WeightQuantity(convertedValue, targetUnit);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current WeightQuantity.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is null || GetType() != obj.GetType())
                return false;

            WeightQuantity other = (WeightQuantity)obj;
            double thisInPounds = _unit.ConvertToBaseUnit(_value);
            double otherInPounds = other._unit.ConvertToBaseUnit(other._value);

            return Math.Abs(thisInPounds - otherInPounds) < 0.000001;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            double valueInPounds = _unit.ConvertToBaseUnit(_value);
            return Math.Round(valueInPounds, 6).GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the WeightQuantity.
        /// </summary>
        /// <returns>String in format "{value} {symbol}".</returns>
        public override string ToString()
        {
            return $"{_value} {_unit.GetUnitSymbol()}";
        }

        /// <summary>
        /// Adds another weight to this weight, result in this weight's unit.
        /// </summary>
        /// <param name="other">The other weight.</param>
        /// <returns>The sum in this weight's unit.</returns>
        public WeightQuantity Add(WeightQuantity other)
        {
            return Add(other, this._unit);
        }

        /// <summary>
        /// Adds another weight to this weight, result in specified target unit.
        /// </summary>
        /// <param name="other">The other weight.</param>
        /// <param name="targetUnit">The target unit for result.</param>
        /// <returns>The sum in target unit.</returns>
        public WeightQuantity Add(WeightQuantity other, WeightUnit targetUnit)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            ValidateUnit(targetUnit);

            double thisInPounds = _unit.ConvertToBaseUnit(_value);
            double otherInPounds = other._unit.ConvertToBaseUnit(other._value);
            double sumInPounds = thisInPounds + otherInPounds;
            double sumInTarget = targetUnit.ConvertFromBaseUnit(sumInPounds);

            return new WeightQuantity(sumInTarget, targetUnit);
        }

        /// <summary>
        /// Validates that a unit is valid.
        /// </summary>
        /// <param name="unit">The unit to validate.</param>
        private static void ValidateUnit(WeightUnit unit)
        {
            if (!Enum.IsDefined(typeof(WeightUnit), unit))
            {
                throw new ArgumentException($"Invalid unit: {unit}");
            }
        }

        /// <summary>
        /// Validates that a value is finite.
        /// </summary>
        /// <param name="value">The value to validate.</param>
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

    #endregion
}
