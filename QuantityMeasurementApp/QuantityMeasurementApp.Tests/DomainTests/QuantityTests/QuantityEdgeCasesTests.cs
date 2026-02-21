using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.QuantityTests
{
    /// <summary>
    /// Test class for Quantity edge cases and error handling.
    /// </summary>
    [TestClass]
    public class QuantityEdgeCasesTests
    {
        private const double Tolerance = 0.000001;

        /// <summary>
        /// Tests conversion with very large values.
        /// </summary>
        [TestMethod]
        public void ConvertTo_LargeValues_MaintainsPrecision()
        {
            // Arrange
            double largeValue = 1000000.0;
            var largeQuantity = new Quantity(largeValue, LengthUnit.FEET);

            // Act
            var convertedQuantity = largeQuantity.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(
                12000000.0,
                convertedQuantity.Value,
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
            var smallQuantity = new Quantity(smallValue, LengthUnit.FEET);

            // Act
            var convertedQuantity = smallQuantity.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(
                0.000012,
                convertedQuantity.Value,
                Tolerance,
                "0.000001 feet should equal 0.000012 inches"
            );
        }

        /// <summary>
        /// Tests addition with very large values.
        /// </summary>
        [TestMethod]
        public void Add_LargeValues_HandlesCorrectly()
        {
            // Arrange
            double nearMaxValue = double.MaxValue / 4.0;
            var firstLargeQuantity = new Quantity(nearMaxValue, LengthUnit.FEET);
            var secondLargeQuantity = new Quantity(nearMaxValue, LengthUnit.FEET);

            // Act
            var sumQuantity = firstLargeQuantity.Add(secondLargeQuantity, LengthUnit.YARD);

            // Assert
            Assert.IsFalse(double.IsInfinity(sumQuantity.Value), "Result should not be infinite");
            Assert.IsFalse(double.IsNaN(sumQuantity.Value), "Result should not be NaN");
        }

        /// <summary>
        /// Tests addition with values that have repeating decimals.
        /// </summary>
        [TestMethod]
        public void Add_RepeatingDecimals_MaintainsAccuracy()
        {
            // Arrange
            var oneThirdFootQuantity = new Quantity(1.0 / 3.0, LengthUnit.FEET); // 4 inches
            var fourInchesQuantity = new Quantity(4.0, LengthUnit.INCH);

            // Act
            var sumInInches = oneThirdFootQuantity.Add(fourInchesQuantity, LengthUnit.INCH);

            // Assert
            Assert.AreEqual(8.0, sumInInches.Value, Tolerance, "1/3 ft + 4 in should equal 8 in");
        }

        /// <summary>
        /// Tests that invalid value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void Constructor_NaNValue_ThrowsException()
        {
            // Act - Should throw
            var invalidQuantity = new Quantity(double.NaN, LengthUnit.FEET);
        }

        /// <summary>
        /// Tests that infinite value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void Constructor_InfinityValue_ThrowsException()
        {
            // Act - Should throw
            var invalidQuantity = new Quantity(double.PositiveInfinity, LengthUnit.FEET);
        }

        /// <summary>
        /// Tests that invalid unit throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidUnitException))]
        public void Constructor_InvalidUnit_ThrowsException()
        {
            // Arrange
            LengthUnit invalidUnit = (LengthUnit)99;

            // Act - Should throw
            var invalidQuantity = new Quantity(1.0, invalidUnit);
        }
    }
}
