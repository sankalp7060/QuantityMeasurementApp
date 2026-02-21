using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Test class for Quantity conversion edge cases and special scenarios.
    /// Tests boundary conditions, extreme values, and error handling.
    /// </summary>
    [TestClass]
    public class QuantityConversionEdgeCasesTests
    {
        private const double Tolerance = 0.000001;

        #region Extreme Value Tests

        /// <summary>
        /// Tests conversion of extremely large values.
        /// Verifies that conversion handles double.MaxValue gracefully.
        /// </summary>
        [TestMethod]
        public void Convert_ExtremelyLargeValue_HandlesCorrectly()
        {
            double largeValue = double.MaxValue / 1000; // Avoid overflow

            double result = Quantity.Convert(largeValue, LengthUnit.FEET, LengthUnit.INCH);
            double expected = largeValue * 12.0;

            // Check if result is finite (not overflow)
            Assert.IsFalse(double.IsInfinity(result), "Result should not be infinite");
            Assert.IsFalse(double.IsNaN(result), "Result should not be NaN");
        }

        /// <summary>
        /// Tests conversion of extremely small values.
        /// Verifies that conversion handles double.Epsilon gracefully.
        /// </summary>
        [TestMethod]
        public void Convert_ExtremelySmallValue_HandlesCorrectly()
        {
            double smallValue = double.Epsilon;

            double result = Quantity.Convert(smallValue, LengthUnit.FEET, LengthUnit.INCH);

            Assert.IsFalse(double.IsInfinity(result), "Result should not be infinite");
            Assert.IsFalse(double.IsNaN(result), "Result should not be NaN");
        }

        #endregion

        #region Unit Boundary Tests

        /// <summary>
        /// Tests conversion between all possible unit combinations.
        /// Verifies that no combination throws unexpected exceptions.
        /// </summary>
        [TestMethod]
        public void Convert_AllUnitCombinations_NoExceptions()
        {
            LengthUnit[] units =
            {
                LengthUnit.FEET,
                LengthUnit.INCH,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER,
            };
            double value = 1.0;

            foreach (LengthUnit source in units)
            {
                foreach (LengthUnit target in units)
                {
                    try
                    {
                        double result = Quantity.Convert(value, source, target);

                        // Basic sanity check
                        Assert.IsFalse(
                            double.IsNaN(result),
                            $"Conversion from {source} to {target} produced NaN"
                        );
                        Assert.IsFalse(
                            double.IsInfinity(result),
                            $"Conversion from {source} to {target} produced infinity"
                        );
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(
                            $"Conversion from {source} to {target} threw exception: {ex.Message}"
                        );
                    }
                }
            }
        }

        #endregion

        #region Consistency Tests

        /// <summary>
        /// Tests that conversion is consistent across different instances.
        /// Verifies that multiple conversions of same value yield same result.
        /// </summary>
        [TestMethod]
        public void Convert_ConsistentResults_MultipleCalls()
        {
            double value = 2.5;

            double result1 = Quantity.Convert(value, LengthUnit.YARD, LengthUnit.INCH);
            double result2 = Quantity.Convert(value, LengthUnit.YARD, LengthUnit.INCH);
            double result3 = Quantity.Convert(value, LengthUnit.YARD, LengthUnit.INCH);

            Assert.AreEqual(
                result1,
                result2,
                Tolerance,
                "First and second conversions should match"
            );
            Assert.AreEqual(
                result2,
                result3,
                Tolerance,
                "Second and third conversions should match"
            );
        }

        /// <summary>
        /// Tests that conversion through base unit matches direct conversion.
        /// Verifies that multi-step conversion is equivalent to direct conversion.
        /// </summary>
        [TestMethod]
        public void Convert_ThroughBaseUnit_MatchesDirect()
        {
            double value = 3.0;

            // Direct: Yards to Inches
            double direct = Quantity.Convert(value, LengthUnit.YARD, LengthUnit.INCH);

            // Through base (feet)
            double toFeet = Quantity.Convert(value, LengthUnit.YARD, LengthUnit.FEET);
            double throughBase = Quantity.Convert(toFeet, LengthUnit.FEET, LengthUnit.INCH);

            Assert.AreEqual(
                direct,
                throughBase,
                Tolerance,
                "Direct conversion should match conversion through base unit"
            );
        }

        #endregion

        #region Validation Tests

        /// <summary>
        /// Tests that undefined enum values are properly rejected.
        /// Verifies that casting invalid integers to enum is caught.
        /// </summary>
        [TestMethod]
        public void Convert_UndefinedEnumValue_ThrowsArgumentException()
        {
            // Test with various invalid enum values
            int[] invalidValues = { -1, 4, 5, 10, 100 };

            foreach (int invalid in invalidValues)
            {
                LengthUnit invalidUnit = (LengthUnit)invalid;

                Assert.ThrowsException<ArgumentException>(
                    () => Quantity.Convert(1.0, invalidUnit, LengthUnit.FEET),
                    $"Invalid source unit {invalid} should throw"
                );

                Assert.ThrowsException<ArgumentException>(
                    () => Quantity.Convert(1.0, LengthUnit.FEET, invalidUnit),
                    $"Invalid target unit {invalid} should throw"
                );
            }
        }

        #endregion

        #region Mathematical Property Tests

        /// <summary>
        /// Tests that conversion is linear (proportional).
        /// Verifies that convert(k*x) = k*convert(x).
        /// </summary>
        [TestMethod]
        public void Convert_IsLinear_Proportional()
        {
            double x = 2.0;
            double k = 3.0;

            // convert(k*x)
            double scaled = Quantity.Convert(k * x, LengthUnit.FEET, LengthUnit.INCH);

            // k * convert(x)
            double kTimesConverted = k * Quantity.Convert(x, LengthUnit.FEET, LengthUnit.INCH);

            Assert.AreEqual(
                scaled,
                kTimesConverted,
                Tolerance,
                "Conversion should be linear: convert(k*x) = k*convert(x)"
            );
        }

        /// <summary>
        /// Tests that conversion is additive.
        /// Verifies that convert(a+b) = convert(a) + convert(b).
        /// </summary>
        [TestMethod]
        public void Convert_IsAdditive_SumPreserved()
        {
            double a = 1.5;
            double b = 2.5;

            // convert(a+b)
            double sumConverted = Quantity.Convert(a + b, LengthUnit.FEET, LengthUnit.INCH);

            // convert(a) + convert(b)
            double individualSum =
                Quantity.Convert(a, LengthUnit.FEET, LengthUnit.INCH)
                + Quantity.Convert(b, LengthUnit.FEET, LengthUnit.INCH);

            Assert.AreEqual(
                sumConverted,
                individualSum,
                Tolerance,
                "Conversion should be additive: convert(a+b) = convert(a) + convert(b)"
            );
        }

        #endregion

        #region Zero and Sign Tests

        /// <summary>
        /// Tests conversion of positive zero and negative zero.
        /// Verifies that -0.0 converts to -0.0 in target unit.
        /// </summary>
        [TestMethod]
        public void Convert_NegativeZero_ReturnsNegativeZero()
        {
            double negativeZero = -0.0;

            double result = Quantity.Convert(negativeZero, LengthUnit.FEET, LengthUnit.INCH);

            // Check that result is negative zero
            Assert.IsTrue(
                double.IsNegativeInfinity(1.0 / result),
                "Negative zero should remain negative zero after conversion"
            );
        }

        #endregion

        #region Rounding Tests

        /// <summary>
        /// Tests conversion with values that might cause rounding issues.
        /// Verifies that rounding is handled consistently.
        /// </summary>
        [TestMethod]
        public void Convert_RoundingBehavior_Consistent()
        {
            // Test values that might cause binary floating-point rounding issues
            double[] testValues = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 / 3.0 };

            foreach (double value in testValues)
            {
                double result = Quantity.Convert(value, LengthUnit.FEET, LengthUnit.INCH);
                double expected = value * 12.0;

                // Should be within tolerance, even with rounding issues
                Assert.AreEqual(
                    expected,
                    result,
                    Tolerance,
                    $"Conversion of {value} feet to inches should be accurate"
                );
            }
        }

        #endregion
    }
}
