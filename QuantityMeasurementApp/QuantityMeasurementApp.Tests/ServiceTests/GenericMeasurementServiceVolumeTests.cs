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
    /// </summary>
    [TestClass]
    public class GenericMeasurementServiceVolumeTests
    {
        private GenericMeasurementService _measurementService = null!;
        private const double Tolerance = 0.000001;
        private const double GallonTolerance = 0.001;

        [TestInitialize]
        public void Setup()
        {
            _measurementService = new GenericMeasurementService();
        }

        /// <summary>
        /// Tests AreQuantitiesEqual for volume with equal quantities.
        /// </summary>
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

        /// <summary>
        /// Tests AreQuantitiesEqual for volume with different quantities.
        /// </summary>
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

        /// <summary>
        /// Tests ConvertValue for volume.
        /// </summary>
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

        /// <summary>
        /// Tests ConvertValue for volume with litres to gallons.
        /// </summary>
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
                GallonTolerance,
                "3.78541 L should convert to approximately 1 gal"
            );
        }

        /// <summary>
        /// Tests AddQuantities for volume with default unit.
        /// </summary>
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

        /// <summary>
        /// Tests AddQuantitiesWithTarget for volume with millilitres target.
        /// </summary>
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

        /// <summary>
        /// Tests AddQuantitiesWithTarget for volume with gallons target.
        /// </summary>
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
            double expectedValue = 1.264172; // 1 gal + 0.264172 gal = 1.264172 gal
            Assert.AreEqual(
                expectedValue,
                sumVolume.Value,
                GallonTolerance,
                "1 gal + 1000 mL in gallons should be correct"
            );
            Assert.AreEqual(VolumeUnit.GALLON, sumVolume.Unit, "Result should be in gallons");
        }

        /// <summary>
        /// Tests CreateQuantityFromString for volume with valid input.
        /// </summary>
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

        /// <summary>
        /// Tests CreateQuantityFromString for volume with invalid input.
        /// </summary>
        [TestMethod]
        public void CreateQuantityFromString_Volume_InvalidInput_ReturnsNull()
        {
            // Arrange
            string invalidInput = "abc";
            VolumeUnit unitOfMeasure = VolumeUnit.LITRE;

            // Act
            var createdQuantity = _measurementService.CreateQuantityFromString(
                invalidInput,
                unitOfMeasure
            );

            // Assert
            Assert.IsNull(createdQuantity, "Invalid input should return null");
        }

        /// <summary>
        /// Tests that volume cannot be compared with length.
        /// </summary>
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

        /// <summary>
        /// Tests that volume cannot be compared with weight.
        /// </summary>
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
