using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.GenericQuantityTests
{
    /// <summary>
    /// Test class for GenericQuantity subtraction operations.
    /// UC12: Tests subtraction functionality for all measurement categories.
    /// </summary>
    [TestClass]
    public class GenericQuantitySubtractionTests
    {
        private const double Tolerance = 0.000001;

        #region Length Subtraction Tests

        /// <summary>
        /// Tests subtraction of two lengths in same unit (feet).
        /// </summary>
        [TestMethod]
        public void Subtract_Length_SameUnit_Feet_ReturnsCorrectDifference()
        {
            // Arrange
            var firstLength = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var secondLength = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);

            // Act
            var difference = firstLength.Subtract(secondLength);

            // Assert
            Assert.AreEqual(5.0, difference.Value, Tolerance, "10 ft - 5 ft should equal 5 ft");
            Assert.AreEqual(LengthUnit.FEET, difference.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests subtraction of two lengths in different units (feet - inches) with result in first unit.
        /// </summary>
        [TestMethod]
        public void Subtract_Length_CrossUnit_ResultInFirstUnit_ReturnsCorrectDifference()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(6.0, LengthUnit.INCH);

            // Act
            var difference = feetLength.Subtract(inchesLength);

            // Assert
            Assert.AreEqual(9.5, difference.Value, Tolerance, "10 ft - 6 in should equal 9.5 ft");
            Assert.AreEqual(LengthUnit.FEET, difference.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests subtraction of two lengths in different units (feet - inches) with result in second unit.
        /// </summary>
        [TestMethod]
        public void Subtract_Length_CrossUnit_ResultInSecondUnit_ReturnsCorrectDifference()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(6.0, LengthUnit.INCH);

            // Act
            var difference = feetLength.Subtract(inchesLength, LengthUnit.INCH);

            // Assert
            Assert.AreEqual(
                114.0,
                difference.Value,
                Tolerance,
                "10 ft - 6 in in inches should equal 114 in"
            );
            Assert.AreEqual(LengthUnit.INCH, difference.Unit, "Result should be in inches");
        }

        /// <summary>
        /// Tests subtraction with explicit target unit (yards).
        /// </summary>
        [TestMethod]
        public void Subtract_Length_ExplicitTarget_Yards_ReturnsCorrectDifference()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(6.0, LengthUnit.INCH);

            // Act
            var difference = feetLength.Subtract(inchesLength, LengthUnit.YARD);

            // Assert
            double expectedValue = 9.5 / 3.0; // 9.5 feet in yards
            Assert.AreEqual(
                expectedValue,
                difference.Value,
                Tolerance,
                "10 ft - 6 in in yards should equal 3.1667 yd"
            );
            Assert.AreEqual(LengthUnit.YARD, difference.Unit, "Result should be in yards");
        }

        /// <summary>
        /// Tests subtraction resulting in negative value.
        /// </summary>
        [TestMethod]
        public void Subtract_Length_NegativeResult_ReturnsCorrectDifference()
        {
            // Arrange
            var firstLength = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);
            var secondLength = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);

            // Act
            var difference = firstLength.Subtract(secondLength);

            // Assert
            Assert.AreEqual(-5.0, difference.Value, Tolerance, "5 ft - 10 ft should equal -5 ft");
        }

        /// <summary>
        /// Tests subtraction resulting in zero.
        /// </summary>
        [TestMethod]
        public void Subtract_Length_ZeroResult_ReturnsZero()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(120.0, LengthUnit.INCH);

            // Act
            var difference = feetLength.Subtract(inchesLength);

            // Assert
            Assert.AreEqual(0.0, difference.Value, Tolerance, "10 ft - 120 in should equal 0 ft");
        }

        /// <summary>
        /// Tests that subtraction is not commutative.
        /// </summary>
        [TestMethod]
        public void Subtract_Length_NotCommutative_ReturnsDifferentResults()
        {
            // Arrange
            var a = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);

            // Act
            var aMinusB = a.Subtract(b);
            var bMinusA = b.Subtract(a);

            // Assert
            Assert.AreEqual(5.0, aMinusB.Value, Tolerance, "10 - 5 = 5");
            Assert.AreEqual(-5.0, bMinusA.Value, Tolerance, "5 - 10 = -5");
            Assert.AreNotEqual(
                aMinusB.Value,
                bMinusA.Value,
                "Subtraction should not be commutative"
            );
        }

        #endregion

        #region Weight Subtraction Tests

        /// <summary>
        /// Tests subtraction of two weights in same unit (kilograms).
        /// </summary>
        [TestMethod]
        public void Subtract_Weight_SameUnit_Kilograms_ReturnsCorrectDifference()
        {
            // Arrange
            var firstWeight = new GenericQuantity<WeightUnit>(10.0, WeightUnit.KILOGRAM);
            var secondWeight = new GenericQuantity<WeightUnit>(5.0, WeightUnit.KILOGRAM);

            // Act
            var difference = firstWeight.Subtract(secondWeight);

            // Assert
            Assert.AreEqual(5.0, difference.Value, Tolerance, "10 kg - 5 kg should equal 5 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, difference.Unit, "Result should be in kilograms");
        }

        /// <summary>
        /// Tests subtraction of two weights in different units (kg - g) with result in first unit.
        /// </summary>
        [TestMethod]
        public void Subtract_Weight_CrossUnit_ResultInFirstUnit_ReturnsCorrectDifference()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(500.0, WeightUnit.GRAM);

            // Act
            var difference = kgWeight.Subtract(gWeight);

            // Assert
            Assert.AreEqual(1.5, difference.Value, Tolerance, "2 kg - 500 g should equal 1.5 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, difference.Unit, "Result should be in kilograms");
        }

        /// <summary>
        /// Tests subtraction with explicit target unit (grams).
        /// </summary>
        [TestMethod]
        public void Subtract_Weight_ExplicitTarget_Grams_ReturnsCorrectDifference()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(500.0, WeightUnit.GRAM);

            // Act
            var difference = kgWeight.Subtract(gWeight, WeightUnit.GRAM);

            // Assert
            Assert.AreEqual(
                1500.0,
                difference.Value,
                Tolerance,
                "2 kg - 500 g in grams should equal 1500 g"
            );
            Assert.AreEqual(WeightUnit.GRAM, difference.Unit, "Result should be in grams");
        }

        #endregion

        #region Volume Subtraction Tests

        /// <summary>
        /// Tests subtraction of two volumes in same unit (litres).
        /// </summary>
        [TestMethod]
        public void Subtract_Volume_SameUnit_Litres_ReturnsCorrectDifference()
        {
            // Arrange
            var firstVolume = new GenericQuantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var secondVolume = new GenericQuantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            // Act
            var difference = firstVolume.Subtract(secondVolume);

            // Assert
            Assert.AreEqual(3.0, difference.Value, Tolerance, "5 L - 2 L should equal 3 L");
            Assert.AreEqual(VolumeUnit.LITRE, difference.Unit, "Result should be in litres");
        }

        /// <summary>
        /// Tests subtraction of two volumes in different units (L - mL) with result in first unit.
        /// </summary>
        [TestMethod]
        public void Subtract_Volume_CrossUnit_ResultInFirstUnit_ReturnsCorrectDifference()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            // Act
            var difference = litreVolume.Subtract(mlVolume);

            // Assert
            Assert.AreEqual(4.5, difference.Value, Tolerance, "5 L - 500 mL should equal 4.5 L");
            Assert.AreEqual(VolumeUnit.LITRE, difference.Unit, "Result should be in litres");
        }

        /// <summary>
        /// Tests subtraction with explicit target unit (millilitres).
        /// </summary>
        [TestMethod]
        public void Subtract_Volume_ExplicitTarget_Millilitres_ReturnsCorrectDifference()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            // Act
            var difference = litreVolume.Subtract(mlVolume, VolumeUnit.MILLILITRE);

            // Assert
            Assert.AreEqual(
                4500.0,
                difference.Value,
                Tolerance,
                "5 L - 500 mL in mL should equal 4500 mL"
            );
            Assert.AreEqual(
                VolumeUnit.MILLILITRE,
                difference.Unit,
                "Result should be in millilitres"
            );
        }

        #endregion

        #region Edge Cases

        /// <summary>
        /// Tests subtraction with zero.
        /// </summary>
        [TestMethod]
        public void Subtract_WithZero_ReturnsOriginalValue()
        {
            // Arrange
            var original = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);
            var zero = new GenericQuantity<LengthUnit>(0.0, LengthUnit.INCH);

            // Act
            var result = original.Subtract(zero);

            // Assert
            Assert.AreEqual(5.0, result.Value, Tolerance, "5 ft - 0 in should equal 5 ft");
        }

        /// <summary>
        /// Tests subtraction with negative values.
        /// </summary>
        [TestMethod]
        public void Subtract_WithNegativeValues_ReturnsCorrectDifference()
        {
            // Arrange
            var positive = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);
            var negative = new GenericQuantity<LengthUnit>(-2.0, LengthUnit.FEET);

            // Act
            var result = positive.Subtract(negative);

            // Assert
            Assert.AreEqual(7.0, result.Value, Tolerance, "5 ft - (-2 ft) should equal 7 ft");
        }

        /// <summary>
        /// Tests that subtracting null throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subtract_NullOperand_ThrowsException()
        {
            // Arrange
            var valid = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act - Should throw
            valid.Subtract(null!);
        }

        #endregion
    }
}
