using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests covering end-to-end scenarios with generic implementation.
    /// UC10: Tests complete workflows across all measurement categories.
    /// </summary>
    [TestClass]
    public class EndToEndTests
    {
        private GenericMeasurementService _measurementService = null!;
        private const double Tolerance = 0.000001;
        private const double PoundTolerance = 0.001;

        [TestInitialize]
        public void Setup()
        {
            _measurementService = new GenericMeasurementService();
        }

        /// <summary>
        /// Tests a complete length workflow: create, convert, add, compare.
        /// </summary>
        [TestMethod]
        public void CompleteWorkflow_Length_AllOperations_WorkCorrectly()
        {
            // Step 1: Create quantities from string inputs
            var firstLength = _measurementService.CreateQuantityFromString("2", LengthUnit.YARD);
            var secondLength = _measurementService.CreateQuantityFromString("36", LengthUnit.INCH);

            Assert.IsNotNull(firstLength, "First length should not be null");
            Assert.IsNotNull(secondLength, "Second length should not be null");

            // Step 2: Convert first length to feet
            var firstLengthInFeet = firstLength!.ConvertTo(LengthUnit.FEET);
            Assert.AreEqual(6.0, firstLengthInFeet.Value, Tolerance, "2 yards should equal 6 feet");

            // Step 3: Add both lengths and get result in yards
            var sumInYards = _measurementService.AddQuantitiesWithTarget(
                firstLength,
                secondLength!,
                LengthUnit.YARD
            );

            // 2 yd + 36 in = 2 yd + 1 yd = 3 yd
            Assert.AreEqual(3.0, sumInYards.Value, Tolerance, "2 yd + 36 in should equal 3 yd");

            // Step 4: Compare with expected length
            var expectedLength = new GenericQuantity<LengthUnit>(3.0, LengthUnit.YARD);
            bool areEqual = _measurementService.AreQuantitiesEqual(sumInYards, expectedLength);

            Assert.IsTrue(areEqual, "Sum should equal expected length");
        }

        /// <summary>
        /// Tests a complete weight workflow: create, convert, add, compare.
        /// </summary>
        [TestMethod]
        public void CompleteWorkflow_Weight_AllOperations_WorkCorrectly()
        {
            // Step 1: Create quantities from string inputs
            var firstWeight = _measurementService.CreateQuantityFromString(
                "2",
                WeightUnit.KILOGRAM
            );
            var secondWeight = _measurementService.CreateQuantityFromString(
                "2000",
                WeightUnit.GRAM
            );

            Assert.IsNotNull(firstWeight, "First weight should not be null");
            Assert.IsNotNull(secondWeight, "Second weight should not be null");

            // Step 2: Convert first weight to grams
            var firstWeightInGrams = firstWeight!.ConvertTo(WeightUnit.GRAM);
            Assert.AreEqual(
                2000.0,
                firstWeightInGrams.Value,
                Tolerance,
                "2 kg should equal 2000 g"
            );

            // Step 3: Add both weights and get result in kilograms
            var sumInKg = _measurementService.AddQuantitiesWithTarget(
                firstWeight,
                secondWeight!,
                WeightUnit.KILOGRAM
            );

            // 2 kg + 2000 g = 2 kg + 2 kg = 4 kg
            Assert.AreEqual(4.0, sumInKg.Value, Tolerance, "2 kg + 2000 g should equal 4 kg");

            // Step 4: Compare with expected weight
            var expectedWeight = new GenericQuantity<WeightUnit>(4.0, WeightUnit.KILOGRAM);
            bool areEqual = _measurementService.AreQuantitiesEqual(sumInKg, expectedWeight);

            Assert.IsTrue(areEqual, "Sum should equal expected weight");
        }

        /// <summary>
        /// Tests cross-unit comparison across length units.
        /// </summary>
        [TestMethod]
        public void CompareAcrossUnits_Length_ReturnsCorrectResult()
        {
            // Arrange
            var yardsLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.YARD);
            var feetLength = new GenericQuantity<LengthUnit>(3.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(36.0, LengthUnit.INCH);
            var cmLength = new GenericQuantity<LengthUnit>(91.44, LengthUnit.CENTIMETER);

            // Act & Assert
            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(yardsLength, feetLength),
                "1 yd should equal 3 ft"
            );
            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(yardsLength, inchesLength),
                "1 yd should equal 36 in"
            );
            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(yardsLength, cmLength),
                "1 yd should equal 91.44 cm"
            );
        }

        /// <summary>
        /// Tests cross-unit comparison across weight units.
        /// </summary>
        [TestMethod]
        public void CompareAcrossUnits_Weight_ReturnsCorrectResult()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);
            var lbWeight = new GenericQuantity<WeightUnit>(2.20462262185, WeightUnit.POUND);

            // Act & Assert
            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(kgWeight, gWeight),
                "1 kg should equal 1000 g"
            );
            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(kgWeight, lbWeight),
                "1 kg should approximately equal 2.20462 lb"
            );
        }

        /// <summary>
        /// Tests cross-unit addition with different target units.
        /// </summary>
        [TestMethod]
        public void CrossUnitAddition_DifferentTargets_ReturnsCorrectResults()
        {
            // Arrange - Length
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act - Length with different target units
            var sumInFeet = _measurementService.AddQuantitiesWithTarget(
                feetLength,
                inchesLength,
                LengthUnit.FEET
            );
            var sumInInches = _measurementService.AddQuantitiesWithTarget(
                feetLength,
                inchesLength,
                LengthUnit.INCH
            );
            var sumInYards = _measurementService.AddQuantitiesWithTarget(
                feetLength,
                inchesLength,
                LengthUnit.YARD
            );
            var sumInCm = _measurementService.AddQuantitiesWithTarget(
                feetLength,
                inchesLength,
                LengthUnit.CENTIMETER
            );

            // Assert - Length
            Assert.AreEqual(2.0, sumInFeet.Value, Tolerance, "Sum in feet should be 2 ft");
            Assert.AreEqual(24.0, sumInInches.Value, Tolerance, "Sum in inches should be 24 in");
            Assert.AreEqual(
                2.0 / 3.0,
                sumInYards.Value,
                Tolerance,
                "Sum in yards should be 2/3 yd"
            );
            Assert.AreEqual(60.96, sumInCm.Value, Tolerance, "Sum in cm should be 60.96 cm");

            // Arrange - Weight
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(500.0, WeightUnit.GRAM);

            // Act - Weight with different target units
            var sumInKg = _measurementService.AddQuantitiesWithTarget(
                kgWeight,
                gWeight,
                WeightUnit.KILOGRAM
            );
            var sumInG = _measurementService.AddQuantitiesWithTarget(
                kgWeight,
                gWeight,
                WeightUnit.GRAM
            );
            var sumInLb = _measurementService.AddQuantitiesWithTarget(
                kgWeight,
                gWeight,
                WeightUnit.POUND
            );

            // Assert - Weight
            Assert.AreEqual(1.5, sumInKg.Value, Tolerance, "Sum in kg should be 1.5 kg");
            Assert.AreEqual(1500.0, sumInG.Value, Tolerance, "Sum in grams should be 1500 g");
            Assert.AreEqual(
                1.5 * 2.20462262185,
                sumInLb.Value,
                PoundTolerance,
                "Sum in pounds should be correct"
            );
        }

        /// <summary>
        /// Tests round-trip conversion through multiple units.
        /// </summary>
        [TestMethod]
        public void RoundTripConversion_PreservesOriginalValue()
        {
            // Arrange - Length
            double originalLengthValue = 5.0;
            var originalLength = new GenericQuantity<LengthUnit>(
                originalLengthValue,
                LengthUnit.FEET
            );

            // Act - Length: Feet -> Inches -> Centimeters -> Yards -> Feet
            var inInches = originalLength.ConvertTo(LengthUnit.INCH);
            var inCentimeters = inInches.ConvertTo(LengthUnit.CENTIMETER);
            var inYards = inCentimeters.ConvertTo(LengthUnit.YARD);
            var backToFeet = inYards.ConvertTo(LengthUnit.FEET);

            // Assert - Length
            Assert.AreEqual(
                originalLengthValue,
                backToFeet.Value,
                Tolerance,
                "Round-trip length conversion should return original value"
            );

            // Arrange - Weight
            double originalWeightValue = 5.0;
            var originalWeight = new GenericQuantity<WeightUnit>(
                originalWeightValue,
                WeightUnit.KILOGRAM
            );

            // Act - Weight: Kg -> G -> Lb -> Kg
            var inGrams = originalWeight.ConvertTo(WeightUnit.GRAM);
            var inPounds = inGrams.ConvertTo(WeightUnit.POUND);
            var backToKg = inPounds.ConvertTo(WeightUnit.KILOGRAM);

            // Assert - Weight
            Assert.AreEqual(
                originalWeightValue,
                backToKg.Value,
                PoundTolerance,
                "Round-trip weight conversion should return original value"
            );
        }

        /// <summary>
        /// Tests that different measurement categories are independent.
        /// </summary>
        [TestMethod]
        public void DifferentCategories_AreIndependent()
        {
            // Arrange
            var length = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var weight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Assert - Different types
            Assert.AreNotEqual(
                length.GetType(),
                weight.GetType(),
                "Length and Weight should be different types"
            );

            // Assert - Cannot be compared (compiler prevents this, but we test runtime)
            Assert.IsFalse(length.Equals(weight), "Length and weight should not be equal");
        }

        /// <summary>
        /// Tests a complete volume workflow: create, convert, add, compare.
        /// </summary>
        [TestMethod]
        public void CompleteWorkflow_Volume_AllOperations_WorkCorrectly()
        {
            // Step 1: Create quantities from string inputs
            var firstVolume = _measurementService.CreateQuantityFromString("2", VolumeUnit.LITRE);
            var secondVolume = _measurementService.CreateQuantityFromString(
                "1000",
                VolumeUnit.MILLILITRE
            );

            Assert.IsNotNull(firstVolume, "First volume should not be null");
            Assert.IsNotNull(secondVolume, "Second volume should not be null");

            // Step 2: Convert first volume to millilitres
            var firstVolumeInMl = firstVolume!.ConvertTo(VolumeUnit.MILLILITRE);
            Assert.AreEqual(2000.0, firstVolumeInMl.Value, Tolerance, "2 L should equal 2000 mL");

            // Step 3: Add both volumes and get result in litres
            var sumInLitres = _measurementService.AddQuantitiesWithTarget(
                firstVolume,
                secondVolume!,
                VolumeUnit.LITRE
            );

            // 2 L + 1000 mL = 2 L + 1 L = 3 L
            Assert.AreEqual(3.0, sumInLitres.Value, Tolerance, "2 L + 1000 mL should equal 3 L");

            // Step 4: Compare with expected volume
            var expectedVolume = new GenericQuantity<VolumeUnit>(3.0, VolumeUnit.LITRE);
            bool areEqual = _measurementService.AreQuantitiesEqual(sumInLitres, expectedVolume);

            Assert.IsTrue(areEqual, "Sum should equal expected volume");
        }

        /// <summary>
        /// Tests cross-unit comparison across volume units.
        /// </summary>
        [TestMethod]
        public void CompareAcrossUnits_Volume_ReturnsCorrectResult()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var galVolume = new GenericQuantity<VolumeUnit>(0.264172, VolumeUnit.GALLON);

            // Act & Assert
            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(litreVolume, mlVolume),
                "1 L should equal 1000 mL"
            );
            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(litreVolume, galVolume),
                "1 L should approximately equal 0.264172 gal"
            );
        }

        /// <summary>
        /// Tests cross-unit addition with different target units for volume.
        /// </summary>
        [TestMethod]
        public void CrossUnitAddition_Volume_DifferentTargets_ReturnsCorrectResults()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            // Act - Volume with different target units
            var sumInLitres = _measurementService.AddQuantitiesWithTarget(
                litreVolume,
                mlVolume,
                VolumeUnit.LITRE
            );
            var sumInMl = _measurementService.AddQuantitiesWithTarget(
                litreVolume,
                mlVolume,
                VolumeUnit.MILLILITRE
            );
            var sumInGal = _measurementService.AddQuantitiesWithTarget(
                litreVolume,
                mlVolume,
                VolumeUnit.GALLON
            );

            // Assert
            Assert.AreEqual(1.5, sumInLitres.Value, Tolerance, "Sum in litres should be 1.5 L");
            Assert.AreEqual(1500.0, sumInMl.Value, Tolerance, "Sum in mL should be 1500 mL");

            double expectedGal = 1.5 * 0.264172; // 1.5 L in gallons
            Assert.AreEqual(expectedGal, sumInGal.Value, 0.001, "Sum in gallons should be correct");
        }

        /// <summary>
        /// Tests round-trip conversion through multiple volume units.
        /// </summary>
        [TestMethod]
        public void RoundTripConversion_Volume_PreservesOriginalValue()
        {
            // Arrange
            double originalValue = 5.0;
            var originalVolume = new GenericQuantity<VolumeUnit>(originalValue, VolumeUnit.LITRE);

            // Act - Litres -> Millilitres -> Gallons -> Litres
            var inMl = originalVolume.ConvertTo(VolumeUnit.MILLILITRE);
            var inGal = inMl.ConvertTo(VolumeUnit.GALLON);
            var backToLitres = inGal.ConvertTo(VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(
                originalValue,
                backToLitres.Value,
                0.01,
                "Round-trip volume conversion should return original value"
            );
        }
    }
}
