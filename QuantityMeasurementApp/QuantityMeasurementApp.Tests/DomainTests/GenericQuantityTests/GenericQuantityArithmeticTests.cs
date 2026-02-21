using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.GenericQuantityTests
{
    /// <summary>
    /// Test class for GenericQuantity arithmetic operations.
    /// UC10: Tests addition functionality for all measurement categories.
    /// Verifies addition with default and explicit target units.
    /// </summary>
    [TestClass]
    public class GenericQuantityArithmeticTests
    {
        private const double Tolerance = 0.000001;
        private const double PoundTolerance = 0.001;

        #region Length Addition Tests

        /// <summary>
        /// Tests addition of two length quantities in same unit (feet).
        /// Verifies UC6 functionality preserved.
        /// </summary>
        [TestMethod]
        public void Add_Length_SameUnit_Feet_ReturnsCorrectSum()
        {
            // Arrange
            var firstLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var secondLength = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);

            // Act
            var sumLength = firstLength.Add(secondLength);

            // Assert
            Assert.AreEqual(3.0, sumLength.Value, Tolerance, "1 ft + 2 ft should equal 3 ft");
            Assert.AreEqual(LengthUnit.FEET, sumLength.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests addition of two length quantities in same unit (inches).
        /// </summary>
        [TestMethod]
        public void Add_Length_SameUnit_Inches_ReturnsCorrectSum()
        {
            // Arrange
            var firstLength = new GenericQuantity<LengthUnit>(6.0, LengthUnit.INCH);
            var secondLength = new GenericQuantity<LengthUnit>(6.0, LengthUnit.INCH);

            // Act
            var sumLength = firstLength.Add(secondLength);

            // Assert
            Assert.AreEqual(12.0, sumLength.Value, Tolerance, "6 in + 6 in should equal 12 in");
            Assert.AreEqual(LengthUnit.INCH, sumLength.Unit, "Result should be in inches");
        }

        /// <summary>
        /// Tests addition of length quantities in different units with result in first unit.
        /// </summary>
        [TestMethod]
        public void Add_Length_CrossUnit_ResultInFirstUnit_ReturnsCorrectSum()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            var sumLength = feetLength.Add(inchesLength);

            // Assert
            Assert.AreEqual(2.0, sumLength.Value, Tolerance, "1 ft + 12 in should equal 2 ft");
            Assert.AreEqual(LengthUnit.FEET, sumLength.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests addition of length quantities in different units with result in second unit.
        /// </summary>
        [TestMethod]
        public void Add_Length_CrossUnit_ResultInSecondUnit_ReturnsCorrectSum()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            var sumLength = feetLength.Add(inchesLength, LengthUnit.INCH);

            // Assert
            Assert.AreEqual(
                24.0,
                sumLength.Value,
                Tolerance,
                "1 ft + 12 in in inches should equal 24 in"
            );
            Assert.AreEqual(LengthUnit.INCH, sumLength.Unit, "Result should be in inches");
        }

        /// <summary>
        /// Tests addition of length quantities with explicit target unit (yards).
        /// Verifies UC7 functionality preserved.
        /// </summary>
        [TestMethod]
        public void Add_Length_ExplicitTarget_Yards_ReturnsCorrectSum()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            var sumInYards = feetLength.Add(inchesLength, LengthUnit.YARD);

            // Assert
            double expectedValue = 2.0 / 3.0; // 2 feet = 2/3 yards
            Assert.AreEqual(
                expectedValue,
                sumInYards.Value,
                Tolerance,
                "1 ft + 12 in in yards should equal 2/3 yd"
            );
            Assert.AreEqual(LengthUnit.YARD, sumInYards.Unit, "Result should be in yards");
        }

        /// <summary>
        /// Tests addition of length quantities with explicit target unit (centimeters).
        /// </summary>
        [TestMethod]
        public void Add_Length_ExplicitTarget_Centimeters_ReturnsCorrectSum()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            var sumInCm = feetLength.Add(inchesLength, LengthUnit.CENTIMETER);

            // Assert
            double expectedValue = 2.0 * 30.48; // 2 feet in cm
            Assert.AreEqual(
                expectedValue,
                sumInCm.Value,
                Tolerance,
                "1 ft + 12 in in cm should equal 60.96 cm"
            );
            Assert.AreEqual(LengthUnit.CENTIMETER, sumInCm.Unit, "Result should be in centimeters");
        }

        #endregion

        #region Weight Addition Tests

        /// <summary>
        /// Tests addition of two weight quantities in same unit (kilograms).
        /// Verifies UC9 functionality preserved.
        /// </summary>
        [TestMethod]
        public void Add_Weight_SameUnit_Kilograms_ReturnsCorrectSum()
        {
            // Arrange
            var firstWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var secondWeight = new GenericQuantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);

            // Act
            var sumWeight = firstWeight.Add(secondWeight);

            // Assert
            Assert.AreEqual(3.0, sumWeight.Value, Tolerance, "1 kg + 2 kg should equal 3 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, sumWeight.Unit, "Result should be in kilograms");
        }

        /// <summary>
        /// Tests addition of weight quantities in different units with result in first unit.
        /// </summary>
        [TestMethod]
        public void Add_Weight_CrossUnit_ResultInFirstUnit_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(500.0, WeightUnit.GRAM);

            // Act
            var sumWeight = kgWeight.Add(gWeight);

            // Assert
            Assert.AreEqual(1.5, sumWeight.Value, Tolerance, "1 kg + 500 g should equal 1.5 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, sumWeight.Unit, "Result should be in kilograms");
        }

        /// <summary>
        /// Tests addition of weight quantities in different units with result in second unit.
        /// </summary>
        [TestMethod]
        public void Add_Weight_CrossUnit_ResultInSecondUnit_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(500.0, WeightUnit.GRAM);

            // Act
            var sumWeight = kgWeight.Add(gWeight, WeightUnit.GRAM);

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
        /// Tests addition of weight quantities with explicit target unit (pounds).
        /// </summary>
        [TestMethod]
        public void Add_Weight_ExplicitTarget_Pounds_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(500.0, WeightUnit.GRAM);

            // Act
            var sumInPounds = kgWeight.Add(gWeight, WeightUnit.POUND);

            // Assert
            double expectedValue = 1.5 * 2.20462262185; // 1.5 kg in pounds
            Assert.AreEqual(
                expectedValue,
                sumInPounds.Value,
                PoundTolerance,
                "1 kg + 500 g in pounds should be correct"
            );
            Assert.AreEqual(WeightUnit.POUND, sumInPounds.Unit, "Result should be in pounds");
        }

        #endregion

        #region Static Add Method Tests

        /// <summary>
        /// Tests static Add method for length.
        /// </summary>
        [TestMethod]
        public void Add_Static_Length_ReturnsCorrectSum()
        {
            // Act
            var sumLength = GenericQuantity<LengthUnit>.Add(
                1.0,
                LengthUnit.FEET,
                12.0,
                LengthUnit.INCH,
                LengthUnit.FEET
            );

            // Assert
            Assert.AreEqual(2.0, sumLength.Value, Tolerance, "1 ft + 12 in should equal 2 ft");
            Assert.AreEqual(LengthUnit.FEET, sumLength.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests static Add method for weight.
        /// </summary>
        [TestMethod]
        public void Add_Static_Weight_ReturnsCorrectSum()
        {
            // Act
            var sumWeight = GenericQuantity<WeightUnit>.Add(
                1.0,
                WeightUnit.KILOGRAM,
                1000.0,
                WeightUnit.GRAM,
                WeightUnit.KILOGRAM
            );

            // Assert
            Assert.AreEqual(2.0, sumWeight.Value, Tolerance, "1 kg + 1000 g should equal 2 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, sumWeight.Unit, "Result should be in kilograms");
        }

        #endregion

        #region Commutativity Tests

        /// <summary>
        /// Tests that addition is commutative for length.
        /// </summary>
        [TestMethod]
        public void Add_Length_IsCommutative_ReturnsTrue()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);
            var targetUnit = LengthUnit.YARD;

            // Act
            var firstSum = feetLength.Add(inchesLength, targetUnit);
            var secondSum = inchesLength.Add(feetLength, targetUnit);

            // Assert
            Assert.AreEqual(
                firstSum.Value,
                secondSum.Value,
                Tolerance,
                "a + b should equal b + a when using same target unit"
            );
        }

        /// <summary>
        /// Tests that addition is commutative for weight.
        /// </summary>
        [TestMethod]
        public void Add_Weight_IsCommutative_ReturnsTrue()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);
            var targetUnit = WeightUnit.POUND;

            // Act
            var firstSum = kgWeight.Add(gWeight, targetUnit);
            var secondSum = gWeight.Add(kgWeight, targetUnit);

            // Assert
            Assert.AreEqual(
                firstSum.Value,
                secondSum.Value,
                PoundTolerance,
                "a + b should equal b + a when using same target unit"
            );
        }

        #endregion

        #region Zero and Negative Value Tests

        /// <summary>
        /// Tests addition with zero.
        /// </summary>
        [TestMethod]
        public void Add_WithZero_ReturnsOriginalValue()
        {
            // Arrange - Length
            var originalLength = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);
            var zeroLength = new GenericQuantity<LengthUnit>(0.0, LengthUnit.INCH);

            // Act - Length
            var sumLength = originalLength.Add(zeroLength);

            // Assert - Length
            Assert.AreEqual(5.0, sumLength.Value, Tolerance, "5 ft + 0 in should equal 5 ft");

            // Arrange - Weight
            var originalWeight = new GenericQuantity<WeightUnit>(5.0, WeightUnit.KILOGRAM);
            var zeroWeight = new GenericQuantity<WeightUnit>(0.0, WeightUnit.GRAM);

            // Act - Weight
            var sumWeight = originalWeight.Add(zeroWeight);

            // Assert - Weight
            Assert.AreEqual(5.0, sumWeight.Value, Tolerance, "5 kg + 0 g should equal 5 kg");
        }

        /// <summary>
        /// Tests addition with negative values.
        /// </summary>
        [TestMethod]
        public void Add_WithNegativeValues_ReturnsCorrectSum()
        {
            // Arrange - Length
            var positiveLength = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);
            var negativeLength = new GenericQuantity<LengthUnit>(-2.0, LengthUnit.FEET);

            // Act - Length
            var sumLength = positiveLength.Add(negativeLength);

            // Assert - Length
            Assert.AreEqual(3.0, sumLength.Value, Tolerance, "5 ft + (-2 ft) should equal 3 ft");

            // Arrange - Weight
            var positiveWeight = new GenericQuantity<WeightUnit>(5.0, WeightUnit.KILOGRAM);
            var negativeWeight = new GenericQuantity<WeightUnit>(-2000.0, WeightUnit.GRAM);

            // Act - Weight
            var sumWeight = positiveWeight.Add(negativeWeight);

            // Assert - Weight
            Assert.AreEqual(3.0, sumWeight.Value, Tolerance, "5 kg + (-2000 g) should equal 3 kg");
        }

        #endregion

        #region Null Argument Tests

        /// <summary>
        /// Tests that adding null throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullOperand_ThrowsException()
        {
            // Arrange
            var validQuantity = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act - Should throw
            validQuantity.Add(null!);
        }

        #endregion
    }
}
