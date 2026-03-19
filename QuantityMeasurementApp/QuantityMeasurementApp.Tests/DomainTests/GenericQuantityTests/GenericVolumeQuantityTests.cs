using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.GenericQuantityTests
{
    /// <summary>
    /// Test class for GenericQuantity with VolumeUnit.
    /// UC11: Tests volume measurements equality, conversion, and addition.
    /// Updated to use rounded tolerance for explicit target unit tests.
    /// </summary>
    [TestClass]
    public class GenericVolumeQuantityTests
    {
        private const double Tolerance = 0.000001;
        private const double RoundedTolerance = 0.01; // For rounded values (2 decimal places)
        private const double GallonTolerance = 0.001;

        #region Equality Tests

        [TestMethod]
        public void Equals_Volume_LitreSameValue_ReturnsTrue()
        {
            // Arrange
            var firstVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var secondVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            bool areEqual = firstVolume.Equals(secondVolume);

            // Assert
            Assert.IsTrue(areEqual, "1 L should equal 1 L");
        }

        [TestMethod]
        public void Equals_Volume_LitreDifferentValue_ReturnsFalse()
        {
            // Arrange
            var firstVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var secondVolume = new GenericQuantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            // Act
            bool areEqual = firstVolume.Equals(secondVolume);

            // Assert
            Assert.IsFalse(areEqual, "1 L should not equal 2 L");
        }

        [TestMethod]
        public void Equals_Volume_LitreToMillilitreEquivalent_ReturnsTrue()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            bool areEqual = litreVolume.Equals(mlVolume);

            // Assert
            Assert.IsTrue(areEqual, "1 L should equal 1000 mL");
        }

        [TestMethod]
        public void Equals_Volume_LitreToGallonEquivalent_ReturnsTrue()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var gallonVolume = new GenericQuantity<VolumeUnit>(0.264172, VolumeUnit.GALLON);

            // Act
            bool areEqual = litreVolume.Equals(gallonVolume);

            // Assert
            Assert.IsTrue(areEqual, "1 L should approximately equal 0.264172 gal");
        }

        [TestMethod]
        public void Equals_Volume_GallonToLitreEquivalent_ReturnsTrue()
        {
            // Arrange
            var gallonVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.GALLON);
            var litreVolume = new GenericQuantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);

            // Act
            bool areEqual = gallonVolume.Equals(litreVolume);

            // Assert
            Assert.IsTrue(areEqual, "1 gal should approximately equal 3.78541 L");
        }

        [TestMethod]
        public void Equals_Volume_Reflexive_ReturnsTrue()
        {
            // Arrange
            var volume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            bool isEqualToItself = volume.Equals(volume);

            // Assert
            Assert.IsTrue(isEqualToItself, "Object should equal itself");
        }

        [TestMethod]
        public void Equals_Volume_Symmetric_ReturnsTrue()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            bool litreEqualsMl = litreVolume.Equals(mlVolume);
            bool mlEqualsLitre = mlVolume.Equals(litreVolume);

            // Assert
            Assert.IsTrue(litreEqualsMl && mlEqualsLitre, "Equality should be symmetric");
        }

        [TestMethod]
        public void Equals_Volume_NullComparison_ReturnsFalse()
        {
            // Arrange
            var volume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            bool isEqualToNull = volume.Equals(null);

            // Assert
            Assert.IsFalse(isEqualToNull, "Object should not equal null");
        }

        [TestMethod]
        public void Equals_VolumeVsLength_ReturnsFalse()
        {
            // Arrange
            var volume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var length = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act
            bool areEqual = volume.Equals(length);

            // Assert
            Assert.IsFalse(areEqual, "Volume and length should not be equal");
        }

        [TestMethod]
        public void Equals_VolumeVsWeight_ReturnsFalse()
        {
            // Arrange
            var volume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var weight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            bool areEqual = volume.Equals(weight);

            // Assert
            Assert.IsFalse(areEqual, "Volume and weight should not be equal");
        }

        #endregion

        #region Conversion Tests

        [TestMethod]
        public void ConvertTo_Volume_LitresToMillilitres_ReturnsCorrectQuantity()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            var mlVolume = litreVolume.ConvertTo(VolumeUnit.MILLILITRE);

            // Assert
            Assert.AreEqual(1000.0, mlVolume.Value, Tolerance, "1 L should convert to 1000 mL");
            Assert.AreEqual(VolumeUnit.MILLILITRE, mlVolume.Unit, "Unit should be millilitres");
        }

        [TestMethod]
        public void ConvertTo_Volume_MillilitresToLitres_ReturnsCorrectQuantity()
        {
            // Arrange
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            var litreVolume = mlVolume.ConvertTo(VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(1.0, litreVolume.Value, Tolerance, "1000 mL should convert to 1 L");
            Assert.AreEqual(VolumeUnit.LITRE, litreVolume.Unit, "Unit should be litres");
        }

        [TestMethod]
        public void ConvertTo_Volume_LitresToGallons_ReturnsCorrectQuantity()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);

            // Act
            var gallonVolume = litreVolume.ConvertTo(VolumeUnit.GALLON);

            // Assert
            Assert.AreEqual(
                1.0,
                gallonVolume.Value,
                GallonTolerance,
                "3.78541 L should convert to approximately 1 gal"
            );
            Assert.AreEqual(VolumeUnit.GALLON, gallonVolume.Unit, "Unit should be gallons");
        }

        [TestMethod]
        public void ConvertTo_Volume_GallonsToLitres_ReturnsCorrectQuantity()
        {
            // Arrange
            var gallonVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.GALLON);

            // Act
            var litreVolume = gallonVolume.ConvertTo(VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(
                3.78541,
                litreVolume.Value,
                GallonTolerance,
                "1 gal should convert to approximately 3.78541 L"
            );
            Assert.AreEqual(VolumeUnit.LITRE, litreVolume.Unit, "Unit should be litres");
        }

        [TestMethod]
        public void ConvertTo_Volume_MillilitresToGallons_ReturnsCorrectQuantity()
        {
            // Arrange
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            var gallonVolume = mlVolume.ConvertTo(VolumeUnit.GALLON);

            // Assert
            double expectedValue = 0.264172; // 1000 mL = 0.264172 gal
            Assert.AreEqual(
                expectedValue,
                gallonVolume.Value,
                GallonTolerance,
                "1000 mL should convert to approximately 0.264172 gal"
            );
            Assert.AreEqual(VolumeUnit.GALLON, gallonVolume.Unit, "Unit should be gallons");
        }

        [TestMethod]
        public void ConvertTo_Volume_RoundTrip_ReturnsOriginalValue()
        {
            // Arrange
            double originalValue = 2.5;
            var originalVolume = new GenericQuantity<VolumeUnit>(originalValue, VolumeUnit.LITRE);

            // Act
            var inMl = originalVolume.ConvertTo(VolumeUnit.MILLILITRE);
            var backToLitre = inMl.ConvertTo(VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(
                originalValue,
                backToLitre.Value,
                Tolerance,
                "Round-trip L->mL->L should return original"
            );
        }

        #endregion

        #region Addition Tests

        [TestMethod]
        public void Add_Volume_SameUnit_Litres_ReturnsCorrectSum()
        {
            // Arrange
            var firstVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var secondVolume = new GenericQuantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            // Act
            var sumVolume = firstVolume.Add(secondVolume);

            // Assert
            Assert.AreEqual(3.0, sumVolume.Value, Tolerance, "1 L + 2 L should equal 3 L");
            Assert.AreEqual(VolumeUnit.LITRE, sumVolume.Unit, "Result should be in litres");
        }

        [TestMethod]
        public void Add_Volume_SameUnit_Millilitres_ReturnsCorrectSum()
        {
            // Arrange
            var firstVolume = new GenericQuantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);
            var secondVolume = new GenericQuantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            // Act
            var sumVolume = firstVolume.Add(secondVolume);

            // Assert
            Assert.AreEqual(
                1000.0,
                sumVolume.Value,
                Tolerance,
                "500 mL + 500 mL should equal 1000 mL"
            );
            Assert.AreEqual(
                VolumeUnit.MILLILITRE,
                sumVolume.Unit,
                "Result should be in millilitres"
            );
        }

        [TestMethod]
        public void Add_Volume_CrossUnit_ResultInFirstUnit_ReturnsCorrectSum()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            // Act
            var sumVolume = litreVolume.Add(mlVolume);

            // Assert
            Assert.AreEqual(1.5, sumVolume.Value, Tolerance, "1 L + 500 mL should equal 1.5 L");
            Assert.AreEqual(VolumeUnit.LITRE, sumVolume.Unit, "Result should be in litres");
        }

        [TestMethod]
        public void Add_Volume_CrossUnit_ResultInSecondUnit_ReturnsCorrectSum()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            // Act
            var sumVolume = litreVolume.Add(mlVolume, VolumeUnit.MILLILITRE);

            // Assert
            Assert.AreEqual(
                1500.0,
                sumVolume.Value,
                Tolerance,
                "1 L + 500 mL in mL should equal 1500 mL"
            );
            Assert.AreEqual(
                VolumeUnit.MILLILITRE,
                sumVolume.Unit,
                "Result should be in millilitres"
            );
        }

        [TestMethod]
        public void Add_Volume_ExplicitTarget_Gallons_ReturnsCorrectSum()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(3.78541, VolumeUnit.LITRE); // 1 gallon
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE); // 1 litre

            // Act
            var sumInGallons = litreVolume.Add(mlVolume, VolumeUnit.GALLON);

            // Assert
            double expectedValue = 1.26; // 1 gal + 0.264 gal = 1.264 gal rounded to 2 decimal places
            Assert.AreEqual(
                expectedValue,
                sumInGallons.Value,
                RoundedTolerance,
                "1 gal + 1000 mL in gallons should be 1.26 gal (rounded)"
            );
            Assert.AreEqual(VolumeUnit.GALLON, sumInGallons.Unit, "Result should be in gallons");
        }

        [TestMethod]
        public void Add_Volume_Static_ReturnsCorrectSum()
        {
            // Act
            var sumVolume = GenericQuantity<VolumeUnit>.Add(
                1.0,
                VolumeUnit.LITRE,
                1000.0,
                VolumeUnit.MILLILITRE,
                VolumeUnit.LITRE
            );

            // Assert
            Assert.AreEqual(2.0, sumVolume.Value, Tolerance, "1 L + 1000 mL should equal 2 L");
            Assert.AreEqual(VolumeUnit.LITRE, sumVolume.Unit, "Result should be in litres");
        }

        [TestMethod]
        public void Add_Volume_WithZero_ReturnsOriginalValue()
        {
            // Arrange
            var originalVolume = new GenericQuantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var zeroVolume = new GenericQuantity<VolumeUnit>(0.0, VolumeUnit.MILLILITRE);

            // Act
            var sumVolume = originalVolume.Add(zeroVolume);

            // Assert
            Assert.AreEqual(5.0, sumVolume.Value, Tolerance, "5 L + 0 mL should equal 5 L");
        }

        [TestMethod]
        public void Add_Volume_WithNegativeValues_ReturnsCorrectSum()
        {
            // Arrange
            var positiveVolume = new GenericQuantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var negativeVolume = new GenericQuantity<VolumeUnit>(-2000.0, VolumeUnit.MILLILITRE);

            // Act
            var sumVolume = positiveVolume.Add(negativeVolume);

            // Assert
            Assert.AreEqual(3.0, sumVolume.Value, Tolerance, "5 L + (-2000 mL) should equal 3 L");
        }

        [TestMethod]
        public void Add_Volume_IsCommutative_ReturnsTrue()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var targetUnit = VolumeUnit.GALLON;

            // Act
            var firstSum = litreVolume.Add(mlVolume, targetUnit);
            var secondSum = mlVolume.Add(litreVolume, targetUnit);

            // Assert
            Assert.AreEqual(
                firstSum.Value,
                secondSum.Value,
                RoundedTolerance,
                "a + b should equal b + a when using same target unit"
            );
        }

        #endregion

        #region Zero and Edge Cases

        [TestMethod]
        public void Equals_Volume_ZeroValue_AllUnitsEqual()
        {
            // Arrange
            var zeroLitre = new GenericQuantity<VolumeUnit>(0.0, VolumeUnit.LITRE);
            var zeroMl = new GenericQuantity<VolumeUnit>(0.0, VolumeUnit.MILLILITRE);
            var zeroGal = new GenericQuantity<VolumeUnit>(0.0, VolumeUnit.GALLON);

            // Act & Assert
            Assert.IsTrue(zeroLitre.Equals(zeroMl), "0 L should equal 0 mL");
            Assert.IsTrue(zeroLitre.Equals(zeroGal), "0 L should equal 0 gal");
            Assert.IsTrue(zeroMl.Equals(zeroGal), "0 mL should equal 0 gal");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void Constructor_Volume_NaNValue_ThrowsException()
        {
            // Act - Should throw
            var invalidVolume = new GenericQuantity<VolumeUnit>(double.NaN, VolumeUnit.LITRE);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void Constructor_Volume_InfinityValue_ThrowsException()
        {
            // Act - Should throw
            var invalidVolume = new GenericQuantity<VolumeUnit>(
                double.PositiveInfinity,
                VolumeUnit.LITRE
            );
        }

        #endregion

        #region GetHashCode Tests

        [TestMethod]
        public void GetHashCode_Volume_EqualObjects_ReturnsSameHashCode()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            int litreHash = litreVolume.GetHashCode();
            int mlHash = mlVolume.GetHashCode();

            // Assert
            Assert.AreEqual(litreHash, mlHash, "Equal volumes should have equal hash codes");
        }

        [TestMethod]
        public void GetHashCode_Volume_DifferentObjects_ReturnsDifferentHashCode()
        {
            // Arrange
            var litre1 = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var litre2 = new GenericQuantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            // Act
            int hash1 = litre1.GetHashCode();
            int hash2 = litre2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hash1, hash2, "Different volumes should have different hash codes");
        }

        #endregion

        #region ToString Tests

        [TestMethod]
        public void ToString_Volume_ReturnsCorrectFormat()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.5, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);
            var galVolume = new GenericQuantity<VolumeUnit>(2.2, VolumeUnit.GALLON);

            // Assert
            Assert.AreEqual("1.5 L", litreVolume.ToString());
            Assert.AreEqual("500 mL", mlVolume.ToString());
            Assert.AreEqual("2.2 gal", galVolume.ToString());
        }

        #endregion
    }
}
