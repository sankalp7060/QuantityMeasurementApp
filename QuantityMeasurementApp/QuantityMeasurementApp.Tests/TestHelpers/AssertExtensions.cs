using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.TestHelpers
{
    /// <summary>
    /// Extension methods for assertions.
    /// </summary>
    public static class AssertExtensions
    {
        private const double DefaultTolerance = 0.000001;

        /// <summary>
        /// Asserts that two quantities are approximately equal.
        /// </summary>
        /// <param name="expected">The expected quantity.</param>
        /// <param name="actual">The actual quantity.</param>
        /// <param name="tolerance">The tolerance for comparison.</param>
        public static void AreApproximatelyEqual(
            Quantity expected,
            Quantity actual,
            double tolerance = DefaultTolerance
        )
        {
            Quantity expectedInBase = expected.ConvertTo(LengthUnit.FEET);
            Quantity actualInBase = actual.ConvertTo(LengthUnit.FEET);

            Assert.AreEqual(
                expectedInBase.Value,
                actualInBase.Value,
                tolerance,
                $"Expected {expected}, but got {actual}"
            );
        }

        /// <summary>
        /// Asserts that a quantity is approximately equal to an expected value.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual quantity.</param>
        /// <param name="tolerance">The tolerance for comparison.</param>
        public static void AreApproximatelyEqual(
            double expected,
            Quantity actual,
            double tolerance = DefaultTolerance
        )
        {
            Assert.AreEqual(
                expected,
                actual.Value,
                tolerance,
                $"Expected {expected} {actual.Unit.GetSymbol()}, but got {actual.Value}"
            );
        }

        /// <summary>
        /// Asserts that two double values are approximately equal.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="tolerance">The tolerance for comparison.</param>
        public static void ShouldBeApproximately(
            this double actual,
            double expected,
            double tolerance = DefaultTolerance
        )
        {
            Assert.AreEqual(expected, actual, tolerance, $"Expected {expected}, but got {actual}");
        }
    }
}
