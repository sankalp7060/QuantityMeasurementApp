using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Unit tests for the Inch class
    /// </summary>
    [TestClass]
    public class InchTests
    {
        // Tests Inch.Equals(object?) method for same value comparison
        [TestMethod]
        public void Equals_SameValue_ReturnsTrue()
        {
            var inch1 = new Inch(1.0);
            var inch2 = new Inch(1.0);
            bool result = inch1.Equals(inch2);
            Assert.IsTrue(result, "1.0 in should equal 1.0 in");
        }

        // Tests Inch.Equals(object?) method for different value comparison
        [TestMethod]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            var inch1 = new Inch(1.0);
            var inch2 = new Inch(2.0);
            bool result = inch1.Equals(inch2);
            Assert.IsFalse(result, "1.0 in should not equal 2.0 in");
        }

        // Tests Inch.Equals(object?) method for same reference comparison
        [TestMethod]
        public void Equals_SameReference_ReturnsTrue()
        {
            var inch = new Inch(1.0);
            bool result = inch.Equals(inch);
            Assert.IsTrue(result, "Object should equal itself");
        }

        // Tests Inch.Equals(object?) method when compared with null
        [TestMethod]
        public void Equals_NullComparison_ReturnsFalse()
        {
            var inch = new Inch(1.0);
            bool result = inch.Equals(null);
            Assert.IsFalse(result, "Object should not equal null");
        }

        // Tests symmetric property of Inch.Equals(object?)
        [TestMethod]
        public void Equals_SymmetricProperty_ReturnsTrue()
        {
            var inch1 = new Inch(1.5);
            var inch2 = new Inch(1.5);
            bool result1 = inch1.Equals(inch2);
            bool result2 = inch2.Equals(inch1);
            Assert.IsTrue(result1 && result2, "Equality should be symmetric");
        }

        // Tests transitive property of Inch.Equals(object?)
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

        // Tests Inch.Equals(object?) method with different object type
        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var inch = new Inch(1.0);
            var obj = new object();
            bool result = inch.Equals(obj);
            Assert.IsFalse(result, "Inch should not equal object of different type");
        }

        // Tests consistency of multiple calls to Inch.Equals(object?)
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

        // Tests Inch.Equals(object?) method for floating point precision handling
        [TestMethod]
        public void Equals_FloatingPointPrecision_HandlesCorrectly()
        {
            var inch1 = new Inch(1.000001);
            var inch2 = new Inch(1.000002);
            bool result = inch1.Equals(inch2);
            Assert.IsFalse(result, "Even very close values should be considered different");
        }

        // Tests Inch.GetHashCode() method for equal objects
        [TestMethod]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode()
        {
            var inch1 = new Inch(5.0);
            var inch2 = new Inch(5.0);
            int hash1 = inch1.GetHashCode();
            int hash2 = inch2.GetHashCode();
            Assert.AreEqual(hash1, hash2, "Equal objects should have equal hash codes");
        }

        // Tests Inch.GetHashCode() method for different objects
        [TestMethod]
        public void GetHashCode_DifferentObjects_ReturnsDifferentHashCode()
        {
            var inch1 = new Inch(5.0);
            var inch2 = new Inch(6.0);
            int hash1 = inch1.GetHashCode();
            int hash2 = inch2.GetHashCode();
            Assert.AreNotEqual(hash1, hash2, "Different objects should have different hash codes");
        }

        // Tests Inch.ToString() method formatting
        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var inch = new Inch(7.5);
            string result = inch.ToString();
            Assert.AreEqual("7.5 in", result, "ToString should return value with unit");
        }
    }
}
