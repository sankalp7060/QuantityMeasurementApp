using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.TestHelpers
{
    /// <summary>
    /// Extension methods for assertions with GenericQuantity.
    /// </summary>
    public static class AssertExtensions
    {
        private const double DefaultTolerance = 0.000001;

        /// <summary>
        /// Asserts that two generic quantities are approximately equal.
        /// </summary>
        /// <typeparam name="T">The unit type (must implement IMeasurable).</typeparam>
        /// <param name="expected">The expected quantity.</param>
        /// <param name="actual">The actual quantity.</param>
        /// <param name="tolerance">The tolerance for comparison.</param>
        public static void AreApproximatelyEqual<T>(
            GenericQuantity<T> expected,
            GenericQuantity<T> actual,
            double tolerance = DefaultTolerance
        )
            where T : class, IMeasurable
        {
            // Convert both to base unit of expected
            var expectedInBase = expected.ConvertTo(expected.Unit);
            var actualInBase = actual.ConvertTo(expected.Unit);

            Assert.AreEqual(
                expectedInBase.Value,
                actualInBase.Value,
                tolerance,
                $"Expected {expected}, but got {actual}"
            );
        }

        /// <summary>
        /// Asserts that a generic quantity is approximately equal to an expected value.
        /// </summary>
        /// <typeparam name="T">The unit type (must implement IMeasurable).</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual quantity.</param>
        /// <param name="tolerance">The tolerance for comparison.</param>
        public static void AreApproximatelyEqual<T>(
            double expected,
            GenericQuantity<T> actual,
            double tolerance = DefaultTolerance
        )
            where T : class, IMeasurable
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
        /// <param name="actual">The actual value.</param>
        /// <param name="expected">The expected value.</param>
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
