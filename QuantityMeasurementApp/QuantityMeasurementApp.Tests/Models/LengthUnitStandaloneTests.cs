using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Test class for standalone LengthUnit enum with conversion responsibilities.
    /// Tests that LengthUnit correctly handles all unit conversion logic independently.
    /// </summary>
    [TestClass]
    public class LengthUnitStandaloneTests
    {
        private const double Tolerance = 0.000001;

        #region Unit Constant Tests

        /// <summary>
        /// Tests that LengthUnit.FEET constant is accessible.
        /// Verifies the enum constant exists and can be used.
        /// </summary>
        [TestMethod]
        public void LengthUnitEnum_FeetConstant_IsAccessible()
        {
            LengthUnit unit = LengthUnit.FEET;
            Assert.AreEqual(LengthUnit.FEET, unit);
        }

        /// <summary>
        /// Tests that LengthUnit.INCH constant is accessible.
        /// Verifies the enum constant exists and can be used.
        /// </summary>
        [TestMethod]
        public void LengthUnitEnum_InchConstant_IsAccessible()
        {
            LengthUnit unit = LengthUnit.INCH;
            Assert.AreEqual(LengthUnit.INCH, unit);
        }

        /// <summary>
        /// Tests that LengthUnit.YARD constant is accessible.
        /// Verifies the enum constant exists and can be used.
        /// </summary>
        [TestMethod]
        public void LengthUnitEnum_YardConstant_IsAccessible()
        {
            LengthUnit unit = LengthUnit.YARD;
            Assert.AreEqual(LengthUnit.YARD, unit);
        }

        /// <summary>
        /// Tests that LengthUnit.CENTIMETER constant is accessible.
        /// Verifies the enum constant exists and can be used.
        /// </summary>
        [TestMethod]
        public void LengthUnitEnum_CentimeterConstant_IsAccessible()
        {
            LengthUnit unit = LengthUnit.CENTIMETER;
            Assert.AreEqual(LengthUnit.CENTIMETER, unit);
        }

        #endregion

        #region ConvertToBaseUnit Tests

        /// <summary>
        /// Tests conversion from FEET to base unit (feet).
        /// Verifies that converting feet to feet returns the same value.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_FeetToFeet_ReturnsSameValue()
        {
            double result = LengthUnit.FEET.ConvertToBaseUnit(5.0);
            Assert.AreEqual(5.0, result, Tolerance, "FEET to base unit should return same value");
        }

        /// <summary>
        /// Tests conversion from INCH to base unit (feet).
        /// Verifies that 12 inches convert to 1 foot.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_InchesToFeet_ReturnsCorrectValue()
        {
            double result = LengthUnit.INCH.ConvertToBaseUnit(12.0);
            Assert.AreEqual(1.0, result, Tolerance, "12 inches should equal 1 foot");
        }

        /// <summary>
        /// Tests conversion from YARD to base unit (feet).
        /// Verifies that 1 yard converts to 3 feet.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_YardsToFeet_ReturnsCorrectValue()
        {
            double result = LengthUnit.YARD.ConvertToBaseUnit(1.0);
            Assert.AreEqual(3.0, result, Tolerance, "1 yard should equal 3 feet");
        }

        /// <summary>
        /// Tests conversion from CENTIMETER to base unit (feet).
        /// Verifies that 30.48 cm converts to 1 foot.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_CentimetersToFeet_ReturnsCorrectValue()
        {
            double result = LengthUnit.CENTIMETER.ConvertToBaseUnit(30.48);
            Assert.AreEqual(1.0, result, Tolerance, "30.48 cm should equal 1 foot");
        }

        /// <summary>
        /// Tests conversion with zero value.
        /// Verifies that zero in any unit converts to zero.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_ZeroValue_ReturnsZero()
        {
            double result = LengthUnit.INCH.ConvertToBaseUnit(0.0);
            Assert.AreEqual(0.0, result, Tolerance, "Zero inches should convert to zero feet");

            result = LengthUnit.YARD.ConvertToBaseUnit(0.0);
            Assert.AreEqual(0.0, result, Tolerance, "Zero yards should convert to zero feet");
        }

        /// <summary>
        /// Tests conversion with negative value.
        /// Verifies that sign is preserved.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_NegativeValue_PreservesSign()
        {
            double result = LengthUnit.INCH.ConvertToBaseUnit(-12.0);
            Assert.AreEqual(-1.0, result, Tolerance, "-12 inches should equal -1 foot");
        }

        #endregion

        #region ConvertFromBaseUnit Tests

        /// <summary>
        /// Tests conversion from base unit (feet) to FEET.
        /// Verifies that converting feet to feet returns same value.
        /// </summary>
        [TestMethod]
        public void ConvertFromBaseUnit_FeetToFeet_ReturnsSameValue()
        {
            double result = LengthUnit.FEET.ConvertFromBaseUnit(5.0);
            Assert.AreEqual(5.0, result, Tolerance, "FEET from base unit should return same value");
        }

        /// <summary>
        /// Tests conversion from base unit (feet) to INCH.
        /// Verifies that 1 foot converts to 12 inches.
        /// </summary>
        [TestMethod]
        public void ConvertFromBaseUnit_FeetToInches_ReturnsCorrectValue()
        {
            double result = LengthUnit.INCH.ConvertFromBaseUnit(1.0);
            Assert.AreEqual(12.0, result, Tolerance, "1 foot should equal 12 inches");
        }

        /// <summary>
        /// Tests conversion from base unit (feet) to YARD.
        /// Verifies that 3 feet convert to 1 yard.
        /// </summary>
        [TestMethod]
        public void ConvertFromBaseUnit_FeetToYards_ReturnsCorrectValue()
        {
            double result = LengthUnit.YARD.ConvertFromBaseUnit(3.0);
            Assert.AreEqual(1.0, result, Tolerance, "3 feet should equal 1 yard");
        }

        /// <summary>
        /// Tests conversion from base unit (feet) to CENTIMETER.
        /// Verifies that 1 foot converts to 30.48 cm.
        /// </summary>
        [TestMethod]
        public void ConvertFromBaseUnit_FeetToCentimeters_ReturnsCorrectValue()
        {
            double result = LengthUnit.CENTIMETER.ConvertFromBaseUnit(1.0);
            Assert.AreEqual(30.48, result, Tolerance, "1 foot should equal 30.48 cm");
        }

        /// <summary>
        /// Tests conversion from base unit with zero value.
        /// Verifies that zero converts to zero.
        /// </summary>
        [TestMethod]
        public void ConvertFromBaseUnit_ZeroValue_ReturnsZero()
        {
            double result = LengthUnit.INCH.ConvertFromBaseUnit(0.0);
            Assert.AreEqual(0.0, result, Tolerance, "Zero feet should convert to zero inches");
        }

        #endregion

        #region Direct Unit-to-Unit Conversion Tests

        /// <summary>
        /// Tests direct conversion between units using the Convert method.
        /// Verifies that 1 foot converts to 12 inches.
        /// </summary>
        [TestMethod]
        public void Convert_Direct_FeetToInches_ReturnsCorrectValue()
        {
            double result = LengthUnit.FEET.Convert(LengthUnit.INCH, 1.0);
            Assert.AreEqual(12.0, result, Tolerance, "1 ft should convert to 12 in");
        }

        /// <summary>
        /// Tests direct conversion between units using the Convert method.
        /// Verifies that 12 inches convert to 1 foot.
        /// </summary>
        [TestMethod]
        public void Convert_Direct_InchesToFeet_ReturnsCorrectValue()
        {
            double result = LengthUnit.INCH.Convert(LengthUnit.FEET, 12.0);
            Assert.AreEqual(1.0, result, Tolerance, "12 in should convert to 1 ft");
        }

        /// <summary>
        /// Tests direct conversion between units using the Convert method.
        /// Verifies that 1 yard converts to 36 inches.
        /// </summary>
        [TestMethod]
        public void Convert_Direct_YardsToInches_ReturnsCorrectValue()
        {
            double result = LengthUnit.YARD.Convert(LengthUnit.INCH, 1.0);
            Assert.AreEqual(36.0, result, Tolerance, "1 yd should convert to 36 in");
        }

        /// <summary>
        /// Tests direct conversion between units using the Convert method.
        /// Verifies that 1 cm converts to approximately 0.3937 inches.
        /// </summary>
        [TestMethod]
        public void Convert_Direct_CentimetersToInches_ReturnsCorrectValue()
        {
            double result = LengthUnit.CENTIMETER.Convert(LengthUnit.INCH, 1.0);
            Assert.AreEqual(
                0.393700787,
                result,
                Tolerance,
                "1 cm should convert to 0.393700787 in"
            );
        }

        #endregion

        #region Round-Trip Conversion Tests

        /// <summary>
        /// Tests round-trip conversion using the unit's conversion methods.
        /// Verifies that converting A->B->A returns original value.
        /// </summary>
        [TestMethod]
        public void Convert_RoundTrip_ReturnsOriginalValue()
        {
            double originalValue = 5.0;

            // Feet -> Inches -> Feet
            double toInches = LengthUnit.FEET.Convert(LengthUnit.INCH, originalValue);
            double backToFeet = LengthUnit.INCH.Convert(LengthUnit.FEET, toInches);

            Assert.AreEqual(
                originalValue,
                backToFeet,
                Tolerance,
                "Round-trip feet->inches->feet should return original"
            );

            // Yards -> Centimeters -> Yards
            double toCm = LengthUnit.YARD.Convert(LengthUnit.CENTIMETER, originalValue);
            double backToYards = LengthUnit.CENTIMETER.Convert(LengthUnit.YARD, toCm);

            Assert.AreEqual(
                originalValue,
                backToYards,
                Tolerance,
                "Round-trip yards->cm->yards should return original"
            );
        }

        #endregion

        #region Unit Symbol and Name Tests

        /// <summary>
        /// Tests that GetUnitSymbol returns correct symbol for each unit.
        /// </summary>
        [TestMethod]
        public void GetUnitSymbol_ReturnsCorrectSymbol()
        {
            Assert.AreEqual("ft", LengthUnit.FEET.GetUnitSymbol());
            Assert.AreEqual("in", LengthUnit.INCH.GetUnitSymbol());
            Assert.AreEqual("yd", LengthUnit.YARD.GetUnitSymbol());
            Assert.AreEqual("cm", LengthUnit.CENTIMETER.GetUnitSymbol());
        }

        /// <summary>
        /// Tests that GetUnitName returns correct name for each unit.
        /// </summary>
        [TestMethod]
        public void GetUnitName_ReturnsCorrectName()
        {
            Assert.AreEqual("feet", LengthUnit.FEET.GetUnitName());
            Assert.AreEqual("inches", LengthUnit.INCH.GetUnitName());
            Assert.AreEqual("yards", LengthUnit.YARD.GetUnitName());
            Assert.AreEqual("centimeters", LengthUnit.CENTIMETER.GetUnitName());
        }

        #endregion

        #region Invalid Input Tests

        /// <summary>
        /// Tests ConvertToBaseUnit with invalid value (NaN).
        /// Verifies that ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToBaseUnit_NaNValue_ThrowsArgumentException()
        {
            LengthUnit.FEET.ConvertToBaseUnit(double.NaN);
        }

        /// <summary>
        /// Tests ConvertToBaseUnit with infinite value.
        /// Verifies that ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToBaseUnit_InfinityValue_ThrowsArgumentException()
        {
            LengthUnit.FEET.ConvertToBaseUnit(double.PositiveInfinity);
        }

        /// <summary>
        /// Tests ConvertFromBaseUnit with invalid value.
        /// Verifies that ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertFromBaseUnit_NaNValue_ThrowsArgumentException()
        {
            LengthUnit.INCH.ConvertFromBaseUnit(double.NaN);
        }

        #endregion
    }
}
