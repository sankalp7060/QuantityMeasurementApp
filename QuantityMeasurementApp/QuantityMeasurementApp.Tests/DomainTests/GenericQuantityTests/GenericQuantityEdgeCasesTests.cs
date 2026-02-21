using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.GenericQuantityTests
{
    /// <summary>
    /// Test class for GenericQuantity edge cases and error handling.
    /// UC10: Tests boundary conditions, extreme values, and validation.
    /// </summary>
    [TestClass]
    public class GenericQuantityEdgeCasesTests
    {
        private const double Tolerance = 0.000001;

        #region Constructor Validation Tests

        /// <summary>
        /// Tests that constructor rejects NaN values.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void Constructor_NaNValue_ThrowsException()
        {
            // Act - Should throw
            var invalidLength = new GenericQuantity<LengthUnit>(double.NaN, LengthUnit.FEET);
        }

        /// <summary>
        /// Tests that constructor rejects Infinity values.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void Constructor_InfinityValue_ThrowsException()
        {
            // Act - Should throw
            var invalidLength = new GenericQuantity<LengthUnit>(
                double.PositiveInfinity,
                LengthUnit.FEET
            );
        }

        /// <summary>
        /// Tests that constructor rejects null unit.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullUnit_ThrowsException()
        {
            // Act - Should throw
            var invalidLength = new GenericQuantity<LengthUnit>(1.0, null!);
        }

        #endregion

        #region Large Value Tests

        /// <summary>
        /// Tests conversion with very large values.
        /// </summary>
        [TestMethod]
        public void ConvertTo_LargeValues_MaintainsPrecision()
        {
            // Arrange
            double largeValue = 1000000.0;
            var largeLength = new GenericQuantity<LengthUnit>(largeValue, LengthUnit.FEET);

            // Act
            var convertedLength = largeLength.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(
                12000000.0,
                convertedLength.Value,
                Tolerance * 1000000,
                "1,000,000 feet should equal 12,000,000 inches"
            );
        }

        /// <summary>
        /// Tests conversion with very small values.
        /// </summary>
        [TestMethod]
        public void ConvertTo_SmallValues_MaintainsPrecision()
        {
            // Arrange
            double smallValue = 0.000001;
            var smallLength = new GenericQuantity<LengthUnit>(smallValue, LengthUnit.FEET);

            // Act
            var convertedLength = smallLength.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(
                0.000012,
                convertedLength.Value,
                Tolerance,
                "0.000001 feet should equal 0.000012 inches"
            );
        }

        /// <summary>
        /// Tests addition with values approaching double.MaxValue.
        /// </summary>
        [TestMethod]
        public void Add_NearMaxValue_HandlesCorrectly()
        {
            // Arrange
            double nearMax = double.MaxValue / 4.0;
            var firstLarge = new GenericQuantity<LengthUnit>(nearMax, LengthUnit.FEET);
            var secondLarge = new GenericQuantity<LengthUnit>(nearMax, LengthUnit.FEET);

            // Act
            var sumQuantity = firstLarge.Add(secondLarge, LengthUnit.YARD);

            // Assert
            Assert.IsFalse(double.IsInfinity(sumQuantity.Value), "Result should not be infinite");
            Assert.IsFalse(double.IsNaN(sumQuantity.Value), "Result should not be NaN");
        }

        #endregion

        #region Floating Point Precision Tests

        /// <summary>
        /// Tests conversion precision with many decimal places.
        /// </summary>
        [TestMethod]
        public void ConvertTo_PrecisionTest_MaintainsAccuracy()
        {
            // Arrange
            double preciseValue = 1.23456789;
            var preciseLength = new GenericQuantity<LengthUnit>(preciseValue, LengthUnit.FEET);

            // Act
            var convertedLength = preciseLength.ConvertTo(LengthUnit.INCH);

            // Assert
            double expectedValue = preciseValue * 12.0;
            Assert.AreEqual(
                expectedValue,
                convertedLength.Value,
                0.000001,
                "Conversion should maintain precision"
            );
        }

        /// <summary>
        /// Tests addition with values that have repeating decimals.
        /// </summary>
        [TestMethod]
        public void Add_RepeatingDecimals_MaintainsAccuracy()
        {
            // Arrange
            var oneThirdFoot = new GenericQuantity<LengthUnit>(1.0 / 3.0, LengthUnit.FEET); // 4 inches
            var fourInches = new GenericQuantity<LengthUnit>(4.0, LengthUnit.INCH);

            // Act
            var sumInInches = oneThirdFoot.Add(fourInches, LengthUnit.INCH);

            // Assert
            Assert.AreEqual(8.0, sumInInches.Value, Tolerance, "1/3 ft + 4 in should equal 8 in");
        }

        #endregion

        #region Invalid Operation Tests

        /// <summary>
        /// Tests that adding quantities with different unit types is prevented at compile time.
        /// </summary>
        [TestMethod]
        public void GenericTypeSafety_PreventsCrossCategoryOperations()
        {
            // Arrange
            var length = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var weight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Assert - Different generic instantiations are different types
            Assert.AreNotEqual(
                length.GetType(),
                weight.GetType(),
                "Length and Weight should be different types"
            );
        }

        #endregion

        #region Extreme Unit Combinations Tests

        /// <summary>
        /// Tests conversion between extreme unit combinations.
        /// </summary>
        [TestMethod]
        public void ConvertTo_ExtremeUnitCombinations_WorksCorrectly()
        {
            // Arrange
            var value = 1.0;

            // Act - Length: Yards to Centimeters
            var yardsLength = new GenericQuantity<LengthUnit>(value, LengthUnit.YARD);
            var cmLength = yardsLength.ConvertTo(LengthUnit.CENTIMETER);

            // Assert
            Assert.AreEqual(91.44, cmLength.Value, Tolerance, "1 yd should convert to 91.44 cm");

            // Act - Weight: Pounds to Grams
            var poundsWeight = new GenericQuantity<WeightUnit>(value, WeightUnit.POUND);
            var gramsWeight = poundsWeight.ConvertTo(WeightUnit.GRAM);

            // Assert
            Assert.AreEqual(
                453.59237,
                gramsWeight.Value,
                0.001,
                "1 lb should convert to 453.592 g"
            );
        }

        #endregion
    }
}
