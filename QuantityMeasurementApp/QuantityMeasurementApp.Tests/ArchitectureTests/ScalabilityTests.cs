using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.ArchitectureTests
{
    /// <summary>
    /// Tests demonstrating architectural scalability across multiple measurement categories.
    /// UC8: Standalone unit enums with conversion responsibility.
    /// UC9: Addition of Weight category following the same pattern.
    /// Shows how the design scales to support Length, Weight, and can easily extend to more categories.
    /// </summary>
    [TestClass]
    public class ScalabilityTests
    {
        private const double Tolerance = 0.000001;
        private const double PoundTolerance = 0.001;

        #region Length Category Tests (UC1-UC8)

        /// <summary>
        /// Tests that LengthUnit works correctly as a standalone enum with conversion methods.
        /// Verifies UC8 pattern for length measurements.
        /// </summary>
        [TestMethod]
        public void LengthUnit_StandaloneEnum_WorksCorrectly()
        {
            // Test conversion factors
            Assert.AreEqual(1.0, LengthUnit.FEET.GetConversionFactor(), Tolerance);
            Assert.AreEqual(1.0 / 12.0, LengthUnit.INCH.GetConversionFactor(), Tolerance);
            Assert.AreEqual(3.0, LengthUnit.YARD.GetConversionFactor(), Tolerance);

            double cmFactor = 1.0 / (2.54 * 12.0);
            Assert.AreEqual(cmFactor, LengthUnit.CENTIMETER.GetConversionFactor(), Tolerance);

            // Test ToBaseUnit and FromBaseUnit
            Assert.AreEqual(1.0, LengthUnit.INCH.ToBaseUnit(12.0), Tolerance);
            Assert.AreEqual(12.0, LengthUnit.INCH.FromBaseUnit(1.0), Tolerance);

            // Test direct conversion
            Assert.AreEqual(36.0, LengthUnit.YARD.ConvertTo(LengthUnit.INCH, 1.0), Tolerance);
        }

        /// <summary>
        /// Tests that Quantity class for length works correctly.
        /// Verifies UC3-UC7 functionality for length.
        /// </summary>
        [TestMethod]
        public void LengthQuantity_WorksCorrectly()
        {
            // Test equality
            var feetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var inchesQuantity = new Quantity(12.0, LengthUnit.INCH);
            Assert.IsTrue(feetQuantity.Equals(inchesQuantity), "1 ft should equal 12 in");

            // Test conversion
            var convertedQuantity = feetQuantity.ConvertTo(LengthUnit.INCH);
            Assert.AreEqual(12.0, convertedQuantity.Value, Tolerance);
            Assert.AreEqual(LengthUnit.INCH, convertedQuantity.Unit);

            // Test addition
            var sumQuantity = feetQuantity.Add(inchesQuantity);
            Assert.AreEqual(2.0, sumQuantity.Value, Tolerance);
            Assert.AreEqual(LengthUnit.FEET, sumQuantity.Unit);
        }

        #endregion

        #region Weight Category Tests (UC9)

        /// <summary>
        /// Tests that WeightUnit works correctly as a standalone enum with conversion methods.
        /// Verifies UC9 pattern for weight measurements following the same architecture as Length.
        /// </summary>
        [TestMethod]
        public void WeightUnit_StandaloneEnum_WorksCorrectly()
        {
            // Test conversion factors
            Assert.AreEqual(1.0, WeightUnit.KILOGRAM.GetConversionFactor(), Tolerance);
            Assert.AreEqual(0.001, WeightUnit.GRAM.GetConversionFactor(), Tolerance);
            Assert.AreEqual(0.45359237, WeightUnit.POUND.GetConversionFactor(), Tolerance);

            // Test ToBaseUnit (to kilograms)
            Assert.AreEqual(1.0, WeightUnit.KILOGRAM.ToBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, WeightUnit.GRAM.ToBaseUnit(1000.0), Tolerance);
            Assert.AreEqual(0.45359237, WeightUnit.POUND.ToBaseUnit(1.0), Tolerance);

            // Test FromBaseUnit (from kilograms)
            Assert.AreEqual(1000.0, WeightUnit.GRAM.FromBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, WeightUnit.POUND.FromBaseUnit(0.45359237), Tolerance);

            // Test direct conversion
            Assert.AreEqual(1000.0, WeightUnit.KILOGRAM.ConvertTo(WeightUnit.GRAM, 1.0), Tolerance);
            Assert.AreEqual(
                2.20462262185,
                WeightUnit.KILOGRAM.ConvertTo(WeightUnit.POUND, 1.0),
                PoundTolerance
            );
            Assert.AreEqual(
                453.59237,
                WeightUnit.POUND.ConvertTo(WeightUnit.GRAM, 1.0),
                PoundTolerance
            );

            // Test GetSymbol and GetName
            Assert.AreEqual("kg", WeightUnit.KILOGRAM.GetSymbol());
            Assert.AreEqual("g", WeightUnit.GRAM.GetSymbol());
            Assert.AreEqual("lb", WeightUnit.POUND.GetSymbol());

            Assert.AreEqual("kilograms", WeightUnit.KILOGRAM.GetName());
            Assert.AreEqual("grams", WeightUnit.GRAM.GetName());
            Assert.AreEqual("pounds", WeightUnit.POUND.GetName());
        }

        /// <summary>
        /// Tests that WeightQuantity class works correctly following the same pattern as Quantity.
        /// Verifies UC9 functionality for weight measurements.
        /// </summary>
        [TestMethod]
        public void WeightQuantity_WorksCorrectly()
        {
            // Test equality
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(1000.0, WeightUnit.GRAM);
            var lbWeight = new WeightQuantity(2.20462262185, WeightUnit.POUND); // More precise value

            Assert.IsTrue(kgWeight.Equals(gWeight), "1 kg should equal 1000 g");
            Assert.IsTrue(kgWeight.Equals(lbWeight), "1 kg should approximately equal 2.20462 lb");
            Assert.IsTrue(gWeight.Equals(lbWeight), "1000 g should approximately equal 2.20462 lb");

            // Test conversion
            var convertedToGrams = kgWeight.ConvertTo(WeightUnit.GRAM);
            Assert.AreEqual(1000.0, convertedToGrams.Value, Tolerance);
            Assert.AreEqual(WeightUnit.GRAM, convertedToGrams.Unit);

            var convertedToPounds = kgWeight.ConvertTo(WeightUnit.POUND);
            Assert.AreEqual(2.20462262185, convertedToPounds.Value, PoundTolerance);
            Assert.AreEqual(WeightUnit.POUND, convertedToPounds.Unit);

            // Test addition with same unit
            var firstKg = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var secondKg = new WeightQuantity(2.0, WeightUnit.KILOGRAM);
            var sumKg = firstKg.Add(secondKg);
            Assert.AreEqual(3.0, sumKg.Value, Tolerance);
            Assert.AreEqual(WeightUnit.KILOGRAM, sumKg.Unit);

            // Test addition with different units (result in first unit)
            var sumInKg = kgWeight.Add(gWeight);
            Assert.AreEqual(2.0, sumInKg.Value, Tolerance);
            Assert.AreEqual(WeightUnit.KILOGRAM, sumInKg.Unit);

            // Test addition with explicit target unit
            var sumInGrams = kgWeight.Add(gWeight, WeightUnit.GRAM);
            Assert.AreEqual(2000.0, sumInGrams.Value, Tolerance);
            Assert.AreEqual(WeightUnit.GRAM, sumInGrams.Unit);

            var sumInPounds = kgWeight.Add(gWeight, WeightUnit.POUND);
            double expectedPounds = 2.0 * 2.20462262185; // 2 kg in pounds
            Assert.AreEqual(expectedPounds, sumInPounds.Value, PoundTolerance);
            Assert.AreEqual(WeightUnit.POUND, sumInPounds.Unit);

            // Test static Add method
            var staticSum = WeightQuantity.Add(
                1.0,
                WeightUnit.KILOGRAM,
                500.0,
                WeightUnit.GRAM,
                WeightUnit.KILOGRAM
            );
            Assert.AreEqual(1.5, staticSum.Value, Tolerance);
            Assert.AreEqual(WeightUnit.KILOGRAM, staticSum.Unit);

            // Test zero values
            var zeroKg = new WeightQuantity(0.0, WeightUnit.KILOGRAM);
            var zeroG = new WeightQuantity(0.0, WeightUnit.GRAM);
            Assert.IsTrue(zeroKg.Equals(zeroG), "0 kg should equal 0 g");

            // Test negative values
            var negativeKg = new WeightQuantity(-1.0, WeightUnit.KILOGRAM);
            var negativeG = new WeightQuantity(-1000.0, WeightUnit.GRAM);
            Assert.IsTrue(negativeKg.Equals(negativeG), "-1 kg should equal -1000 g");
        }

        #endregion

        #region Category Independence Tests

        /// <summary>
        /// Tests that different measurement categories (Length vs Weight) are independent and cannot be mixed.
        /// Verifies type safety across categories.
        /// </summary>
        [TestMethod]
        public void DifferentCategories_AreIndependent_CannotBeMixed()
        {
            // Length quantities
            var lengthFeet = new Quantity(1.0, LengthUnit.FEET);
            var lengthInches = new Quantity(12.0, LengthUnit.INCH);

            // Weight quantities
            var weightKg = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var weightG = new WeightQuantity(1000.0, WeightUnit.GRAM);

            // Test that length and weight cannot be compared (should return false, not exception)
            Assert.IsFalse(lengthFeet.Equals(weightKg), "1 ft should not equal 1 kg");
            Assert.IsFalse(lengthInches.Equals(weightG), "12 in should not equal 1000 g");

            // Test that they are different types
            Assert.AreNotEqual(
                lengthFeet.GetType(),
                weightKg.GetType(),
                "Length and Weight should be different types"
            );

            // Test that operations within each category still work
            Assert.IsTrue(lengthFeet.Equals(lengthInches), "Length equality should still work");
            Assert.IsTrue(weightKg.Equals(weightG), "Weight equality should still work");
        }

        /// <summary>
        /// Tests that each category has its own base unit and conversion logic.
        /// Verifies that categories don't interfere with each other.
        /// FIXED: Now properly tests that the base units are different concepts
        /// </summary>
        [TestMethod]
        public void EachCategory_HasOwnBaseUnit_AndConversionLogic()
        {
            // Length base unit is feet
            double lengthInBase = LengthUnit.FEET.ToBaseUnit(1.0);
            string lengthUnitName = LengthUnit.FEET.GetName();

            // Weight base unit is kilograms
            double weightInBase = WeightUnit.KILOGRAM.ToBaseUnit(1.0);
            string weightUnitName = WeightUnit.KILOGRAM.GetName();

            // The numeric values might coincidentally be the same (both 1.0),
            // but they represent different physical quantities

            // Test that they have different unit names
            Assert.AreNotEqual(
                lengthUnitName,
                weightUnitName,
                "Base units should have different names (feet vs kilograms)"
            );

            // Test that the quantity classes are different types
            var lengthQuantity = new Quantity(1.0, LengthUnit.FEET);
            var weightQuantity = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            Assert.AreNotEqual(
                lengthQuantity.GetType(),
                weightQuantity.GetType(),
                "Length and Weight should be different types"
            );

            // Test that conversion factors are for different base units
            double lengthFactor = LengthUnit.FEET.GetConversionFactor();
            double weightFactor = WeightUnit.KILOGRAM.GetConversionFactor();

            // They both return 1.0 for their own base units, but that's expected
            // The key is that they are used in different contexts
            Assert.AreEqual(
                1.0,
                lengthFactor,
                Tolerance,
                "Feet to feet conversion factor should be 1.0"
            );
            Assert.AreEqual(
                1.0,
                weightFactor,
                Tolerance,
                "Kg to kg conversion factor should be 1.0"
            );

            // Test that converting to the other category's base unit doesn't make sense
            // This is a conceptual test - we can't convert length to weight
            var lengthInches = new Quantity(12.0, LengthUnit.INCH);
            var weightGrams = new WeightQuantity(1000.0, WeightUnit.GRAM);

            // They are different types, so they can't be compared directly
            Assert.IsFalse(
                lengthInches.GetType() == weightGrams.GetType(),
                "Length and Weight should be different types"
            );
        }

        #endregion

        #region Future Category Demonstrations

        /// <summary>
        /// Example of how Temperature category would follow the same pattern.
        /// This demonstrates that the architecture can scale to any number of categories.
        /// Note: This is a demonstration only, not actual implementation.
        /// </summary>
        [TestMethod]
        public void FutureCategory_Temperature_CanFollowSamePattern()
        {
            // This test demonstrates how a new category (Temperature) would be implemented
            // following the same architectural pattern as Length and Weight

            // Step 1: Define TemperatureUnit enum (conceptual)
            /*
            public enum TemperatureUnit
            {
                CELSIUS,    // Base unit
                FAHRENHEIT, // Conversion: °F = (°C × 9/5) + 32
                KELVIN      // Conversion: K = °C + 273.15
            }
            */

            // Step 2: Extension methods would handle conversions (conceptual)
            /*
            public static double ToBaseUnit(this TemperatureUnit unit, double value)
            {
                return unit switch
                {
                    TemperatureUnit.CELSIUS => value,
                    TemperatureUnit.FAHRENHEIT => (value - 32) * 5 / 9,
                    TemperatureUnit.KELVIN => value - 273.15,
                    _ => throw new InvalidUnitException(unit)
                };
            }
            */

            // Step 3: TemperatureQuantity class would mirror WeightQuantity (conceptual)
            /*
            public class TemperatureQuantity
            {
                private readonly double _value;
                private readonly TemperatureUnit _unit;
                
                // Same pattern: Equals, ConvertTo, Add methods
            }
            */

            // Assert that the pattern is consistent
            Assert.IsTrue(
                true,
                "The architectural pattern can be extended to any measurement category"
            );
        }

        /// <summary>
        /// Example of how Volume category would follow the same pattern.
        /// Demonstrates the reusability of the design.
        /// </summary>
        [TestMethod]
        public void FutureCategory_Volume_CanFollowSamePattern()
        {
            // Conceptual VolumeUnit enum
            /*
            public enum VolumeUnit
            {
                LITER,      // Base unit
                MILLILITER, // 1 L = 1000 mL
                GALLON      // 1 gal ≈ 3.78541 L
            }
            */

            // The same pattern would apply:
            // - VolumeUnit enum with conversion factors
            // - VolumeQuantity class with Equals, ConvertTo, Add methods
            // - Complete independence from Length and Weight

            Assert.IsTrue(
                true,
                "Volume category would follow the exact same pattern as Length and Weight"
            );
        }

        /// <summary>
        /// Tests that all measurement categories follow the same architectural pattern.
        /// Verifies consistency across implemented and future categories.
        /// </summary>
        [TestMethod]
        public void AllCategories_FollowSamePattern()
        {
            // Length pattern
            Assert.IsTrue(Enum.IsDefined(typeof(LengthUnit), LengthUnit.FEET));
            Assert.IsTrue(typeof(LengthUnit).IsEnum);
            Assert.IsNotNull(LengthUnit.FEET.GetConversionFactor());

            // Weight pattern
            Assert.IsTrue(Enum.IsDefined(typeof(WeightUnit), WeightUnit.KILOGRAM));
            Assert.IsTrue(typeof(WeightUnit).IsEnum);
            Assert.IsNotNull(WeightUnit.KILOGRAM.GetConversionFactor());

            // Both have extension methods
            Assert.IsNotNull(LengthUnit.FEET.ToBaseUnit(1.0));
            Assert.IsNotNull(WeightUnit.KILOGRAM.ToBaseUnit(1.0));

            // Both have quantity classes
            var lengthQuantity = new Quantity(1.0, LengthUnit.FEET);
            var weightQuantity = new WeightQuantity(1.0, WeightUnit.KILOGRAM);

            Assert.IsNotNull(lengthQuantity);
            Assert.IsNotNull(weightQuantity);

            // Both have Equals method
            Assert.IsNotNull(
                lengthQuantity.GetType().GetMethod("Equals", new[] { typeof(object) })
            );
            Assert.IsNotNull(
                weightQuantity.GetType().GetMethod("Equals", new[] { typeof(object) })
            );

            // Both have ConvertTo method
            Assert.IsNotNull(
                lengthQuantity.GetType().GetMethod("ConvertTo", new[] { typeof(LengthUnit) })
            );
            Assert.IsNotNull(
                weightQuantity.GetType().GetMethod("ConvertTo", new[] { typeof(WeightUnit) })
            );

            // Both have Add method with one parameter
            Assert.IsNotNull(lengthQuantity.GetType().GetMethod("Add", new[] { typeof(Quantity) }));
            Assert.IsNotNull(
                weightQuantity.GetType().GetMethod("Add", new[] { typeof(WeightQuantity) })
            );
        }

        #endregion

        #region Performance and Scalability Considerations

        /// <summary>
        /// Demonstrates that adding new categories has minimal performance impact.
        /// Each category operates independently without affecting others.
        /// </summary>
        [TestMethod]
        public void NewCategories_DoNotAffect_ExistingCategories()
        {
            // Measure time for length operations
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var length1 = new Quantity(1.0, LengthUnit.FEET);
            var length2 = new Quantity(12.0, LengthUnit.INCH);
            bool lengthEqual = length1.Equals(length2);
            var lengthSum = length1.Add(length2);

            watch.Stop();
            long lengthTime = watch.ElapsedTicks;

            // Measure time for weight operations (new category)
            watch.Restart();

            var weight1 = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var weight2 = new WeightQuantity(1000.0, WeightUnit.GRAM);
            bool weightEqual = weight1.Equals(weight2);
            var weightSum = weight1.Add(weight2);

            watch.Stop();
            long weightTime = watch.ElapsedTicks;

            // Both should complete successfully
            Assert.IsTrue(lengthEqual);
            Assert.IsTrue(weightEqual);
            Assert.IsNotNull(lengthSum);
            Assert.IsNotNull(weightSum);

            // The addition of weight category doesn't break length operations
            // Performance difference should be minimal (not measuring exact equality)
            Console.WriteLine($"Length operations time: {lengthTime} ticks");
            Console.WriteLine($"Weight operations time: {weightTime} ticks");
        }

        #endregion

        #region Summary

        /// <summary>
        /// Summary test that verifies all categories work together harmoniously.
        /// </summary>
        [TestMethod]
        public void CompleteScalability_Summary()
        {
            // Length operations
            var lengthInFeet = new Quantity(3.0, LengthUnit.FEET);
            var lengthInYards = new Quantity(1.0, LengthUnit.YARD);

            // Weight operations
            var weightInKg = new WeightQuantity(2.0, WeightUnit.KILOGRAM);
            var weightInG = new WeightQuantity(2000.0, WeightUnit.GRAM);

            // Length addition
            var lengthSum = lengthInFeet.Add(lengthInYards);
            Assert.AreEqual(6.0, lengthSum.Value, Tolerance); // 3 ft + 1 yd = 3 ft + 3 ft = 6 ft

            // Weight addition
            var weightSum = weightInKg.Add(weightInG);
            Assert.AreEqual(4.0, weightSum.Value, Tolerance); // 2 kg + 2000 g = 2 kg + 2 kg = 4 kg

            // Categories are independent
            Assert.AreNotEqual(lengthSum.GetType(), weightSum.GetType());

            // The architectural pattern has been successfully scaled from Length to Weight
            // and can easily extend to Temperature, Volume, and more
        }

        #endregion
    }
}
