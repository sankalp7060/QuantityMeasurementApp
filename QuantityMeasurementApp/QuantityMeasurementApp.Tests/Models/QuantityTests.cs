using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Contains unit tests for Quantity class validating equality logic,
    /// unit conversions, hash code consistency, and string formatting behavior.
    /// </summary>
    [TestClass]
    public class QuantityTests
    {
        // Tests Quantity.Equals(object) for same unit and same value
        [TestMethod]
        public void Equals_SameUnitSameValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 ft should equal 1.0 ft");
        }

        // Tests Quantity.Equals(object) for same unit with different values
        [TestMethod]
        public void Equals_SameUnitDifferentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(2.0, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 ft should not equal 2.0 ft");
        }

        // Tests Quantity.Equals(object) for Inch to Inch same value
        [TestMethod]
        public void Equals_InchToInchSameValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.INCH);
            var q2 = new Quantity(1.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 in should equal 1.0 in");
        }

        // Tests Quantity.Equals(object) for Inch to Inch different value
        [TestMethod]
        public void Equals_InchToInchDifferentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.INCH);
            var q2 = new Quantity(2.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 in should not equal 2.0 in");
        }

        // Tests Quantity.Equals(object) for Feet to Inch equivalent values
        [TestMethod]
        public void Equals_FeetToInchEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 ft should equal 12.0 in");
        }

        // Tests Quantity.Equals(object) for Inch to Feet equivalent values
        [TestMethod]
        public void Equals_InchToFeetEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(12.0, LengthUnit.INCH);
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "12.0 in should equal 1.0 ft");
        }

        // Tests Quantity.Equals(object) for non-equivalent Feet and Inch values
        [TestMethod]
        public void Equals_FeetToInchNonEquivalentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(13.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 ft should not equal 13.0 in");
        }

        // Tests Quantity.Equals(object) for reflexive property
        [TestMethod]
        public void Equals_SameReference_ReturnsTrue()
        {
            var q = new Quantity(1.0, LengthUnit.FEET);
            bool result = q.Equals(q);
            Assert.IsTrue(result, "Object should equal itself");
        }

        // Tests Quantity.Equals(object) for null comparison
        [TestMethod]
        public void Equals_NullComparison_ReturnsFalse()
        {
            var q = new Quantity(1.0, LengthUnit.FEET);
            bool result = q.Equals(null);
            Assert.IsFalse(result, "Object should not equal null");
        }

        // Tests Quantity.Equals(object) for symmetric property
        [TestMethod]
        public void Equals_SymmetricProperty_ReturnsTrue()
        {
            var q1 = new Quantity(1.5, LengthUnit.FEET);
            var q2 = new Quantity(1.5, LengthUnit.FEET);
            bool result1 = q1.Equals(q2);
            bool result2 = q2.Equals(q1);
            Assert.IsTrue(result1 && result2, "Equality should be symmetric");
        }

        // Tests Quantity.Equals(object) for transitive property
        [TestMethod]
        public void Equals_TransitiveProperty_ReturnsTrue()
        {
            var qA = new Quantity(2.5, LengthUnit.FEET);
            var qB = new Quantity(2.5, LengthUnit.FEET);
            var qC = new Quantity(2.5, LengthUnit.FEET);
            bool aEqualsB = qA.Equals(qB);
            bool bEqualsC = qB.Equals(qC);
            bool aEqualsC = qA.Equals(qC);
            Assert.IsTrue(aEqualsB && bEqualsC && aEqualsC, "Equality should be transitive");
        }

        // Tests Quantity.Equals(object) for different type comparison
        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var q = new Quantity(1.0, LengthUnit.FEET);
            var obj = new object();
            bool result = q.Equals(obj);
            Assert.IsFalse(result, "Quantity should not equal object of different type");
        }

        // Tests Quantity.Equals(object) for consistent repeated calls
        [TestMethod]
        public void Equals_ConsistentProperty_ReturnsTrue()
        {
            var q1 = new Quantity(3.0, LengthUnit.FEET);
            var q2 = new Quantity(3.0, LengthUnit.FEET);
            bool result1 = q1.Equals(q2);
            bool result2 = q1.Equals(q2);
            bool result3 = q1.Equals(q2);
            Assert.IsTrue(
                result1 && result2 && result3,
                "Multiple calls should return consistent results"
            );
        }

        // Tests Quantity.Equals(object) for floating point precision handling
        [TestMethod]
        public void Equals_FloatingPointPrecision_HandlesCorrectly()
        {
            var q1 = new Quantity(1.000001, LengthUnit.FEET);
            var q2 = new Quantity(1.000002, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "Even very close values should be considered different");
        }

        // Tests Quantity.GetHashCode() for equal objects
        [TestMethod]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode()
        {
            var q1 = new Quantity(5.0, LengthUnit.FEET);
            var q2 = new Quantity(5.0, LengthUnit.FEET);
            int hash1 = q1.GetHashCode();
            int hash2 = q2.GetHashCode();
            Assert.AreEqual(hash1, hash2);
        }

        // Tests Quantity.GetHashCode() for different objects
        [TestMethod]
        public void GetHashCode_DifferentObjects_ReturnsDifferentHashCode()
        {
            var q1 = new Quantity(5.0, LengthUnit.FEET);
            var q2 = new Quantity(6.0, LengthUnit.FEET);
            int hash1 = q1.GetHashCode();
            int hash2 = q2.GetHashCode();
            Assert.AreNotEqual(hash1, hash2);
        }

        // Tests Quantity.GetHashCode() for equivalent cross-unit objects
        [TestMethod]
        public void GetHashCode_EquivalentCrossUnitObjects_ReturnsSameHashCode()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);
            int hash1 = q1.GetHashCode();
            int hash2 = q2.GetHashCode();
            Assert.AreEqual(hash1, hash2, "Equivalent quantities should have equal hash codes");
        }

        // Tests Quantity.ToString() for Feet formatting
        [TestMethod]
        public void ToString_FeetUnit_ReturnsFormattedString()
        {
            var q = new Quantity(7.5, LengthUnit.FEET);
            string result = q.ToString();
            Assert.AreEqual("7.5 ft", result);
        }

        // Tests Quantity.ToString() for Inch formatting
        [TestMethod]
        public void ToString_InchUnit_ReturnsFormattedString()
        {
            var q = new Quantity(7.5, LengthUnit.INCH);
            string result = q.ToString();
            Assert.AreEqual("7.5 in", result);
        }
    }
}
