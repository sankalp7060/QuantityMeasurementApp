using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.UnitTests
{
    /// <summary>
    /// Test class for WeightUnit enum and extensions.
    /// Tests weight unit conversion methods and properties.
    /// </summary>
    [TestClass]
    public class WeightUnitTests
    {
        private const double Tolerance = 0.000001;

        #region GetConversionFactor Tests

        /// <summary>
        /// Tests GetConversionFactor for KILOGRAM unit.
        /// </summary>
        [TestMethod]
        public void GetConversionFactor_Kilogram_ReturnsOne()
        {
            // Act
            double conversionFactor = WeightUnit.KILOGRAM.GetConversionFactor();

            // Assert
            Assert.AreEqual(1.0, conversionFactor, Tolerance);
        }

        /// <summary>
        /// Tests GetConversionFactor for GRAM unit.
        /// </summary>
        [TestMethod]
        public void GetConversionFactor_Gram_ReturnsZeroPointZeroZeroOne()
        {
            // Act
            double conversionFactor = WeightUnit.GRAM.GetConversionFactor();

            // Assert
            Assert.AreEqual(0.001, conversionFactor, Tolerance);
        }

        /// <summary>
        /// Tests GetConversionFactor for POUND unit.
        /// </summary>
        [TestMethod]
        public void GetConversionFactor_Pound_ReturnsCorrectValue()
        {
            // Act
            double conversionFactor = WeightUnit.POUND.GetConversionFactor();

            // Assert
            Assert.AreEqual(0.453592, conversionFactor, Tolerance);
        }

        /// <summary>
        /// Tests that invalid unit throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidUnitException))]
        public void GetConversionFactor_InvalidUnit_ThrowsException()
        {
            // Arrange
            WeightUnit invalidUnit = (WeightUnit)99;

            // Act - Should throw
            invalidUnit.GetConversionFactor();
        }

        #endregion

        #region ToBaseUnit Tests

        /// <summary>
        /// Tests ToBaseUnit for KILOGRAM unit.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_Kilogram_ReturnsSameValue()
        {
            // Arrange
            double inputValue = 5.0;

            // Act
            double resultInBase = WeightUnit.KILOGRAM.ToBaseUnit(inputValue);

            // Assert
            Assert.AreEqual(inputValue, resultInBase, Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit for GRAM unit.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_Gram_ReturnsCorrectValue()
        {
            // Arrange
            double gramsValue = 1000.0;

            // Act
            double kgValue = WeightUnit.GRAM.ToBaseUnit(gramsValue);

            // Assert
            Assert.AreEqual(1.0, kgValue, Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit for POUND unit.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_Pound_ReturnsCorrectValue()
        {
            // Arrange
            double poundsValue = 1.0;

            // Act
            double kgValue = WeightUnit.POUND.ToBaseUnit(poundsValue);

            // Assert
            Assert.AreEqual(0.453592, kgValue, Tolerance);
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
            double resultInBase = WeightUnit.GRAM.ToBaseUnit(zeroValue);

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
            double negativeValue = -1000.0;

            // Act
            double resultInBase = WeightUnit.GRAM.ToBaseUnit(negativeValue);

            // Assert
            Assert.AreEqual(-1.0, resultInBase, Tolerance);
        }

        #endregion

        #region FromBaseUnit Tests

        /// <summary>
        /// Tests FromBaseUnit for KILOGRAM unit.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_Kilogram_ReturnsSameValue()
        {
            // Arrange
            double kgValue = 5.0;

            // Act
            double result = WeightUnit.KILOGRAM.FromBaseUnit(kgValue);

            // Assert
            Assert.AreEqual(kgValue, result, Tolerance);
        }

        /// <summary>
        /// Tests FromBaseUnit for GRAM unit.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_Gram_ReturnsCorrectValue()
        {
            // Arrange
            double kgValue = 1.0;

            // Act
            double gramsValue = WeightUnit.GRAM.FromBaseUnit(kgValue);

            // Assert
            Assert.AreEqual(1000.0, gramsValue, Tolerance);
        }

        /// <summary>
        /// Tests FromBaseUnit for POUND unit.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_Pound_ReturnsCorrectValue()
        {
            // Arrange
            double kgValue = 0.453592;

            // Act
            double poundsValue = WeightUnit.POUND.FromBaseUnit(kgValue);

            // Assert
            Assert.AreEqual(1.0, poundsValue, Tolerance);
        }

        #endregion

        #region ConvertTo Tests

        /// <summary>
        /// Tests ConvertTo for Kilograms to Grams.
        /// </summary>
        [TestMethod]
        public void ConvertTo_KilogramsToGrams_ReturnsCorrectValue()
        {
            // Arrange
            double kgValue = 1.0;

            // Act
            double gramsValue = WeightUnit.KILOGRAM.ConvertTo(WeightUnit.GRAM, kgValue);

            // Assert
            Assert.AreEqual(1000.0, gramsValue, Tolerance);
        }

        /// <summary>
        /// Tests ConvertTo for Grams to Kilograms.
        /// </summary>
        [TestMethod]
        public void ConvertTo_GramsToKilograms_ReturnsCorrectValue()
        {
            // Arrange
            double gramsValue = 1000.0;

            // Act
            double kgValue = WeightUnit.GRAM.ConvertTo(WeightUnit.KILOGRAM, gramsValue);

            // Assert
            Assert.AreEqual(1.0, kgValue, Tolerance);
        }

        /// <summary>
        /// Tests ConvertTo for Kilograms to Pounds.
        /// </summary>
        [TestMethod]
        public void ConvertTo_KilogramsToPounds_ReturnsCorrectValue()
        {
            // Arrange
            double kgValue = 1.0;

            // Act
            double poundsValue = WeightUnit.KILOGRAM.ConvertTo(WeightUnit.POUND, kgValue);

            // Assert
            Assert.AreEqual(2.20462, poundsValue, 0.001);
        }

        /// <summary>
        /// Tests ConvertTo for Pounds to Kilograms.
        /// </summary>
        [TestMethod]
        public void ConvertTo_PoundsToKilograms_ReturnsCorrectValue()
        {
            // Arrange
            double poundsValue = 2.20462;

            // Act
            double kgValue = WeightUnit.POUND.ConvertTo(WeightUnit.KILOGRAM, poundsValue);

            // Assert
            Assert.AreEqual(1.0, kgValue, 0.001);
        }

        /// <summary>
        /// Tests ConvertTo for Grams to Pounds.
        /// </summary>
        [TestMethod]
        public void ConvertTo_GramsToPounds_ReturnsCorrectValue()
        {
            // Arrange
            double gramsValue = 453.592;

            // Act
            double poundsValue = WeightUnit.GRAM.ConvertTo(WeightUnit.POUND, gramsValue);

            // Assert
            Assert.AreEqual(1.0, poundsValue, 0.001);
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
            double toGrams = WeightUnit.KILOGRAM.ConvertTo(WeightUnit.GRAM, originalValue);
            double backToKg = WeightUnit.GRAM.ConvertTo(WeightUnit.KILOGRAM, toGrams);

            // Assert
            Assert.AreEqual(originalValue, backToKg, Tolerance);
        }

        #endregion

        #region GetSymbol Tests

        /// <summary>
        /// Tests GetSymbol returns correct symbol for each weight unit.
        /// </summary>
        [TestMethod]
        public void GetSymbol_ReturnsCorrectSymbol()
        {
            // Assert
            Assert.AreEqual("kg", WeightUnit.KILOGRAM.GetSymbol());
            Assert.AreEqual("g", WeightUnit.GRAM.GetSymbol());
            Assert.AreEqual("lb", WeightUnit.POUND.GetSymbol());
        }

        #endregion

        #region GetName Tests

        /// <summary>
        /// Tests GetName returns correct name for each weight unit.
        /// </summary>
        [TestMethod]
        public void GetName_ReturnsCorrectName()
        {
            // Assert
            Assert.AreEqual("kilograms", WeightUnit.KILOGRAM.GetName());
            Assert.AreEqual("grams", WeightUnit.GRAM.GetName());
            Assert.AreEqual("pounds", WeightUnit.POUND.GetName());
        }

        #endregion
    }
}
