using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Test class for Quantity addition edge cases and special scenarios.
    /// Tests boundary conditions, extreme values, and error handling.
    /// </summary>
    [TestClass]
    public class QuantityAdditionEdgeCasesTests
    {
        private const double Tolerance = 0.000001;

        #region Extreme Value Tests

        /// <summary>
        /// Tests addition with values approaching double.MaxValue.
        /// Verifies that addition handles near-maximum values without overflow.
        /// </summary>
        [TestMethod]
        public void Add_NearMaxValue_HandlesCorrectly()
        {
            double nearMax = double.MaxValue / 2.0;

            var q1 = new Quantity(nearMax, LengthUnit.FEET);
            var q2 = new Quantity(nearMax, LengthUnit.FEET);

            var result = q1.Add(q2);

            Assert.IsFalse(double.IsInfinity(result.Value), "Result should not be infinite");
            Assert.IsFalse(double.IsNaN(result.Value), "Result should not be NaN");
        }

        /// <summary>
        /// Tests addition with very large values that might cause precision loss.
        /// Verifies that relative error is acceptable.
        /// </summary>
        [TestMethod]
        public void Add_LargeValues_PrecisionLossWithinTolerance()
        {
            double largeValue = 1e12;

            var q1 = new Quantity(largeValue, LengthUnit.FEET);
            var q2 = new Quantity(1.0, LengthUnit.INCH);

            var result = q1.Add(q2);

            // Expected: largeValue feet + (1/12) feet
            double expected = largeValue + (1.0 / 12.0);

            // Relative error should be small
            double relativeError = Math.Abs((result.Value - expected) / expected);
            Assert.IsTrue(relativeError < 1e-12, $"Relative error {relativeError} is too large");
        }

        #endregion

        #region Precision Tests

        /// <summary>
        /// Tests addition with values that have repeating decimals.
        /// Verifies that conversion maintains mathematical accuracy.
        /// </summary>
        [TestMethod]
        public void Add_RepeatingDecimals_MaintainsAccuracy()
        {
            // 1/3 foot in inches should be 4 inches
            var q1 = new Quantity(1.0 / 3.0, LengthUnit.FEET);
            var q2 = new Quantity(4.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.INCH);

            // 4 inches + 4 inches = 8 inches
            Assert.AreEqual(8.0, result.Value, Tolerance, "1/3 ft + 4 in should equal 8 in");
        }

        /// <summary>
        /// Tests addition with irrational conversions.
        /// Verifies that precision is maintained within tolerance.
        /// </summary>
        [TestMethod]
        public void Add_IrrationalConversions_MaintainsPrecision()
        {
            // 1 cm in inches is irrational
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.CENTIMETER);

            var result = q1.Add(q2, LengthUnit.INCH);

            // 2 cm should equal 0.7874015748 inches
            double expected = 2.0 * 0.393700787;
            Assert.AreEqual(
                expected,
                result.Value,
                Tolerance,
                "2 cm should convert correctly to inches"
            );
        }

        #endregion

        #region Identity and Zero Tests

        /// <summary>
        /// Tests that adding zero to any quantity returns the same quantity.
        /// Verifies identity property of addition.
        /// </summary>
        [TestMethod]
        public void Add_IdentityProperty_HoldsForAllUnits()
        {
            LengthUnit[] units =
            {
                LengthUnit.FEET,
                LengthUnit.INCH,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER,
            };
            var zero = new Quantity(0.0, LengthUnit.FEET);

            foreach (var unit in units)
            {
                var q = new Quantity(5.0, unit);
                var result = q.Add(zero, unit);

                Assert.AreEqual(
                    5.0,
                    result.Value,
                    Tolerance,
                    $"5 {unit} + 0 should equal 5 {unit}"
                );
                Assert.AreEqual(unit, result.Unit);
            }
        }

        /// <summary>
        /// Tests that adding zero in different units still returns the original value.
        /// </summary>
        [TestMethod]
        public void Add_ZeroInDifferentUnits_ReturnsOriginalValue()
        {
            var q = new Quantity(5.0, LengthUnit.FEET);
            var zeroInInches = new Quantity(0.0, LengthUnit.INCH);
            var zeroInYards = new Quantity(0.0, LengthUnit.YARD);
            var zeroInCm = new Quantity(0.0, LengthUnit.CENTIMETER);

            var result1 = q.Add(zeroInInches);
            var result2 = q.Add(zeroInYards);
            var result3 = q.Add(zeroInCm);

            Assert.AreEqual(
                5.0,
                result1.Value,
                Tolerance,
                "Adding zero inches should not change value"
            );
            Assert.AreEqual(
                5.0,
                result2.Value,
                Tolerance,
                "Adding zero yards should not change value"
            );
            Assert.AreEqual(
                5.0,
                result3.Value,
                Tolerance,
                "Adding zero cm should not change value"
            );
        }

        #endregion

        #region Sign Tests

        /// <summary>
        /// Tests addition of positive and negative values that cancel out.
        /// Verifies that 2 feet + (-24 inches) = 0 feet.
        /// </summary>
        [TestMethod]
        public void Add_PositiveAndNegative_CancelOut()
        {
            var feet = new Quantity(2.0, LengthUnit.FEET);
            var inches = new Quantity(-24.0, LengthUnit.INCH);

            var result = feet.Add(inches);

            Assert.AreEqual(0.0, result.Value, Tolerance, "2 ft + (-24 in) should equal 0 ft");
        }

        /// <summary>
        /// Tests addition where result is negative.
        /// Verifies that 1 foot + (-24 inches) = -1 foot.
        /// </summary>
        [TestMethod]
        public void Add_ResultIsNegative_ReturnsCorrectValue()
        {
            var feet = new Quantity(1.0, LengthUnit.FEET);
            var inches = new Quantity(-24.0, LengthUnit.INCH);

            var result = feet.Add(inches);

            Assert.AreEqual(-1.0, result.Value, Tolerance, "1 ft + (-24 in) should equal -1 ft");
        }

        #endregion

        #region Associativity Tests

        /// <summary>
        /// Tests that addition is associative (a + b) + c = a + (b + c).
        /// Verifies the associative property of addition.
        /// </summary>
        [TestMethod]
        public void Add_IsAssociative_WhenComparedInBaseUnit()
        {
            var a = new Quantity(1.0, LengthUnit.FEET);
            var b = new Quantity(12.0, LengthUnit.INCH);
            var c = new Quantity(0.5, LengthUnit.YARD);

            // (a + b) + c
            var ab = a.Add(b);
            var left = ab.Add(c);

            // a + (b + c)
            var bc = b.Add(c);
            var right = a.Add(bc);

            // Compare in base unit
            var leftInFeet = left.ConvertTo(LengthUnit.FEET);
            var rightInFeet = right.ConvertTo(LengthUnit.FEET);

            Assert.IsTrue(
                leftInFeet.Equals(rightInFeet),
                "Addition should be associative: (a+b)+c = a+(b+c)"
            );
        }

        #endregion

        #region Overflow/Underflow Tests

        /// <summary>
        /// Tests addition of very small numbers that might underflow.
        /// Verifies that underflow is handled gracefully.
        /// </summary>
        [TestMethod]
        public void Add_VerySmallNumbers_HandlesUnderflow()
        {
            double verySmall = double.Epsilon * 100;

            var q1 = new Quantity(verySmall, LengthUnit.FEET);
            var q2 = new Quantity(verySmall, LengthUnit.FEET);

            var result = q1.Add(q2);

            Assert.IsFalse(double.IsInfinity(result.Value), "Result should not be infinite");
            Assert.IsFalse(double.IsNaN(result.Value), "Result should not be NaN");
        }

        #endregion

        #region Rounding Tests

        /// <summary>
        /// Tests addition with values that require rounding.
        /// Verifies that rounding is handled consistently.
        /// </summary>
        [TestMethod]
        public void Add_RoundingBehavior_Consistent()
        {
            var q1 = new Quantity(1.0 / 3.0, LengthUnit.FEET);
            var q2 = new Quantity(1.0 / 3.0, LengthUnit.FEET);
            var q3 = new Quantity(1.0 / 3.0, LengthUnit.FEET);

            var result1 = q1.Add(q2);
            var result2 = result1.Add(q3);

            // 1/3 + 1/3 + 1/3 = 1
            Assert.AreEqual(
                1.0,
                result2.Value,
                Tolerance,
                "1/3 ft + 1/3 ft + 1/3 ft should equal 1 ft"
            );
        }

        #endregion
    }
}
