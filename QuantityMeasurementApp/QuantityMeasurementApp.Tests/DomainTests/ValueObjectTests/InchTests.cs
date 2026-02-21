using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.ValueObjects;

namespace QuantityMeasurementApp.Tests.DomainTests.ValueObjectTests
{
    /// <summary>
    /// Legacy test class for Inch value object (UC2).
    /// </summary>
    [TestClass]
    public class InchTests
    {
        /// <summary>
        /// Tests that two Inch objects with same value are equal.
        /// </summary>
        [TestMethod]
        public void Equals_SameValue_ReturnsTrue()
        {
            // Arrange
            var firstInch = new Inch(1.0);
            var secondInch = new Inch(1.0);

            // Act
            bool areEqual = firstInch.Equals(secondInch);

            // Assert
            Assert.IsTrue(areEqual, "1.0 in should equal 1.0 in");
        }

        /// <summary>
        /// Tests that two Inch objects with different values are not equal.
        /// </summary>
        [TestMethod]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            // Arrange
            var firstInch = new Inch(1.0);
            var secondInch = new Inch(2.0);

            // Act
            bool areEqual = firstInch.Equals(secondInch);

            // Assert
            Assert.IsFalse(areEqual, "1.0 in should not equal 2.0 in");
        }

        /// <summary>
        /// Tests reflexive property.
        /// </summary>
        [TestMethod]
        public void Equals_SameReference_ReturnsTrue()
        {
            // Arrange
            var inch = new Inch(1.0);

            // Act
            bool isEqualToItself = inch.Equals(inch);

            // Assert
            Assert.IsTrue(isEqualToItself, "Object should equal itself");
        }

        /// <summary>
        /// Tests null comparison.
        /// </summary>
        [TestMethod]
        public void Equals_NullComparison_ReturnsFalse()
        {
            // Arrange
            var inch = new Inch(1.0);

            // Act
            bool isEqualToNull = inch.Equals(null);

            // Assert
            Assert.IsFalse(isEqualToNull, "Object should not equal null");
        }
    }
}
