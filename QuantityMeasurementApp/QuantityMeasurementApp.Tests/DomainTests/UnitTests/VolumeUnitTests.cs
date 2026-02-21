using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.UnitTests
{
    /// <summary>
    /// Test class for VolumeUnit implementing IMeasurable interface.
    /// UC11: Verifies that VolumeUnit correctly implements IMeasurable.
    /// </summary>
    [TestClass]
    public class VolumeUnitTests
    {
        private const double Tolerance = 0.000001;
        private const double GallonTolerance = 0.001;

        #region IMeasurable Implementation Tests

        /// <summary>
        /// Tests that VolumeUnit implements IMeasurable interface.
        /// </summary>
        [TestMethod]
        public void VolumeUnit_Implements_IMeasurable()
        {
            // Arrange
            var unit = VolumeUnit.LITRE;

            // Assert
            Assert.IsTrue(unit is IMeasurable, "VolumeUnit should implement IMeasurable");
        }

        /// <summary>
        /// Tests GetConversionFactor for all VolumeUnit values.
        /// </summary>
        [TestMethod]
        public void GetConversionFactor_AllValues_ReturnsCorrectFactors()
        {
            // Act & Assert
            Assert.AreEqual(1.0, VolumeUnit.LITRE.GetConversionFactor(), Tolerance);
            Assert.AreEqual(0.001, VolumeUnit.MILLILITRE.GetConversionFactor(), Tolerance);
            Assert.AreEqual(3.78541, VolumeUnit.GALLON.GetConversionFactor(), Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit for all VolumeUnit values.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_AllValues_ReturnsCorrectValues()
        {
            // Act & Assert
            Assert.AreEqual(1.0, VolumeUnit.LITRE.ToBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, VolumeUnit.MILLILITRE.ToBaseUnit(1000.0), Tolerance);
            Assert.AreEqual(3.78541, VolumeUnit.GALLON.ToBaseUnit(1.0), Tolerance);
        }

        /// <summary>
        /// Tests FromBaseUnit for all VolumeUnit values.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_AllValues_ReturnsCorrectValues()
        {
            // Act & Assert
            Assert.AreEqual(1.0, VolumeUnit.LITRE.FromBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1000.0, VolumeUnit.MILLILITRE.FromBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, VolumeUnit.GALLON.FromBaseUnit(3.78541), GallonTolerance);
        }

        /// <summary>
        /// Tests GetSymbol for all VolumeUnit values.
        /// </summary>
        [TestMethod]
        public void GetSymbol_AllValues_ReturnsCorrectSymbols()
        {
            // Assert
            Assert.AreEqual("L", VolumeUnit.LITRE.GetSymbol());
            Assert.AreEqual("mL", VolumeUnit.MILLILITRE.GetSymbol());
            Assert.AreEqual("gal", VolumeUnit.GALLON.GetSymbol());
        }

        /// <summary>
        /// Tests GetName for all VolumeUnit values.
        /// </summary>
        [TestMethod]
        public void GetName_AllValues_ReturnsCorrectNames()
        {
            // Assert
            Assert.AreEqual("litres", VolumeUnit.LITRE.GetName());
            Assert.AreEqual("millilitres", VolumeUnit.MILLILITRE.GetName());
            Assert.AreEqual("gallons", VolumeUnit.GALLON.GetName());
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
            VolumeUnit.LITRE.ToBaseUnit(double.NaN);
        }

        /// <summary>
        /// Tests FromBaseUnit with invalid value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void FromBaseUnit_NaNValue_ThrowsException()
        {
            // Act - Should throw
            VolumeUnit.LITRE.FromBaseUnit(double.NaN);
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
            Assert.IsTrue(VolumeUnit.LITRE.Equals(VolumeUnit.LITRE), "Same unit should be equal");
            Assert.IsTrue(
                VolumeUnit.MILLILITRE.Equals(VolumeUnit.MILLILITRE),
                "Same unit should be equal"
            );
            Assert.IsTrue(VolumeUnit.GALLON.Equals(VolumeUnit.GALLON), "Same unit should be equal");
        }

        /// <summary>
        /// Tests that different unit instances are not equal.
        /// </summary>
        [TestMethod]
        public void Equals_DifferentUnit_ReturnsFalse()
        {
            // Assert
            Assert.IsFalse(
                VolumeUnit.LITRE.Equals(VolumeUnit.MILLILITRE),
                "Different units should not be equal"
            );
            Assert.IsFalse(
                VolumeUnit.LITRE.Equals(VolumeUnit.GALLON),
                "Different units should not be equal"
            );
        }

        #endregion
    }
}
