using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.ValueObjects;

namespace QuantityMeasurementApp.Tests.DomainTests.ValueObjectTests
{
    /// <summary>
    /// Legacy test class for Feet value object (UC1).
    /// </summary>
    [TestClass]
    public class FeetTests
    {
        /// <summary>
        /// Tests that two Feet objects with same value are equal.
        /// </summary>
        [TestMethod]
        public void Equals_SameValue_ReturnsTrue()
        {
            // Arrange
            var firstFeet = new Feet(1.0);
            var secondFeet = new Feet(1.0);

            // Act
            bool areEqual = firstFeet.Equals(secondFeet);

            // Assert
            Assert.IsTrue(areEqual, "1.0 ft should equal 1.0 ft");
        }

        /// <summary>
        /// Tests that two Feet objects with different values are not equal.
        /// </summary>
        [TestMethod]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            // Arrange
            var firstFeet = new Feet(1.0);
            var secondFeet = new Feet(2.0);

            // Act
            bool areEqual = firstFeet.Equals(secondFeet);

            // Assert
            Assert.IsFalse(areEqual, "1.0 ft should not equal 2.0 ft");
        }

        /// <summary>
        /// Tests reflexive property.
        /// </summary>
        [TestMethod]
        public void Equals_SameReference_ReturnsTrue()
        {
            // Arrange
            var feet = new Feet(1.0);

            // Act
            bool isEqualToItself = feet.Equals(feet);

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
            var feet = new Feet(1.0);

            // Act
            bool isEqualToNull = feet.Equals(null);

            // Assert
            Assert.IsFalse(isEqualToNull, "Object should not equal null");
        }
    }
}
