using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Test class for Quantity addition validation and edge cases with explicit target unit.
    /// Tests boundary conditions, extreme values, and error handling for UC7.
    /// </summary>
    [TestClass]
    public class QuantityAdditionValidationTests
    {
        private const double Tolerance = 0.000001;

        #region Extreme Value Tests

        /// <summary>
        /// Tests addition with values approaching double.MaxValue.
        /// Verifies that addition handles near-maximum values without overflow.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTarget_NearMaxValue_HandlesCorrectly()
        {
            double nearMax = double.MaxValue / 4.0; // Divide by 4 to leave room for addition

            var q1 = new Quantity(nearMax, LengthUnit.FEET);
            var q2 = new Quantity(nearMax, LengthUnit.FEET);

            var result = q1.Add(q2, LengthUnit.YARD);

            Assert.IsFalse(double.IsInfinity(result.Value), "Result should not be infinite");
            Assert.IsFalse(double.IsNaN(result.Value), "Result should not be NaN");
        }

        /// <summary>
        /// Tests addition with extremely small values.
        /// Verifies that underflow is handled gracefully.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTarget_VerySmallValues_HandlesUnderflow()
        {
            double verySmall = double.Epsilon * 1000;

            var q1 = new Quantity(verySmall, LengthUnit.FEET);
            var q2 = new Quantity(verySmall, LengthUnit.FEET);

            var result = q1.Add(q2, LengthUnit.INCH);

            Assert.IsFalse(double.IsInfinity(result.Value), "Result should not be infinite");
            Assert.IsFalse(double.IsNaN(result.Value), "Result should not be NaN");
        }

        #endregion

        #region Unit Conversion Boundary Tests

        /// <summary>
        /// Tests addition where conversion might cause precision loss.
        /// Verifies that results are still within acceptable tolerance.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTarget_UnitConversionBoundary_PreservesPrecision()
        {
            // Test with values that have repeating decimals in conversions
            var q1 = new Quantity(1.0 / 3.0, LengthUnit.FEET); // 4 inches
            var q2 = new Quantity(1.0 / 3.0, LengthUnit.YARD); // 1 foot = 12 inches

            var result = q1.Add(q2, LengthUnit.INCH);

            // 4 inches + 12 inches = 16 inches
            Assert.AreEqual(
                16.0,
                result.Value,
                Tolerance,
                "1/3 ft + 1/3 yd should equal 16 inches"
            );
        }

        #endregion

        #region Cross-Category Unit Validation

        /// <summary>
        /// Note: All units in our enum are length units, so cross-category mixing isn't possible.
        /// This test verifies that all enum values are valid length units.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTarget_AllEnumValues_AreValidLengthUnits()
        {
            LengthUnit[] units = (LengthUnit[])Enum.GetValues(typeof(LengthUnit));

            foreach (var unit in units)
            {
                try
                {
                    var q = new Quantity(1.0, unit);
                    Assert.IsNotNull(q, $"Should create quantity for {unit}");
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Failed for unit {unit}: {ex.Message}");
                }
            }
        }

        #endregion

        #region Mathematical Consistency Tests

        /// <summary>
        /// Tests that addition results are consistent across different target units.
        /// Verifies that converting result to base unit yields same physical quantity.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTarget_ResultsConsistent_WhenConvertedToBaseUnit()
        {
            var q1 = new Quantity(2.0, LengthUnit.YARD);
            var q2 = new Quantity(3.0, LengthUnit.FEET);

            LengthUnit[] targets =
            {
                LengthUnit.FEET,
                LengthUnit.INCH,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER,
            };
            double[] resultsInBaseUnit = new double[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                var result = q1.Add(q2, targets[i]);
                var resultInFeet = result.ConvertTo(LengthUnit.FEET);
                resultsInBaseUnit[i] = resultInFeet.Value;
            }

            // All results should represent the same physical quantity
            for (int i = 1; i < resultsInBaseUnit.Length; i++)
            {
                Assert.AreEqual(
                    resultsInBaseUnit[0],
                    resultsInBaseUnit[i],
                    Tolerance,
                    $"Results in different target units should be equivalent in base unit"
                );
            }
        }

        /// <summary>
        /// Tests that addition is associative with explicit target units.
        /// Verifies (a + b) + c = a + (b + c) when compared in base unit.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTarget_IsAssociative_WhenComparedInBaseUnit()
        {
            var a = new Quantity(1.0, LengthUnit.FEET);
            var b = new Quantity(12.0, LengthUnit.INCH);
            var c = new Quantity(0.5, LengthUnit.YARD);

            // (a + b) + c
            var ab = a.Add(b, LengthUnit.FEET);
            var left = ab.Add(c, LengthUnit.FEET);

            // a + (b + c)
            var bc = b.Add(c, LengthUnit.FEET);
            var right = a.Add(bc, LengthUnit.FEET);

            // Compare in base unit
            Assert.AreEqual(
                left.Value,
                right.Value,
                Tolerance,
                "Addition should be associative: (a+b)+c = a+(b+c)"
            );
        }

        #endregion

        #region Rounding and Precision Tests

        /// <summary>
        /// Tests addition with values that might cause rounding issues in conversion.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTarget_RoundingBehavior_Consistent()
        {
            double[] testValues = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };

            foreach (double val in testValues)
            {
                var q1 = new Quantity(val, LengthUnit.FEET);
                var q2 = new Quantity(val, LengthUnit.INCH);

                var result = q1.Add(q2, LengthUnit.YARD);

                // Just verify result is finite and not NaN
                Assert.IsFalse(
                    double.IsInfinity(result.Value),
                    $"Result should be finite for {val}"
                );
                Assert.IsFalse(double.IsNaN(result.Value), $"Result should not be NaN for {val}");
            }
        }

        #endregion

        #region Large Magnitude Scale Tests

        /// <summary>
        /// Tests addition with large values converted to much smaller units.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTarget_LargeValuesToSmallUnit_MaintainsPrecision()
        {
            double largeValue = 1000000.0;

            var q1 = new Quantity(largeValue, LengthUnit.YARD);
            var q2 = new Quantity(largeValue, LengthUnit.YARD);

            var result = q1.Add(q2, LengthUnit.CENTIMETER);

            // 2,000,000 yards = 182,880,000 cm (approx)
            double expected = 2 * largeValue * 91.44;
            Assert.AreEqual(
                expected,
                result.Value,
                expected * 1e-12,
                "Large value conversion should maintain relative precision"
            );
        }

        #endregion

        #region Invalid Operation Tests

        /// <summary>
        /// Tests that operations with invalid unit combinations throw appropriate exceptions.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTarget_InvalidUnitCombinations_ThrowException()
        {
            // Test with undefined enum value
            LengthUnit invalidUnit = (LengthUnit)99;

            Assert.ThrowsException<ArgumentException>(
                () => new Quantity(1.0, invalidUnit),
                "Creating quantity with invalid unit should throw"
            );

            var q = new Quantity(1.0, LengthUnit.FEET);

            Assert.ThrowsException<ArgumentException>(
                () => q.Add(q, invalidUnit),
                "Adding with invalid target unit should throw"
            );
        }

        #endregion
    }
}
