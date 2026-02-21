using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Test class for Quantity addition functionality.
    /// Tests the Add method and addition operations between all supported units.
    /// </summary>
    [TestClass]
    public class QuantityAdditionTests
    {
        private const double Tolerance = 0.000001;

        #region Same-Unit Addition Tests

        /// <summary>
        /// Tests addition of two feet measurements.
        /// Verifies that 1 foot + 2 feet = 3 feet.
        /// </summary>
        [TestMethod]
        public void Add_SameUnit_FeetPlusFeet_ReturnsCorrectSum()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(2.0, LengthUnit.FEET);

            var result = q1.Add(q2);

            Assert.AreEqual(3.0, result.Value, Tolerance, "1 ft + 2 ft should equal 3 ft");
            Assert.AreEqual(LengthUnit.FEET, result.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests addition of two inch measurements.
        /// Verifies that 6 inches + 6 inches = 12 inches.
        /// </summary>
        [TestMethod]
        public void Add_SameUnit_InchPlusInch_ReturnsCorrectSum()
        {
            var q1 = new Quantity(6.0, LengthUnit.INCH);
            var q2 = new Quantity(6.0, LengthUnit.INCH);

            var result = q1.Add(q2);

            Assert.AreEqual(12.0, result.Value, Tolerance, "6 in + 6 in should equal 12 in");
            Assert.AreEqual(LengthUnit.INCH, result.Unit, "Result should be in inches");
        }

        /// <summary>
        /// Tests addition of two yard measurements.
        /// Verifies that 2 yards + 3 yards = 5 yards.
        /// </summary>
        [TestMethod]
        public void Add_SameUnit_YardPlusYard_ReturnsCorrectSum()
        {
            var q1 = new Quantity(2.0, LengthUnit.YARD);
            var q2 = new Quantity(3.0, LengthUnit.YARD);

            var result = q1.Add(q2);

            Assert.AreEqual(5.0, result.Value, Tolerance, "2 yd + 3 yd should equal 5 yd");
            Assert.AreEqual(LengthUnit.YARD, result.Unit, "Result should be in yards");
        }

        /// <summary>
        /// Tests addition of two centimeter measurements.
        /// Verifies that 5 cm + 5 cm = 10 cm.
        /// </summary>
        [TestMethod]
        public void Add_SameUnit_CentimeterPlusCentimeter_ReturnsCorrectSum()
        {
            var q1 = new Quantity(5.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(5.0, LengthUnit.CENTIMETER);

            var result = q1.Add(q2);

            Assert.AreEqual(10.0, result.Value, Tolerance, "5 cm + 5 cm should equal 10 cm");
            Assert.AreEqual(LengthUnit.CENTIMETER, result.Unit, "Result should be in centimeters");
        }

        #endregion

        #region Cross-Unit Addition Tests

        /// <summary>
        /// Tests addition of feet and inches with result in feet.
        /// Verifies that 1 foot + 12 inches = 2 feet.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_FeetPlusInches_ResultInFeet_ReturnsCorrectSum()
        {
            var feet = new Quantity(1.0, LengthUnit.FEET);
            var inches = new Quantity(12.0, LengthUnit.INCH);

            var result = feet.Add(inches); // Result in feet (first operand's unit)

            Assert.AreEqual(2.0, result.Value, Tolerance, "1 ft + 12 in should equal 2 ft");
            Assert.AreEqual(LengthUnit.FEET, result.Unit, "Result should be in feet");
        }

        /// <summary>
        /// Tests addition of feet and inches with result in inches.
        /// Verifies that 12 inches + 1 foot = 24 inches.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_InchesPlusFeet_ResultInInches_ReturnsCorrectSum()
        {
            var inches = new Quantity(12.0, LengthUnit.INCH);
            var feet = new Quantity(1.0, LengthUnit.FEET);

            var result = inches.Add(feet); // Result in inches (first operand's unit)

            Assert.AreEqual(24.0, result.Value, Tolerance, "12 in + 1 ft should equal 24 in");
            Assert.AreEqual(LengthUnit.INCH, result.Unit, "Result should be in inches");
        }

        /// <summary>
        /// Tests addition of yards and feet with result in yards.
        /// Verifies that 1 yard + 3 feet = 2 yards.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_YardPlusFeet_ResultInYards_ReturnsCorrectSum()
        {
            var yard = new Quantity(1.0, LengthUnit.YARD);
            var feet = new Quantity(3.0, LengthUnit.FEET);

            var result = yard.Add(feet); // Result in yards

            Assert.AreEqual(2.0, result.Value, Tolerance, "1 yd + 3 ft should equal 2 yd");
            Assert.AreEqual(LengthUnit.YARD, result.Unit, "Result should be in yards");
        }

        /// <summary>
        /// Tests addition of yards and inches with result in yards.
        /// Verifies that 1 yard + 36 inches = 2 yards.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_YardPlusInches_ResultInYards_ReturnsCorrectSum()
        {
            var yard = new Quantity(1.0, LengthUnit.YARD);
            var inches = new Quantity(36.0, LengthUnit.INCH);

            var result = yard.Add(inches); // Result in yards

            Assert.AreEqual(2.0, result.Value, Tolerance, "1 yd + 36 in should equal 2 yd");
            Assert.AreEqual(LengthUnit.YARD, result.Unit, "Result should be in yards");
        }

        /// <summary>
        /// Tests addition of centimeters and inches with result in centimeters.
        /// Verifies that 2.54 cm + 1 inch = 5.08 cm.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_CentimeterPlusInch_ResultInCentimeters_ReturnsCorrectSum()
        {
            var cm = new Quantity(2.54, LengthUnit.CENTIMETER);
            var inch = new Quantity(1.0, LengthUnit.INCH);

            var result = cm.Add(inch); // Result in centimeters

            Assert.AreEqual(5.08, result.Value, Tolerance, "2.54 cm + 1 in should equal 5.08 cm");
            Assert.AreEqual(LengthUnit.CENTIMETER, result.Unit, "Result should be in centimeters");
        }

        /// <summary>
        /// Tests addition of inches and centimeters with result in inches.
        /// Verifies that 1 inch + 2.54 cm = 2 inches.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_InchPlusCentimeter_ResultInInches_ReturnsCorrectSum()
        {
            var inch = new Quantity(1.0, LengthUnit.INCH);
            var cm = new Quantity(2.54, LengthUnit.CENTIMETER);

            var result = inch.Add(cm); // Result in inches

            Assert.AreEqual(2.0, result.Value, Tolerance, "1 in + 2.54 cm should equal 2 in");
            Assert.AreEqual(LengthUnit.INCH, result.Unit, "Result should be in inches");
        }

        /// <summary>
        /// Tests addition of centimeters and feet with result in centimeters.
        /// Verifies that 30.48 cm + 1 foot = 60.96 cm.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_CentimeterPlusFeet_ResultInCentimeters_ReturnsCorrectSum()
        {
            var cm = new Quantity(30.48, LengthUnit.CENTIMETER);
            var feet = new Quantity(1.0, LengthUnit.FEET);

            var result = cm.Add(feet); // Result in centimeters

            Assert.AreEqual(
                60.96,
                result.Value,
                Tolerance,
                "30.48 cm + 1 ft should equal 60.96 cm"
            );
            Assert.AreEqual(LengthUnit.CENTIMETER, result.Unit, "Result should be in centimeters");
        }

        #endregion

        #region Static Add Method Tests

        /// <summary>
        /// Tests static Add method with explicit target unit.
        /// Verifies that adding 1 foot and 12 inches with target feet returns 2 feet.
        /// </summary>
        [TestMethod]
        public void Add_Static_WithTargetUnit_ReturnsCorrectSum()
        {
            var result = Quantity.Add(1.0, LengthUnit.FEET, 12.0, LengthUnit.INCH, LengthUnit.FEET);

            Assert.AreEqual(2.0, result.Value, Tolerance, "1 ft + 12 in should equal 2 ft");
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        /// <summary>
        /// Tests static Add method with Quantity objects.
        /// Verifies that adding 1 yard and 3 feet with target yards returns 2 yards.
        /// </summary>
        [TestMethod]
        public void Add_Static_WithQuantityObjects_ReturnsCorrectSum()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(3.0, LengthUnit.FEET);

            var result = Quantity.Add(q1, q2, LengthUnit.YARD);

            Assert.AreEqual(2.0, result.Value, Tolerance, "1 yd + 3 ft should equal 2 yd");
            Assert.AreEqual(LengthUnit.YARD, result.Unit);
        }

        #endregion

        #region Commutativity Tests

        /// <summary>
        /// Tests that addition is commutative (a + b = b + a when compared in base unit).
        /// Verifies that order doesn't affect the physical sum.
        /// </summary>
        [TestMethod]
        public void Add_IsCommutative_WhenComparedInBaseUnit()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);

            var sum1 = q1.Add(q2); // Result in feet
            var sum2 = q2.Add(q1); // Result in inches

            // Convert both to base unit for comparison
            var sum1InFeet = sum1.ConvertTo(LengthUnit.FEET);
            var sum2InFeet = sum2.ConvertTo(LengthUnit.FEET);

            Assert.IsTrue(
                sum1InFeet.Equals(sum2InFeet),
                "a + b should equal b + a when compared in base unit"
            );
        }

        /// <summary>
        /// Tests that addition is commutative with various unit combinations.
        /// </summary>
        [TestMethod]
        public void Add_IsCommutative_MultipleCombinations()
        {
            var testCases = new[]
            {
                new
                {
                    Q1 = new Quantity(2.0, LengthUnit.FEET),
                    Q2 = new Quantity(24.0, LengthUnit.INCH),
                },
                new
                {
                    Q1 = new Quantity(1.5, LengthUnit.YARD),
                    Q2 = new Quantity(4.5, LengthUnit.FEET),
                },
                new
                {
                    Q1 = new Quantity(5.0, LengthUnit.CENTIMETER),
                    Q2 = new Quantity(2.0, LengthUnit.INCH),
                },
            };

            foreach (var test in testCases)
            {
                var sum1 = test.Q1.Add(test.Q2);
                var sum2 = test.Q2.Add(test.Q1);

                var sum1InFeet = sum1.ConvertTo(LengthUnit.FEET);
                var sum2InFeet = sum2.ConvertTo(LengthUnit.FEET);

                Assert.IsTrue(
                    sum1InFeet.Equals(sum2InFeet),
                    $"Addition should be commutative for {test.Q1} and {test.Q2}"
                );
            }
        }

        #endregion

        #region Zero Addition Tests

        /// <summary>
        /// Tests adding zero to a measurement.
        /// Verifies that adding zero returns the original value.
        /// </summary>
        [TestMethod]
        public void Add_WithZero_ReturnsOriginalValue()
        {
            var q = new Quantity(5.0, LengthUnit.FEET);
            var zero = new Quantity(0.0, LengthUnit.INCH);

            var result = q.Add(zero);

            Assert.AreEqual(5.0, result.Value, Tolerance, "5 ft + 0 in should equal 5 ft");
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        /// <summary>
        /// Tests adding zero in same unit.
        /// Verifies that 5 feet + 0 feet = 5 feet.
        /// </summary>
        [TestMethod]
        public void Add_WithZero_SameUnit_ReturnsOriginalValue()
        {
            var q = new Quantity(5.0, LengthUnit.FEET);
            var zero = new Quantity(0.0, LengthUnit.FEET);

            var result = q.Add(zero);

            Assert.AreEqual(5.0, result.Value, Tolerance, "5 ft + 0 ft should equal 5 ft");
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        #endregion

        #region Negative Value Tests

        /// <summary>
        /// Tests addition with negative values.
        /// Verifies that 5 feet + (-2 feet) = 3 feet.
        /// </summary>
        [TestMethod]
        public void Add_WithNegativeValues_ReturnsCorrectSum()
        {
            var q1 = new Quantity(5.0, LengthUnit.FEET);
            var q2 = new Quantity(-2.0, LengthUnit.FEET);

            var result = q1.Add(q2);

            Assert.AreEqual(3.0, result.Value, Tolerance, "5 ft + (-2 ft) should equal 3 ft");
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        /// <summary>
        /// Tests addition with negative cross-unit values.
        /// Verifies that 2 feet + (-12 inches) = 1 foot.
        /// </summary>
        [TestMethod]
        public void Add_WithNegativeCrossUnit_ReturnsCorrectSum()
        {
            var feet = new Quantity(2.0, LengthUnit.FEET);
            var inches = new Quantity(-12.0, LengthUnit.INCH);

            var result = feet.Add(inches);

            Assert.AreEqual(1.0, result.Value, Tolerance, "2 ft + (-12 in) should equal 1 ft");
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        #endregion

        #region Large Value Tests

        /// <summary>
        /// Tests addition with very large values.
        /// Verifies that conversion maintains precision for large numbers.
        /// </summary>
        [TestMethod]
        public void Add_LargeValues_MaintainsPrecision()
        {
            var q1 = new Quantity(1000000.0, LengthUnit.FEET);
            var q2 = new Quantity(1000000.0, LengthUnit.FEET);

            var result = q1.Add(q2);

            Assert.AreEqual(
                2000000.0,
                result.Value,
                Tolerance * 1000000,
                "1,000,000 ft + 1,000,000 ft should equal 2,000,000 ft"
            );
        }

        /// <summary>
        /// Tests addition with very large cross-unit values.
        /// </summary>
        [TestMethod]
        public void Add_LargeCrossUnitValues_MaintainsPrecision()
        {
            var feet = new Quantity(1000000.0, LengthUnit.FEET);
            var inches = new Quantity(12000000.0, LengthUnit.INCH); // 1,000,000 feet

            var result = feet.Add(inches);

            Assert.AreEqual(
                2000000.0,
                result.Value,
                Tolerance * 1000000,
                "1,000,000 ft + 12,000,000 in should equal 2,000,000 ft"
            );
        }

        #endregion

        #region Small Value Tests

        /// <summary>
        /// Tests addition with very small values.
        /// Verifies that conversion maintains precision for small numbers.
        /// </summary>
        [TestMethod]
        public void Add_SmallValues_MaintainsPrecision()
        {
            var q1 = new Quantity(0.000001, LengthUnit.FEET);
            var q2 = new Quantity(0.000002, LengthUnit.FEET);

            var result = q1.Add(q2);

            Assert.AreEqual(
                0.000003,
                result.Value,
                Tolerance,
                "0.000001 ft + 0.000002 ft should equal 0.000003 ft"
            );
        }

        #endregion

        #region Null and Invalid Input Tests

        /// <summary>
        /// Tests adding null quantity.
        /// Verifies that ArgumentNullException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullOperand_ThrowsArgumentNullException()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            Quantity? q2 = null;

            q1.Add(q2!); // Will throw ArgumentNullException
        }

        /// <summary>
        /// Tests static Add with null first quantity.
        /// Verifies that ArgumentNullException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Static_NullFirstOperand_ThrowsArgumentNullException()
        {
            Quantity? q1 = null;
            var q2 = new Quantity(1.0, LengthUnit.FEET);

            Quantity.Add(q1!, q2, LengthUnit.FEET); // Will throw ArgumentNullException
        }

        /// <summary>
        /// Tests static Add with null second quantity.
        /// Verifies that ArgumentNullException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Static_NullSecondOperand_ThrowsArgumentNullException()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            Quantity? q2 = null;

            Quantity.Add(q1, q2!, LengthUnit.FEET); // Will throw ArgumentNullException
        }

        /// <summary>
        /// Tests Add with invalid target unit.
        /// Verifies that ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_InvalidTargetUnit_ThrowsArgumentException()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            LengthUnit invalidUnit = (LengthUnit)99;

            q1.Add(q2, invalidUnit); // Will throw ArgumentException
        }

        #endregion

        #region Multiple Unit Combinations Tests

        /// <summary>
        /// Tests addition with all possible unit combinations.
        /// Verifies that no combination throws unexpected exceptions.
        /// </summary>
        [TestMethod]
        public void Add_AllUnitCombinations_NoExceptions()
        {
            LengthUnit[] units =
            {
                LengthUnit.FEET,
                LengthUnit.INCH,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER,
            };

            foreach (var unit1 in units)
            {
                foreach (var unit2 in units)
                {
                    try
                    {
                        var q1 = new Quantity(1.0, unit1);
                        var q2 = new Quantity(1.0, unit2);

                        var result = q1.Add(q2);

                        Assert.IsNotNull(
                            result,
                            $"Addition of {unit1} and {unit2} should return a result"
                        );
                        Assert.IsFalse(
                            double.IsNaN(result.Value),
                            $"Result value should not be NaN"
                        );
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(
                            $"Addition of {unit1} and {unit2} threw exception: {ex.Message}"
                        );
                    }
                }
            }
        }

        #endregion

        #region Exact Mathematical Tests

        /// <summary>
        /// Tests addition with exact mathematical relationships.
        /// Verifies that 1 yard + 2 feet + 12 inches = 2 yards when expressed properly.
        /// </summary>
        [TestMethod]
        public void Add_MultipleSteps_ReturnsExactSum()
        {
            var yard = new Quantity(1.0, LengthUnit.YARD);
            var feet = new Quantity(2.0, LengthUnit.FEET);
            var inches = new Quantity(12.0, LengthUnit.INCH);

            // First add yard and feet
            var temp = yard.Add(feet); // 1 yd + 2 ft = 1.666... yd
            // Then add inches
            var result = temp.Add(inches);

            // Expected: 1 yd + 2 ft + 12 in = 2 yd
            Assert.AreEqual(2.0, result.Value, Tolerance, "1 yd + 2 ft + 12 in should equal 2 yd");
        }

        #endregion
    }
}
