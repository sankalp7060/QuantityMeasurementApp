using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Tests.ServiceTests
{
    /// <summary>
    /// Test class for GenericMeasurementService.
    /// UC10: Tests the generic service layer for all measurement categories.
    /// </summary>
    [TestClass]
    public class GenericMeasurementServiceTests
    {
        private GenericMeasurementService _measurementService = null!;
        private const double Tolerance = 0.000001;
        private const double PoundTolerance = 0.001;

        [TestInitialize]
        public void Setup()
        {
            _measurementService = new GenericMeasurementService();
        }

        #region Length Service Tests

        /// <summary>
        /// Tests AreQuantitiesEqual for length with equal quantities.
        /// </summary>
        [TestMethod]
        public void AreQuantitiesEqual_Length_EqualQuantities_ReturnsTrue()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            bool areEqual = _measurementService.AreQuantitiesEqual(feetLength, inchesLength);

            // Assert
            Assert.IsTrue(areEqual, "1 ft and 12 in should be equal");
        }

        /// <summary>
        /// Tests AreQuantitiesEqual for length with different quantities.
        /// </summary>
        [TestMethod]
        public void AreQuantitiesEqual_Length_DifferentQuantities_ReturnsFalse()
        {
            // Arrange
            var firstLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var secondLength = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);

            // Act
            bool areEqual = _measurementService.AreQuantitiesEqual(firstLength, secondLength);

            // Assert
            Assert.IsFalse(areEqual, "1 ft and 2 ft should not be equal");
        }

        /// <summary>
        /// Tests ConvertValue for length.
        /// </summary>
        [TestMethod]
        public void ConvertValue_Length_FeetToInches_ReturnsCorrectValue()
        {
            // Arrange
            double feetValue = 1.0;

            // Act
            double inchesValue = _measurementService.ConvertValue(
                feetValue,
                LengthUnit.FEET,
                LengthUnit.INCH
            );

            // Assert
            Assert.AreEqual(12.0, inchesValue, Tolerance, "1 ft should convert to 12 in");
        }

        /// <summary>
        /// Tests ConvertValue for length with yards to centimeters.
        /// </summary>
        [TestMethod]
        public void ConvertValue_Length_YardsToCentimeters_ReturnsCorrectValue()
        {
            // Arrange
            double yardsValue = 1.0;

            // Act
            double cmValue = _measurementService.ConvertValue(
                yardsValue,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER
            );

            // Assert
            Assert.AreEqual(91.44, cmValue, Tolerance, "1 yd should convert to 91.44 cm");
        }

        /// <summary>
        /// Tests AddQuantities for length with default unit.
        /// </summary>
        [TestMethod]
        public void AddQuantities_Length_DefaultUnit_ReturnsCorrectSum()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            var sumLength = _measurementService.AddQuantities(feetLength, inchesLength);

            // Assert
            Assert.AreEqual(2.0, sumLength.Value, Tolerance, "1 ft + 12 in should equal 2 ft");
            Assert.AreEqual(LengthUnit.FEET, sumLength.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests AddQuantitiesWithTarget for length with yards target.
        /// </summary>
        [TestMethod]
        public void AddQuantitiesWithTarget_Length_YardsTarget_ReturnsCorrectSum()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);
            var targetUnit = LengthUnit.YARD;

            // Act
            var sumLength = _measurementService.AddQuantitiesWithTarget(
                feetLength,
                inchesLength,
                targetUnit
            );

            // Assert
            double expectedValue = 2.0 / 3.0;
            Assert.AreEqual(
                expectedValue,
                sumLength.Value,
                Tolerance,
                "1 ft + 12 in in yards should equal 2/3 yd"
            );
            Assert.AreEqual(LengthUnit.YARD, sumLength.Unit, "Result should be in yards");
        }

        #endregion

        #region Weight Service Tests

        /// <summary>
        /// Tests AreQuantitiesEqual for weight with equal quantities.
        /// </summary>
        [TestMethod]
        public void AreQuantitiesEqual_Weight_EqualQuantities_ReturnsTrue()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            // Act
            bool areEqual = _measurementService.AreQuantitiesEqual(kgWeight, gWeight);

            // Assert
            Assert.IsTrue(areEqual, "1 kg and 1000 g should be equal");
        }

        /// <summary>
        /// Tests AreQuantitiesEqual for weight with different quantities.
        /// </summary>
        [TestMethod]
        public void AreQuantitiesEqual_Weight_DifferentQuantities_ReturnsFalse()
        {
            // Arrange
            var firstWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var secondWeight = new GenericQuantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);

            // Act
            bool areEqual = _measurementService.AreQuantitiesEqual(firstWeight, secondWeight);

            // Assert
            Assert.IsFalse(areEqual, "1 kg and 2 kg should not be equal");
        }

        /// <summary>
        /// Tests ConvertValue for weight.
        /// </summary>
        [TestMethod]
        public void ConvertValue_Weight_KgToG_ReturnsCorrectValue()
        {
            // Arrange
            double kgValue = 1.0;

            // Act
            double gValue = _measurementService.ConvertValue(
                kgValue,
                WeightUnit.KILOGRAM,
                WeightUnit.GRAM
            );

            // Assert
            Assert.AreEqual(1000.0, gValue, Tolerance, "1 kg should convert to 1000 g");
        }

        /// <summary>
        /// Tests ConvertValue for weight with kg to pounds.
        /// </summary>
        [TestMethod]
        public void ConvertValue_Weight_KgToLb_ReturnsCorrectValue()
        {
            // Arrange
            double kgValue = 1.0;

            // Act
            double lbValue = _measurementService.ConvertValue(
                kgValue,
                WeightUnit.KILOGRAM,
                WeightUnit.POUND
            );

            // Assert
            Assert.AreEqual(
                2.20462262185,
                lbValue,
                PoundTolerance,
                "1 kg should convert to approximately 2.20462 lb"
            );
        }

        /// <summary>
        /// Tests AddQuantities for weight with default unit.
        /// </summary>
        [TestMethod]
        public void AddQuantities_Weight_DefaultUnit_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(500.0, WeightUnit.GRAM);

            // Act
            var sumWeight = _measurementService.AddQuantities(kgWeight, gWeight);

            // Assert
            Assert.AreEqual(1.5, sumWeight.Value, Tolerance, "1 kg + 500 g should equal 1.5 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, sumWeight.Unit, "Result should be in kilograms");
        }

        /// <summary>
        /// Tests AddQuantitiesWithTarget for weight with grams target.
        /// </summary>
        [TestMethod]
        public void AddQuantitiesWithTarget_Weight_GramsTarget_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(500.0, WeightUnit.GRAM);
            var targetUnit = WeightUnit.GRAM;

            // Act
            var sumWeight = _measurementService.AddQuantitiesWithTarget(
                kgWeight,
                gWeight,
                targetUnit
            );

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
        /// Tests AddQuantitiesWithTarget for weight with pounds target.
        /// </summary>
        [TestMethod]
        public void AddQuantitiesWithTarget_Weight_PoundsTarget_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(500.0, WeightUnit.GRAM);
            var targetUnit = WeightUnit.POUND;

            // Act
            var sumWeight = _measurementService.AddQuantitiesWithTarget(
                kgWeight,
                gWeight,
                targetUnit
            );

            // Assert
            double expectedValue = 1.5 * 2.20462262185; // 1.5 kg in pounds
            Assert.AreEqual(
                expectedValue,
                sumWeight.Value,
                PoundTolerance,
                "1 kg + 500 g in pounds should be correct"
            );
            Assert.AreEqual(WeightUnit.POUND, sumWeight.Unit, "Result should be in pounds");
        }

        #endregion

        #region CreateFromString Tests

        /// <summary>
        /// Tests CreateQuantityFromString for length with valid input.
        /// </summary>
        [TestMethod]
        public void CreateQuantityFromString_Length_ValidInput_ReturnsQuantity()
        {
            // Arrange
            string inputValue = "3.5";
            LengthUnit unitOfMeasure = LengthUnit.FEET;

            // Act
            var createdQuantity = _measurementService.CreateQuantityFromString(
                inputValue,
                unitOfMeasure
            );

            // Assert
            Assert.IsNotNull(createdQuantity, "Should return non-null Quantity");
            Assert.AreEqual(3.5, createdQuantity!.Value, Tolerance, "Value should match input");
            Assert.AreEqual(LengthUnit.FEET, createdQuantity.Unit, "Unit should be feet");
        }

        /// <summary>
        /// Tests CreateQuantityFromString for weight with valid input.
        /// </summary>
        [TestMethod]
        public void CreateQuantityFromString_Weight_ValidInput_ReturnsQuantity()
        {
            // Arrange
            string inputValue = "3.5";
            WeightUnit unitOfMeasure = WeightUnit.KILOGRAM;

            // Act
            var createdQuantity = _measurementService.CreateQuantityFromString(
                inputValue,
                unitOfMeasure
            );

            // Assert
            Assert.IsNotNull(createdQuantity, "Should return non-null Quantity");
            Assert.AreEqual(3.5, createdQuantity!.Value, Tolerance, "Value should match input");
            Assert.AreEqual(WeightUnit.KILOGRAM, createdQuantity.Unit, "Unit should be kilograms");
        }

        /// <summary>
        /// Tests CreateQuantityFromString with invalid input.
        /// </summary>
        [TestMethod]
        public void CreateQuantityFromString_InvalidInput_ReturnsNull()
        {
            // Arrange
            string invalidInput = "abc";
            LengthUnit unitOfMeasure = LengthUnit.FEET;

            // Act
            var createdQuantity = _measurementService.CreateQuantityFromString(
                invalidInput,
                unitOfMeasure
            );

            // Assert
            Assert.IsNull(createdQuantity, "Invalid input should return null");
        }

        /// <summary>
        /// Tests CreateQuantityFromString with empty input.
        /// </summary>
        [TestMethod]
        public void CreateQuantityFromString_EmptyInput_ReturnsNull()
        {
            // Arrange
            string emptyInput = "";
            LengthUnit unitOfMeasure = LengthUnit.FEET;

            // Act
            var createdQuantity = _measurementService.CreateQuantityFromString(
                emptyInput,
                unitOfMeasure
            );

            // Assert
            Assert.IsNull(createdQuantity, "Empty input should return null");
        }

        #endregion

        #region Cross-Category Tests

        /// <summary>
        /// Tests AreQuantitiesFromDifferentCategoriesEqual always returns false.
        /// </summary>
        [TestMethod]
        public void AreQuantitiesFromDifferentCategoriesEqual_Always_ReturnsFalse()
        {
            // Arrange
            var length = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var weight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            bool areEqual = _measurementService.AreQuantitiesFromDifferentCategoriesEqual(
                length,
                weight
            );

            // Assert
            Assert.IsFalse(areEqual, "Length and weight should never be equal");
        }

        #endregion

        #region Null Argument Tests

        /// <summary>
        /// Tests AreQuantitiesEqual with null first argument returns false.
        /// </summary>
        [TestMethod]
        public void AreQuantitiesEqual_NullFirst_ReturnsFalse()
        {
            // Arrange
            var validLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act
            bool areEqual = _measurementService.AreQuantitiesEqual<LengthUnit>(null!, validLength);

            // Assert
            Assert.IsFalse(areEqual, "Null first argument should return false");
        }

        /// <summary>
        /// Tests AreQuantitiesEqual with null second argument returns false.
        /// </summary>
        [TestMethod]
        public void AreQuantitiesEqual_NullSecond_ReturnsFalse()
        {
            // Arrange
            var validLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act
            bool areEqual = _measurementService.AreQuantitiesEqual(validLength, null!);

            // Assert
            Assert.IsFalse(areEqual, "Null second argument should return false");
        }

        /// <summary>
        /// Tests AddQuantities with null arguments throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddQuantities_NullFirst_ThrowsException()
        {
            // Arrange
            var validLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act - Should throw
            _measurementService.AddQuantities<LengthUnit>(null!, validLength);
        }

        #endregion
    }
}
