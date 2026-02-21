using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Contains unit tests for the Quantity class to verify equality, hash code consistency,
    /// unit conversions between feet and inches, object comparison properties (reflexive,
    /// symmetric, transitive, consistent), null and type safety checks, floating point precision handling,
    /// and string representation formatting.
    /// </summary>
    [TestClass]
    public class QuantityTests
    {
        // Test: Same unit and same value should be equal
        [TestMethod]
        public void Equals_SameUnitSameValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 ft should equal 1.0 ft");
        }

        // Test: Same unit different values should not be equal
        [TestMethod]
        public void Equals_SameUnitDifferentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(2.0, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 ft should not equal 2.0 ft");
        }

        // Test: Inch to Inch same value
        [TestMethod]
        public void Equals_InchToInchSameValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.INCH);
            var q2 = new Quantity(1.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 in should equal 1.0 in");
        }

        // Test: Inch to Inch different value
        [TestMethod]
        public void Equals_InchToInchDifferentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.INCH);
            var q2 = new Quantity(2.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 in should not equal 2.0 in");
        }

        // Test: Feet to Inch equivalent values (1 ft = 12 in)
        [TestMethod]
        public void Equals_FeetToInchEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 ft should equal 12.0 in");
        }

        // Test: Inch to Feet equivalent values (12 in = 1 ft) - Symmetry test
        [TestMethod]
        public void Equals_InchToFeetEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(12.0, LengthUnit.INCH);
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "12.0 in should equal 1.0 ft");
        }

        // Test: Feet to Inch non-equivalent values
        [TestMethod]
        public void Equals_FeetToInchNonEquivalentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(13.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 ft should not equal 13.0 in");
        }

        // Test: Reflexive property
        [TestMethod]
        public void Equals_SameReference_ReturnsTrue()
        {
            var q = new Quantity(1.0, LengthUnit.FEET);
            bool result = q.Equals(q);
            Assert.IsTrue(result, "Object should equal itself");
        }

        // Test: Null comparison
        [TestMethod]
        public void Equals_NullComparison_ReturnsFalse()
        {
            var q = new Quantity(1.0, LengthUnit.FEET);
            bool result = q.Equals(null);
            Assert.IsFalse(result, "Object should not equal null");
        }

        // Test: Symmetric property
        [TestMethod]
        public void Equals_SymmetricProperty_ReturnsTrue()
        {
            var q1 = new Quantity(1.5, LengthUnit.FEET);
            var q2 = new Quantity(1.5, LengthUnit.FEET);
            bool result1 = q1.Equals(q2);
            bool result2 = q2.Equals(q1);
            Assert.IsTrue(result1 && result2, "Equality should be symmetric");
        }

        // Test: Transitive property
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

        // Test: Different types should not be equal
        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var q = new Quantity(1.0, LengthUnit.FEET);
            var obj = new object();
            bool result = q.Equals(obj);
            Assert.IsFalse(result, "Quantity should not equal object of different type");
        }

        // Test: Consistent property
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

        // Test: Floating point precision with very close values
        [TestMethod]
        public void Equals_FloatingPointPrecision_HandlesCorrectly()
        {
            var q1 = new Quantity(1.000001, LengthUnit.FEET);
            var q2 = new Quantity(1.000002, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "Even very close values should be considered different");
        }

        // Test: GetHashCode for equal objects
        [TestMethod]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode()
        {
            var q1 = new Quantity(5.0, LengthUnit.FEET);
            var q2 = new Quantity(5.0, LengthUnit.FEET);
            int hash1 = q1.GetHashCode();
            int hash2 = q2.GetHashCode();
            Assert.AreEqual(hash1, hash2);
        }

        // Test: GetHashCode for different objects
        [TestMethod]
        public void GetHashCode_DifferentObjects_ReturnsDifferentHashCode()
        {
            var q1 = new Quantity(5.0, LengthUnit.FEET);
            var q2 = new Quantity(6.0, LengthUnit.FEET);
            int hash1 = q1.GetHashCode();
            int hash2 = q2.GetHashCode();
            Assert.AreNotEqual(hash1, hash2);
        }

        // Test: GetHashCode for equivalent cross-unit objects
        [TestMethod]
        public void GetHashCode_EquivalentCrossUnitObjects_ReturnsSameHashCode()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);
            int hash1 = q1.GetHashCode();
            int hash2 = q2.GetHashCode();
            Assert.AreEqual(hash1, hash2, "Equivalent quantities should have equal hash codes");
        }

        // Test: ToString returns formatted string for feet
        [TestMethod]
        public void ToString_FeetUnit_ReturnsFormattedString()
        {
            var q = new Quantity(7.5, LengthUnit.FEET);
            string result = q.ToString();
            Assert.AreEqual("7.5 ft", result);
        }

        // Test: ToString returns formatted string for inches
        [TestMethod]
        public void ToString_InchUnit_ReturnsFormattedString()
        {
            var q = new Quantity(7.5, LengthUnit.INCH);
            string result = q.ToString();
            Assert.AreEqual("7.5 in", result);
        }
    }
}
