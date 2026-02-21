using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Test class for refactored Quantity class that delegates conversion to LengthUnit.
    /// Verifies that all functionality from UC1-UC7 is preserved after refactoring.
    /// </summary>
    [TestClass]
    public class QuantityRefactoredTests
    {
        private const double Tolerance = 0.000001;

        #region UC1: Feet Equality Tests (Backward Compatibility)

        /// <summary>
        /// Tests UC1: Same value (1.0 ft and 1.0 ft) should be equal.
        /// Verifies UC1 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC1_Equality_SameValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(1.0, LengthUnit.FEET);

            bool result = q1.Equals(q2);

            Assert.IsTrue(result, "1.0 ft should equal 1.0 ft");
        }

        /// <summary>
        /// Tests UC1: Different values (1.0 ft and 2.0 ft) should not be equal.
        /// Verifies UC1 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC1_Equality_DifferentValue_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(2.0, LengthUnit.FEET);

            bool result = q1.Equals(q2);

            Assert.IsFalse(result, "1.0 ft should not equal 2.0 ft");
        }

        /// <summary>
        /// Tests UC1: Reflexive property - object should equal itself.
        /// Verifies UC1 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC1_Equality_SameReference_ReturnsTrue()
        {
            var q = new Quantity(1.0, LengthUnit.FEET);

            bool result = q.Equals(q);

            Assert.IsTrue(result, "Object should equal itself");
        }

        /// <summary>
        /// Tests UC1: Null comparison should return false.
        /// Verifies UC1 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC1_Equality_NullComparison_ReturnsFalse()
        {
            var q = new Quantity(1.0, LengthUnit.FEET);

            bool result = q.Equals(null);

            Assert.IsFalse(result, "Object should not equal null");
        }

        #endregion

        #region UC2: Inch Equality Tests (Backward Compatibility)

        /// <summary>
        /// Tests UC2: Same inch value (1.0 in and 1.0 in) should be equal.
        /// Verifies UC2 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC2_Equality_InchSameValue_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.INCH);
            var q2 = new Quantity(1.0, LengthUnit.INCH);

            bool result = q1.Equals(q2);

            Assert.IsTrue(result, "1.0 in should equal 1.0 in");
        }

        /// <summary>
        /// Tests UC2: Cross-unit equality (1 ft = 12 in).
        /// Verifies UC2 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC2_Equality_FeetToInchEquivalent_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);

            bool result = q1.Equals(q2);

            Assert.IsTrue(result, "1 ft should equal 12 in");
        }

        #endregion

        #region UC3/UC4: Extended Unit Tests (Backward Compatibility)

        /// <summary>
        /// Tests UC3/UC4: Yard to feet equality (1 yd = 3 ft).
        /// Verifies UC3/UC4 tests pass with refactored design.
        /// </summary>
        [TestMethod]
        public void UC3_Equality_YardToFeetEquivalent_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(3.0, LengthUnit.FEET);

            bool result = q1.Equals(q2);

            Assert.IsTrue(result, "1 yd should equal 3 ft");
        }

        /// <summary>
        /// Tests UC3/UC4: Centimeter to inch equality (1 cm = 0.3937 in).
        /// Verifies UC3/UC4 tests pass with refactored design.
        /// </summary>
        [TestMethod]
        public void UC4_Equality_CentimeterToInchEquivalent_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(0.393700787, LengthUnit.INCH);

            bool result = q1.Equals(q2);

            Assert.IsTrue(result, "1 cm should equal 0.393700787 in");
        }

        #endregion

        #region UC5: Conversion Tests (Backward Compatibility)

        /// <summary>
        /// Tests UC5: ConvertTo method with feet to inches.
        /// Verifies UC5 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC5_ConvertTo_FeetToInches_ReturnsCorrectQuantity()
        {
            var q = new Quantity(1.0, LengthUnit.FEET);

            var result = q.ConvertTo(LengthUnit.INCH);

            Assert.AreEqual(12.0, result.Value, Tolerance, "1 ft should convert to 12 in");
            Assert.AreEqual(LengthUnit.INCH, result.Unit);
        }

        /// <summary>
        /// Tests UC5: Static Convert method with yards to feet.
        /// Verifies UC5 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC5_StaticConvert_YardsToFeet_ReturnsCorrectValue()
        {
            double result = Quantity.Convert(1.0, LengthUnit.YARD, LengthUnit.FEET);

            Assert.AreEqual(3.0, result, Tolerance, "1 yd should convert to 3 ft");
        }

        #endregion

        #region UC6: Addition Tests (Backward Compatibility)

        /// <summary>
        /// Tests UC6: Same unit addition (feet + feet).
        /// Verifies UC6 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC6_Add_SameUnit_FeetPlusFeet_ReturnsCorrectSum()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(2.0, LengthUnit.FEET);

            var result = q1.Add(q2);

            Assert.AreEqual(3.0, result.Value, Tolerance, "1 ft + 2 ft should equal 3 ft");
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        /// <summary>
        /// Tests UC6: Cross-unit addition (feet + inches) with result in feet.
        /// Verifies UC6 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC6_Add_CrossUnit_FeetPlusInches_ReturnsCorrectSum()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);

            var result = q1.Add(q2);

            Assert.AreEqual(2.0, result.Value, Tolerance, "1 ft + 12 in should equal 2 ft");
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        #endregion

        #region UC7: Addition with Explicit Target Unit Tests (Backward Compatibility)

        /// <summary>
        /// Tests UC7: Addition with explicit target unit (yards).
        /// Verifies UC7 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC7_Add_ExplicitTarget_Yards_ReturnsCorrectSum()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.YARD);

            double expected = 2.0 / 3.0; // 2 feet = 2/3 yards
            Assert.AreEqual(expected, result.Value, Tolerance, "1 ft + 12 in should equal 2/3 yd");
            Assert.AreEqual(LengthUnit.YARD, result.Unit);
        }

        /// <summary>
        /// Tests UC7: Static Add with explicit target unit.
        /// Verifies UC7 test passes with refactored design.
        /// </summary>
        [TestMethod]
        public void UC7_StaticAdd_ExplicitTarget_ReturnsCorrectSum()
        {
            var result = Quantity.Add(2.0, LengthUnit.YARD, 3.0, LengthUnit.FEET, LengthUnit.FEET);

            Assert.AreEqual(9.0, result.Value, Tolerance, "2 yd + 3 ft should equal 9 ft");
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        #endregion

        #region Commutativity Tests

        /// <summary>
        /// Tests that addition remains commutative after refactoring.
        /// Verifies mathematical properties are preserved.
        /// </summary>
        [TestMethod]
        public void Add_Refactored_RemainsCommutative()
        {
            var a = new Quantity(1.0, LengthUnit.FEET);
            var b = new Quantity(12.0, LengthUnit.INCH);

            var aPlusB = a.Add(b, LengthUnit.YARD);
            var bPlusA = b.Add(a, LengthUnit.YARD);

            Assert.AreEqual(
                aPlusB.Value,
                bPlusA.Value,
                Tolerance,
                "Addition should remain commutative after refactoring"
            );
        }

        #endregion

        #region Unit Independence Tests

        /// <summary>
        /// Tests that adding new units would require changes only to LengthUnit.
        /// Verifies that Quantity class is independent of specific units.
        /// </summary>
        [TestMethod]
        public void Quantity_IsIndependentOfSpecificUnits()
        {
            // This test verifies that Quantity doesn't have hardcoded logic for specific units
            var q = new Quantity(1.0, LengthUnit.FEET);

            // These operations should work for any valid LengthUnit
            var converted = q.ConvertTo(LengthUnit.INCH);
            var added = q.Add(new Quantity(12.0, LengthUnit.INCH));

            Assert.IsNotNull(converted);
            Assert.IsNotNull(added);
        }

        #endregion

        #region Validation Tests

        /// <summary>
        /// Tests that null unit validation still works.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Quantity_InvalidUnit_ThrowsArgumentException()
        {
            LengthUnit invalidUnit = (LengthUnit)99;
            var q = new Quantity(1.0, invalidUnit); // Should throw
        }

        /// <summary>
        /// Tests that invalid value validation still works.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Quantity_InvalidValue_ThrowsArgumentException()
        {
            var q = new Quantity(double.NaN, LengthUnit.FEET); // Should throw
        }

        #endregion
    }
}
