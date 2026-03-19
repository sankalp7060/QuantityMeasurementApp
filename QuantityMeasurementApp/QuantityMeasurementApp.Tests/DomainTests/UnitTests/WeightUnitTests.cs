using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.UnitTests
{
    /// <summary>
    /// Test class for WeightUnit implementing IMeasurable interface.
    /// UC10: Verifies that WeightUnit correctly implements IMeasurable.
    /// </summary>
    [TestClass]
    public class WeightUnitTests
    {
        private const double Tolerance = 0.000001;
        private const double PoundTolerance = 0.001;

        #region IMeasurable Implementation Tests

        /// <summary>
        /// Tests that WeightUnit implements IMeasurable interface.
        /// </summary>
        [TestMethod]
        public void WeightUnit_Implements_IMeasurable()
        {
            // Arrange
            var unit = WeightUnit.KILOGRAM;

            // Assert
            Assert.IsTrue(unit is IMeasurable, "WeightUnit should implement IMeasurable");
        }

        /// <summary>
        /// Tests GetConversionFactor for all WeightUnit values.
        /// </summary>
        [TestMethod]
        public void GetConversionFactor_AllValues_ReturnsCorrectFactors()
        {
            // Act & Assert
            Assert.AreEqual(1.0, WeightUnit.KILOGRAM.GetConversionFactor(), Tolerance);
            Assert.AreEqual(0.001, WeightUnit.GRAM.GetConversionFactor(), Tolerance);
            Assert.AreEqual(0.45359237, WeightUnit.POUND.GetConversionFactor(), Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit for all WeightUnit values.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_AllValues_ReturnsCorrectValues()
        {
            // Act & Assert
            Assert.AreEqual(1.0, WeightUnit.KILOGRAM.ToBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, WeightUnit.GRAM.ToBaseUnit(1000.0), Tolerance);
            Assert.AreEqual(0.45359237, WeightUnit.POUND.ToBaseUnit(1.0), Tolerance);
        }

        /// <summary>
        /// Tests FromBaseUnit for all WeightUnit values.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_AllValues_ReturnsCorrectValues()
        {
            // Act & Assert
            Assert.AreEqual(1.0, WeightUnit.KILOGRAM.FromBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1000.0, WeightUnit.GRAM.FromBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, WeightUnit.POUND.FromBaseUnit(0.45359237), Tolerance);
        }

        /// <summary>
        /// Tests GetSymbol for all WeightUnit values.
        /// </summary>
        [TestMethod]
        public void GetSymbol_AllValues_ReturnsCorrectSymbols()
        {
            // Assert
            Assert.AreEqual("kg", WeightUnit.KILOGRAM.GetSymbol());
            Assert.AreEqual("g", WeightUnit.GRAM.GetSymbol());
            Assert.AreEqual("lb", WeightUnit.POUND.GetSymbol());
        }

        /// <summary>
        /// Tests GetName for all WeightUnit values.
        /// </summary>
        [TestMethod]
        public void GetName_AllValues_ReturnsCorrectNames()
        {
            // Assert
            Assert.AreEqual("kilograms", WeightUnit.KILOGRAM.GetName());
            Assert.AreEqual("grams", WeightUnit.GRAM.GetName());
            Assert.AreEqual("pounds", WeightUnit.POUND.GetName());
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
            WeightUnit.KILOGRAM.ToBaseUnit(double.NaN);
        }

        /// <summary>
        /// Tests FromBaseUnit with invalid value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void FromBaseUnit_NaNValue_ThrowsException()
        {
            // Act - Should throw
            WeightUnit.KILOGRAM.FromBaseUnit(double.NaN);
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
            Assert.IsTrue(
                WeightUnit.KILOGRAM.Equals(WeightUnit.KILOGRAM),
                "Same unit should be equal"
            );
            Assert.IsTrue(WeightUnit.GRAM.Equals(WeightUnit.GRAM), "Same unit should be equal");
            Assert.IsTrue(WeightUnit.POUND.Equals(WeightUnit.POUND), "Same unit should be equal");
        }

        /// <summary>
        /// Tests that different unit instances are not equal.
        /// </summary>
        [TestMethod]
        public void Equals_DifferentUnit_ReturnsFalse()
        {
            // Assert
            Assert.IsFalse(
                WeightUnit.KILOGRAM.Equals(WeightUnit.GRAM),
                "Different units should not be equal"
            );
            Assert.IsFalse(
                WeightUnit.KILOGRAM.Equals(WeightUnit.POUND),
                "Different units should not be equal"
            );
        }

        #endregion
    }
}
