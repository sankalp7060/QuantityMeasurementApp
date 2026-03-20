using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Tests.ServiceTests
{
    /// <summary>
    /// Test class for GenericMeasurementService with Volume measurements.
    /// UC11: Tests that the generic service works with VolumeUnit without any modifications.
    /// Updated to expect rounded values (2 decimal places).
    /// </summary>
    [TestClass]
    public class GenericMeasurementServiceVolumeTests
    {
        private GenericMeasurementService _measurementService = null!;
        private const double Tolerance = 0.000001;
        private const double RoundedTolerance = 0.01; // For rounded values (2 decimal places)

        [TestInitialize]
        public void Setup()
        {
            _measurementService = new GenericMeasurementService();
        }

        [TestMethod]
        public void AreQuantitiesEqual_Volume_EqualQuantities_ReturnsTrue()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            bool areEqual = _measurementService.AreQuantitiesEqual(litreVolume, mlVolume);

            // Assert
            Assert.IsTrue(areEqual, "1 L and 1000 mL should be equal");
        }

        [TestMethod]
        public void AreQuantitiesEqual_Volume_DifferentQuantities_ReturnsFalse()
        {
            // Arrange
            var firstVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var secondVolume = new GenericQuantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            // Act
            bool areEqual = _measurementService.AreQuantitiesEqual(firstVolume, secondVolume);

            // Assert
            Assert.IsFalse(areEqual, "1 L and 2 L should not be equal");
        }

        [TestMethod]
        public void ConvertValue_Volume_LitresToMillilitres_ReturnsCorrectValue()
        {
            // Arrange
            double litreValue = 1.0;

            // Act
            double mlValue = _measurementService.ConvertValue(
                litreValue,
                VolumeUnit.LITRE,
                VolumeUnit.MILLILITRE
            );

            // Assert
            Assert.AreEqual(1000.0, mlValue, Tolerance, "1 L should convert to 1000 mL");
        }

        [TestMethod]
        public void ConvertValue_Volume_LitresToGallons_ReturnsCorrectValue()
        {
            // Arrange
            double litreValue = 3.78541;

            // Act
            double galValue = _measurementService.ConvertValue(
                litreValue,
                VolumeUnit.LITRE,
                VolumeUnit.GALLON
            );

            // Assert
            Assert.AreEqual(
                1.0,
                galValue,
                0.001,
                "3.78541 L should convert to approximately 1 gal"
            );
        }

        [TestMethod]
        public void AddQuantities_Volume_DefaultUnit_ReturnsCorrectSum()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            // Act
            var sumVolume = _measurementService.AddQuantities(litreVolume, mlVolume);

            // Assert
            Assert.AreEqual(1.5, sumVolume.Value, Tolerance, "1 L + 500 mL should equal 1.5 L");
            Assert.AreEqual(VolumeUnit.LITRE, sumVolume.Unit, "Result should be in litres");
        }

        [TestMethod]
        public void AddQuantitiesWithTarget_Volume_MillilitresTarget_ReturnsCorrectSum()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);
            var targetUnit = VolumeUnit.MILLILITRE;

            // Act
            var sumVolume = _measurementService.AddQuantitiesWithTarget(
                litreVolume,
                mlVolume,
                targetUnit
            );

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
        public void AddQuantitiesWithTarget_Volume_GallonsTarget_ReturnsCorrectSum()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var targetUnit = VolumeUnit.GALLON;

            // Act
            var sumVolume = _measurementService.AddQuantitiesWithTarget(
                litreVolume,
                mlVolume,
                targetUnit
            );

            // Assert
            double expectedValue = 1.26; // 1 gal + 0.264 gal = 1.264 gal rounded to 2 decimal places = 1.26
            Assert.AreEqual(
                expectedValue,
                sumVolume.Value,
                RoundedTolerance,
                "1 gal + 1000 mL in gallons should be 1.26 gal (rounded)"
            );
            Assert.AreEqual(VolumeUnit.GALLON, sumVolume.Unit, "Result should be in gallons");
        }

        [TestMethod]
        public void CreateQuantityFromString_Volume_ValidInput_ReturnsQuantity()
        {
            // Arrange
            string inputValue = "3.5";
            VolumeUnit unitOfMeasure = VolumeUnit.LITRE;

            // Act
            var createdQuantity = _measurementService.CreateQuantityFromString(
                inputValue,
                unitOfMeasure
            );

            // Assert
            Assert.IsNotNull(createdQuantity, "Should return non-null Quantity");
            Assert.AreEqual(3.5, createdQuantity!.Value, Tolerance, "Value should match input");
            Assert.AreEqual(VolumeUnit.LITRE, createdQuantity.Unit, "Unit should be litres");
        }

        [TestMethod]
        public void AreQuantitiesFromDifferentCategoriesEqual_VolumeVsLength_ReturnsFalse()
        {
            // Arrange
            var volume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var length = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act
            bool areEqual = _measurementService.AreQuantitiesFromDifferentCategoriesEqual(
                volume,
                length
            );

            // Assert
            Assert.IsFalse(areEqual, "Volume and length should never be equal");
        }

        [TestMethod]
        public void AreQuantitiesFromDifferentCategoriesEqual_VolumeVsWeight_ReturnsFalse()
        {
            // Arrange
            var volume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var weight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            bool areEqual = _measurementService.AreQuantitiesFromDifferentCategoriesEqual(
                volume,
                weight
            );

            // Assert
            Assert.IsFalse(areEqual, "Volume and weight should never be equal");
        }
    }
}
