using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    // Test class for Feet model
    // Contains unit tests to verify the equality contract and behavior of Feet class
    [TestClass]
    public class FeetTests
    {
        // Test: Same value (1.0 ft and 1.0 ft) should be equal
        // Verifies that two Feet objects with the same value are considered equal
        [TestMethod]
        public void Equals_SameValue_ReturnsTrue()
        {
            // Arrange: Create two Feet objects with the same value
            var feet1 = new Feet(1.0);
            var feet2 = new Feet(1.0);

            // Act: Compare them using Equals method
            bool result = feet1.Equals(feet2);

            // Assert: They should be equal
            Assert.IsTrue(result, "1.0 ft should equal 1.0 ft");
        }

        // Test: Different values (1.0 ft and 2.0 ft) should not be equal
        // Verifies that Feet objects with different values are not equal
        [TestMethod]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            // Arrange: Create two Feet objects with different values
            var feet1 = new Feet(1.0);
            var feet2 = new Feet(2.0);

            // Act: Compare them using Equals method
            bool result = feet1.Equals(feet2);

            // Assert: They should not be equal
            Assert.IsFalse(result, "1.0 ft should not equal 2.0 ft");
        }

        // Test: Reflexive property - object should equal itself
        // Verifies that a Feet object is equal to itself (a.equals(a) must be true)
        [TestMethod]
        public void Equals_SameReference_ReturnsTrue()
        {
            // Arrange: Create a single Feet object
            var feet = new Feet(1.0);

            // Act: Compare the object with itself
            bool result = feet.Equals(feet);

            // Assert: An object should always equal itself
            Assert.IsTrue(result, "Object should equal itself (reflexive property)");
        }

        // Test: Null comparison should return false
        // Verifies that no Feet object equals null (a.equals(null) must be false)
        [TestMethod]
        public void Equals_NullComparison_ReturnsFalse()
        {
            // Arrange: Create a Feet object
            var feet = new Feet(1.0);

            // Act: Compare with null
            bool result = feet.Equals(null);

            // Assert: Object should not equal null
            Assert.IsFalse(result, "Object should not equal null");
        }

        // Test: Symmetric property - if a equals b, then b equals a
        // Verifies that equality is symmetric (a.equals(b) == b.equals(a))
        [TestMethod]
        public void Equals_SymmetricProperty_ReturnsTrue()
        {
            // Arrange: Create two Feet objects with the same value
            var feet1 = new Feet(1.5);
            var feet2 = new Feet(1.5);

            // Act: Compare both ways
            bool result1 = feet1.Equals(feet2);
            bool result2 = feet2.Equals(feet1);

            // Assert: Both comparisons should yield the same result
            Assert.IsTrue(result1 && result2, "Equality should be symmetric");
        }

        // Test: Transitive property - if a equals b and b equals c, then a equals c
        // Verifies that equality is transitive
        [TestMethod]
        public void Equals_TransitiveProperty_ReturnsTrue()
        {
            // Arrange: Create three Feet objects with the same value
            var feetA = new Feet(2.5);
            var feetB = new Feet(2.5);
            var feetC = new Feet(2.5);

            // Act: Check all combinations
            bool aEqualsB = feetA.Equals(feetB);
            bool bEqualsC = feetB.Equals(feetC);
            bool aEqualsC = feetA.Equals(feetC);

            // Assert: If a=b and b=c, then a=c must be true
            Assert.IsTrue(aEqualsB && bEqualsC && aEqualsC, "Equality should be transitive");
        }

        // Test: Different types should not be equal
        // Verifies that Feet objects only equal other Feet objects, not different types
        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Arrange: Create a Feet object and a regular object
            var feet = new Feet(1.0);
            var obj = new object();

            // Act: Compare Feet with a different type
            bool result = feet.Equals(obj);

            // Assert: Different types should not be equal
            Assert.IsFalse(result, "Feet should not equal object of different type");
        }

        // Test: Consistent property - multiple calls return same result
        // Verifies that equality results are consistent over multiple calls
        [TestMethod]
        public void Equals_ConsistentProperty_ReturnsTrue()
        {
            // Arrange: Create two equal Feet objects
            var feet1 = new Feet(3.0);
            var feet2 = new Feet(3.0);

            // Act: Compare them multiple times
            bool result1 = feet1.Equals(feet2);
            bool result2 = feet1.Equals(feet2);
            bool result3 = feet1.Equals(feet2);

            // Assert: All comparisons should yield the same result
            Assert.IsTrue(
                result1 && result2 && result3,
                "Multiple calls should return consistent results"
            );
        }

        // Test: Floating point precision with very close values
        // Verifies that even very close values are considered different
        [TestMethod]
        public void Equals_FloatingPointPrecision_HandlesCorrectly()
        {
            // Arrange: Create two Feet objects with very close but different values
            var feet1 = new Feet(1.000001);
            var feet2 = new Feet(1.000002);

            // Act: Compare them
            bool result = feet1.Equals(feet2);

            // Assert: They should be considered different
            Assert.IsFalse(result, "Even very close values should be considered different");
        }

        // Test: GetHashCode returns same value for equal objects
        // Verifies that equal objects have equal hash codes
        [TestMethod]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode()
        {
            // Arrange: Create two equal Feet objects
            var feet1 = new Feet(5.0);
            var feet2 = new Feet(5.0);

            // Act: Get hash codes
            int hash1 = feet1.GetHashCode();
            int hash2 = feet2.GetHashCode();

            // Assert: Equal objects must have equal hash codes
            Assert.AreEqual(hash1, hash2, "Equal objects should have equal hash codes");
        }

        // Test: GetHashCode returns different values for different objects
        // Verifies that different objects typically have different hash codes
        [TestMethod]
        public void GetHashCode_DifferentObjects_ReturnsDifferentHashCode()
        {
            // Arrange: Create two different Feet objects
            var feet1 = new Feet(5.0);
            var feet2 = new Feet(6.0);

            // Act: Get hash codes
            int hash1 = feet1.GetHashCode();
            int hash2 = feet2.GetHashCode();

            // Assert: Different objects should have different hash codes
            Assert.AreNotEqual(hash1, hash2, "Different objects should have different hash codes");
        }

        // Test: ToString returns formatted string
        // Verifies that ToString returns the expected format
        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            // Arrange: Create a Feet object
            var feet = new Feet(7.5);

            // Act: Get string representation
            string result = feet.ToString();

            // Assert: String should be in expected format
            Assert.AreEqual("7.5 ft", result, "ToString should return value with unit");
        }
    }
}
