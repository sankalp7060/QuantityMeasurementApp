using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Tests.ServiceTests
{
    /// <summary>
    /// Test class for WeightMeasurementService.
    /// </summary>
    [TestClass]
    public class WeightMeasurementServiceTests
    {
        private WeightMeasurementService _weightService = null!;
        private const double Tolerance = 0.000001;

        [TestInitialize]
        public void Setup()
        {
            _weightService = new WeightMeasurementService();
        }

        /// <summary>
        /// Tests AreWeightsEqual method with equal weights.
        /// </summary>
        [TestMethod]
        public void AreWeightsEqual_EqualWeights_ReturnsTrue()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(1000.0, WeightUnit.GRAM);

            // Act
            bool areEqual = _weightService.AreWeightsEqual(kgWeight, gWeight);

            // Assert
            Assert.IsTrue(areEqual, "1 kg and 1000 g should be equal");
        }

        /// <summary>
        /// Tests AreWeightsEqual method with different weights.
        /// </summary>
        [TestMethod]
        public void AreWeightsEqual_DifferentWeights_ReturnsFalse()
        {
            // Arrange
            var firstWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var secondWeight = new WeightQuantity(2.0, WeightUnit.KILOGRAM);

            // Act
            bool areEqual = _weightService.AreWeightsEqual(firstWeight, secondWeight);

            // Assert
            Assert.IsFalse(areEqual, "1 kg and 2 kg should not be equal");
        }

        /// <summary>
        /// Tests ConvertWeightValue method with kg to g.
        /// </summary>
        [TestMethod]
        public void ConvertWeightValue_KgToG_ReturnsCorrectValue()
        {
            // Arrange
            double kgValue = 1.0;

            // Act
            double gValue = _weightService.ConvertWeightValue(
                kgValue,
                WeightUnit.KILOGRAM,
                WeightUnit.GRAM
            );

            // Assert
            Assert.AreEqual(1000.0, gValue, Tolerance, "1 kg should convert to 1000 g");
        }

        /// <summary>
        /// Tests ConvertWeightValue method with kg to lb.
        /// </summary>
        [TestMethod]
        public void ConvertWeightValue_KgToLb_ReturnsCorrectValue()
        {
            // Arrange
            double kgValue = 1.0;

            // Act
            double lbValue = _weightService.ConvertWeightValue(
                kgValue,
                WeightUnit.KILOGRAM,
                WeightUnit.POUND
            );

            // Assert
            Assert.AreEqual(
                2.20462,
                lbValue,
                0.001,
                "1 kg should convert to approximately 2.20462 lb"
            );
        }

        /// <summary>
        /// Tests AddWeights method with default unit.
        /// </summary>
        [TestMethod]
        public void AddWeights_DefaultUnit_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(500.0, WeightUnit.GRAM);

            // Act
            var sumWeight = _weightService.AddWeights(kgWeight, gWeight);

            // Assert
            Assert.AreEqual(1.5, sumWeight.Value, Tolerance, "1 kg + 500 g should equal 1.5 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, sumWeight.Unit, "Result should be in kilograms");
        }

        /// <summary>
        /// Tests AddWeightsWithTarget method with grams target.
        /// </summary>
        [TestMethod]
        public void AddWeightsWithTarget_GramsTarget_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(500.0, WeightUnit.GRAM);
            var targetUnit = WeightUnit.GRAM;

            // Act
            var sumWeight = _weightService.AddWeightsWithTarget(kgWeight, gWeight, targetUnit);

            // Assert
            Assert.AreEqual(
                1500.0,
                sumWeight.Value,
                Tolerance,
                "1 kg + 500 g in grams should equal 1500 g"
            );
            Assert.AreEqual(WeightUnit.GRAM, sumWeight.Unit, "Result should be in grams");
        }

        /// <summary>
        /// Tests AddWeightsWithTarget method with pounds target.
        /// </summary>
        [TestMethod]
        public void AddWeightsWithTarget_PoundsTarget_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(500.0, WeightUnit.GRAM);
            var targetUnit = WeightUnit.POUND;

            // Act
            var sumWeight = _weightService.AddWeightsWithTarget(kgWeight, gWeight, targetUnit);

            // Assert
            double expectedValue = 1.5 * 2.20462; // 1.5 kg in pounds
            Assert.AreEqual(
                expectedValue,
                sumWeight.Value,
                0.001,
                "Sum in pounds should be correct"
            );
            Assert.AreEqual(WeightUnit.POUND, sumWeight.Unit, "Result should be in pounds");
        }

        /// <summary>
        /// Tests CreateWeightFromString with valid input.
        /// </summary>
        [TestMethod]
        public void CreateWeightFromString_ValidInput_ReturnsWeight()
        {
            // Arrange
            string inputValue = "3.5";
            WeightUnit unitOfMeasure = WeightUnit.KILOGRAM;

            // Act
            var createdWeight = _weightService.CreateWeightFromString(inputValue, unitOfMeasure);

            // Assert
            Assert.IsNotNull(createdWeight, "Should return non-null Weight");
            Assert.AreEqual(3.5, createdWeight!.Value, Tolerance, "Value should match input");
            Assert.AreEqual(WeightUnit.KILOGRAM, createdWeight.Unit, "Unit should be kilograms");
        }

        /// <summary>
        /// Tests CreateWeightFromString with invalid input.
        /// </summary>
        [TestMethod]
        public void CreateWeightFromString_InvalidInput_ReturnsNull()
        {
            // Arrange
            string invalidInput = "abc";
            WeightUnit unitOfMeasure = WeightUnit.KILOGRAM;

            // Act
            var createdWeight = _weightService.CreateWeightFromString(invalidInput, unitOfMeasure);

            // Assert
            Assert.IsNull(createdWeight, "Invalid input should return null");
        }

        /// <summary>
        /// Tests AreWeightAndLengthEqual always returns false.
        /// FIXED: Changed FOOT to FEET
        /// </summary>
        [TestMethod]
        public void AreWeightAndLengthEqual_Always_ReturnsFalse()
        {
            // Arrange
            var weight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var length = new Quantity(1.0, LengthUnit.FEET);

            // Act
            bool areEqual = _weightService.AreWeightAndLengthEqual(weight, length);

            // Assert
            Assert.IsFalse(areEqual, "Weight and length should never be equal");
        }
    }
}
