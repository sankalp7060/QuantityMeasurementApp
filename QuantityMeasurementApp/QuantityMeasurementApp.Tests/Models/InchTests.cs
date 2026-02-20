using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    [TestClass]
    public class InchTests
    {
        [TestMethod]
        public void Equals_SameValue_ReturnsTrue()
        {
            var inch1 = new Inch(1.0);
            var inch2 = new Inch(1.0);
            bool result = inch1.Equals(inch2);
            Assert.IsTrue(result, "1.0 in should equal 1.0 in");
        }

        [TestMethod]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            var inch1 = new Inch(1.0);
            var inch2 = new Inch(2.0);
            bool result = inch1.Equals(inch2);
            Assert.IsFalse(result, "1.0 in should not equal 2.0 in");
        }

        [TestMethod]
        public void Equals_SameReference_ReturnsTrue()
        {
            var inch = new Inch(1.0);
            bool result = inch.Equals(inch);
            Assert.IsTrue(result, "Object should equal itself");
        }

        [TestMethod]
        public void Equals_NullComparison_ReturnsFalse()
        {
            var inch = new Inch(1.0);
            bool result = inch.Equals(null);
            Assert.IsFalse(result, "Object should not equal null");
        }

        [TestMethod]
        public void Equals_SymmetricProperty_ReturnsTrue()
        {
            var inch1 = new Inch(1.5);
            var inch2 = new Inch(1.5);
            bool result1 = inch1.Equals(inch2);
            bool result2 = inch2.Equals(inch1);
            Assert.IsTrue(result1 && result2, "Equality should be symmetric");
        }

        [TestMethod]
        public void Equals_TransitiveProperty_ReturnsTrue()
        {
            var inchA = new Inch(2.5);
            var inchB = new Inch(2.5);
            var inchC = new Inch(2.5);
            bool aEqualsB = inchA.Equals(inchB);
            bool bEqualsC = inchB.Equals(inchC);
            bool aEqualsC = inchA.Equals(inchC);
            Assert.IsTrue(aEqualsB && bEqualsC && aEqualsC, "Equality should be transitive");
        }

        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var inch = new Inch(1.0);
            var obj = new object();
            bool result = inch.Equals(obj);
            Assert.IsFalse(result, "Inch should not equal object of different type");
        }

        [TestMethod]
        public void Equals_ConsistentProperty_ReturnsTrue()
        {
            var inch1 = new Inch(3.0);
            var inch2 = new Inch(3.0);
            bool result1 = inch1.Equals(inch2);
            bool result2 = inch1.Equals(inch2);
            bool result3 = inch1.Equals(inch2);
            Assert.IsTrue(
                result1 && result2 && result3,
                "Multiple calls should return consistent results"
            );
        }

        [TestMethod]
        public void Equals_FloatingPointPrecision_HandlesCorrectly()
        {
            var inch1 = new Inch(1.000001);
            var inch2 = new Inch(1.000002);
            bool result = inch1.Equals(inch2);
            Assert.IsFalse(result, "Even very close values should be considered different");
        }

        [TestMethod]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode()
        {
            var inch1 = new Inch(5.0);
            var inch2 = new Inch(5.0);
            int hash1 = inch1.GetHashCode();
            int hash2 = inch2.GetHashCode();
            Assert.AreEqual(hash1, hash2, "Equal objects should have equal hash codes");
        }

        [TestMethod]
        public void GetHashCode_DifferentObjects_ReturnsDifferentHashCode()
        {
            var inch1 = new Inch(5.0);
            var inch2 = new Inch(6.0);
            int hash1 = inch1.GetHashCode();
            int hash2 = inch2.GetHashCode();
            Assert.AreNotEqual(hash1, hash2, "Different objects should have different hash codes");
        }

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var inch = new Inch(7.5);
            string result = inch.ToString();
            Assert.AreEqual("7.5 in", result, "ToString should return value with unit");
        }
    }
}
