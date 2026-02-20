using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    [TestClass]
    public class FeetTests
    {
        [TestMethod]
        public void Equals_SameValue_ReturnsTrue()
        {
            var feet1 = new Feet(1.0);
            var feet2 = new Feet(1.0);
            bool result = feet1.Equals(feet2);
            Assert.IsTrue(result, "1.0 ft should equal 1.0 ft");
        }

        [TestMethod]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            var feet1 = new Feet(1.0);
            var feet2 = new Feet(2.0);
            bool result = feet1.Equals(feet2);
            Assert.IsFalse(result, "1.0 ft should not equal 2.0 ft");
        }

        [TestMethod]
        public void Equals_SameReference_ReturnsTrue()
        {
            var feet = new Feet(1.0);
            bool result = feet.Equals(feet);
            Assert.IsTrue(result, "Object should equal itself");
        }

        [TestMethod]
        public void Equals_NullComparison_ReturnsFalse()
        {
            var feet = new Feet(1.0);
            bool result = feet.Equals(null);
            Assert.IsFalse(result, "Object should not equal null");
        }

        [TestMethod]
        public void Equals_SymmetricProperty_ReturnsTrue()
        {
            var feet1 = new Feet(1.5);
            var feet2 = new Feet(1.5);
            bool result1 = feet1.Equals(feet2);
            bool result2 = feet2.Equals(feet1);
            Assert.IsTrue(result1 && result2, "Equality should be symmetric");
        }

        [TestMethod]
        public void Equals_TransitiveProperty_ReturnsTrue()
        {
            var feetA = new Feet(2.5);
            var feetB = new Feet(2.5);
            var feetC = new Feet(2.5);
            bool aEqualsB = feetA.Equals(feetB);
            bool bEqualsC = feetB.Equals(feetC);
            bool aEqualsC = feetA.Equals(feetC);
            Assert.IsTrue(aEqualsB && bEqualsC && aEqualsC, "Equality should be transitive");
        }

        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var feet = new Feet(1.0);
            var obj = new object();
            bool result = feet.Equals(obj);
            Assert.IsFalse(result, "Feet should not equal object of different type");
        }

        [TestMethod]
        public void Equals_ConsistentProperty_ReturnsTrue()
        {
            var feet1 = new Feet(3.0);
            var feet2 = new Feet(3.0);
            bool result1 = feet1.Equals(feet2);
            bool result2 = feet1.Equals(feet2);
            bool result3 = feet1.Equals(feet2);
            Assert.IsTrue(
                result1 && result2 && result3,
                "Multiple calls should return consistent results"
            );
        }

        [TestMethod]
        public void Equals_FloatingPointPrecision_HandlesCorrectly()
        {
            var feet1 = new Feet(1.000001);
            var feet2 = new Feet(1.000002);
            bool result = feet1.Equals(feet2);
            Assert.IsFalse(result, "Even very close values should be considered different");
        }

        [TestMethod]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode()
        {
            var feet1 = new Feet(5.0);
            var feet2 = new Feet(5.0);
            int hash1 = feet1.GetHashCode();
            int hash2 = feet2.GetHashCode();
            Assert.AreEqual(hash1, hash2, "Equal objects should have equal hash codes");
        }

        [TestMethod]
        public void GetHashCode_DifferentObjects_ReturnsDifferentHashCode()
        {
            var feet1 = new Feet(5.0);
            var feet2 = new Feet(6.0);
            int hash1 = feet1.GetHashCode();
            int hash2 = feet2.GetHashCode();
            Assert.AreNotEqual(hash1, hash2, "Different objects should have different hash codes");
        }

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var feet = new Feet(7.5);
            string result = feet.ToString();
            Assert.AreEqual("7.5 ft", result, "ToString should return value with unit");
        }
    }
}
