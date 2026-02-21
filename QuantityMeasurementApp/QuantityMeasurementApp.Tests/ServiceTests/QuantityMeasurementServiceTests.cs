using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Tests.ServiceTests
{
    /// <summary>
    /// Test class for QuantityMeasurementService.
    /// </summary>
    [TestClass]
    public class QuantityMeasurementServiceTests
    {
        private QuantityMeasurementService _measurementService = null!;

        [TestInitialize]
        public void Setup()
        {
            _measurementService = new QuantityMeasurementService();
        }

        /// <summary>
        /// Tests AreQuantitiesEqual method with equal quantities.
        /// </summary>
        [TestMethod]
        public void AreQuantitiesEqual_EqualQuantities_ReturnsTrue()
        {
            // Arrange
            var firstQuantity = new Quantity(1.0, LengthUnit.FEET);
            var secondQuantity = new Quantity(12.0, LengthUnit.INCH);

            // Act
            bool areEqual = _measurementService.AreQuantitiesEqual(firstQuantity, secondQuantity);

            // Assert
            Assert.IsTrue(areEqual, "1 ft and 12 in should be equal");
        }

        /// <summary>
        /// Tests AreQuantitiesEqual method with different quantities.
        /// </summary>
        [TestMethod]
        public void AreQuantitiesEqual_DifferentQuantities_ReturnsFalse()
        {
            // Arrange
            var firstQuantity = new Quantity(1.0, LengthUnit.FEET);
            var secondQuantity = new Quantity(2.0, LengthUnit.FEET);

            // Act
            bool areEqual = _measurementService.AreQuantitiesEqual(firstQuantity, secondQuantity);

            // Assert
            Assert.IsFalse(areEqual, "1 ft and 2 ft should not be equal");
        }

        /// <summary>
        /// Tests ConvertValue method with feet to inches.
        /// </summary>
        [TestMethod]
        public void ConvertValue_FeetToInches_ReturnsCorrectValue()
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
            Assert.AreEqual(12.0, inchesValue, 0.000001, "1 ft should convert to 12 in");
        }

        /// <summary>
        /// Tests ConvertValue method with yards to centimeters.
        /// </summary>
        [TestMethod]
        public void ConvertValue_YardsToCentimeters_ReturnsCorrectValue()
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
            Assert.AreEqual(91.44, cmValue, 0.000001, "1 yd should convert to 91.44 cm");
        }

        /// <summary>
        /// Tests AddQuantities method with default unit.
        /// </summary>
        [TestMethod]
        public void AddQuantities_DefaultUnit_ReturnsCorrectSum()
        {
            // Arrange
            var feetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var inchesQuantity = new Quantity(12.0, LengthUnit.INCH);

            // Act
            var sumQuantity = _measurementService.AddQuantities(feetQuantity, inchesQuantity);

            // Assert
            Assert.AreEqual(2.0, sumQuantity.Value, 0.000001, "1 ft + 12 in should equal 2 ft");
            Assert.AreEqual(LengthUnit.FEET, sumQuantity.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests AddQuantitiesWithTarget method with yards target.
        /// </summary>
        [TestMethod]
        public void AddQuantitiesWithTarget_YardsTarget_ReturnsCorrectSum()
        {
            // Arrange
            var feetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var inchesQuantity = new Quantity(12.0, LengthUnit.INCH);
            var targetUnit = LengthUnit.YARD;

            // Act
            var sumQuantity = _measurementService.AddQuantitiesWithTarget(
                feetQuantity,
                inchesQuantity,
                targetUnit
            );

            // Assert
            double expectedValue = 2.0 / 3.0;
            Assert.AreEqual(
                expectedValue,
                sumQuantity.Value,
                0.000001,
                "1 ft + 12 in in yards should equal 2/3 yd"
            );
            Assert.AreEqual(LengthUnit.YARD, sumQuantity.Unit, "Result should be in yards");
        }

        /// <summary>
        /// Tests AddQuantitiesWithTarget method with centimeters target.
        /// </summary>
        [TestMethod]
        public void AddQuantitiesWithTarget_CentimetersTarget_ReturnsCorrectSum()
        {
            // Arrange
            var firstInchQuantity = new Quantity(1.0, LengthUnit.INCH);
            var secondInchQuantity = new Quantity(1.0, LengthUnit.INCH);
            var targetUnit = LengthUnit.CENTIMETER;

            // Act
            var sumQuantity = _measurementService.AddQuantitiesWithTarget(
                firstInchQuantity,
                secondInchQuantity,
                targetUnit
            );

            // Assert
            Assert.AreEqual(
                5.08,
                sumQuantity.Value,
                0.000001,
                "1 in + 1 in in cm should equal 5.08 cm"
            );
            Assert.AreEqual(
                LengthUnit.CENTIMETER,
                sumQuantity.Unit,
                "Result should be in centimeters"
            );
        }

        /// <summary>
        /// Tests CreateQuantityFromString with valid input.
        /// </summary>
        [TestMethod]
        public void CreateQuantityFromString_ValidInput_ReturnsQuantity()
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
            Assert.AreEqual(3.5, createdQuantity!.Value, 0.000001, "Value should match input");
            Assert.AreEqual(LengthUnit.FEET, createdQuantity.Unit, "Unit should be feet");
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

        /// <summary>
        /// Tests legacy method CreateFeetFromString.
        /// </summary>
        [TestMethod]
        public void CreateFeetFromString_ValidInput_ReturnsFeet()
        {
            // Arrange
            string inputValue = "3.5";

            // Act
            var feetObject = _measurementService.CreateFeetFromString(inputValue);

            // Assert
            Assert.IsNotNull(feetObject, "Should return non-null Feet object");
            Assert.AreEqual(3.5, feetObject!.Value, 0.000001, "Value should match input");
        }

        /// <summary>
        /// Tests legacy method CreateInchFromString.
        /// </summary>
        [TestMethod]
        public void CreateInchFromString_ValidInput_ReturnsInch()
        {
            // Arrange
            string inputValue = "3.5";

            // Act
            var inchObject = _measurementService.CreateInchFromString(inputValue);

            // Assert
            Assert.IsNotNull(inchObject, "Should return non-null Inch object");
            Assert.AreEqual(3.5, inchObject!.Value, 0.000001, "Value should match input");
        }

        /// <summary>
        /// Tests legacy method AreFeetEqual.
        /// </summary>
        [TestMethod]
        public void AreFeetEqual_EqualValues_ReturnsTrue()
        {
            // Arrange
            var firstFeet = _measurementService.CreateFeetFromString("1.0");
            var secondFeet = _measurementService.CreateFeetFromString("1.0");

            // Act
            bool areEqual = _measurementService.AreFeetEqual(firstFeet, secondFeet);

            // Assert
            Assert.IsTrue(areEqual, "1.0 ft should equal 1.0 ft");
        }

        /// <summary>
        /// Tests legacy method AreInchesEqual.
        /// </summary>
        [TestMethod]
        public void AreInchesEqual_EqualValues_ReturnsTrue()
        {
            // Arrange
            var firstInch = _measurementService.CreateInchFromString("1.0");
            var secondInch = _measurementService.CreateInchFromString("1.0");

            // Act
            bool areEqual = _measurementService.AreInchesEqual(firstInch, secondInch);

            // Assert
            Assert.IsTrue(areEqual, "1.0 in should equal 1.0 in");
        }
    }
}
