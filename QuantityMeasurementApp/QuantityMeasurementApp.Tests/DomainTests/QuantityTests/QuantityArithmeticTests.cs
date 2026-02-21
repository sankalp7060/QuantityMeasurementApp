using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.QuantityTests
{
    /// <summary>
    /// Test class for Quantity arithmetic operations.
    /// Covers UC6 (Default unit) and UC7 (Explicit target unit).
    /// </summary>
    [TestClass]
    public class QuantityArithmeticTests
    {
        private const double Tolerance = 0.000001;

        /// <summary>
        /// Tests UC6: Same unit addition.
        /// </summary>
        [TestMethod]
        public void Add_SameUnit_ReturnsCorrectSum()
        {
            // Arrange
            var firstFeetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var secondFeetQuantity = new Quantity(2.0, LengthUnit.FEET);

            // Act
            var sumQuantity = firstFeetQuantity.Add(secondFeetQuantity);

            // Assert
            Assert.AreEqual(3.0, sumQuantity.Value, Tolerance, "1 ft + 2 ft should equal 3 ft");
            Assert.AreEqual(LengthUnit.FEET, sumQuantity.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests UC6: Cross-unit addition with result in first unit.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_ResultInFirstUnit_ReturnsCorrectSum()
        {
            // Arrange
            var feetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var inchesQuantity = new Quantity(12.0, LengthUnit.INCH);

            // Act
            var sumQuantity = feetQuantity.Add(inchesQuantity);

            // Assert
            Assert.AreEqual(2.0, sumQuantity.Value, Tolerance, "1 ft + 12 in should equal 2 ft");
            Assert.AreEqual(LengthUnit.FEET, sumQuantity.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests UC6: Cross-unit addition with result in second unit.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_ResultInSecondUnit_ReturnsCorrectSum()
        {
            // Arrange
            var feetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var inchesQuantity = new Quantity(12.0, LengthUnit.INCH);

            // Act
            var sumQuantity = feetQuantity.Add(inchesQuantity, LengthUnit.INCH);

            // Assert
            Assert.AreEqual(
                24.0,
                sumQuantity.Value,
                Tolerance,
                "1 ft + 12 in in inches should equal 24 in"
            );
            Assert.AreEqual(LengthUnit.INCH, sumQuantity.Unit, "Result should be in inches");
        }

        /// <summary>
        /// Tests UC7: Addition with explicit target unit (yards).
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetYards_ReturnsCorrectSum()
        {
            // Arrange
            var feetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var inchesQuantity = new Quantity(12.0, LengthUnit.INCH);

            // Act
            var sumInYards = feetQuantity.Add(inchesQuantity, LengthUnit.YARD);

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
        /// Tests UC7: Addition with explicit target unit (centimeters).
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetCentimeters_ReturnsCorrectSum()
        {
            // Arrange
            var firstInchQuantity = new Quantity(1.0, LengthUnit.INCH);
            var secondInchQuantity = new Quantity(1.0, LengthUnit.INCH);

            // Act
            var sumInCentimeters = firstInchQuantity.Add(secondInchQuantity, LengthUnit.CENTIMETER);

            // Assert
            Assert.AreEqual(
                5.08,
                sumInCentimeters.Value,
                Tolerance,
                "1 in + 1 in in cm should equal 5.08 cm"
            );
            Assert.AreEqual(
                LengthUnit.CENTIMETER,
                sumInCentimeters.Unit,
                "Result should be in centimeters"
            );
        }

        /// <summary>
        /// Tests addition with zero.
        /// </summary>
        [TestMethod]
        public void Add_WithZero_ReturnsOriginalValue()
        {
            // Arrange
            var originalQuantity = new Quantity(5.0, LengthUnit.FEET);
            var zeroQuantity = new Quantity(0.0, LengthUnit.INCH);

            // Act
            var sumQuantity = originalQuantity.Add(zeroQuantity);

            // Assert
            Assert.AreEqual(5.0, sumQuantity.Value, Tolerance, "5 ft + 0 in should equal 5 ft");
        }

        /// <summary>
        /// Tests addition with negative values.
        /// </summary>
        [TestMethod]
        public void Add_WithNegativeValues_ReturnsCorrectSum()
        {
            // Arrange
            var positiveQuantity = new Quantity(5.0, LengthUnit.FEET);
            var negativeQuantity = new Quantity(-2.0, LengthUnit.FEET);

            // Act
            var sumQuantity = positiveQuantity.Add(negativeQuantity);

            // Assert
            Assert.AreEqual(3.0, sumQuantity.Value, Tolerance, "5 ft + (-2 ft) should equal 3 ft");
        }

        /// <summary>
        /// Tests commutativity property: a + b should equal b + a.
        /// </summary>
        [TestMethod]
        public void Add_IsCommutative_ReturnsTrue()
        {
            // Arrange
            var firstQuantity = new Quantity(1.0, LengthUnit.FEET);
            var secondQuantity = new Quantity(12.0, LengthUnit.INCH);
            var targetUnit = LengthUnit.YARD;

            // Act
            var firstSum = firstQuantity.Add(secondQuantity, targetUnit);
            var secondSum = secondQuantity.Add(firstQuantity, targetUnit);

            // Assert
            Assert.AreEqual(
                firstSum.Value,
                secondSum.Value,
                Tolerance,
                "a + b should equal b + a when using same target unit"
            );
        }

        /// <summary>
        /// Tests that adding null throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullOperand_ThrowsException()
        {
            // Arrange
            var validQuantity = new Quantity(1.0, LengthUnit.FEET);

            // Act - Should throw
            validQuantity.Add(null!);
        }
    }
}
