using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.QuantityTests
{
    /// <summary>
    /// Test class for WeightQuantity operations.
    /// Covers UC9: Weight measurements equality, conversion, and addition.
    /// </summary>
    [TestClass]
    public class WeightQuantityTests
    {
        private const double Tolerance = 0.001; // Increased tolerance for pound conversions
        private const double HighPrecisionTolerance = 0.000001;

        #region Equality Tests

        /// <summary>
        /// Tests that two weights in kilograms with same value are equal.
        /// </summary>
        [TestMethod]
        public void Equals_KilogramSameValue_ReturnsTrue()
        {
            // Arrange
            var firstWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var secondWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);

            // Act
            bool areEqual = firstWeight.Equals(secondWeight);

            // Assert
            Assert.IsTrue(areEqual, "1 kg should equal 1 kg");
        }

        /// <summary>
        /// Tests that two weights in kilograms with different values are not equal.
        /// </summary>
        [TestMethod]
        public void Equals_KilogramDifferentValue_ReturnsFalse()
        {
            // Arrange
            var firstWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var secondWeight = new WeightQuantity(2.0, WeightUnit.KILOGRAM);

            // Act
            bool areEqual = firstWeight.Equals(secondWeight);

            // Assert
            Assert.IsFalse(areEqual, "1 kg should not equal 2 kg");
        }

        /// <summary>
        /// Tests that two weights in grams with same value are equal.
        /// </summary>
        [TestMethod]
        public void Equals_GramSameValue_ReturnsTrue()
        {
            // Arrange
            var firstWeight = new WeightQuantity(1000.0, WeightUnit.GRAM);
            var secondWeight = new WeightQuantity(1000.0, WeightUnit.GRAM);

            // Act
            bool areEqual = firstWeight.Equals(secondWeight);

            // Assert
            Assert.IsTrue(areEqual, "1000 g should equal 1000 g");
        }

        /// <summary>
        /// Tests cross-unit equality: 1 kg = 1000 g.
        /// </summary>
        [TestMethod]
        public void Equals_KilogramToGramEquivalent_ReturnsTrue()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(1000.0, WeightUnit.GRAM);

            // Act
            bool areEqual = kgWeight.Equals(gWeight);

            // Assert
            Assert.IsTrue(areEqual, "1 kg should equal 1000 g");
        }

        /// <summary>
        /// Tests cross-unit equality: 1 kg ≈ 2.20462 lb.
        /// FIXED: Using more precise value and appropriate tolerance
        /// </summary>
        [TestMethod]
        public void Equals_KilogramToPoundEquivalent_ReturnsTrue()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var lbWeight = new WeightQuantity(2.20462262185, WeightUnit.POUND); // More precise value

            // Act
            bool areEqual = kgWeight.Equals(lbWeight);

            // Assert
            Assert.IsTrue(areEqual, "1 kg should approximately equal 2.20462 lb");
        }

        /// <summary>
        /// Tests cross-unit equality: 453.592 g ≈ 1 lb.
        /// </summary>
        [TestMethod]
        public void Equals_GramToPoundEquivalent_ReturnsTrue()
        {
            // Arrange
            var gWeight = new WeightQuantity(453.59237, WeightUnit.GRAM); // More precise value
            var lbWeight = new WeightQuantity(1.0, WeightUnit.POUND);

            // Act
            bool areEqual = gWeight.Equals(lbWeight);

            // Assert
            Assert.IsTrue(areEqual, "453.592 g should approximately equal 1 lb");
        }

        /// <summary>
        /// Tests reflexive property.
        /// </summary>
        [TestMethod]
        public void Equals_Reflexive_ReturnsTrue()
        {
            // Arrange
            var weight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);

            // Act
            bool isEqualToItself = weight.Equals(weight);

            // Assert
            Assert.IsTrue(isEqualToItself, "Object should equal itself");
        }

        /// <summary>
        /// Tests symmetric property.
        /// </summary>
        [TestMethod]
        public void Equals_Symmetric_ReturnsTrue()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(1000.0, WeightUnit.GRAM);

            // Act
            bool kgEqualsG = kgWeight.Equals(gWeight);
            bool gEqualsKg = gWeight.Equals(kgWeight);

            // Assert
            Assert.IsTrue(kgEqualsG && gEqualsKg, "Equality should be symmetric");
        }

        /// <summary>
        /// Tests null comparison.
        /// </summary>
        [TestMethod]
        public void Equals_NullComparison_ReturnsFalse()
        {
            // Arrange
            var weight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);

            // Act
            bool isEqualToNull = weight.Equals(null);

            // Assert
            Assert.IsFalse(isEqualToNull, "Object should not equal null");
        }

        /// <summary>
        /// Tests that weight and length are not equal (different categories).
        /// </summary>
        [TestMethod]
        public void Equals_WeightVsLength_ReturnsFalse()
        {
            // Arrange
            var weight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var length = new Quantity(1.0, LengthUnit.FEET);

            // Act
            bool areEqual = weight.Equals(length);

            // Assert
            Assert.IsFalse(areEqual, "Weight and length should not be equal");
        }

        #endregion

        #region Conversion Tests

        /// <summary>
        /// Tests ConvertTo for kilograms to grams.
        /// </summary>
        [TestMethod]
        public void ConvertTo_KilogramsToGrams_ReturnsCorrectWeight()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);

            // Act
            var gWeight = kgWeight.ConvertTo(WeightUnit.GRAM);

            // Assert
            Assert.AreEqual(1000.0, gWeight.Value, Tolerance, "1 kg should convert to 1000 g");
            Assert.AreEqual(WeightUnit.GRAM, gWeight.Unit, "Unit should be grams");
        }

        /// <summary>
        /// Tests ConvertTo for grams to kilograms.
        /// </summary>
        [TestMethod]
        public void ConvertTo_GramsToKilograms_ReturnsCorrectWeight()
        {
            // Arrange
            var gWeight = new WeightQuantity(1000.0, WeightUnit.GRAM);

            // Act
            var kgWeight = gWeight.ConvertTo(WeightUnit.KILOGRAM);

            // Assert
            Assert.AreEqual(1.0, kgWeight.Value, Tolerance, "1000 g should convert to 1 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, kgWeight.Unit, "Unit should be kilograms");
        }

        /// <summary>
        /// Tests ConvertTo for kilograms to pounds.
        /// </summary>
        [TestMethod]
        public void ConvertTo_KilogramsToPounds_ReturnsCorrectWeight()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);

            // Act
            var lbWeight = kgWeight.ConvertTo(WeightUnit.POUND);

            // Assert
            Assert.AreEqual(
                2.20462,
                lbWeight.Value,
                0.001,
                "1 kg should convert to approximately 2.20462 lb"
            );
            Assert.AreEqual(WeightUnit.POUND, lbWeight.Unit, "Unit should be pounds");
        }

        /// <summary>
        /// Tests ConvertTo for pounds to kilograms.
        /// </summary>
        [TestMethod]
        public void ConvertTo_PoundsToKilograms_ReturnsCorrectWeight()
        {
            // Arrange
            var lbWeight = new WeightQuantity(2.20462, WeightUnit.POUND);

            // Act
            var kgWeight = lbWeight.ConvertTo(WeightUnit.KILOGRAM);

            // Assert
            Assert.AreEqual(
                1.0,
                kgWeight.Value,
                0.001,
                "2.20462 lb should convert to approximately 1 kg"
            );
            Assert.AreEqual(WeightUnit.KILOGRAM, kgWeight.Unit, "Unit should be kilograms");
        }

        /// <summary>
        /// Tests ConvertTo for grams to pounds.
        /// </summary>
        [TestMethod]
        public void ConvertTo_GramsToPounds_ReturnsCorrectWeight()
        {
            // Arrange
            var gWeight = new WeightQuantity(453.592, WeightUnit.GRAM);

            // Act
            var lbWeight = gWeight.ConvertTo(WeightUnit.POUND);

            // Assert
            Assert.AreEqual(
                1.0,
                lbWeight.Value,
                0.001,
                "453.592 g should convert to approximately 1 lb"
            );
            Assert.AreEqual(WeightUnit.POUND, lbWeight.Unit, "Unit should be pounds");
        }

        /// <summary>
        /// Tests ConvertToDouble method.
        /// </summary>
        [TestMethod]
        public void ConvertToDouble_KilogramsToGrams_ReturnsCorrectValue()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);

            // Act
            double gramsValue = kgWeight.ConvertToDouble(WeightUnit.GRAM);

            // Assert
            Assert.AreEqual(1000.0, gramsValue, Tolerance, "1 kg should convert to 1000 g");
        }

        /// <summary>
        /// Tests round-trip conversion.
        /// </summary>
        [TestMethod]
        public void ConvertTo_RoundTrip_ReturnsOriginalValue()
        {
            // Arrange
            double originalValue = 2.5;
            var originalWeight = new WeightQuantity(originalValue, WeightUnit.KILOGRAM);

            // Act
            var inGrams = originalWeight.ConvertTo(WeightUnit.GRAM);
            var backToKg = inGrams.ConvertTo(WeightUnit.KILOGRAM);

            // Assert
            Assert.AreEqual(
                originalValue,
                backToKg.Value,
                Tolerance,
                "Round-trip kg->g->kg should return original"
            );
        }

        #endregion

        #region Addition Tests

        /// <summary>
        /// Tests addition of two weights in same unit (kilograms).
        /// </summary>
        [TestMethod]
        public void Add_SameUnit_Kilograms_ReturnsCorrectSum()
        {
            // Arrange
            var firstWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var secondWeight = new WeightQuantity(2.0, WeightUnit.KILOGRAM);

            // Act
            var sumWeight = firstWeight.Add(secondWeight);

            // Assert
            Assert.AreEqual(3.0, sumWeight.Value, Tolerance, "1 kg + 2 kg should equal 3 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, sumWeight.Unit, "Result should be in kilograms");
        }

        /// <summary>
        /// Tests addition of two weights in same unit (grams).
        /// </summary>
        [TestMethod]
        public void Add_SameUnit_Grams_ReturnsCorrectSum()
        {
            // Arrange
            var firstWeight = new WeightQuantity(500.0, WeightUnit.GRAM);
            var secondWeight = new WeightQuantity(500.0, WeightUnit.GRAM);

            // Act
            var sumWeight = firstWeight.Add(secondWeight);

            // Assert
            Assert.AreEqual(
                1000.0,
                sumWeight.Value,
                Tolerance,
                "500 g + 500 g should equal 1000 g"
            );
            Assert.AreEqual(WeightUnit.GRAM, sumWeight.Unit, "Result should be in grams");
        }

        /// <summary>
        /// Tests addition of weights in different units (kg + g) with result in first unit.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_ResultInFirstUnit_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(500.0, WeightUnit.GRAM);

            // Act
            var sumWeight = kgWeight.Add(gWeight);

            // Assert
            Assert.AreEqual(1.5, sumWeight.Value, Tolerance, "1 kg + 500 g should equal 1.5 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, sumWeight.Unit, "Result should be in kilograms");
        }

        /// <summary>
        /// Tests addition of weights in different units (kg + g) with result in second unit.
        /// </summary>
        [TestMethod]
        public void Add_CrossUnit_ResultInSecondUnit_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(500.0, WeightUnit.GRAM);

            // Act
            var sumWeight = kgWeight.Add(gWeight, WeightUnit.GRAM);

            // Assert
            Assert.AreEqual(
                1500.0,
                sumWeight.Value,
                Tolerance,
                "1 kg + 500 g in grams should equal 1500 g"
            );
            Assert.AreEqual(WeightUnit.GRAM, sumWeight.Unit, "Result should be in grams");
        }

        /// <summary>
        /// Tests addition with explicit target unit (pounds).
        /// </summary>
        [TestMethod]
        public void Add_ExplicitTarget_Pounds_ReturnsCorrectSum()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(500.0, WeightUnit.GRAM);

            // Act
            var sumInPounds = kgWeight.Add(gWeight, WeightUnit.POUND);

            // Assert
            double expectedValue = 1.5 * 2.20462; // 1.5 kg in pounds
            Assert.AreEqual(
                expectedValue,
                sumInPounds.Value,
                0.001,
                "1 kg + 500 g in pounds should be correct"
            );
            Assert.AreEqual(WeightUnit.POUND, sumInPounds.Unit, "Result should be in pounds");
        }

        /// <summary>
        /// Tests static Add method.
        /// </summary>
        [TestMethod]
        public void Add_Static_ReturnsCorrectSum()
        {
            // Act
            var sumWeight = WeightQuantity.Add(
                1.0,
                WeightUnit.KILOGRAM,
                1000.0,
                WeightUnit.GRAM,
                WeightUnit.KILOGRAM
            );

            // Assert
            Assert.AreEqual(2.0, sumWeight.Value, Tolerance, "1 kg + 1000 g should equal 2 kg");
            Assert.AreEqual(WeightUnit.KILOGRAM, sumWeight.Unit, "Result should be in kilograms");
        }

        /// <summary>
        /// Tests addition with zero.
        /// </summary>
        [TestMethod]
        public void Add_WithZero_ReturnsOriginalValue()
        {
            // Arrange
            var originalWeight = new WeightQuantity(5.0, WeightUnit.KILOGRAM);
            var zeroWeight = new WeightQuantity(0.0, WeightUnit.GRAM);

            // Act
            var sumWeight = originalWeight.Add(zeroWeight);

            // Assert
            Assert.AreEqual(5.0, sumWeight.Value, Tolerance, "5 kg + 0 g should equal 5 kg");
        }

        /// <summary>
        /// Tests addition with negative values.
        /// </summary>
        [TestMethod]
        public void Add_WithNegativeValues_ReturnsCorrectSum()
        {
            // Arrange
            var positiveWeight = new WeightQuantity(5.0, WeightUnit.KILOGRAM);
            var negativeWeight = new WeightQuantity(-2000.0, WeightUnit.GRAM);

            // Act
            var sumWeight = positiveWeight.Add(negativeWeight);

            // Assert
            Assert.AreEqual(3.0, sumWeight.Value, Tolerance, "5 kg + (-2000 g) should equal 3 kg");
        }

        /// <summary>
        /// Tests commutativity property.
        /// </summary>
        [TestMethod]
        public void Add_IsCommutative_ReturnsTrue()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(1000.0, WeightUnit.GRAM);
            var targetUnit = WeightUnit.POUND;

            // Act
            var firstSum = kgWeight.Add(gWeight, targetUnit);
            var secondSum = gWeight.Add(kgWeight, targetUnit);

            // Assert
            Assert.AreEqual(
                firstSum.Value,
                secondSum.Value,
                Tolerance,
                "a + b should equal b + a when using same target unit"
            );
        }

        /// <summary>
        /// Tests that adding null throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullOperand_ThrowsException()
        {
            // Arrange
            var validWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);

            // Act - Should throw
            validWeight.Add(null!);
        }

        #endregion

        #region Edge Cases Tests

        /// <summary>
        /// Tests equality with zero values across units.
        /// </summary>
        [TestMethod]
        public void Equals_ZeroValue_AllUnitsEqual()
        {
            // Arrange
            var zeroKg = new WeightQuantity(0.0, WeightUnit.KILOGRAM);
            var zeroG = new WeightQuantity(0.0, WeightUnit.GRAM);
            var zeroLb = new WeightQuantity(0.0, WeightUnit.POUND);

            // Act & Assert
            Assert.IsTrue(zeroKg.Equals(zeroG), "0 kg should equal 0 g");
            Assert.IsTrue(zeroKg.Equals(zeroLb), "0 kg should equal 0 lb");
            Assert.IsTrue(zeroG.Equals(zeroLb), "0 g should equal 0 lb");
        }

        /// <summary>
        /// Tests that invalid value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void Constructor_NaNValue_ThrowsException()
        {
            // Act - Should throw
            var invalidWeight = new WeightQuantity(double.NaN, WeightUnit.KILOGRAM);
        }

        /// <summary>
        /// Tests that infinite value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void Constructor_InfinityValue_ThrowsException()
        {
            // Act - Should throw
            var invalidWeight = new WeightQuantity(double.PositiveInfinity, WeightUnit.KILOGRAM);
        }

        /// <summary>
        /// Tests that invalid unit throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidUnitException))]
        public void Constructor_InvalidUnit_ThrowsException()
        {
            // Arrange
            WeightUnit invalidUnit = (WeightUnit)99;

            // Act - Should throw
            var invalidWeight = new WeightQuantity(1.0, invalidUnit);
        }

        #endregion

        #region GetHashCode Tests

        /// <summary>
        /// Tests that GetHashCode returns same value for equal objects.
        /// </summary>
        [TestMethod]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(1000.0, WeightUnit.GRAM);

            // Act
            int kgHash = kgWeight.GetHashCode();
            int gHash = gWeight.GetHashCode();

            // Assert
            Assert.AreEqual(kgHash, gHash, "Equal weights should have equal hash codes");
        }

        /// <summary>
        /// Tests that GetHashCode returns different values for different objects.
        /// </summary>
        [TestMethod]
        public void GetHashCode_DifferentObjects_ReturnsDifferentHashCode()
        {
            // Arrange
            var kgWeight1 = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var kgWeight2 = new WeightQuantity(2.0, WeightUnit.KILOGRAM);

            // Act
            int hash1 = kgWeight1.GetHashCode();
            int hash2 = kgWeight2.GetHashCode();

            // Assert
            Assert.AreNotEqual(hash1, hash2, "Different weights should have different hash codes");
        }

        #endregion

        #region ToString Tests

        /// <summary>
        /// Tests ToString returns correct format for each unit.
        /// </summary>
        [TestMethod]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var kgWeight = new WeightQuantity(1.5, WeightUnit.KILOGRAM);
            var gWeight = new WeightQuantity(500.0, WeightUnit.GRAM);
            var lbWeight = new WeightQuantity(2.2, WeightUnit.POUND);

            // Assert
            Assert.AreEqual("1.5 kg", kgWeight.ToString());
            Assert.AreEqual("500 g", gWeight.ToString());
            Assert.AreEqual("2.2 lb", lbWeight.ToString());
        }

        #endregion
    }
}
