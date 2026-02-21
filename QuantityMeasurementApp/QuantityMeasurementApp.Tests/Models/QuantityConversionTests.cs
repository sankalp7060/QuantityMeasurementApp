using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Test class for Quantity conversion functionality.
    /// Tests the Convert method and conversion operations between all supported units.
    /// </summary>
    [TestClass]
    public class QuantityConversionTests
    {
        private const double Tolerance = 0.000001;

        #region Basic Unit Conversion Tests

        /// <summary>
        /// Tests conversion from Feet to Inches.
        /// Verifies that 1 foot converts to 12 inches.
        /// </summary>
        [TestMethod]
        public void Convert_FeetToInches_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(1.0, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(12.0, result, Tolerance, "1 foot should equal 12 inches");
        }

        /// <summary>
        /// Tests conversion from Inches to Feet.
        /// Verifies that 24 inches convert to 2 feet.
        /// </summary>
        [TestMethod]
        public void Convert_InchesToFeet_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(24.0, LengthUnit.INCH, LengthUnit.FEET);
            Assert.AreEqual(2.0, result, Tolerance, "24 inches should equal 2 feet");
        }

        /// <summary>
        /// Tests conversion from Yards to Feet.
        /// Verifies that 1 yard converts to 3 feet.
        /// </summary>
        [TestMethod]
        public void Convert_YardsToFeet_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(1.0, LengthUnit.YARD, LengthUnit.FEET);
            Assert.AreEqual(3.0, result, Tolerance, "1 yard should equal 3 feet");
        }

        /// <summary>
        /// Tests conversion from Feet to Yards.
        /// Verifies that 6 feet convert to 2 yards.
        /// </summary>
        [TestMethod]
        public void Convert_FeetToYards_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(6.0, LengthUnit.FEET, LengthUnit.YARD);
            Assert.AreEqual(2.0, result, Tolerance, "6 feet should equal 2 yards");
        }

        /// <summary>
        /// Tests conversion from Yards to Inches.
        /// Verifies that 1 yard converts to 36 inches.
        /// </summary>
        [TestMethod]
        public void Convert_YardsToInches_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(1.0, LengthUnit.YARD, LengthUnit.INCH);
            Assert.AreEqual(36.0, result, Tolerance, "1 yard should equal 36 inches");
        }

        /// <summary>
        /// Tests conversion from Inches to Yards.
        /// Verifies that 72 inches convert to 2 yards.
        /// </summary>
        [TestMethod]
        public void Convert_InchesToYards_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(72.0, LengthUnit.INCH, LengthUnit.YARD);
            Assert.AreEqual(2.0, result, Tolerance, "72 inches should equal 2 yards");
        }

        /// <summary>
        /// Tests conversion from Centimeters to Inches.
        /// Verifies that 2.54 cm converts to 1 inch.
        /// </summary>
        [TestMethod]
        public void Convert_CentimetersToInches_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(2.54, LengthUnit.CENTIMETER, LengthUnit.INCH);
            Assert.AreEqual(1.0, result, Tolerance, "2.54 cm should equal 1 inch");
        }

        /// <summary>
        /// Tests conversion from Inches to Centimeters.
        /// Verifies that 1 inch converts to 2.54 cm.
        /// </summary>
        [TestMethod]
        public void Convert_InchesToCentimeters_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(1.0, LengthUnit.INCH, LengthUnit.CENTIMETER);
            Assert.AreEqual(2.54, result, Tolerance, "1 inch should equal 2.54 cm");
        }

        /// <summary>
        /// Tests conversion from Centimeters to Feet.
        /// Verifies that 30.48 cm converts to 1 foot.
        /// </summary>
        [TestMethod]
        public void Convert_CentimetersToFeet_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(30.48, LengthUnit.CENTIMETER, LengthUnit.FEET);
            Assert.AreEqual(1.0, result, Tolerance, "30.48 cm should equal 1 foot");
        }

        /// <summary>
        /// Tests conversion from Feet to Centimeters.
        /// Verifies that 1 foot converts to 30.48 cm.
        /// </summary>
        [TestMethod]
        public void Convert_FeetToCentimeters_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(1.0, LengthUnit.FEET, LengthUnit.CENTIMETER);
            Assert.AreEqual(30.48, result, Tolerance, "1 foot should equal 30.48 cm");
        }

        /// <summary>
        /// Tests conversion from Centimeters to Yards.
        /// Verifies that 91.44 cm converts to 1 yard.
        /// </summary>
        [TestMethod]
        public void Convert_CentimetersToYards_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(91.44, LengthUnit.CENTIMETER, LengthUnit.YARD);
            Assert.AreEqual(1.0, result, Tolerance, "91.44 cm should equal 1 yard");
        }

        /// <summary>
        /// Tests conversion from Yards to Centimeters.
        /// Verifies that 1 yard converts to 91.44 cm.
        /// </summary>
        [TestMethod]
        public void Convert_YardsToCentimeters_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(1.0, LengthUnit.YARD, LengthUnit.CENTIMETER);
            Assert.AreEqual(91.44, result, Tolerance, "1 yard should equal 91.44 cm");
        }

        #endregion

        #region Cross-Unit Conversion Tests

        /// <summary>
        /// Tests conversion from Yards directly to Centimeters with decimal value.
        /// Verifies the multi-step conversion through base unit is accurate.
        /// </summary>
        [TestMethod]
        public void Convert_YardsToCentimeters_DecimalValue_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(2.5, LengthUnit.YARD, LengthUnit.CENTIMETER);
            double expected = 2.5 * 91.44; // 2.5 yards * 91.44 cm per yard
            Assert.AreEqual(
                expected,
                result,
                Tolerance,
                "2.5 yards should equal the expected centimeters"
            );
        }

        /// <summary>
        /// Tests conversion from Inches to Centimeters with fractional value.
        /// Verifies conversion between non-adjacent units.
        /// </summary>
        [TestMethod]
        public void Convert_InchesToCentimeters_FractionalValue_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(12.5, LengthUnit.INCH, LengthUnit.CENTIMETER);
            double expected = 12.5 * 2.54; // 12.5 inches * 2.54 cm per inch
            Assert.AreEqual(
                expected,
                result,
                Tolerance,
                "12.5 inches should equal correct centimeters"
            );
        }

        /// <summary>
        /// Tests conversion from Feet to Centimeters with decimal value.
        /// Verifies conversion between non-adjacent units.
        /// </summary>
        [TestMethod]
        public void Convert_FeetToCentimeters_DecimalValue_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(3.5, LengthUnit.FEET, LengthUnit.CENTIMETER);
            double expected = 3.5 * 30.48; // 3.5 feet * 30.48 cm per foot
            Assert.AreEqual(
                expected,
                result,
                Tolerance,
                "3.5 feet should equal correct centimeters"
            );
        }

        #endregion

        #region Zero Value Tests

        /// <summary>
        /// Tests conversion of zero value.
        /// Verifies that zero in any unit converts to zero.
        /// </summary>
        [TestMethod]
        public void Convert_ZeroValue_ReturnsZero()
        {
            double result = Quantity.Convert(0.0, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(0.0, result, Tolerance, "Zero feet should convert to zero inches");

            result = Quantity.Convert(0.0, LengthUnit.YARD, LengthUnit.CENTIMETER);
            Assert.AreEqual(0.0, result, Tolerance, "Zero yards should convert to zero cm");

            result = Quantity.Convert(0.0, LengthUnit.INCH, LengthUnit.FEET);
            Assert.AreEqual(0.0, result, Tolerance, "Zero inches should convert to zero feet");
        }

        #endregion

        #region Negative Value Tests

        /// <summary>
        /// Tests conversion of negative values.
        /// Verifies that sign is preserved during conversion.
        /// </summary>
        [TestMethod]
        public void Convert_NegativeValue_PreservesSign()
        {
            double result = Quantity.Convert(-1.0, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(-12.0, result, Tolerance, "-1 foot should convert to -12 inches");

            result = Quantity.Convert(-2.5, LengthUnit.YARD, LengthUnit.FEET);
            Assert.AreEqual(-7.5, result, Tolerance, "-2.5 yards should convert to -7.5 feet");

            result = Quantity.Convert(-30.48, LengthUnit.CENTIMETER, LengthUnit.FEET);
            Assert.AreEqual(-1.0, result, Tolerance, "-30.48 cm should convert to -1 foot");
        }

        #endregion

        #region Large Value Tests

        /// <summary>
        /// Tests conversion of very large values.
        /// Verifies that conversion maintains precision for large numbers.
        /// </summary>
        [TestMethod]
        public void Convert_LargeValues_MaintainsPrecision()
        {
            double result = Quantity.Convert(1000000.0, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(
                12000000.0,
                result,
                Tolerance * 1000000,
                "1,000,000 feet should equal 12,000,000 inches"
            );
        }

        #endregion

        #region Small Value Tests

        /// <summary>
        /// Tests conversion of very small values.
        /// Verifies that conversion maintains precision for small numbers.
        /// </summary>
        [TestMethod]
        public void Convert_SmallValues_MaintainsPrecision()
        {
            double result = Quantity.Convert(0.000001, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(
                0.000012,
                result,
                Tolerance,
                "0.000001 feet should equal 0.000012 inches"
            );

            result = Quantity.Convert(0.000001, LengthUnit.CENTIMETER, LengthUnit.INCH);
            double expected = 0.000001 * 0.393700787;
            Assert.AreEqual(expected, result, Tolerance, "0.000001 cm should convert correctly");
        }

        #endregion

        #region Same-Unit Tests

        /// <summary>
        /// Tests conversion where source and target units are the same.
        /// Verifies that value remains unchanged.
        /// </summary>
        [TestMethod]
        public void Convert_SameUnit_ReturnsOriginalValue()
        {
            double result = Quantity.Convert(5.0, LengthUnit.FEET, LengthUnit.FEET);
            Assert.AreEqual(
                5.0,
                result,
                Tolerance,
                "Converting feet to feet should return original value"
            );

            result = Quantity.Convert(7.5, LengthUnit.INCH, LengthUnit.INCH);
            Assert.AreEqual(
                7.5,
                result,
                Tolerance,
                "Converting inches to inches should return original value"
            );

            result = Quantity.Convert(3.2, LengthUnit.YARD, LengthUnit.YARD);
            Assert.AreEqual(
                3.2,
                result,
                Tolerance,
                "Converting yards to yards should return original value"
            );

            result = Quantity.Convert(10.0, LengthUnit.CENTIMETER, LengthUnit.CENTIMETER);
            Assert.AreEqual(
                10.0,
                result,
                Tolerance,
                "Converting cm to cm should return original value"
            );
        }

        #endregion

        #region Round-Trip Tests

        /// <summary>
        /// Tests round-trip conversion (A → B → A).
        /// Verifies that conversion is reversible within tolerance.
        /// </summary>
        [TestMethod]
        public void Convert_RoundTrip_ReturnsOriginalValue()
        {
            double originalValue = 3.5;

            double toInches = Quantity.Convert(originalValue, LengthUnit.FEET, LengthUnit.INCH);
            double backToFeet = Quantity.Convert(toInches, LengthUnit.INCH, LengthUnit.FEET);

            Assert.AreEqual(
                originalValue,
                backToFeet,
                Tolerance,
                "Round-trip feet→inches→feet should return original"
            );

            originalValue = 2.0;
            double toCm = Quantity.Convert(originalValue, LengthUnit.YARD, LengthUnit.CENTIMETER);
            double backToYards = Quantity.Convert(toCm, LengthUnit.CENTIMETER, LengthUnit.YARD);

            Assert.AreEqual(
                originalValue,
                backToYards,
                Tolerance,
                "Round-trip yards→cm→yards should return original"
            );
        }

        /// <summary>
        /// Tests multiple round-trip conversions through all units.
        /// Verifies that value is preserved through multiple conversions.
        /// </summary>
        [TestMethod]
        public void Convert_MultiStepRoundTrip_ReturnsOriginalValue()
        {
            double originalValue = 1.0;

            // Feet → Inches → Centimeters → Yards → Feet
            double toInches = Quantity.Convert(originalValue, LengthUnit.FEET, LengthUnit.INCH);
            double toCm = Quantity.Convert(toInches, LengthUnit.INCH, LengthUnit.CENTIMETER);
            double toYards = Quantity.Convert(toCm, LengthUnit.CENTIMETER, LengthUnit.YARD);
            double backToFeet = Quantity.Convert(toYards, LengthUnit.YARD, LengthUnit.FEET);

            Assert.AreEqual(
                originalValue,
                backToFeet,
                Tolerance,
                "Multi-step conversion should return original value"
            );
        }

        #endregion

        #region Invalid Input Tests

        /// <summary>
        /// Tests conversion with invalid source unit.
        /// Verifies that ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Convert_InvalidSourceUnit_ThrowsArgumentException()
        {
            LengthUnit invalidUnit = (LengthUnit)99;
            Quantity.Convert(1.0, invalidUnit, LengthUnit.FEET);
        }

        /// <summary>
        /// Tests conversion with invalid target unit.
        /// Verifies that ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Convert_InvalidTargetUnit_ThrowsArgumentException()
        {
            LengthUnit invalidUnit = (LengthUnit)99;
            Quantity.Convert(1.0, LengthUnit.FEET, invalidUnit);
        }

        /// <summary>
        /// Tests conversion with NaN value.
        /// Verifies that ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Convert_NaNValue_ThrowsArgumentException()
        {
            Quantity.Convert(double.NaN, LengthUnit.FEET, LengthUnit.INCH);
        }

        /// <summary>
        /// Tests conversion with Positive Infinity value.
        /// Verifies that ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Convert_PositiveInfinity_ThrowsArgumentException()
        {
            Quantity.Convert(double.PositiveInfinity, LengthUnit.FEET, LengthUnit.INCH);
        }

        /// <summary>
        /// Tests conversion with Negative Infinity value.
        /// Verifies that ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Convert_NegativeInfinity_ThrowsArgumentException()
        {
            Quantity.Convert(double.NegativeInfinity, LengthUnit.FEET, LengthUnit.INCH);
        }

        #endregion

        #region Instance Method Tests

        /// <summary>
        /// Tests instance ConvertTo method.
        /// Verifies that instance method returns correct converted value.
        /// </summary>
        [TestMethod]
        public void ConvertTo_InstanceMethod_ReturnsCorrectQuantity()
        {
            var quantity = new Quantity(1.0, LengthUnit.FEET);
            var converted = quantity.ConvertTo(LengthUnit.INCH);

            Assert.AreEqual(
                12.0,
                converted.Value,
                Tolerance,
                "Instance ConvertTo should return correct value"
            );
            Assert.AreEqual(
                LengthUnit.INCH,
                converted.Unit,
                "Instance ConvertTo should set correct target unit"
            );
        }

        /// <summary>
        /// Tests instance ConvertTo method with zero value.
        /// Verifies that zero converts correctly.
        /// </summary>
        [TestMethod]
        public void ConvertTo_InstanceMethod_ZeroValue_ReturnsZero()
        {
            var quantity = new Quantity(0.0, LengthUnit.FEET);
            var converted = quantity.ConvertTo(LengthUnit.INCH);

            Assert.AreEqual(0.0, converted.Value, Tolerance, "Zero should convert to zero");
            Assert.AreEqual(LengthUnit.INCH, converted.Unit);
        }

        /// <summary>
        /// Tests instance ConvertTo method with negative value.
        /// Verifies that negative values preserve sign.
        /// </summary>
        [TestMethod]
        public void ConvertTo_InstanceMethod_NegativeValue_PreservesSign()
        {
            var quantity = new Quantity(-2.5, LengthUnit.YARD);
            var converted = quantity.ConvertTo(LengthUnit.FEET);

            Assert.AreEqual(
                -7.5,
                converted.Value,
                Tolerance,
                "Negative values should preserve sign"
            );
            Assert.AreEqual(LengthUnit.FEET, converted.Unit);
        }

        /// <summary>
        /// Tests instance ConvertTo method with same unit.
        /// Verifies that value remains unchanged.
        /// </summary>
        [TestMethod]
        public void ConvertTo_InstanceMethod_SameUnit_ReturnsSameValue()
        {
            var quantity = new Quantity(5.0, LengthUnit.FEET);
            var converted = quantity.ConvertTo(LengthUnit.FEET);

            Assert.AreEqual(
                5.0,
                converted.Value,
                Tolerance,
                "Same unit conversion should return same value"
            );
            Assert.AreEqual(LengthUnit.FEET, converted.Unit);
        }

        #endregion

        #region Precision Tests

        /// <summary>
        /// Tests conversion precision with many decimal places.
        /// Verifies that conversion maintains mathematical accuracy.
        /// </summary>
        [TestMethod]
        public void Convert_PrecisionTest_MaintainsAccuracy()
        {
            // Test with values that have many decimal places
            double result = Quantity.Convert(1.23456789, LengthUnit.FEET, LengthUnit.INCH);
            double expected = 1.23456789 * 12.0;
            Assert.AreEqual(expected, result, 0.000001, "Conversion should maintain precision");

            result = Quantity.Convert(2.3456789, LengthUnit.YARD, LengthUnit.CENTIMETER);
            expected = 2.3456789 * 91.44;
            Assert.AreEqual(
                expected,
                result,
                0.0001,
                "Yard to cm conversion should maintain precision"
            );

            result = Quantity.Convert(3.456789, LengthUnit.CENTIMETER, LengthUnit.INCH);
            expected = 3.456789 * 0.393700787;
            Assert.AreEqual(
                expected,
                result,
                0.000001,
                "Cm to inch conversion should maintain precision"
            );
        }

        #endregion

        #region Bidirectional Tests

        /// <summary>
        /// Tests bidirectional conversion (A→B and B→A are inverses).
        /// Verifies that conversion factors are mathematically consistent.
        /// </summary>
        [TestMethod]
        public void Convert_Bidirectional_AreInverses()
        {
            double value = 5.0;

            double feetToInches = Quantity.Convert(value, LengthUnit.FEET, LengthUnit.INCH);
            double inchesToFeet = Quantity.Convert(feetToInches, LengthUnit.INCH, LengthUnit.FEET);
            Assert.AreEqual(
                value,
                inchesToFeet,
                Tolerance,
                "Feet↔Inches conversions should be inverses"
            );

            double yardsToFeet = Quantity.Convert(value, LengthUnit.YARD, LengthUnit.FEET);
            double feetToYards = Quantity.Convert(yardsToFeet, LengthUnit.FEET, LengthUnit.YARD);
            Assert.AreEqual(
                value,
                feetToYards,
                Tolerance,
                "Yards↔Feet conversions should be inverses"
            );

            double cmToInches = Quantity.Convert(value, LengthUnit.CENTIMETER, LengthUnit.INCH);
            double inchesToCm = Quantity.Convert(
                cmToInches,
                LengthUnit.INCH,
                LengthUnit.CENTIMETER
            );
            Assert.AreEqual(
                value,
                inchesToCm,
                Tolerance,
                "Cm↔Inches conversions should be inverses"
            );
        }

        #endregion
    }
}
