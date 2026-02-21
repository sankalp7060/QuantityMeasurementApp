using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.UnitTests
{
    /// <summary>
    /// Test class for LengthUnit enum and extensions.
    /// Tests unit conversion methods and properties.
    /// </summary>
    [TestClass]
    public class LengthUnitTests
    {
        private const double Tolerance = 0.000001;

        #region GetConversionFactor Tests

        /// <summary>
        /// Tests GetConversionFactor for FEET unit.
        /// </summary>
        [TestMethod]
        public void GetConversionFactor_Feet_ReturnsOne()
        {
            // Act
            double conversionFactor = LengthUnit.FEET.GetConversionFactor();

            // Assert
            Assert.AreEqual(1.0, conversionFactor, Tolerance);
        }

        /// <summary>
        /// Tests GetConversionFactor for INCH unit.
        /// </summary>
        [TestMethod]
        public void GetConversionFactor_Inch_ReturnsOneTwelfth()
        {
            // Act
            double conversionFactor = LengthUnit.INCH.GetConversionFactor();

            // Assert
            Assert.AreEqual(1.0 / 12.0, conversionFactor, Tolerance);
        }

        /// <summary>
        /// Tests GetConversionFactor for YARD unit.
        /// </summary>
        [TestMethod]
        public void GetConversionFactor_Yard_ReturnsThree()
        {
            // Act
            double conversionFactor = LengthUnit.YARD.GetConversionFactor();

            // Assert
            Assert.AreEqual(3.0, conversionFactor, Tolerance);
        }

        /// <summary>
        /// Tests GetConversionFactor for CENTIMETER unit.
        /// </summary>
        [TestMethod]
        public void GetConversionFactor_Centimeter_ReturnsCorrectValue()
        {
            // Act
            double conversionFactor = LengthUnit.CENTIMETER.GetConversionFactor();

            // Assert
            double expectedValue = 1.0 / (2.54 * 12.0);
            Assert.AreEqual(expectedValue, conversionFactor, Tolerance);
        }

        /// <summary>
        /// Tests that invalid unit throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidUnitException))]
        public void GetConversionFactor_InvalidUnit_ThrowsException()
        {
            // Arrange
            LengthUnit invalidUnit = (LengthUnit)99;

            // Act - Should throw
            invalidUnit.GetConversionFactor();
        }

        #endregion

        #region ToBaseUnit Tests

        /// <summary>
        /// Tests ToBaseUnit for FEET unit.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_Feet_ReturnsSameValue()
        {
            // Arrange
            double inputValue = 5.0;

            // Act
            double resultInBase = LengthUnit.FEET.ToBaseUnit(inputValue);

            // Assert
            Assert.AreEqual(inputValue, resultInBase, Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit for INCH unit.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_Inch_ReturnsCorrectValue()
        {
            // Arrange
            double inchesValue = 12.0;

            // Act
            double feetValue = LengthUnit.INCH.ToBaseUnit(inchesValue);

            // Assert
            Assert.AreEqual(1.0, feetValue, Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit for YARD unit.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_Yard_ReturnsCorrectValue()
        {
            // Arrange
            double yardsValue = 1.0;

            // Act
            double feetValue = LengthUnit.YARD.ToBaseUnit(yardsValue);

            // Assert
            Assert.AreEqual(3.0, feetValue, Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit for CENTIMETER unit.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_Centimeter_ReturnsCorrectValue()
        {
            // Arrange
            double cmValue = 30.48;

            // Act
            double feetValue = LengthUnit.CENTIMETER.ToBaseUnit(cmValue);

            // Assert
            Assert.AreEqual(1.0, feetValue, Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit with zero value.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_ZeroValue_ReturnsZero()
        {
            // Arrange
            double zeroValue = 0.0;

            // Act
            double resultInBase = LengthUnit.INCH.ToBaseUnit(zeroValue);

            // Assert
            Assert.AreEqual(0.0, resultInBase, Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit with negative value.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_NegativeValue_PreservesSign()
        {
            // Arrange
            double negativeValue = -12.0;

            // Act
            double resultInBase = LengthUnit.INCH.ToBaseUnit(negativeValue);

            // Assert
            Assert.AreEqual(-1.0, resultInBase, Tolerance);
        }

        #endregion

        #region FromBaseUnit Tests

        /// <summary>
        /// Tests FromBaseUnit for FEET unit.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_Feet_ReturnsSameValue()
        {
            // Arrange
            double feetValue = 5.0;

            // Act
            double result = LengthUnit.FEET.FromBaseUnit(feetValue);

            // Assert
            Assert.AreEqual(feetValue, result, Tolerance);
        }

        /// <summary>
        /// Tests FromBaseUnit for INCH unit.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_Inch_ReturnsCorrectValue()
        {
            // Arrange
            double feetValue = 1.0;

            // Act
            double inchesValue = LengthUnit.INCH.FromBaseUnit(feetValue);

            // Assert
            Assert.AreEqual(12.0, inchesValue, Tolerance);
        }

        /// <summary>
        /// Tests FromBaseUnit for YARD unit.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_Yard_ReturnsCorrectValue()
        {
            // Arrange
            double feetValue = 3.0;

            // Act
            double yardsValue = LengthUnit.YARD.FromBaseUnit(feetValue);

            // Assert
            Assert.AreEqual(1.0, yardsValue, Tolerance);
        }

        /// <summary>
        /// Tests FromBaseUnit for CENTIMETER unit.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_Centimeter_ReturnsCorrectValue()
        {
            // Arrange
            double feetValue = 1.0;

            // Act
            double cmValue = LengthUnit.CENTIMETER.FromBaseUnit(feetValue);

            // Assert
            Assert.AreEqual(30.48, cmValue, Tolerance);
        }

        #endregion

        #region ConvertTo Tests

        /// <summary>
        /// Tests ConvertTo for Feet to Inches.
        /// </summary>
        [TestMethod]
        public void ConvertTo_FeetToInches_ReturnsCorrectValue()
        {
            // Arrange
            double feetValue = 1.0;

            // Act
            double inchesValue = LengthUnit.FEET.ConvertTo(LengthUnit.INCH, feetValue);

            // Assert
            Assert.AreEqual(12.0, inchesValue, Tolerance);
        }

        /// <summary>
        /// Tests ConvertTo for Inches to Feet.
        /// </summary>
        [TestMethod]
        public void ConvertTo_InchesToFeet_ReturnsCorrectValue()
        {
            // Arrange
            double inchesValue = 12.0;

            // Act
            double feetValue = LengthUnit.INCH.ConvertTo(LengthUnit.FEET, inchesValue);

            // Assert
            Assert.AreEqual(1.0, feetValue, Tolerance);
        }

        /// <summary>
        /// Tests ConvertTo for Yards to Inches.
        /// </summary>
        [TestMethod]
        public void ConvertTo_YardsToInches_ReturnsCorrectValue()
        {
            // Arrange
            double yardsValue = 1.0;

            // Act
            double inchesValue = LengthUnit.YARD.ConvertTo(LengthUnit.INCH, yardsValue);

            // Assert
            Assert.AreEqual(36.0, inchesValue, Tolerance);
        }

        /// <summary>
        /// Tests ConvertTo for Centimeters to Inches.
        /// </summary>
        [TestMethod]
        public void ConvertTo_CentimetersToInches_ReturnsCorrectValue()
        {
            // Arrange
            double cmValue = 2.54;

            // Act
            double inchesValue = LengthUnit.CENTIMETER.ConvertTo(LengthUnit.INCH, cmValue);

            // Assert
            Assert.AreEqual(1.0, inchesValue, Tolerance);
        }

        /// <summary>
        /// Tests round-trip conversion.
        /// </summary>
        [TestMethod]
        public void ConvertTo_RoundTrip_ReturnsOriginalValue()
        {
            // Arrange
            double originalValue = 5.0;

            // Act
            double toInches = LengthUnit.FEET.ConvertTo(LengthUnit.INCH, originalValue);
            double backToFeet = LengthUnit.INCH.ConvertTo(LengthUnit.FEET, toInches);

            // Assert
            Assert.AreEqual(originalValue, backToFeet, Tolerance);
        }

        #endregion

        #region GetSymbol Tests

        /// <summary>
        /// Tests GetSymbol returns correct symbol for each unit.
        /// </summary>
        [TestMethod]
        public void GetSymbol_ReturnsCorrectSymbol()
        {
            // Assert
            Assert.AreEqual("ft", LengthUnit.FEET.GetSymbol());
            Assert.AreEqual("in", LengthUnit.INCH.GetSymbol());
            Assert.AreEqual("yd", LengthUnit.YARD.GetSymbol());
            Assert.AreEqual("cm", LengthUnit.CENTIMETER.GetSymbol());
        }

        #endregion

        #region GetName Tests

        /// <summary>
        /// Tests GetName returns correct name for each unit.
        /// </summary>
        [TestMethod]
        public void GetName_ReturnsCorrectName()
        {
            // Assert
            Assert.AreEqual("feet", LengthUnit.FEET.GetName());
            Assert.AreEqual("inches", LengthUnit.INCH.GetName());
            Assert.AreEqual("yards", LengthUnit.YARD.GetName());
            Assert.AreEqual("centimeters", LengthUnit.CENTIMETER.GetName());
        }

        #endregion
    }
}
