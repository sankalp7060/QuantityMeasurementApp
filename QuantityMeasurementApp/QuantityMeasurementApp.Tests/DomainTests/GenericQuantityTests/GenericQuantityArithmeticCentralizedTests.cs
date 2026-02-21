using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.GenericQuantityTests
{
    /// <summary>
    /// Test class for centralized arithmetic operations in GenericQuantity.
    /// UC13: Tests that arithmetic operations delegate to centralized helpers and maintain consistency.
    /// </summary>
    [TestClass]
    public class GenericQuantityArithmeticCentralizedTests
    {
        private const double Tolerance = 0.000001;
        private const double RoundedTolerance = 0.01; // For rounded values (2 decimal places)
        #region Validation Consistency Tests

        [TestMethod]
        public void AllOperations_NullOperand_ThrowsArgumentNullException()
        {
            // Arrange
            var quantity = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);

            // Act & Assert
            var addException = Assert.ThrowsException<ArgumentNullException>(() =>
                quantity.Add(null!)
            );
            var subtractException = Assert.ThrowsException<ArgumentNullException>(() =>
                quantity.Subtract(null!)
            );
            var divideException = Assert.ThrowsException<ArgumentNullException>(() =>
                quantity.Divide(null!)
            );

            // Verify consistent error messages
            StringAssert.Contains(addException.Message, "Other quantity cannot be null");
            StringAssert.Contains(subtractException.Message, "Other quantity cannot be null");
            StringAssert.Contains(divideException.Message, "Other quantity cannot be null");
        }

        [TestMethod]
        public void AllOperations_InvalidValue_ThrowsInvalidValueException()
        {
            // We need to create quantities with invalid values - this should throw in constructor
            Assert.ThrowsException<InvalidValueException>(() =>
                new GenericQuantity<LengthUnit>(double.NaN, LengthUnit.FEET)
            );
            Assert.ThrowsException<InvalidValueException>(() =>
                new GenericQuantity<LengthUnit>(double.PositiveInfinity, LengthUnit.FEET)
            );
        }

        #endregion

        #region Addition Operation Tests

        [TestMethod]
        public void Add_SameUnit_FeetPlusFeet_ReturnsCorrectSum()
        {
            // Arrange
            var firstQuantity = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var secondQuantity = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);

            // Act
            var sum = firstQuantity.Add(secondQuantity);

            // Assert
            Assert.AreEqual(3.0, sum.Value, Tolerance, "1 ft + 2 ft should equal 3 ft");
            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
        }

        [TestMethod]
        public void Add_CrossUnit_FeetPlusInches_ReturnsCorrectSum()
        {
            // Arrange
            var feetQuantity = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesQuantity = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            var sum = feetQuantity.Add(inchesQuantity);

            // Assert
            Assert.AreEqual(2.0, sum.Value, Tolerance, "1 ft + 12 in should equal 2 ft");
            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
        }

        [TestMethod]
        public void Add_ExplicitTarget_Yards_ReturnsCorrectSum()
        {
            // Arrange
            var feetQuantity = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesQuantity = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            var sum = feetQuantity.Add(inchesQuantity, LengthUnit.YARD);

            // Assert
            double expected = 0.67; // 2 feet = 2/3 yards rounded to 2 decimal places
            Assert.AreEqual(expected, sum.Value, RoundedTolerance);
            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
        }

        #endregion

        #region Subtraction Operation Tests

        [TestMethod]
        public void Subtract_SameUnit_FeetMinusFeet_ReturnsCorrectDifference()
        {
            // Arrange
            var firstQuantity = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var secondQuantity = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);

            // Act
            var difference = firstQuantity.Subtract(secondQuantity);

            // Assert
            Assert.AreEqual(5.0, difference.Value, Tolerance, "10 ft - 5 ft should equal 5 ft");
            Assert.AreEqual(LengthUnit.FEET, difference.Unit);
        }

        [TestMethod]
        public void Subtract_CrossUnit_FeetMinusInches_ReturnsCorrectDifference()
        {
            // Arrange
            var feetQuantity = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var inchesQuantity = new GenericQuantity<LengthUnit>(6.0, LengthUnit.INCH);

            // Act
            var difference = feetQuantity.Subtract(inchesQuantity);

            // Assert
            Assert.AreEqual(9.5, difference.Value, Tolerance, "10 ft - 6 in should equal 9.5 ft");
            Assert.AreEqual(LengthUnit.FEET, difference.Unit);
        }

        [TestMethod]
        public void Subtract_ExplicitTarget_Inches_ReturnsCorrectDifference()
        {
            // Arrange
            var feetQuantity = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var inchesQuantity = new GenericQuantity<LengthUnit>(6.0, LengthUnit.INCH);

            // Act
            var difference = feetQuantity.Subtract(inchesQuantity, LengthUnit.INCH);

            // Assert
            Assert.AreEqual(
                114.0,
                difference.Value,
                Tolerance,
                "10 ft - 6 in in inches should equal 114 in"
            );
            Assert.AreEqual(LengthUnit.INCH, difference.Unit);
        }

        [TestMethod]
        public void Subtract_NotCommutative_ReturnsDifferentResults()
        {
            // Arrange
            var a = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);

            // Act
            var aMinusB = a.Subtract(b);
            var bMinusA = b.Subtract(a);

            // Assert
            Assert.AreEqual(5.0, aMinusB.Value);
            Assert.AreEqual(-5.0, bMinusA.Value);
            Assert.AreNotEqual(aMinusB.Value, bMinusA.Value);
        }

        #endregion

        #region Division Operation Tests

        [TestMethod]
        public void Divide_SameUnit_FeetDividedByFeet_ReturnsCorrectRatio()
        {
            // Arrange
            var firstQuantity = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var secondQuantity = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);

            // Act
            double ratio = firstQuantity.Divide(secondQuantity);

            // Assert
            Assert.AreEqual(5.0, ratio, Tolerance, "10 ft ÷ 2 ft should equal 5.0");
        }

        [TestMethod]
        public void Divide_CrossUnit_FeetDividedByInches_ReturnsCorrectRatio()
        {
            // Arrange
            var feetQuantity = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);
            var inchesQuantity = new GenericQuantity<LengthUnit>(24.0, LengthUnit.INCH);

            // Act
            double ratio = feetQuantity.Divide(inchesQuantity);

            // Assert
            Assert.AreEqual(1.0, ratio, Tolerance, "2 ft ÷ 24 in should equal 1.0");
        }

        [TestMethod]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            // Arrange
            var validQuantity = new GenericQuantity<LengthUnit>(10.0, LengthUnit.FEET);
            var zeroQuantity = new GenericQuantity<LengthUnit>(0.0, LengthUnit.FEET);

            // Act & Assert
            Assert.ThrowsException<DivideByZeroException>(() => validQuantity.Divide(zeroQuantity));
        }

        #endregion

        #region Rounding Tests

        [TestMethod]
        public void Add_Rounding_TwoDecimalPlaces()
        {
            // Arrange
            var quantity1 = new GenericQuantity<LengthUnit>(1.234, LengthUnit.FEET);
            var quantity2 = new GenericQuantity<LengthUnit>(2.345, LengthUnit.FEET);

            // Act
            var sum = quantity1.Add(quantity2);

            // Assert
            Assert.AreEqual(3.58, sum.Value, 0.01); // 1.234 + 2.345 = 3.579 -> rounded to 3.58
        }

        [TestMethod]
        public void Subtract_Rounding_TwoDecimalPlaces()
        {
            // Arrange
            var quantity1 = new GenericQuantity<LengthUnit>(5.678, LengthUnit.FEET);
            var quantity2 = new GenericQuantity<LengthUnit>(2.345, LengthUnit.FEET);

            // Act
            var difference = quantity1.Subtract(quantity2);

            // Assert
            Assert.AreEqual(3.33, difference.Value, 0.01); // 5.678 - 2.345 = 3.333 -> rounded to 3.33
        }

        [TestMethod]
        public void Divide_NoRounding_RawValue()
        {
            // Arrange
            var quantity1 = new GenericQuantity<LengthUnit>(5.0, LengthUnit.FEET);
            var quantity2 = new GenericQuantity<LengthUnit>(3.0, LengthUnit.FEET);

            // Act
            double ratio = quantity1.Divide(quantity2);

            // Assert
            Assert.AreEqual(1.6666666666666667, ratio, 0.000001); // Should be full precision
        }

        #endregion
    }
}
