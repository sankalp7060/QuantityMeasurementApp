using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.GenericQuantityTests
{
    /// <summary>
    /// Test class for GenericQuantity equality operations across all measurement categories.
    /// UC10: Tests equality functionality for Length, Weight, and demonstrates extensibility.
    /// Verifies value-based equality across all units of same category.
    /// </summary>
    [TestClass]
    public class GenericQuantityEqualityTests
    {
        private const double Tolerance = 0.000001;
        private const double PoundTolerance = 0.001;

        #region Length Category Equality Tests

        /// <summary>
        /// Tests that two length quantities in feet with same value are equal.
        /// Verifies UC1 functionality preserved in generic implementation.
        /// </summary>
        [TestMethod]
        public void Equals_Length_FeetSameValue_ReturnsTrue()
        {
            // Arrange
            var firstLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var secondLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act
            bool areEqual = firstLength.Equals(secondLength);

            // Assert
            Assert.IsTrue(areEqual, "1 ft should equal 1 ft");
        }

        /// <summary>
        /// Tests that two length quantities in feet with different values are not equal.
        /// </summary>
        [TestMethod]
        public void Equals_Length_FeetDifferentValue_ReturnsFalse()
        {
            // Arrange
            var firstLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var secondLength = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);

            // Act
            bool areEqual = firstLength.Equals(secondLength);

            // Assert
            Assert.IsFalse(areEqual, "1 ft should not equal 2 ft");
        }

        /// <summary>
        /// Tests cross-unit equality for length: 1 ft = 12 in.
        /// Verifies UC2 functionality preserved.
        /// </summary>
        [TestMethod]
        public void Equals_Length_FeetToInchEquivalent_ReturnsTrue()
        {
            // Arrange
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act
            bool areEqual = feetLength.Equals(inchesLength);

            // Assert
            Assert.IsTrue(areEqual, "1 ft should equal 12 in");
        }

        /// <summary>
        /// Tests cross-unit equality for length: 1 yd = 3 ft.
        /// Verifies UC3 functionality preserved.
        /// </summary>
        [TestMethod]
        public void Equals_Length_YardToFeetEquivalent_ReturnsTrue()
        {
            // Arrange
            var yardLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.YARD);
            var feetLength = new GenericQuantity<LengthUnit>(3.0, LengthUnit.FEET);

            // Act
            bool areEqual = yardLength.Equals(feetLength);

            // Assert
            Assert.IsTrue(areEqual, "1 yd should equal 3 ft");
        }

        /// <summary>
        /// Tests cross-unit equality for length: 1 yd = 36 in.
        /// </summary>
        [TestMethod]
        public void Equals_Length_YardToInchEquivalent_ReturnsTrue()
        {
            // Arrange
            var yardLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.YARD);
            var inchesLength = new GenericQuantity<LengthUnit>(36.0, LengthUnit.INCH);

            // Act
            bool areEqual = yardLength.Equals(inchesLength);

            // Assert
            Assert.IsTrue(areEqual, "1 yd should equal 36 in");
        }

        /// <summary>
        /// Tests cross-unit equality for length: 1 cm = 0.393701 in.
        /// Verifies UC4 functionality preserved.
        /// </summary>
        [TestMethod]
        public void Equals_Length_CentimeterToInchEquivalent_ReturnsTrue()
        {
            // Arrange
            var cmLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.CENTIMETER);
            var inchesLength = new GenericQuantity<LengthUnit>(0.393700787, LengthUnit.INCH);

            // Act
            bool areEqual = cmLength.Equals(inchesLength);

            // Assert
            Assert.IsTrue(areEqual, "1 cm should equal 0.393700787 in");
        }

        /// <summary>
        /// Tests cross-unit equality for length: 30.48 cm = 1 ft.
        /// </summary>
        [TestMethod]
        public void Equals_Length_CentimeterToFeetEquivalent_ReturnsTrue()
        {
            // Arrange
            var cmLength = new GenericQuantity<LengthUnit>(30.48, LengthUnit.CENTIMETER);
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);

            // Act
            bool areEqual = cmLength.Equals(feetLength);

            // Assert
            Assert.IsTrue(areEqual, "30.48 cm should equal 1 ft");
        }

        /// <summary>
        /// Tests cross-unit equality for length: 91.44 cm = 1 yd.
        /// </summary>
        [TestMethod]
        public void Equals_Length_CentimeterToYardEquivalent_ReturnsTrue()
        {
            // Arrange
            var cmLength = new GenericQuantity<LengthUnit>(91.44, LengthUnit.CENTIMETER);
            var yardLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.YARD);

            // Act
            bool areEqual = cmLength.Equals(yardLength);

            // Assert
            Assert.IsTrue(areEqual, "91.44 cm should equal 1 yd");
        }

        #endregion

        #region Weight Category Equality Tests

        /// <summary>
        /// Tests that two weight quantities in kilograms with same value are equal.
        /// Verifies UC9 functionality preserved in generic implementation.
        /// </summary>
        [TestMethod]
        public void Equals_Weight_KilogramSameValue_ReturnsTrue()
        {
            // Arrange
            var firstWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var secondWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            bool areEqual = firstWeight.Equals(secondWeight);

            // Assert
            Assert.IsTrue(areEqual, "1 kg should equal 1 kg");
        }

        /// <summary>
        /// Tests that two weight quantities in kilograms with different values are not equal.
        /// </summary>
        [TestMethod]
        public void Equals_Weight_KilogramDifferentValue_ReturnsFalse()
        {
            // Arrange
            var firstWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var secondWeight = new GenericQuantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);

            // Act
            bool areEqual = firstWeight.Equals(secondWeight);

            // Assert
            Assert.IsFalse(areEqual, "1 kg should not equal 2 kg");
        }

        /// <summary>
        /// Tests cross-unit equality for weight: 1 kg = 1000 g.
        /// </summary>
        [TestMethod]
        public void Equals_Weight_KilogramToGramEquivalent_ReturnsTrue()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            // Act
            bool areEqual = kgWeight.Equals(gWeight);

            // Assert
            Assert.IsTrue(areEqual, "1 kg should equal 1000 g");
        }

        /// <summary>
        /// Tests cross-unit equality for weight: 1 kg ≈ 2.20462 lb.
        /// </summary>
        [TestMethod]
        public void Equals_Weight_KilogramToPoundEquivalent_ReturnsTrue()
        {
            // Arrange
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var lbWeight = new GenericQuantity<WeightUnit>(2.20462262185, WeightUnit.POUND);

            // Act
            bool areEqual = kgWeight.Equals(lbWeight);

            // Assert
            Assert.IsTrue(areEqual, "1 kg should approximately equal 2.20462 lb");
        }

        /// <summary>
        /// Tests cross-unit equality for weight: 453.592 g ≈ 1 lb.
        /// </summary>
        [TestMethod]
        public void Equals_Weight_GramToPoundEquivalent_ReturnsTrue()
        {
            // Arrange
            var gWeight = new GenericQuantity<WeightUnit>(453.59237, WeightUnit.GRAM);
            var lbWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.POUND);

            // Act
            bool areEqual = gWeight.Equals(lbWeight);

            // Assert
            Assert.IsTrue(areEqual, "453.592 g should approximately equal 1 lb");
        }

        #endregion

        #region Equality Contract Tests

        /// <summary>
        /// Tests reflexive property: an object must equal itself.
        /// </summary>
        [TestMethod]
        public void Equals_Reflexive_ReturnsTrue()
        {
            // Arrange
            var length = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var weight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act & Assert
            Assert.IsTrue(length.Equals(length), "Length should equal itself");
            Assert.IsTrue(weight.Equals(weight), "Weight should equal itself");
        }

        /// <summary>
        /// Tests symmetric property: if a equals b then b equals a.
        /// </summary>
        [TestMethod]
        public void Equals_Symmetric_ReturnsTrue()
        {
            // Arrange - Length
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act - Length
            bool feetEqualsInches = feetLength.Equals(inchesLength);
            bool inchesEqualsFeet = inchesLength.Equals(feetLength);

            // Assert - Length
            Assert.IsTrue(
                feetEqualsInches && inchesEqualsFeet,
                "Length equality should be symmetric"
            );

            // Arrange - Weight
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            // Act - Weight
            bool kgEqualsG = kgWeight.Equals(gWeight);
            bool gEqualsKg = gWeight.Equals(kgWeight);

            // Assert - Weight
            Assert.IsTrue(kgEqualsG && gEqualsKg, "Weight equality should be symmetric");
        }

        /// <summary>
        /// Tests transitive property: if a=b and b=c then a=c.
        /// </summary>
        [TestMethod]
        public void Equals_Transitive_ReturnsTrue()
        {
            // Arrange - Length
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);
            var cmLength = new GenericQuantity<LengthUnit>(30.48, LengthUnit.CENTIMETER);

            // Act - Length
            bool feetEqualsInches = feetLength.Equals(inchesLength);
            bool inchesEqualsCm = inchesLength.Equals(cmLength);
            bool feetEqualsCm = feetLength.Equals(cmLength);

            // Assert - Length
            Assert.IsTrue(
                feetEqualsInches && inchesEqualsCm && feetEqualsCm,
                "Length equality should be transitive"
            );

            // Arrange - Weight
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);
            var lbWeight = new GenericQuantity<WeightUnit>(2.20462262185, WeightUnit.POUND);

            // Act - Weight
            bool kgEqualsG = kgWeight.Equals(gWeight);
            bool gEqualsLb = gWeight.Equals(lbWeight);
            bool kgEqualsLb = kgWeight.Equals(lbWeight);

            // Assert - Weight
            Assert.IsTrue(
                kgEqualsG && gEqualsLb && kgEqualsLb,
                "Weight equality should be transitive"
            );
        }

        /// <summary>
        /// Tests null comparison.
        /// </summary>
        [TestMethod]
        public void Equals_NullComparison_ReturnsFalse()
        {
            // Arrange
            var length = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var weight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act & Assert
            Assert.IsFalse(length.Equals(null), "Length should not equal null");
            Assert.IsFalse(weight.Equals(null), "Weight should not equal null");
        }

        #endregion

        #region Cross-Category Prevention Tests

        /// <summary>
        /// Tests that quantities from different categories are not equal.
        /// Verifies type safety across categories.
        /// </summary>
        [TestMethod]
        public void Equals_DifferentCategories_ReturnsFalse()
        {
            // Arrange
            var length = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var weight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            // Note: This comparison should be prevented by generics at compile time,
            // but we're testing the runtime behavior if somehow compared as objects
            bool areEqual = length.Equals(weight);

            // Assert
            Assert.IsFalse(areEqual, "Length and weight should not be equal");
            Assert.AreNotEqual(length.GetType(), weight.GetType(), "Types should be different");
        }

        /// <summary>
        /// Tests that the generic type parameter ensures category safety.
        /// </summary>
        [TestMethod]
        public void GenericTypeParameter_EnsuresCategorySafety()
        {
            // Arrange - This code should compile without errors
            var length1 = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var length2 = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            var weight1 = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var weight2 = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            // Act - Same category comparisons are allowed
            bool lengthEqual = length1.Equals(length2);
            bool weightEqual = weight1.Equals(weight2);

            // Assert
            Assert.IsTrue(lengthEqual, "Length comparison should work");
            Assert.IsTrue(weightEqual, "Weight comparison should work");

            // The following line would cause a compile-time error if uncommented:
            // bool invalid = length1.Equals(weight1); // Compiler error - good!
        }

        #endregion

        #region Zero and Edge Cases

        /// <summary>
        /// Tests equality with zero values across units.
        /// </summary>
        [TestMethod]
        public void Equals_ZeroValue_AllUnitsEqual()
        {
            // Arrange - Length
            var zeroFeet = new GenericQuantity<LengthUnit>(0.0, LengthUnit.FEET);
            var zeroInches = new GenericQuantity<LengthUnit>(0.0, LengthUnit.INCH);
            var zeroYards = new GenericQuantity<LengthUnit>(0.0, LengthUnit.YARD);
            var zeroCm = new GenericQuantity<LengthUnit>(0.0, LengthUnit.CENTIMETER);

            // Act & Assert - Length
            Assert.IsTrue(zeroFeet.Equals(zeroInches), "0 ft should equal 0 in");
            Assert.IsTrue(zeroFeet.Equals(zeroYards), "0 ft should equal 0 yd");
            Assert.IsTrue(zeroFeet.Equals(zeroCm), "0 ft should equal 0 cm");

            // Arrange - Weight
            var zeroKg = new GenericQuantity<WeightUnit>(0.0, WeightUnit.KILOGRAM);
            var zeroG = new GenericQuantity<WeightUnit>(0.0, WeightUnit.GRAM);
            var zeroLb = new GenericQuantity<WeightUnit>(0.0, WeightUnit.POUND);

            // Act & Assert - Weight
            Assert.IsTrue(zeroKg.Equals(zeroG), "0 kg should equal 0 g");
            Assert.IsTrue(zeroKg.Equals(zeroLb), "0 kg should equal 0 lb");
            Assert.IsTrue(zeroG.Equals(zeroLb), "0 g should equal 0 lb");
        }

        /// <summary>
        /// Tests equality with negative values.
        /// </summary>
        [TestMethod]
        public void Equals_NegativeValues_PreservesSign()
        {
            // Arrange - Length
            var negativeFeet = new GenericQuantity<LengthUnit>(-1.0, LengthUnit.FEET);
            var negativeInches = new GenericQuantity<LengthUnit>(-12.0, LengthUnit.INCH);

            // Act & Assert - Length
            Assert.IsTrue(negativeFeet.Equals(negativeInches), "-1 ft should equal -12 in");

            // Arrange - Weight
            var negativeKg = new GenericQuantity<WeightUnit>(-1.0, WeightUnit.KILOGRAM);
            var negativeG = new GenericQuantity<WeightUnit>(-1000.0, WeightUnit.GRAM);

            // Act & Assert - Weight
            Assert.IsTrue(negativeKg.Equals(negativeG), "-1 kg should equal -1000 g");
        }

        #endregion

        #region GetHashCode Tests

        /// <summary>
        /// Tests that GetHashCode returns same value for equal objects.
        /// </summary>
        [TestMethod]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode()
        {
            // Arrange - Length
            var feetLength = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesLength = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Act - Length
            int feetHash = feetLength.GetHashCode();
            int inchesHash = inchesLength.GetHashCode();

            // Assert - Length
            Assert.AreEqual(feetHash, inchesHash, "Equal lengths should have equal hash codes");

            // Arrange - Weight
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            // Act - Weight
            int kgHash = kgWeight.GetHashCode();
            int gHash = gWeight.GetHashCode();

            // Assert - Weight
            Assert.AreEqual(kgHash, gHash, "Equal weights should have equal hash codes");
        }

        /// <summary>
        /// Tests that GetHashCode returns different values for different objects.
        /// </summary>
        [TestMethod]
        public void GetHashCode_DifferentObjects_ReturnsDifferentHashCode()
        {
            // Arrange - Length
            var feet1 = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var feet2 = new GenericQuantity<LengthUnit>(2.0, LengthUnit.FEET);

            // Act - Length
            int hash1 = feet1.GetHashCode();
            int hash2 = feet2.GetHashCode();

            // Assert - Length
            Assert.AreNotEqual(hash1, hash2, "Different lengths should have different hash codes");

            // Arrange - Weight
            var kg1 = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var kg2 = new GenericQuantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);

            // Act - Weight
            int kgHash1 = kg1.GetHashCode();
            int kgHash2 = kg2.GetHashCode();

            // Assert - Weight
            Assert.AreNotEqual(
                kgHash1,
                kgHash2,
                "Different weights should have different hash codes"
            );
        }

        #endregion
    }
}
