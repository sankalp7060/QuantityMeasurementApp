using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests covering end-to-end scenarios.
    /// </summary>
    [TestClass]
    public class EndToEndTests
    {
        private QuantityMeasurementService _measurementService = null!;
        private const double Tolerance = 0.000001;

        [TestInitialize]
        public void Setup()
        {
            _measurementService = new QuantityMeasurementService();
        }

        /// <summary>
        /// Tests a complete workflow: create quantities, convert, add, compare.
        /// </summary>
        [TestMethod]
        public void CompleteWorkflow_AllOperations_WorkCorrectly()
        {
            // Step 1: Create quantities from string inputs
            var firstQuantity = _measurementService.CreateQuantityFromString("2", LengthUnit.YARD);
            var secondQuantity = _measurementService.CreateQuantityFromString(
                "36",
                LengthUnit.INCH
            );

            Assert.IsNotNull(firstQuantity, "First quantity should not be null");
            Assert.IsNotNull(secondQuantity, "Second quantity should not be null");

            // Step 2: Convert first quantity to feet
            var firstQuantityInFeet = firstQuantity!.ConvertTo(LengthUnit.FEET);
            Assert.AreEqual(
                6.0,
                firstQuantityInFeet.Value,
                Tolerance,
                "2 yards should equal 6 feet"
            );

            // Step 3: Add both quantities and get result in yards
            var sumInYards = _measurementService.AddQuantitiesWithTarget(
                firstQuantity,
                secondQuantity!,
                LengthUnit.YARD
            );

            // 2 yd + 36 in = 2 yd + 1 yd = 3 yd
            Assert.AreEqual(3.0, sumInYards.Value, Tolerance, "2 yd + 36 in should equal 3 yd");

            // Step 4: Compare with expected quantity
            var expectedQuantity = new Quantity(3.0, LengthUnit.YARD);
            bool areQuantitiesEqual = _measurementService.AreQuantitiesEqual(
                sumInYards,
                expectedQuantity
            );

            Assert.IsTrue(areQuantitiesEqual, "Sum should equal expected quantity");
        }

        /// <summary>
        /// Tests conversion followed by addition workflow.
        /// </summary>
        [TestMethod]
        public void ConvertThenAdd_Workflow_ReturnsCorrectResult()
        {
            // Arrange
            var feetQuantity = new Quantity(3.0, LengthUnit.FEET);
            var yardQuantity = new Quantity(1.0, LengthUnit.YARD);

            // Act - Convert yard to feet, then add
            var yardInFeet = yardQuantity.ConvertTo(LengthUnit.FEET);
            var totalInFeet = feetQuantity.Add(yardInFeet);

            // Assert
            Assert.AreEqual(6.0, totalInFeet.Value, Tolerance, "3 ft + 1 yd should equal 6 ft");
        }

        /// <summary>
        /// Tests comparison workflow across different units.
        /// </summary>
        [TestMethod]
        public void CompareAcrossUnits_Workflow_ReturnsCorrectResult()
        {
            // Arrange - Create quantities that should all be equal to 1 yard
            var yardsQuantity = new Quantity(1.0, LengthUnit.YARD); // 1 yard
            var feetQuantity = new Quantity(3.0, LengthUnit.FEET); // 3 feet = 1 yard
            var inchesQuantity = new Quantity(36.0, LengthUnit.INCH); // 36 inches = 1 yard
            var cmQuantity = new Quantity(91.44, LengthUnit.CENTIMETER); // 91.44 cm = 1 yard

            // Act & Assert - Compare each quantity directly with yardsQuantity using AreQuantitiesEqual
            bool feetEqualsYards = _measurementService.AreQuantitiesEqual(
                feetQuantity,
                yardsQuantity
            );
            bool inchesEqualsYards = _measurementService.AreQuantitiesEqual(
                inchesQuantity,
                yardsQuantity
            );
            bool cmEqualsYards = _measurementService.AreQuantitiesEqual(cmQuantity, yardsQuantity);

            // Assert
            Assert.IsTrue(feetEqualsYards, "3 ft should equal 1 yd");
            Assert.IsTrue(inchesEqualsYards, "36 in should equal 1 yd");
            Assert.IsTrue(cmEqualsYards, "91.44 cm should equal 1 yd");

            // Also test using the Quantity.Equals method directly
            Assert.IsTrue(feetQuantity.Equals(yardsQuantity), "3 ft should equal 1 yd (direct)");
            Assert.IsTrue(inchesQuantity.Equals(yardsQuantity), "36 in should equal 1 yd (direct)");
            Assert.IsTrue(cmQuantity.Equals(yardsQuantity), "91.44 cm should equal 1 yd (direct)");
        }

        /// <summary>
        /// Tests cross-unit addition with different target units.
        /// </summary>
        [TestMethod]
        public void CrossUnitAddition_DifferentTargets_ReturnsCorrectResults()
        {
            // Arrange
            var feetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var inchesQuantity = new Quantity(12.0, LengthUnit.INCH);

            // Act - Add with different target units
            var sumInFeet = _measurementService.AddQuantitiesWithTarget(
                feetQuantity,
                inchesQuantity,
                LengthUnit.FEET
            );
            var sumInInches = _measurementService.AddQuantitiesWithTarget(
                feetQuantity,
                inchesQuantity,
                LengthUnit.INCH
            );
            var sumInYards = _measurementService.AddQuantitiesWithTarget(
                feetQuantity,
                inchesQuantity,
                LengthUnit.YARD
            );
            var sumInCm = _measurementService.AddQuantitiesWithTarget(
                feetQuantity,
                inchesQuantity,
                LengthUnit.CENTIMETER
            );

            // Assert
            Assert.AreEqual(2.0, sumInFeet.Value, Tolerance, "Sum in feet should be 2 ft");
            Assert.AreEqual(24.0, sumInInches.Value, Tolerance, "Sum in inches should be 24 in");
            Assert.AreEqual(
                2.0 / 3.0,
                sumInYards.Value,
                Tolerance,
                "Sum in yards should be 2/3 yd"
            );
            Assert.AreEqual(60.96, sumInCm.Value, Tolerance, "Sum in cm should be 60.96 cm");
        }

        /// <summary>
        /// Tests round-trip conversion through multiple units.
        /// </summary>
        [TestMethod]
        public void RoundTripConversion_PreservesOriginalValue()
        {
            // Arrange
            double originalValue = 5.0;
            var originalQuantity = new Quantity(originalValue, LengthUnit.FEET);

            // Act - Feet -> Inches -> Centimeters -> Yards -> Feet
            var inInches = originalQuantity.ConvertTo(LengthUnit.INCH);
            var inCentimeters = inInches.ConvertTo(LengthUnit.CENTIMETER);
            var inYards = inCentimeters.ConvertTo(LengthUnit.YARD);
            var backToFeet = inYards.ConvertTo(LengthUnit.FEET);

            // Assert
            Assert.AreEqual(
                originalValue,
                backToFeet.Value,
                Tolerance,
                "Round-trip conversion should return original value"
            );
        }
    }
}
