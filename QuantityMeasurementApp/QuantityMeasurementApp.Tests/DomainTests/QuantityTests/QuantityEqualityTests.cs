using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.QuantityTests
{
    /// <summary>
    /// Test class for Quantity equality operations.
    /// Covers UC1 (Feet), UC2 (Inches), UC3-UC4 (Extended units).
    /// </summary>
    [TestClass]
    public class QuantityEqualityTests
    {
        private const double Tolerance = 0.000001;

        /// <summary>
        /// Tests UC1: Feet equality with same value.
        /// </summary>
        [TestMethod]
        public void Equals_FeetSameValue_ReturnsTrue()
        {
            // Arrange
            var firstFeetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var secondFeetQuantity = new Quantity(1.0, LengthUnit.FEET);

            // Act
            bool areEqual = firstFeetQuantity.Equals(secondFeetQuantity);

            // Assert
            Assert.IsTrue(areEqual, "1.0 ft should equal 1.0 ft");
        }

        /// <summary>
        /// Tests UC1: Feet equality with different values.
        /// </summary>
        [TestMethod]
        public void Equals_FeetDifferentValue_ReturnsFalse()
        {
            // Arrange
            var firstFeetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var secondFeetQuantity = new Quantity(2.0, LengthUnit.FEET);

            // Act
            bool areEqual = firstFeetQuantity.Equals(secondFeetQuantity);

            // Assert
            Assert.IsFalse(areEqual, "1.0 ft should not equal 2.0 ft");
        }

        /// <summary>
        /// Tests UC2: Inch equality with same value.
        /// </summary>
        [TestMethod]
        public void Equals_InchSameValue_ReturnsTrue()
        {
            // Arrange
            var firstInchQuantity = new Quantity(1.0, LengthUnit.INCH);
            var secondInchQuantity = new Quantity(1.0, LengthUnit.INCH);

            // Act
            bool areEqual = firstInchQuantity.Equals(secondInchQuantity);

            // Assert
            Assert.IsTrue(areEqual, "1.0 in should equal 1.0 in");
        }

        /// <summary>
        /// Tests UC2: Inch equality with different values.
        /// </summary>
        [TestMethod]
        public void Equals_InchDifferentValue_ReturnsFalse()
        {
            // Arrange
            var firstInchQuantity = new Quantity(1.0, LengthUnit.INCH);
            var secondInchQuantity = new Quantity(2.0, LengthUnit.INCH);

            // Act
            bool areEqual = firstInchQuantity.Equals(secondInchQuantity);

            // Assert
            Assert.IsFalse(areEqual, "1.0 in should not equal 2.0 in");
        }

        /// <summary>
        /// Tests UC2: Cross-unit equality (1 ft = 12 in).
        /// </summary>
        [TestMethod]
        public void Equals_FeetToInchEquivalent_ReturnsTrue()
        {
            // Arrange
            var feetQuantity = new Quantity(1.0, LengthUnit.FEET);
            var inchQuantity = new Quantity(12.0, LengthUnit.INCH);

            // Act
            bool areEqual = feetQuantity.Equals(inchQuantity);

            // Assert
            Assert.IsTrue(areEqual, "1 ft should equal 12 in");
        }

        /// <summary>
        /// Tests UC3: Yard equality (1 yd = 3 ft).
        /// </summary>
        [TestMethod]
        public void Equals_YardToFeetEquivalent_ReturnsTrue()
        {
            // Arrange
            var yardQuantity = new Quantity(1.0, LengthUnit.YARD);
            var feetQuantity = new Quantity(3.0, LengthUnit.FEET);

            // Act
            bool areEqual = yardQuantity.Equals(feetQuantity);

            // Assert
            Assert.IsTrue(areEqual, "1 yd should equal 3 ft");
        }

        /// <summary>
        /// Tests UC3: Yard to inch equality (1 yd = 36 in).
        /// </summary>
        [TestMethod]
        public void Equals_YardToInchEquivalent_ReturnsTrue()
        {
            // Arrange
            var yardQuantity = new Quantity(1.0, LengthUnit.YARD);
            var inchQuantity = new Quantity(36.0, LengthUnit.INCH);

            // Act
            bool areEqual = yardQuantity.Equals(inchQuantity);

            // Assert
            Assert.IsTrue(areEqual, "1 yd should equal 36 in");
        }

        /// <summary>
        /// Tests UC4: Centimeter equality with same value.
        /// </summary>
        [TestMethod]
        public void Equals_CentimeterSameValue_ReturnsTrue()
        {
            // Arrange
            var firstCmQuantity = new Quantity(1.0, LengthUnit.CENTIMETER);
            var secondCmQuantity = new Quantity(1.0, LengthUnit.CENTIMETER);

            // Act
            bool areEqual = firstCmQuantity.Equals(secondCmQuantity);

            // Assert
            Assert.IsTrue(areEqual, "1.0 cm should equal 1.0 cm");
        }

        /// <summary>
        /// Tests UC4: Centimeter to inch equality (1 cm = 0.393701 in).
        /// </summary>
        [TestMethod]
        public void Equals_CentimeterToInchEquivalent_ReturnsTrue()
        {
            // Arrange
            var centimeterQuantity = new Quantity(1.0, LengthUnit.CENTIMETER);
            var inchQuantity = new Quantity(0.393700787, LengthUnit.INCH);

            // Act
            bool areEqual = centimeterQuantity.Equals(inchQuantity);

            // Assert
            Assert.IsTrue(areEqual, "1 cm should equal 0.393700787 in");
        }

        /// <summary>
        /// Tests UC4: Centimeter to feet equality (30.48 cm = 1 ft).
        /// </summary>
        [TestMethod]
        public void Equals_CentimeterToFeetEquivalent_ReturnsTrue()
        {
            // Arrange
            var centimeterQuantity = new Quantity(30.48, LengthUnit.CENTIMETER);
            var feetQuantity = new Quantity(1.0, LengthUnit.FEET);

            // Act
            bool areEqual = centimeterQuantity.Equals(feetQuantity);

            // Assert
            Assert.IsTrue(areEqual, "30.48 cm should equal 1 ft");
        }

        /// <summary>
        /// Tests UC4: Centimeter to yard equality (91.44 cm = 1 yd).
        /// </summary>
        [TestMethod]
        public void Equals_CentimeterToYardEquivalent_ReturnsTrue()
        {
            // Arrange
            var centimeterQuantity = new Quantity(91.44, LengthUnit.CENTIMETER);
            var yardQuantity = new Quantity(1.0, LengthUnit.YARD);

            // Act
            bool areEqual = centimeterQuantity.Equals(yardQuantity);

            // Assert
            Assert.IsTrue(areEqual, "91.44 cm should equal 1 yd");
        }

        /// <summary>
        /// Tests reflexive property: an object must equal itself.
        /// </summary>
        [TestMethod]
        public void Equals_Reflexive_ReturnsTrue()
        {
            // Arrange
            var quantity = new Quantity(1.0, LengthUnit.FEET);

            // Act
            bool isEqualToItself = quantity.Equals(quantity);

            // Assert
            Assert.IsTrue(isEqualToItself, "Object should equal itself");
        }

        /// <summary>
        /// Tests symmetric property: if a equals b then b equals a.
        /// </summary>
        [TestMethod]
        public void Equals_Symmetric_ReturnsTrue()
        {
            // Arrange
            var firstQuantity = new Quantity(1.0, LengthUnit.FEET);
            var secondQuantity = new Quantity(1.0, LengthUnit.FEET);

            // Act
            bool firstEqualsSecond = firstQuantity.Equals(secondQuantity);
            bool secondEqualsFirst = secondQuantity.Equals(firstQuantity);

            // Assert
            Assert.IsTrue(firstEqualsSecond && secondEqualsFirst, "Equality should be symmetric");
        }

        /// <summary>
        /// Tests transitive property: if a=b and b=c then a=c.
        /// </summary>
        [TestMethod]
        public void Equals_Transitive_ReturnsTrue()
        {
            // Arrange
            var firstQuantity = new Quantity(1.0, LengthUnit.FEET);
            var secondQuantity = new Quantity(1.0, LengthUnit.FEET);
            var thirdQuantity = new Quantity(1.0, LengthUnit.FEET);

            // Act
            bool firstEqualsSecond = firstQuantity.Equals(secondQuantity);
            bool secondEqualsThird = secondQuantity.Equals(thirdQuantity);
            bool firstEqualsThird = firstQuantity.Equals(thirdQuantity);

            // Assert
            Assert.IsTrue(
                firstEqualsSecond && secondEqualsThird && firstEqualsThird,
                "Equality should be transitive"
            );
        }

        /// <summary>
        /// Tests null comparison.
        /// </summary>
        [TestMethod]
        public void Equals_NullComparison_ReturnsFalse()
        {
            // Arrange
            var quantity = new Quantity(1.0, LengthUnit.FEET);

            // Act
            bool isEqualToNull = quantity.Equals(null);

            // Assert
            Assert.IsFalse(isEqualToNull, "Object should not equal null");
        }

        /// <summary>
        /// Tests different type comparison.
        /// </summary>
        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Arrange
            var quantity = new Quantity(1.0, LengthUnit.FEET);
            var differentObject = new object();

            // Act
            bool areEqual = quantity.Equals(differentObject);

            // Assert
            Assert.IsFalse(areEqual, "Quantity should not equal object of different type");
        }
    }
}
