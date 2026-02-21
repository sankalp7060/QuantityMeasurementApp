using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Test class for LengthUnit enum with conversion responsibilities.
    /// Tests the standalone LengthUnit enum and its conversion methods.
    /// </summary>
    [TestClass]
    public class LengthUnitTests
    {
        private const double Tolerance = 0.000001;

        #region Unit Constant Tests

        /// <summary>
        /// Tests that LengthUnit.FEET constant is accessible.
        /// </summary>
        [TestMethod]
        public void LengthUnitEnum_FeetConstant_IsAccessible()
        {
            Assert.AreEqual(LengthUnit.FEET, LengthUnit.FEET);
        }

        /// <summary>
        /// Tests that LengthUnit.INCH constant is accessible.
        /// </summary>
        [TestMethod]
        public void LengthUnitEnum_InchConstant_IsAccessible()
        {
            Assert.AreEqual(LengthUnit.INCH, LengthUnit.INCH);
        }

        /// <summary>
        /// Tests that LengthUnit.YARD constant is accessible.
        /// </summary>
        [TestMethod]
        public void LengthUnitEnum_YardConstant_IsAccessible()
        {
            Assert.AreEqual(LengthUnit.YARD, LengthUnit.YARD);
        }

        /// <summary>
        /// Tests that LengthUnit.CENTIMETER constant is accessible.
        /// </summary>
        [TestMethod]
        public void LengthUnitEnum_CentimeterConstant_IsAccessible()
        {
            Assert.AreEqual(LengthUnit.CENTIMETER, LengthUnit.CENTIMETER);
        }

        #endregion

        #region ConvertToBaseUnit Tests

        /// <summary>
        /// Tests ConvertToBaseUnit for FEET unit.
        /// Verifies that converting feet to feet returns same value.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_FeetUnit_ReturnsSameValue()
        {
            double result = LengthUnit.FEET.ConvertToBaseUnit(5.0);
            Assert.AreEqual(5.0, result, Tolerance);
        }

        /// <summary>
        /// Tests ConvertToBaseUnit for INCH unit.
        /// Verifies that 12 inches convert to 1 foot.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_InchUnit_ReturnsCorrectValue()
        {
            double result = LengthUnit.INCH.ConvertToBaseUnit(12.0);
            Assert.AreEqual(1.0, result, Tolerance);
        }

        /// <summary>
        /// Tests ConvertToBaseUnit for YARD unit.
        /// Verifies that 1 yard converts to 3 feet.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_YardUnit_ReturnsCorrectValue()
        {
            double result = LengthUnit.YARD.ConvertToBaseUnit(1.0);
            Assert.AreEqual(3.0, result, Tolerance);
        }

        /// <summary>
        /// Tests ConvertToBaseUnit for CENTIMETER unit.
        /// Verifies that 30.48 cm converts to 1 foot.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_CentimeterUnit_ReturnsCorrectValue()
        {
            double result = LengthUnit.CENTIMETER.ConvertToBaseUnit(30.48);
            Assert.AreEqual(1.0, result, Tolerance);
        }

        #endregion

        #region ConvertFromBaseUnit Tests

        /// <summary>
        /// Tests ConvertFromBaseUnit for FEET unit.
        /// Verifies that converting feet to feet returns same value.
        /// </summary>
        [TestMethod]
        public void ConvertFromBaseUnit_FeetUnit_ReturnsSameValue()
        {
            double result = LengthUnit.FEET.ConvertFromBaseUnit(5.0);
            Assert.AreEqual(5.0, result, Tolerance);
        }

        /// <summary>
        /// Tests ConvertFromBaseUnit for INCH unit.
        /// Verifies that 1 foot converts to 12 inches.
        /// </summary>
        [TestMethod]
        public void ConvertFromBaseUnit_InchUnit_ReturnsCorrectValue()
        {
            double result = LengthUnit.INCH.ConvertFromBaseUnit(1.0);
            Assert.AreEqual(12.0, result, Tolerance);
        }

        /// <summary>
        /// Tests ConvertFromBaseUnit for YARD unit.
        /// Verifies that 3 feet convert to 1 yard.
        /// </summary>
        [TestMethod]
        public void ConvertFromBaseUnit_YardUnit_ReturnsCorrectValue()
        {
            double result = LengthUnit.YARD.ConvertFromBaseUnit(3.0);
            Assert.AreEqual(1.0, result, Tolerance);
        }

        /// <summary>
        /// Tests ConvertFromBaseUnit for CENTIMETER unit.
        /// Verifies that 1 foot converts to 30.48 cm.
        /// </summary>
        [TestMethod]
        public void ConvertFromBaseUnit_CentimeterUnit_ReturnsCorrectValue()
        {
            double result = LengthUnit.CENTIMETER.ConvertFromBaseUnit(1.0);
            Assert.AreEqual(30.48, result, Tolerance);
        }

        #endregion

        #region Direct Conversion Tests

        /// <summary>
        /// Tests direct conversion between units using the Convert method.
        /// </summary>
        [TestMethod]
        public void Convert_Direct_FeetToInches_ReturnsCorrectValue()
        {
            double result = LengthUnit.FEET.Convert(LengthUnit.INCH, 1.0);
            Assert.AreEqual(12.0, result, Tolerance);
        }

        /// <summary>
        /// Tests direct conversion between units using the Convert method.
        /// </summary>
        [TestMethod]
        public void Convert_Direct_InchesToFeet_ReturnsCorrectValue()
        {
            double result = LengthUnit.INCH.Convert(LengthUnit.FEET, 12.0);
            Assert.AreEqual(1.0, result, Tolerance);
        }

        /// <summary>
        /// Tests direct conversion between units using the Convert method.
        /// </summary>
        [TestMethod]
        public void Convert_Direct_YardsToInches_ReturnsCorrectValue()
        {
            double result = LengthUnit.YARD.Convert(LengthUnit.INCH, 1.0);
            Assert.AreEqual(36.0, result, Tolerance);
        }

        /// <summary>
        /// Tests direct conversion between units using the Convert method.
        /// </summary>
        [TestMethod]
        public void Convert_Direct_CentimetersToInches_ReturnsCorrectValue()
        {
            double result = LengthUnit.CENTIMETER.Convert(LengthUnit.INCH, 1.0);
            Assert.AreEqual(0.393700787, result, Tolerance);
        }

        #endregion

        #region Round-Trip Tests

        /// <summary>
        /// Tests round-trip conversion using the unit's conversion methods.
        /// </summary>
        [TestMethod]
        public void Convert_RoundTrip_ReturnsOriginalValue()
        {
            double originalValue = 5.0;

            double toInches = LengthUnit.FEET.Convert(LengthUnit.INCH, originalValue);
            double backToFeet = LengthUnit.INCH.Convert(LengthUnit.FEET, toInches);

            Assert.AreEqual(originalValue, backToFeet, Tolerance);
        }

        #endregion

        #region Unit Symbol Tests

        /// <summary>
        /// Tests GetUnitSymbol returns correct symbol for FEET.
        /// </summary>
        [TestMethod]
        public void GetUnitSymbol_FeetUnit_ReturnsFt()
        {
            string symbol = LengthUnit.FEET.GetUnitSymbol();
            Assert.AreEqual("ft", symbol);
        }

        /// <summary>
        /// Tests GetUnitSymbol returns correct symbol for INCH.
        /// </summary>
        [TestMethod]
        public void GetUnitSymbol_InchUnit_ReturnsIn()
        {
            string symbol = LengthUnit.INCH.GetUnitSymbol();
            Assert.AreEqual("in", symbol);
        }

        /// <summary>
        /// Tests GetUnitSymbol returns correct symbol for YARD.
        /// </summary>
        [TestMethod]
        public void GetUnitSymbol_YardUnit_ReturnsYd()
        {
            string symbol = LengthUnit.YARD.GetUnitSymbol();
            Assert.AreEqual("yd", symbol);
        }

        /// <summary>
        /// Tests GetUnitSymbol returns correct symbol for CENTIMETER.
        /// </summary>
        [TestMethod]
        public void GetUnitSymbol_CentimeterUnit_ReturnsCm()
        {
            string symbol = LengthUnit.CENTIMETER.GetUnitSymbol();
            Assert.AreEqual("cm", symbol);
        }

        #endregion

        #region Unit Name Tests

        /// <summary>
        /// Tests GetUnitName returns correct name for FEET.
        /// </summary>
        [TestMethod]
        public void GetUnitName_FeetUnit_ReturnsFeet()
        {
            string name = LengthUnit.FEET.GetUnitName();
            Assert.AreEqual("feet", name);
        }

        /// <summary>
        /// Tests GetUnitName returns correct name for INCH.
        /// </summary>
        [TestMethod]
        public void GetUnitName_InchUnit_ReturnsInches()
        {
            string name = LengthUnit.INCH.GetUnitName();
            Assert.AreEqual("inches", name);
        }

        /// <summary>
        /// Tests GetUnitName returns correct name for YARD.
        /// </summary>
        [TestMethod]
        public void GetUnitName_YardUnit_ReturnsYards()
        {
            string name = LengthUnit.YARD.GetUnitName();
            Assert.AreEqual("yards", name);
        }

        /// <summary>
        /// Tests GetUnitName returns correct name for CENTIMETER.
        /// </summary>
        [TestMethod]
        public void GetUnitName_CentimeterUnit_ReturnsCentimeters()
        {
            string name = LengthUnit.CENTIMETER.GetUnitName();
            Assert.AreEqual("centimeters", name);
        }

        #endregion

        #region Zero and Negative Value Tests

        /// <summary>
        /// Tests ConvertToBaseUnit with zero value.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_ZeroValue_ReturnsZero()
        {
            double result = LengthUnit.INCH.ConvertToBaseUnit(0.0);
            Assert.AreEqual(0.0, result, Tolerance);
        }

        /// <summary>
        /// Tests ConvertToBaseUnit with negative value.
        /// </summary>
        [TestMethod]
        public void ConvertToBaseUnit_NegativeValue_PreservesSign()
        {
            double result = LengthUnit.INCH.ConvertToBaseUnit(-12.0);
            Assert.AreEqual(-1.0, result, Tolerance);
        }

        #endregion

        #region Invalid Input Tests

        /// <summary>
        /// Tests ConvertToBaseUnit with NaN value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToBaseUnit_NaNValue_ThrowsArgumentException()
        {
            LengthUnit.FEET.ConvertToBaseUnit(double.NaN);
        }

        /// <summary>
        /// Tests ConvertToBaseUnit with Infinity value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToBaseUnit_InfinityValue_ThrowsArgumentException()
        {
            LengthUnit.FEET.ConvertToBaseUnit(double.PositiveInfinity);
        }

        #endregion
    }
}
