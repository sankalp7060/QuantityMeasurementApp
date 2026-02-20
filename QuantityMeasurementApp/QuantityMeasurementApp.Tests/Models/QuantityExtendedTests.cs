using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    [TestClass]
    public class QuantityExtendedTests
    {
        // Tolerance for floating point comparisons
        private const double Tolerance = 0.000001;

        #region Yard Tests

        // Test: Yard to Yard same value
        [TestMethod]
        public void Equals_YardToYardSameValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(1.0, LengthUnit.YARD);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 yd should equal 1.0 yd");
        }

        // Test: Yard to Yard different value
        [TestMethod]
        public void Equals_YardToYardDifferentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(2.0, LengthUnit.YARD);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 yd should not equal 2.0 yd");
        }

        // Test: Yard to Feet equivalent (1 yd = 3 ft)
        [TestMethod]
        public void Equals_YardToFeetEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(3.0, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 yd should equal 3.0 ft");
        }

        // Test: Feet to Yard equivalent (3 ft = 1 yd) - Symmetry
        [TestMethod]
        public void Equals_FeetToYardEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(3.0, LengthUnit.FEET);
            var q2 = new Quantity(1.0, LengthUnit.YARD);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "3.0 ft should equal 1.0 yd");
        }

        // Test: Yard to Inches equivalent (1 yd = 36 in)
        [TestMethod]
        public void Equals_YardToInchesEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(36.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 yd should equal 36.0 in");
        }

        // Test: Inches to Yard equivalent (36 in = 1 yd) - Symmetry
        [TestMethod]
        public void Equals_InchesToYardEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(36.0, LengthUnit.INCH);
            var q2 = new Quantity(1.0, LengthUnit.YARD);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "36.0 in should equal 1.0 yd");
        }

        // Test: Yard to Feet non-equivalent
        [TestMethod]
        public void Equals_YardToFeetNonEquivalentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(2.0, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 yd should not equal 2.0 ft");
        }

        // Test: Yard to Inches non-equivalent
        [TestMethod]
        public void Equals_YardToInchesNonEquivalentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(35.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 yd should not equal 35.0 in");
        }

        #endregion

        #region Centimeter Tests

        // Test: Centimeter to Centimeter same value
        [TestMethod]
        public void Equals_CentimeterToCentimeterSameValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.CENTIMETER);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 cm should equal 1.0 cm");
        }

        // Test: Centimeter to Centimeter different value
        [TestMethod]
        public void Equals_CentimeterToCentimeterDifferentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(2.0, LengthUnit.CENTIMETER);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 cm should not equal 2.0 cm");
        }

        // Test: Centimeter to Inches equivalent (1 cm = 0.393700787 in)
        [TestMethod]
        public void Equals_CentimeterToInchesEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(0.393700787, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 cm should equal 0.393700787 in");
        }

        // Test: Inches to Centimeter equivalent (0.393700787 in = 1 cm) - Symmetry
        [TestMethod]
        public void Equals_InchesToCentimeterEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(0.393700787, LengthUnit.INCH);
            var q2 = new Quantity(1.0, LengthUnit.CENTIMETER);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "0.393700787 in should equal 1.0 cm");
        }

        // Test: Centimeter to Feet equivalent (30.48 cm = 1 ft)
        [TestMethod]
        public void Equals_CentimeterToFeetEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(30.48, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "30.48 cm should equal 1.0 ft");
        }

        // Test: Feet to Centimeter equivalent (1 ft = 30.48 cm)
        [TestMethod]
        public void Equals_FeetToCentimeterEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(30.48, LengthUnit.CENTIMETER);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 ft should equal 30.48 cm");
        }

        // Test: Centimeter to Yard equivalent (91.44 cm = 1 yd)
        [TestMethod]
        public void Equals_CentimeterToYardEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(91.44, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.YARD);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "91.44 cm should equal 1.0 yd");
        }

        // Test: Yard to Centimeter equivalent (1 yd = 91.44 cm)
        [TestMethod]
        public void Equals_YardToCentimeterEquivalentValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(91.44, LengthUnit.CENTIMETER);
            bool result = q1.Equals(q2);
            Assert.IsTrue(result, "1.0 yd should equal 91.44 cm");
        }

        // Test: Centimeter to Inches non-equivalent
        [TestMethod]
        public void Equals_CentimeterToInchesNonEquivalentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.INCH);
            bool result = q1.Equals(q2);
            Assert.IsFalse(result, "1.0 cm should not equal 1.0 in");
        }

        #endregion

        #region Multi-Unit Transitive Tests

        // Test: Transitive property across yards, feet, and inches
        [TestMethod]
        public void Equals_TransitiveYardFeetInches_ReturnsTrue()
        {
            var yards = new Quantity(2.0, LengthUnit.YARD);
            var feet = new Quantity(6.0, LengthUnit.FEET);
            var inches = new Quantity(72.0, LengthUnit.INCH);

            bool yardsEqualsFeet = yards.Equals(feet);
            bool feetEqualsInches = feet.Equals(inches);
            bool yardsEqualsInches = yards.Equals(inches);

            Assert.IsTrue(yardsEqualsFeet, "2 yd should equal 6 ft");
            Assert.IsTrue(feetEqualsInches, "6 ft should equal 72 in");
            Assert.IsTrue(yardsEqualsInches, "Therefore, 2 yd should equal 72 in (transitive)");
        }

        // Test: Transitive property across centimeters, inches, and feet
        [TestMethod]
        public void Equals_TransitiveCmInchesFeet_ReturnsTrue()
        {
            var cm = new Quantity(30.48, LengthUnit.CENTIMETER);
            var inches = new Quantity(12.0, LengthUnit.INCH);
            var feet = new Quantity(1.0, LengthUnit.FEET);

            bool cmEqualsInches = cm.Equals(inches);
            bool inchesEqualsFeet = inches.Equals(feet);
            bool cmEqualsFeet = cm.Equals(feet);

            Assert.IsTrue(cmEqualsInches, "30.48 cm should equal 12 in");
            Assert.IsTrue(inchesEqualsFeet, "12 in should equal 1 ft");
            Assert.IsTrue(cmEqualsFeet, "Therefore, 30.48 cm should equal 1 ft (transitive)");
        }

        // Test: Complex scenario with all units
        [TestMethod]
        public void Equals_AllUnitsComplexScenario_ReturnsTrue()
        {
            // 1 yard = 3 feet = 36 inches = 91.44 cm
            var yard = new Quantity(1.0, LengthUnit.YARD);
            var feet = new Quantity(3.0, LengthUnit.FEET);
            var inches = new Quantity(36.0, LengthUnit.INCH);
            var cm = new Quantity(91.44, LengthUnit.CENTIMETER);

            Assert.IsTrue(yard.Equals(feet), "1 yd should equal 3 ft");
            Assert.IsTrue(yard.Equals(inches), "1 yd should equal 36 in");
            Assert.IsTrue(yard.Equals(cm), "1 yd should equal 91.44 cm");
            Assert.IsTrue(feet.Equals(inches), "3 ft should equal 36 in");
            Assert.IsTrue(feet.Equals(cm), "3 ft should equal 91.44 cm");
            Assert.IsTrue(inches.Equals(cm), "36 in should equal 91.44 cm");
        }

        #endregion

        #region Edge Cases and Validation

        // Test: Yard object equals itself (reflexive)
        [TestMethod]
        public void Equals_YardSameReference_ReturnsTrue()
        {
            var q = new Quantity(1.0, LengthUnit.YARD);
            bool result = q.Equals(q);
            Assert.IsTrue(result, "Yard object should equal itself");
        }

        // Test: Centimeter object equals itself (reflexive)
        [TestMethod]
        public void Equals_CentimeterSameReference_ReturnsTrue()
        {
            var q = new Quantity(1.0, LengthUnit.CENTIMETER);
            bool result = q.Equals(q);
            Assert.IsTrue(result, "Centimeter object should equal itself");
        }

        // Test: Yard with null comparison
        [TestMethod]
        public void Equals_YardNullComparison_ReturnsFalse()
        {
            var q = new Quantity(1.0, LengthUnit.YARD);
            bool result = q.Equals(null);
            Assert.IsFalse(result, "Yard should not equal null");
        }

        // Test: Centimeter with null comparison
        [TestMethod]
        public void Equals_CentimeterNullComparison_ReturnsFalse()
        {
            var q = new Quantity(1.0, LengthUnit.CENTIMETER);
            bool result = q.Equals(null);
            Assert.IsFalse(result, "Centimeter should not equal null");
        }

        // Test: Different types comparison (Yard vs Object)
        [TestMethod]
        public void Equals_YardDifferentType_ReturnsFalse()
        {
            var q = new Quantity(1.0, LengthUnit.YARD);
            var obj = new object();
            bool result = q.Equals(obj);
            Assert.IsFalse(result, "Yard should not equal object of different type");
        }

        #endregion

        #region GetHashCode Tests

        [TestMethod]
        public void GetHashCode_EqualYards_ReturnsSameHashCode()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(1.0, LengthUnit.YARD);
            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_EqualCentimeters_ReturnsSameHashCode()
        {
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.CENTIMETER);
            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_EquivalentYardAndFeet_ReturnsSameHashCode()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(3.0, LengthUnit.FEET);
            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_EquivalentYardAndInches_ReturnsSameHashCode()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(36.0, LengthUnit.INCH);
            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_EquivalentYardAndCentimeter_ReturnsSameHashCode()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(91.44, LengthUnit.CENTIMETER);
            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_EquivalentCentimeterAndInch_ReturnsSameHashCode()
        {
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(0.393700787, LengthUnit.INCH);
            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
        }

        #endregion

        #region ToString Tests

        [TestMethod]
        public void ToString_YardUnit_ReturnsFormattedString()
        {
            var q = new Quantity(7.5, LengthUnit.YARD);
            Assert.AreEqual("7.5 yd", q.ToString());
        }

        [TestMethod]
        public void ToString_CentimeterUnit_ReturnsFormattedString()
        {
            var q = new Quantity(7.5, LengthUnit.CENTIMETER);
            Assert.AreEqual("7.5 cm", q.ToString());
        }

        #endregion
    }
}
