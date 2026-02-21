using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.UnitTests
{
    /// <summary>
    /// Test class for LengthUnit implementing IMeasurable interface.
    /// UC10: Verifies that LengthUnit correctly implements IMeasurable.
    /// </summary>
    [TestClass]
    public class LengthUnitTests
    {
        private const double Tolerance = 0.000001;

        #region IMeasurable Implementation Tests

        /// <summary>
        /// Tests that LengthUnit implements IMeasurable interface.
        /// </summary>
        [TestMethod]
        public void LengthUnit_Implements_IMeasurable()
        {
            // Arrange
            var unit = LengthUnit.FEET;

            // Assert
            Assert.IsTrue(unit is IMeasurable, "LengthUnit should implement IMeasurable");
        }

        /// <summary>
        /// Tests GetConversionFactor for all LengthUnit values.
        /// </summary>
        [TestMethod]
        public void GetConversionFactor_AllValues_ReturnsCorrectFactors()
        {
            // Act & Assert
            Assert.AreEqual(1.0, LengthUnit.FEET.GetConversionFactor(), Tolerance);
            Assert.AreEqual(1.0 / 12.0, LengthUnit.INCH.GetConversionFactor(), Tolerance);
            Assert.AreEqual(3.0, LengthUnit.YARD.GetConversionFactor(), Tolerance);

            double cmFactor = 1.0 / (2.54 * 12.0);
            Assert.AreEqual(cmFactor, LengthUnit.CENTIMETER.GetConversionFactor(), Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit for all LengthUnit values.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_AllValues_ReturnsCorrectValues()
        {
            // Act & Assert
            Assert.AreEqual(5.0, LengthUnit.FEET.ToBaseUnit(5.0), Tolerance);
            Assert.AreEqual(1.0, LengthUnit.INCH.ToBaseUnit(12.0), Tolerance);
            Assert.AreEqual(3.0, LengthUnit.YARD.ToBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, LengthUnit.CENTIMETER.ToBaseUnit(30.48), Tolerance);
        }

        /// <summary>
        /// Tests FromBaseUnit for all LengthUnit values.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_AllValues_ReturnsCorrectValues()
        {
            // Act & Assert
            Assert.AreEqual(5.0, LengthUnit.FEET.FromBaseUnit(5.0), Tolerance);
            Assert.AreEqual(12.0, LengthUnit.INCH.FromBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, LengthUnit.YARD.FromBaseUnit(3.0), Tolerance);
            Assert.AreEqual(30.48, LengthUnit.CENTIMETER.FromBaseUnit(1.0), Tolerance);
        }

        /// <summary>
        /// Tests GetSymbol for all LengthUnit values.
        /// </summary>
        [TestMethod]
        public void GetSymbol_AllValues_ReturnsCorrectSymbols()
        {
            // Assert
            Assert.AreEqual("ft", LengthUnit.FEET.GetSymbol());
            Assert.AreEqual("in", LengthUnit.INCH.GetSymbol());
            Assert.AreEqual("yd", LengthUnit.YARD.GetSymbol());
            Assert.AreEqual("cm", LengthUnit.CENTIMETER.GetSymbol());
        }

        /// <summary>
        /// Tests GetName for all LengthUnit values.
        /// </summary>
        [TestMethod]
        public void GetName_AllValues_ReturnsCorrectNames()
        {
            // Assert
            Assert.AreEqual("feet", LengthUnit.FEET.GetName());
            Assert.AreEqual("inches", LengthUnit.INCH.GetName());
            Assert.AreEqual("yards", LengthUnit.YARD.GetName());
            Assert.AreEqual("centimeters", LengthUnit.CENTIMETER.GetName());
        }

        #endregion

        #region Validation Tests

        /// <summary>
        /// Tests ToBaseUnit with invalid value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void ToBaseUnit_NaNValue_ThrowsException()
        {
            // Act - Should throw
            LengthUnit.FEET.ToBaseUnit(double.NaN);
        }

        /// <summary>
        /// Tests FromBaseUnit with invalid value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void FromBaseUnit_NaNValue_ThrowsException()
        {
            // Act - Should throw
            LengthUnit.FEET.FromBaseUnit(double.NaN);
        }

        #endregion

        #region Equality Tests

        /// <summary>
        /// Tests that the same unit instances are equal.
        /// </summary>
        [TestMethod]
        public void Equals_SameUnit_ReturnsTrue()
        {
            // Assert
            Assert.IsTrue(LengthUnit.FEET.Equals(LengthUnit.FEET), "Same unit should be equal");
            Assert.IsTrue(LengthUnit.INCH.Equals(LengthUnit.INCH), "Same unit should be equal");
        }

        /// <summary>
        /// Tests that different unit instances are not equal.
        /// </summary>
        [TestMethod]
        public void Equals_DifferentUnit_ReturnsFalse()
        {
            // Assert
            Assert.IsFalse(
                LengthUnit.FEET.Equals(LengthUnit.INCH),
                "Different units should not be equal"
            );
        }

        #endregion
    }
}
