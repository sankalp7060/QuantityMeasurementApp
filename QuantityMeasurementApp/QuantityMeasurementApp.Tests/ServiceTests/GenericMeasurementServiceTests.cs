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
    /// Updated to expect rounded values (2 decimal places).
    /// </summary>
    [TestClass]
    public class GenericMeasurementServiceTests
    {
        private GenericMeasurementService _measurementService = null!;
        private const double Tolerance = 0.000001;
        private const double RoundedTolerance = 0.01; // For rounded values (2 decimal places)

        [TestInitialize]
        public void Setup()
        {
            _measurementService = new GenericMeasurementService();
        }

        #region Length Service Tests

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
            double expectedValue = 0.67; // 2/3 yards rounded to 2 decimal places
            Assert.AreEqual(
                expectedValue,
                sumLength.Value,
                RoundedTolerance,
                "1 ft + 12 in in yards should equal 0.67 yd (rounded)"
            );
            Assert.AreEqual(LengthUnit.YARD, sumLength.Unit, "Result should be in yards");
        }

        #endregion

        #region Weight Service Tests

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
                0.001,
                "1 kg should convert to approximately 2.20462 lb"
            );
        }

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
            double expectedValue = 3.31; // 1.5 kg * 2.20462 = 3.30693 rounded to 2 decimal places = 3.31
            Assert.AreEqual(
                expectedValue,
                sumWeight.Value,
                RoundedTolerance,
                "1 kg + 500 g in pounds should be 3.31 lb (rounded)"
            );
            Assert.AreEqual(WeightUnit.POUND, sumWeight.Unit, "Result should be in pounds");
        }

        #endregion

        #region CreateFromString Tests

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

        #endregion

        #region Cross-Category Tests

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
    }
}
