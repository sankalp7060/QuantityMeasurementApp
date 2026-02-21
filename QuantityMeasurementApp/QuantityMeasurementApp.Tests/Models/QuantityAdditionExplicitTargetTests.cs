using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests.Models
{
    /// <summary>
    /// Test class for Quantity addition functionality with explicit target unit specification (UC7).
    /// Tests the Add method with target unit parameter and verifies all unit combinations.
    /// </summary>
    [TestClass]
    public class QuantityAdditionExplicitTargetTests
    {
        private const double Tolerance = 0.000001;

        #region Explicit Target Unit Tests

        /// <summary>
        /// Tests addition with explicit target unit FEET.
        /// Verifies that 1 foot + 12 inches with target feet returns 2 feet.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_Feet_ReturnsCorrectSum()
        {
            var result = Quantity.Add(1.0, LengthUnit.FEET, 12.0, LengthUnit.INCH, LengthUnit.FEET);

            Assert.AreEqual(
                2.0,
                result.Value,
                Tolerance,
                "1 ft + 12 in with target FEET should equal 2 ft"
            );
            Assert.AreEqual(LengthUnit.FEET, result.Unit, "Result unit should be FEET");
        }

        /// <summary>
        /// Tests addition with explicit target unit INCHES.
        /// Verifies that 1 foot + 12 inches with target inches returns 24 inches.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_Inches_ReturnsCorrectSum()
        {
            var result = Quantity.Add(1.0, LengthUnit.FEET, 12.0, LengthUnit.INCH, LengthUnit.INCH);

            Assert.AreEqual(
                24.0,
                result.Value,
                Tolerance,
                "1 ft + 12 in with target INCHES should equal 24 in"
            );
            Assert.AreEqual(LengthUnit.INCH, result.Unit, "Result unit should be INCH");
        }

        /// <summary>
        /// Tests addition with explicit target unit YARDS.
        /// Verifies that 1 foot + 12 inches with target yards returns approximately 0.667 yards.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_Yards_ReturnsCorrectSum()
        {
            var result = Quantity.Add(1.0, LengthUnit.FEET, 12.0, LengthUnit.INCH, LengthUnit.YARD);

            double expected = 2.0 / 3.0; // 2 feet = 2/3 yards
            Assert.AreEqual(
                expected,
                result.Value,
                Tolerance,
                "1 ft + 12 in with target YARDS should equal 2/3 yd"
            );
            Assert.AreEqual(LengthUnit.YARD, result.Unit, "Result unit should be YARD");
        }

        /// <summary>
        /// Tests addition with explicit target unit CENTIMETERS.
        /// Verifies that 1 inch + 1 inch with target centimeters returns approximately 5.08 cm.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_Centimeters_ReturnsCorrectSum()
        {
            var result = Quantity.Add(
                1.0,
                LengthUnit.INCH,
                1.0,
                LengthUnit.INCH,
                LengthUnit.CENTIMETER
            );

            double expected = 5.08; // 2 inches = 5.08 cm
            Assert.AreEqual(
                expected,
                result.Value,
                Tolerance,
                "1 in + 1 in with target CM should equal 5.08 cm"
            );
            Assert.AreEqual(LengthUnit.CENTIMETER, result.Unit, "Result unit should be CENTIMETER");
        }

        #endregion

        #region Target Unit Same as Operand Tests

        /// <summary>
        /// Tests addition where target unit matches first operand unit.
        /// Verifies that 2 yards + 3 feet with target yards returns 3 yards.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_SameAsFirstOperand_ReturnsCorrectSum()
        {
            var result = Quantity.Add(2.0, LengthUnit.YARD, 3.0, LengthUnit.FEET, LengthUnit.YARD);

            Assert.AreEqual(
                3.0,
                result.Value,
                Tolerance,
                "2 yd + 3 ft with target YARDS should equal 3 yd"
            );
            Assert.AreEqual(LengthUnit.YARD, result.Unit);
        }

        /// <summary>
        /// Tests addition where target unit matches second operand unit.
        /// Verifies that 2 yards + 3 feet with target feet returns 9 feet.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_SameAsSecondOperand_ReturnsCorrectSum()
        {
            var result = Quantity.Add(2.0, LengthUnit.YARD, 3.0, LengthUnit.FEET, LengthUnit.FEET);

            Assert.AreEqual(
                9.0,
                result.Value,
                Tolerance,
                "2 yd + 3 ft with target FEET should equal 9 ft"
            );
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        #endregion

        #region Commutativity Tests with Explicit Target

        /// <summary>
        /// Tests commutativity with explicit target unit.
        /// Verifies that add(A,B,target) equals add(B,A,target).
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_Commutativity_HoldsForAllTargets()
        {
            var a = new Quantity(1.0, LengthUnit.FEET);
            var b = new Quantity(12.0, LengthUnit.INCH);

            LengthUnit[] targets =
            {
                LengthUnit.FEET,
                LengthUnit.INCH,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER,
            };

            foreach (var target in targets)
            {
                var aPlusB = Quantity.Add(a, b, target);
                var bPlusA = Quantity.Add(b, a, target);

                Assert.AreEqual(
                    aPlusB.Value,
                    bPlusA.Value,
                    Tolerance,
                    $"Addition should be commutative for target unit {target}"
                );
                Assert.AreEqual(
                    aPlusB.Unit,
                    bPlusA.Unit,
                    $"Result units should be the same for target {target}"
                );
            }
        }

        /// <summary>
        /// Tests commutativity with different operand orders and target units.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_Commutativity_MultipleCombinations()
        {
            var testCases = new[]
            {
                new
                {
                    A = new Quantity(2.0, LengthUnit.YARD),
                    B = new Quantity(6.0, LengthUnit.FEET),
                },
                new
                {
                    A = new Quantity(5.0, LengthUnit.CENTIMETER),
                    B = new Quantity(2.0, LengthUnit.INCH),
                },
                new
                {
                    A = new Quantity(3.0, LengthUnit.FEET),
                    B = new Quantity(36.0, LengthUnit.INCH),
                },
            };

            LengthUnit[] targets =
            {
                LengthUnit.FEET,
                LengthUnit.INCH,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER,
            };

            foreach (var test in testCases)
            {
                foreach (var target in targets)
                {
                    var aPlusB = Quantity.Add(test.A, test.B, target);
                    var bPlusA = Quantity.Add(test.B, test.A, target);

                    Assert.AreEqual(
                        aPlusB.Value,
                        bPlusA.Value,
                        Tolerance,
                        $"Addition should be commutative for {test.A} + {test.B} in {target}"
                    );
                }
            }
        }

        #endregion

        #region Zero Value Tests with Explicit Target

        /// <summary>
        /// Tests addition with zero operand and explicit target unit.
        /// Verifies that 5 feet + 0 inches with target yards returns approximately 1.667 yards.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_WithZero_ReturnsCorrectSum()
        {
            var result = Quantity.Add(5.0, LengthUnit.FEET, 0.0, LengthUnit.INCH, LengthUnit.YARD);

            double expected = 5.0 / 3.0; // 5 feet = 5/3 yards
            Assert.AreEqual(
                expected,
                result.Value,
                Tolerance,
                "5 ft + 0 in with target YARDS should equal 5/3 yd"
            );
            Assert.AreEqual(LengthUnit.YARD, result.Unit);
        }

        /// <summary>
        /// Tests addition where both operands are zero with various target units.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_ZeroOperands_ReturnsZero()
        {
            LengthUnit[] units =
            {
                LengthUnit.FEET,
                LengthUnit.INCH,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER,
            };

            foreach (var target in units)
            {
                var result = Quantity.Add(0.0, LengthUnit.FEET, 0.0, LengthUnit.INCH, target);
                Assert.AreEqual(
                    0.0,
                    result.Value,
                    Tolerance,
                    $"Zero sum should be zero in {target}"
                );
                Assert.AreEqual(target, result.Unit);
            }
        }

        #endregion

        #region Negative Value Tests with Explicit Target

        /// <summary>
        /// Tests addition with negative values and explicit target unit.
        /// Verifies that 5 feet + (-2 feet) with target inches returns 36 inches.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_NegativeValues_ReturnsCorrectSum()
        {
            var result = Quantity.Add(5.0, LengthUnit.FEET, -2.0, LengthUnit.FEET, LengthUnit.INCH);

            Assert.AreEqual(
                36.0,
                result.Value,
                Tolerance,
                "5 ft + (-2 ft) with target INCHES should equal 36 in"
            );
            Assert.AreEqual(LengthUnit.INCH, result.Unit);
        }

        /// <summary>
        /// Tests addition where result is negative with explicit target unit.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_NegativeResult_ReturnsCorrectSum()
        {
            var result = Quantity.Add(
                1.0,
                LengthUnit.FEET,
                -24.0,
                LengthUnit.INCH,
                LengthUnit.YARD
            );

            double expected = -1.0 / 3.0; // -1 foot = -1/3 yard
            Assert.AreEqual(
                expected,
                result.Value,
                Tolerance,
                "1 ft + (-24 in) with target YARDS should equal -1/3 yd"
            );
            Assert.AreEqual(LengthUnit.YARD, result.Unit);
        }

        #endregion

        #region Scale Conversion Tests

        /// <summary>
        /// Tests addition with result converted to smaller scale unit (feet to inches).
        /// Verifies that 1000 feet + 500 feet with target inches returns 18000 inches.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_LargeToSmallScale_ReturnsCorrectSum()
        {
            var result = Quantity.Add(
                1000.0,
                LengthUnit.FEET,
                500.0,
                LengthUnit.FEET,
                LengthUnit.INCH
            );

            Assert.AreEqual(
                18000.0,
                result.Value,
                Tolerance,
                "1000 ft + 500 ft with target INCHES should equal 18000 in"
            );
            Assert.AreEqual(LengthUnit.INCH, result.Unit);
        }

        /// <summary>
        /// Tests addition with result converted to larger scale unit (inches to yards).
        /// Verifies that 12 inches + 12 inches with target yards returns approximately 0.667 yards.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_SmallToLargeScale_ReturnsCorrectSum()
        {
            var result = Quantity.Add(
                12.0,
                LengthUnit.INCH,
                12.0,
                LengthUnit.INCH,
                LengthUnit.YARD
            );

            double expected = 24.0 / 36.0; // 24 inches = 24/36 yards
            Assert.AreEqual(
                expected,
                result.Value,
                Tolerance,
                "12 in + 12 in with target YARDS should equal 24/36 yd"
            );
            Assert.AreEqual(LengthUnit.YARD, result.Unit);
        }

        #endregion

        #region All Unit Combinations Tests

        /// <summary>
        /// Comprehensive test covering all unit pairs with multiple target units.
        /// Verifies mathematical correctness across all valid combinations.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_AllUnitCombinations_ReturnsCorrectResults()
        {
            LengthUnit[] units =
            {
                LengthUnit.FEET,
                LengthUnit.INCH,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER,
            };

            // Test values that are equivalent in base unit
            double[,] testValues = new double[,]
            {
                { 1.0, 12.0, 1.0 / 3.0, 30.48 }, // All equal to 1 foot
                { 2.0, 24.0, 2.0 / 3.0, 60.96 }, // All equal to 2 feet
                { 3.0, 36.0, 1.0, 91.44 }, // All equal to 3 feet
            };

            for (int i = 0; i < testValues.GetLength(0); i++)
            {
                for (int j = 0; j < units.Length; j++)
                {
                    for (int k = 0; k < units.Length; k++)
                    {
                        foreach (var target in units)
                        {
                            double val1 = testValues[i, j];
                            double val2 = testValues[i, k];

                            var result = Quantity.Add(val1, units[j], val2, units[k], target);

                            // The sum should be 2 times the value in target unit
                            double expectedInFeet = 2.0 * testValues[i, 0]; // 2 * feet value
                            double expectedInTarget = target.ConvertFromBaseUnit(expectedInFeet); // FIXED: Using new API

                            Assert.AreEqual(
                                expectedInTarget,
                                result.Value,
                                Tolerance,
                                $"Failed for {val1} {units[j]} + {val2} {units[k]} in {target}"
                            );
                            Assert.AreEqual(target, result.Unit);
                        }
                    }
                }
            }
        }

        #endregion

        #region Instance Method Tests with Explicit Target

        /// <summary>
        /// Tests instance Add method with explicit target unit.
        /// </summary>
        [TestMethod]
        public void Add_InstanceMethod_ExplicitTarget_ReturnsCorrectQuantity()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.YARD);

            double expected = 2.0 / 3.0; // 2 feet = 2/3 yards
            Assert.AreEqual(expected, result.Value, Tolerance);
            Assert.AreEqual(LengthUnit.YARD, result.Unit);
        }

        /// <summary>
        /// Tests instance Add method with explicit target and zero value.
        /// </summary>
        [TestMethod]
        public void Add_InstanceMethod_ExplicitTarget_ZeroValue_ReturnsCorrect()
        {
            var q1 = new Quantity(5.0, LengthUnit.FEET);
            var q2 = new Quantity(0.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.CENTIMETER);

            double expected = 5.0 * 30.48; // 5 feet = 152.4 cm
            Assert.AreEqual(expected, result.Value, Tolerance);
            Assert.AreEqual(LengthUnit.CENTIMETER, result.Unit);
        }

        #endregion

        #region Null and Invalid Input Tests

        /// <summary>
        /// Tests Add with null target unit.
        /// Verifies that ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_ExplicitTargetUnit_NullTargetUnit_ThrowsArgumentException()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);
            LengthUnit invalidUnit = (LengthUnit)99;

            q1.Add(q2, invalidUnit); // Will throw ArgumentException
        }

        /// <summary>
        /// Tests static Add with null first operand.
        /// Verifies that ArgumentNullException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Static_ExplicitTarget_NullFirstOperand_ThrowsArgumentNullException()
        {
            Quantity? q1 = null;
            var q2 = new Quantity(1.0, LengthUnit.FEET);

            Quantity.Add(q1!, q2, LengthUnit.FEET); // Will throw ArgumentNullException
        }

        /// <summary>
        /// Tests static Add with null second operand.
        /// Verifies that ArgumentNullException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Static_ExplicitTarget_NullSecondOperand_ThrowsArgumentNullException()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            Quantity? q2 = null;

            Quantity.Add(q1, q2!, LengthUnit.FEET); // Will throw ArgumentNullException
        }

        #endregion

        #region Precision Tests with Explicit Target

        /// <summary>
        /// Tests addition precision with explicit target unit.
        /// Verifies that multiple additions maintain precision within tolerance.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_PrecisionTolerance_MaintainsAccuracy()
        {
            var q1 = new Quantity(1.0 / 3.0, LengthUnit.FEET);
            var q2 = new Quantity(1.0 / 3.0, LengthUnit.FEET);

            var result = q1.Add(q2, LengthUnit.INCH);

            // 2/3 foot = 8 inches
            Assert.AreEqual(8.0, result.Value, Tolerance, "2/3 ft should equal 8 inches");
            Assert.AreEqual(LengthUnit.INCH, result.Unit);
        }

        /// <summary>
        /// Tests addition with irrational conversions and explicit target.
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTargetUnit_IrrationalConversions_MaintainsPrecision()
        {
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.CENTIMETER);

            var result = q1.Add(q2, LengthUnit.INCH);

            double expected = 2.0 * 0.393700787; // 2 cm in inches
            Assert.AreEqual(
                expected,
                result.Value,
                Tolerance,
                "2 cm should convert correctly to inches"
            );
            Assert.AreEqual(LengthUnit.INCH, result.Unit);
        }

        #endregion
    }
}
