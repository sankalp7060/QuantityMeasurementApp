using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.GenericQuantityTests
{
    /// <summary>
    /// Test class for GenericQuantity division operations.
    /// UC12: Tests division functionality for all measurement categories.
    /// </summary>
    [TestClass]
    public class GenericQuantityDivisionTests
    {
        private const double Tolerance = 0.000001;

        #region Length Division Tests

        /// <summary>
        /// Tests division of two lengths in same unit (feet).
        /// </summary>
        [TestMethod]
        public void Divide_Length_SameUnit_Feet_ReturnsCorrectRatio()
        {
            // Arrange
            var firstLength = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var secondLength = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);

            // Act
            double ratio = firstLength.Divide(secondLength);

            // Assert
            Assert.AreEqual(5.0, ratio, Tolerance, "10 ft ÷ 2 ft should equal 5.0");
        }

        /// <summary>
        /// Tests division of two lengths in different units (feet / inches).
        /// </summary>
        [TestMethod]
        public void Divide_Length_CrossUnit_FeetByInches_ReturnsCorrectRatio()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(24.0, LengthUnit.INCH);

            // Act
            double ratio = feetLength.Divide(inchesLength);

            // Assert
            Assert.AreEqual(1.0, ratio, Tolerance, "2 ft ÷ 24 in should equal 1.0");
        }

        /// <summary>
        /// Tests division resulting in ratio greater than 1.
        /// </summary>
        [TestMethod]
        public void Divide_Length_RatioGreaterThanOne_ReturnsCorrectRatio()
        {
            // Arrange
            var firstLength = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var secondLength = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);

            // Act
            double ratio = firstLength.Divide(secondLength);

            // Assert
            Assert.AreEqual(5.0, ratio, Tolerance, "10 ft ÷ 2 ft = 5.0 (>1)");
            Assert.IsTrue(ratio > 1.0, "Ratio should be greater than 1");
        }

        /// <summary>
        /// Tests division resulting in ratio less than 1.
        /// </summary>
        [TestMethod]
        public void Divide_Length_RatioLessThanOne_ReturnsCorrectRatio()
        {
            // Arrange
            var firstLength = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);
            var secondLength = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);

            // Act
            double ratio = firstLength.Divide(secondLength);

            // Assert
            Assert.AreEqual(0.2, ratio, Tolerance, "2 ft ÷ 10 ft = 0.2 (<1)");
            Assert.IsTrue(ratio < 1.0, "Ratio should be less than 1");
        }

        /// <summary>
        /// Tests division resulting in ratio equal to 1.
        /// </summary>
        [TestMethod]
        public void Divide_Length_RatioEqualToOne_ReturnsCorrectRatio()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            double ratio = feetLength.Divide(inchesLength);

            // Assert
            Assert.AreEqual(1.0, ratio, Tolerance, "1 ft ÷ 12 in = 1.0");
        }

        /// <summary>
        /// Tests that division is not commutative.
        /// </summary>
        [TestMethod]
        public void Divide_Length_NotCommutative_ReturnsDifferentRatios()
        {
            // Arrange
            var a = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);

            // Act
            double aDivB = a.Divide(b);
            double bDivA = b.Divide(a);

            // Assert
            Assert.AreEqual(5.0, aDivB, Tolerance, "10 ÷ 2 = 5");
            Assert.AreEqual(0.2, bDivA, Tolerance, "2 ÷ 10 = 0.2");
            Assert.AreNotEqual(aDivB, bDivA, "Division should not be commutative");
        }

        #endregion

        #region Weight Division Tests

        /// <summary>
        /// Tests division of two weights in same unit (kilograms).
        /// </summary>
        [TestMethod]
        public void Divide_Weight_SameUnit_Kilograms_ReturnsCorrectRatio()
        {
            // Arrange
            var firstWeight = new GenericQuantity<WeightUnit>(10.0, WeightUnit.KILOGRAM);
            var secondWeight = new GenericQuantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);

            // Act
            double ratio = firstWeight.Divide(secondWeight);

            // Assert
            Assert.AreEqual(5.0, ratio, Tolerance, "10 kg ÷ 2 kg should equal 5.0");
        }

        /// <summary>
        /// Tests division of two weights in different units (kg / g).
        /// </summary>
        [TestMethod]
        public void Divide_Weight_CrossUnit_KgByGrams_ReturnsCorrectRatio()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(2000.0, WeightUnit.GRAM);

            // Act
            double ratio = kgWeight.Divide(gWeight);

            // Assert
            Assert.AreEqual(1.0, ratio, Tolerance, "2 kg ÷ 2000 g should equal 1.0");
        }

        #endregion

        #region Volume Division Tests

        /// <summary>
        /// Tests division of two volumes in same unit (litres).
        /// </summary>
        [TestMethod]
        public void Divide_Volume_SameUnit_Litres_ReturnsCorrectRatio()
        {
            // Arrange
            var firstVolume = new GenericQuantity<VolumeUnit>(10.0, VolumeUnit.LITRE);
            var secondVolume = new GenericQuantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            // Act
            double ratio = firstVolume.Divide(secondVolume);

            // Assert
            Assert.AreEqual(5.0, ratio, Tolerance, "10 L ÷ 2 L should equal 5.0");
        }

        /// <summary>
        /// Tests division of two volumes in different units (L / mL).
        /// </summary>
        [TestMethod]
        public void Divide_Volume_CrossUnit_LitresByMillilitres_ReturnsCorrectRatio()
        {
            // Arrange
            var litreVolume = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var mlVolume = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            double ratio = litreVolume.Divide(mlVolume);

            // Assert
            Assert.AreEqual(1.0, ratio, Tolerance, "1 L ÷ 1000 mL should equal 1.0");
        }

        #endregion

        #region Edge Cases

        /// <summary>
        /// Tests division with very large values.
        /// </summary>
        [TestMethod]
        public void Divide_LargeValues_ReturnsCorrectRatio()
        {
            // Arrange
            var large = new GenericQuantity<LengthUnit>(1000000.0, LengthUnit.FEET);
            var small = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act
            double ratio = large.Divide(small);

            // Assert
            Assert.AreEqual(1000000.0, ratio, 0.001, "1,000,000 ÷ 1 = 1,000,000");
        }

        /// <summary>
        /// Tests division with very small values.
        /// </summary>
        [TestMethod]
        public void Divide_SmallValues_ReturnsCorrectRatio()
        {
            // Arrange
            var small = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var large = new GenericQuantity<LengthUnit>(1000000.0, LengthUnit.FEET);

            // Act
            double ratio = small.Divide(large);

            // Assert
            Assert.AreEqual(0.000001, ratio, 0.0000001, "1 ÷ 1,000,000 = 0.000001");
        }

        /// <summary>
        /// Tests division by zero throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void Divide_ByZero_ThrowsException()
        {
            // Arrange
            var valid = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var zero = new GenericQuantity<LengthUnit>(0.0, LengthUnit.FEET);

            // Act - Should throw
            valid.Divide(zero);
        }

        /// <summary>
        /// Tests that dividing by null throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Divide_NullOperand_ThrowsException()
        {
            // Arrange
            var valid = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act - Should throw
            valid.Divide(null!);
        }

        /// <summary>
        /// Tests static Divide method.
        /// </summary>
        [TestMethod]
        public void Divide_Static_ReturnsCorrectRatio()
        {
            // Act
            double ratio = GenericQuantity<LengthUnit>.Divide(
                10.0,
                LengthUnit.FEET,
                2.0,
                LengthUnit.FEET
            );

            // Assert
            Assert.AreEqual(5.0, ratio, Tolerance, "10 ft ÷ 2 ft = 5.0");
        }

        #endregion
    }
}
