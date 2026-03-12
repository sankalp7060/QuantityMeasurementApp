using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.GenericQuantityTests
{
    /// <summary>
    /// Test class for GenericQuantity conversion operations.
    /// UC10: Tests conversion functionality for all measurement categories.
    /// Verifies unit-to-unit conversion across all supported units.
    /// </summary>
    [TestClass]
    public class GenericQuantityConversionTests
    {
        private const double Tolerance = 0.000001;
        private const double PoundTolerance = 0.001;

        #region Length Conversion Tests

        /// <summary>
        /// Tests ConvertTo for feet to inches.
        /// Verifies UC5 functionality preserved.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Length_FeetToInches_ReturnsCorrectQuantity()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act
            var inchesLength = feetLength.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(12.0, inchesLength.Value, Tolerance, "1 ft should convert to 12 in");
            Assert.AreEqual(LengthUnit.INCH, inchesLength.Unit, "Unit should be inches");
        }

        /// <summary>
        /// Tests ConvertTo for inches to feet.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Length_InchesToFeet_ReturnsCorrectQuantity()
        {
            // Arrange
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            var feetLength = inchesLength.ConvertTo(LengthUnit.FEET);

            // Assert
            Assert.AreEqual(1.0, feetLength.Value, Tolerance, "12 in should convert to 1 ft");
            Assert.AreEqual(LengthUnit.FEET, feetLength.Unit, "Unit should be feet");
        }

        /// <summary>
        /// Tests ConvertTo for yards to feet.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Length_YardsToFeet_ReturnsCorrectQuantity()
        {
            // Arrange
            var yardLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.YARD);

            // Act
            var feetLength = yardLength.ConvertTo(LengthUnit.FEET);

            // Assert
            Assert.AreEqual(3.0, feetLength.Value, Tolerance, "1 yd should convert to 3 ft");
            Assert.AreEqual(LengthUnit.FEET, feetLength.Unit, "Unit should be feet");
        }

        /// <summary>
        /// Tests ConvertTo for yards to inches.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Length_YardsToInches_ReturnsCorrectQuantity()
        {
            // Arrange
            var yardLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.YARD);

            // Act
            var inchesLength = yardLength.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(36.0, inchesLength.Value, Tolerance, "1 yd should convert to 36 in");
            Assert.AreEqual(LengthUnit.INCH, inchesLength.Unit, "Unit should be inches");
        }

        /// <summary>
        /// Tests ConvertTo for centimeters to inches.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Length_CentimetersToInches_ReturnsCorrectQuantity()
        {
            // Arrange
            var cmLength = new GenericQuantity<LengthUnit>(2.54, LengthUnit.CENTIMETER);

            // Act
            var inchesLength = cmLength.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(1.0, inchesLength.Value, Tolerance, "2.54 cm should convert to 1 in");
            Assert.AreEqual(LengthUnit.INCH, inchesLength.Unit, "Unit should be inches");
        }

        /// <summary>
        /// Tests ConvertTo for centimeters to feet.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Length_CentimetersToFeet_ReturnsCorrectQuantity()
        {
            // Arrange
            var cmLength = new GenericQuantity<LengthUnit>(30.48, LengthUnit.CENTIMETER);

            // Act
            var feetLength = cmLength.ConvertTo(LengthUnit.FEET);

            // Assert
            Assert.AreEqual(1.0, feetLength.Value, Tolerance, "30.48 cm should convert to 1 ft");
            Assert.AreEqual(LengthUnit.FEET, feetLength.Unit, "Unit should be feet");
        }

        /// <summary>
        /// Tests ConvertTo for centimeters to yards.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Length_CentimetersToYards_ReturnsCorrectQuantity()
        {
            // Arrange
            var cmLength = new GenericQuantity<LengthUnit>(91.44, LengthUnit.CENTIMETER);

            // Act
            var yardLength = cmLength.ConvertTo(LengthUnit.YARD);

            // Assert
            Assert.AreEqual(1.0, yardLength.Value, Tolerance, "91.44 cm should convert to 1 yd");
            Assert.AreEqual(LengthUnit.YARD, yardLength.Unit, "Unit should be yards");
        }

        #endregion

        #region Weight Conversion Tests

        /// <summary>
        /// Tests ConvertTo for kilograms to grams.
        /// Verifies UC9 functionality preserved.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Weight_KilogramsToGrams_ReturnsCorrectQuantity()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            var gWeight = kgWeight.ConvertTo(WeightUnit.GRAM);

            // Assert
            Assert.AreEqual(1000.0, gWeight.Value, Tolerance, "1 kg should convert to 1000 g");
            Assert.AreEqual(WeightUnit.GRAM, gWeight.Unit, "Unit should be grams");
        }

        /// <summary>
        /// Tests ConvertTo for grams to kilograms.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Weight_GramsToKilograms_ReturnsCorrectQuantity()
        {
            // Arrange
            var gWeight = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            // Act
            var kgWeight = gWeight.ConvertTo(WeightUnit.KILOGRAM);

            // Assert
            Assert.AreEqual(1.0, kgWeight.Value, Tolerance, "1000 g should convert to 1 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, kgWeight.Unit, "Unit should be kilograms");
        }

        /// <summary>
        /// Tests ConvertTo for kilograms to pounds.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Weight_KilogramsToPounds_ReturnsCorrectQuantity()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            var lbWeight = kgWeight.ConvertTo(WeightUnit.POUND);

            // Assert
            Assert.AreEqual(
                2.20462262185,
                lbWeight.Value,
                PoundTolerance,
                "1 kg should convert to approximately 2.20462 lb"
            );
            Assert.AreEqual(WeightUnit.POUND, lbWeight.Unit, "Unit should be pounds");
        }

        /// <summary>
        /// Tests ConvertTo for pounds to kilograms.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Weight_PoundsToKilograms_ReturnsCorrectQuantity()
        {
            // Arrange
            var lbWeight = new GenericQuantity<WeightUnit>(2.20462262185, WeightUnit.POUND);

            // Act
            var kgWeight = lbWeight.ConvertTo(WeightUnit.KILOGRAM);

            // Assert
            Assert.AreEqual(
                1.0,
                kgWeight.Value,
                PoundTolerance,
                "2.20462 lb should convert to approximately 1 kg"
            );
            Assert.AreEqual(WeightUnit.KILOGRAM, kgWeight.Unit, "Unit should be kilograms");
        }

        /// <summary>
        /// Tests ConvertTo for grams to pounds.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Weight_GramsToPounds_ReturnsCorrectQuantity()
        {
            // Arrange
            var gWeight = new GenericQuantity<WeightUnit>(453.59237, WeightUnit.GRAM);

            // Act
            var lbWeight = gWeight.ConvertTo(WeightUnit.POUND);

            // Assert
            Assert.AreEqual(
                1.0,
                lbWeight.Value,
                PoundTolerance,
                "453.592 g should convert to approximately 1 lb"
            );
            Assert.AreEqual(WeightUnit.POUND, lbWeight.Unit, "Unit should be pounds");
        }

        #endregion

        #region ConvertToDouble Tests

        /// <summary>
        /// Tests ConvertToDouble method for length.
        /// </summary>
        [TestMethod]
        public void ConvertToDouble_Length_FeetToInches_ReturnsCorrectValue()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act
            double inchesValue = feetLength.ConvertToDouble(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(12.0, inchesValue, Tolerance, "1 ft should convert to 12 in");
        }

        /// <summary>
        /// Tests ConvertToDouble method for weight.
        /// </summary>
        [TestMethod]
        public void ConvertToDouble_Weight_KilogramsToGrams_ReturnsCorrectValue()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            double gramsValue = kgWeight.ConvertToDouble(WeightUnit.GRAM);

            // Assert
            Assert.AreEqual(1000.0, gramsValue, Tolerance, "1 kg should convert to 1000 g");
        }

        #endregion

        #region Round-Trip Conversion Tests

        /// <summary>
        /// Tests round-trip conversion for length.
        /// </summary>
        [TestMethod]
        public void ConvertTo_RoundTrip_Length_ReturnsOriginalValue()
        {
            // Arrange
            double originalValue = 2.5;
            var originalLength = new GenericQuantity<LengthUnit>(originalValue, LengthUnit.FEET);

            // Act
            var inInches = originalLength.ConvertTo(LengthUnit.INCH);
            var backToFeet = inInches.ConvertTo(LengthUnit.FEET);

            // Assert
            Assert.AreEqual(
                originalValue,
                backToFeet.Value,
                Tolerance,
                "Round-trip ft->in->ft should return original"
            );
        }

        /// <summary>
        /// Tests round-trip conversion for weight.
        /// </summary>
        [TestMethod]
        public void ConvertTo_RoundTrip_Weight_ReturnsOriginalValue()
        {
            // Arrange
            double originalValue = 2.5;
            var originalWeight = new GenericQuantity<WeightUnit>(
                originalValue,
                WeightUnit.KILOGRAM
            );

            // Act
            var inGrams = originalWeight.ConvertTo(WeightUnit.GRAM);
            var backToKg = inGrams.ConvertTo(WeightUnit.KILOGRAM);

            // Assert
            Assert.AreEqual(
                originalValue,
                backToKg.Value,
                Tolerance,
                "Round-trip kg->g->kg should return original"
            );
        }

        /// <summary>
        /// Tests multi-step round-trip conversion through multiple units.
        /// </summary>
        [TestMethod]
        public void ConvertTo_MultiStepRoundTrip_ReturnsOriginalValue()
        {
            // Arrange
            double originalValue = 1.0;
            var originalLength = new GenericQuantity<LengthUnit>(originalValue, LengthUnit.FEET);

            // Act - Feet -> Inches -> Centimeters -> Yards -> Feet
            var inInches = originalLength.ConvertTo(LengthUnit.INCH);
            var inCm = inInches.ConvertTo(LengthUnit.CENTIMETER);
            var inYards = inCm.ConvertTo(LengthUnit.YARD);
            var backToFeet = inYards.ConvertTo(LengthUnit.FEET);

            // Assert
            Assert.AreEqual(
                originalValue,
                backToFeet.Value,
                Tolerance,
                "Multi-step conversion should return original value"
            );
        }

        #endregion

        #region Zero and Negative Value Tests

        /// <summary>
        /// Tests conversion with zero value.
        /// </summary>
        [TestMethod]
        public void ConvertTo_ZeroValue_ReturnsZero()
        {
            // Arrange
            var zeroLength = new GenericQuantity<LengthUnit>(0.0, LengthUnit.FEET);
            var zeroWeight = new GenericQuantity<WeightUnit>(0.0, WeightUnit.KILOGRAM);

            // Act
            var convertedLength = zeroLength.ConvertTo(LengthUnit.INCH);
            var convertedWeight = zeroWeight.ConvertTo(WeightUnit.GRAM);

            // Assert
            Assert.AreEqual(0.0, convertedLength.Value, Tolerance, "0 ft should convert to 0 in");
            Assert.AreEqual(0.0, convertedWeight.Value, Tolerance, "0 kg should convert to 0 g");
        }

        /// <summary>
        /// Tests conversion with negative value.
        /// </summary>
        [TestMethod]
        public void ConvertTo_NegativeValue_PreservesSign()
        {
            // Arrange
            var negativeLength = new GenericQuantity<LengthUnit>(-1.0, LengthUnit.FEET);
            var negativeWeight = new GenericQuantity<WeightUnit>(-1.0, WeightUnit.KILOGRAM);

            // Act
            var convertedLength = negativeLength.ConvertTo(LengthUnit.INCH);
            var convertedWeight = negativeWeight.ConvertTo(WeightUnit.GRAM);

            // Assert
            Assert.AreEqual(
                -12.0,
                convertedLength.Value,
                Tolerance,
                "-1 ft should convert to -12 in"
            );
            Assert.AreEqual(
                -1000.0,
                convertedWeight.Value,
                Tolerance,
                "-1 kg should convert to -1000 g"
            );
        }

        #endregion

        #region Same-Unit Conversion Tests

        /// <summary>
        /// Tests conversion where source and target units are the same.
        /// </summary>
        [TestMethod]
        public void ConvertTo_SameUnit_ReturnsOriginalValue()
        {
            // Arrange
            var length = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);
            var weight = new GenericQuantity<WeightUnit>(5.0, WeightUnit.KILOGRAM);

            // Act
            var sameUnitLength = length.ConvertTo(LengthUnit.FEET);
            var sameUnitWeight = weight.ConvertTo(WeightUnit.KILOGRAM);

            // Assert
            Assert.AreEqual(
                5.0,
                sameUnitLength.Value,
                Tolerance,
                "Converting to same unit should return same value"
            );
            Assert.AreEqual(
                5.0,
                sameUnitWeight.Value,
                Tolerance,
                "Converting to same unit should return same value"
            );
        }

        #endregion

        #region Bidirectional Conversion Tests

        /// <summary>
        /// Tests bidirectional conversion (A→B and B→A are inverses).
        /// </summary>
        [TestMethod]
        public void ConvertTo_Bidirectional_AreInverses()
        {
            // Arrange
            double value = 5.0;

            // Act & Assert - Length
            var feetLength = new GenericQuantity<LengthUnit>(value, LengthUnit.FEET);
            var inInches = feetLength.ConvertTo(LengthUnit.INCH);
            var backToFeet = inInches.ConvertTo(LengthUnit.FEET);
            Assert.AreEqual(
                value,
                backToFeet.Value,
                Tolerance,
                "Feet↔Inches conversions should be inverses"
            );

            // Act & Assert - Weight
            var kgWeight = new GenericQuantity<WeightUnit>(value, WeightUnit.KILOGRAM);
            var inGrams = kgWeight.ConvertTo(WeightUnit.GRAM);
            var backToKg = inGrams.ConvertTo(WeightUnit.KILOGRAM);
            Assert.AreEqual(
                value,
                backToKg.Value,
                Tolerance,
                "Kg↔Grams conversions should be inverses"
            );
        }

        #endregion
    }
}
