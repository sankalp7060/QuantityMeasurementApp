using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.QuantityTests
{
    /// <summary>
    /// Test class for Quantity conversion operations.
    /// Covers UC5: Unit-to-unit conversion.
    /// </summary>
    [TestClass]
    public class QuantityConversionTests
    {
        private const double Tolerance = 0.000001;

        /// <summary>
        /// Tests ConvertTo method for feet to inches.
        /// </summary>
        [TestMethod]
        public void ConvertTo_FeetToInches_ReturnsCorrectQuantity()
        {
            // Arrange
            var feetQuantity = new Quantity(1.0, LengthUnit.FEET);

            // Act
            var convertedQuantity = feetQuantity.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(
                12.0,
                convertedQuantity.Value,
                Tolerance,
                "1 ft should convert to 12 in"
            );
            Assert.AreEqual(
                LengthUnit.INCH,
                convertedQuantity.Unit,
                "Result unit should be inches"
            );
        }

        /// <summary>
        /// Tests ConvertToDouble method for yards to feet.
        /// </summary>
        [TestMethod]
        public void ConvertToDouble_YardsToFeet_ReturnsCorrectValue()
        {
            // Arrange
            var yardQuantity = new Quantity(1.0, LengthUnit.YARD);

            // Act
            double convertedValue = yardQuantity.ConvertToDouble(LengthUnit.FEET);

            // Assert
            Assert.AreEqual(3.0, convertedValue, Tolerance, "1 yd should convert to 3 ft");
        }

        /// <summary>
        /// Tests round-trip conversion.
        /// </summary>
        [TestMethod]
        public void ConvertTo_RoundTrip_ReturnsOriginalValue()
        {
            // Arrange
            double originalValue = 5.0;
            var originalQuantity = new Quantity(originalValue, LengthUnit.FEET);

            // Act - Feet to Inches and back
            var inchesQuantity = originalQuantity.ConvertTo(LengthUnit.INCH);
            var backToFeetQuantity = inchesQuantity.ConvertTo(LengthUnit.FEET);

            // Assert
            Assert.AreEqual(
                originalValue,
                backToFeetQuantity.Value,
                Tolerance,
                "Round-trip feet->inches->feet should return original"
            );
        }

        /// <summary>
        /// Tests conversion with zero value.
        /// </summary>
        [TestMethod]
        public void ConvertTo_ZeroValue_ReturnsZero()
        {
            // Arrange
            var zeroQuantity = new Quantity(0.0, LengthUnit.FEET);

            // Act
            var convertedQuantity = zeroQuantity.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(0.0, convertedQuantity.Value, Tolerance, "0 ft should convert to 0 in");
        }

        /// <summary>
        /// Tests conversion with negative value.
        /// </summary>
        [TestMethod]
        public void ConvertTo_NegativeValue_PreservesSign()
        {
            // Arrange
            var negativeQuantity = new Quantity(-1.0, LengthUnit.FEET);

            // Act
            var convertedQuantity = negativeQuantity.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(
                -12.0,
                convertedQuantity.Value,
                Tolerance,
                "-1 ft should convert to -12 in"
            );
        }

        /// <summary>
        /// Tests conversion from yards to centimeters.
        /// </summary>
        [TestMethod]
        public void ConvertTo_YardsToCentimeters_ReturnsCorrectValue()
        {
            // Arrange
            var yardQuantity = new Quantity(1.0, LengthUnit.YARD);

            // Act
            var centimeterQuantity = yardQuantity.ConvertTo(LengthUnit.CENTIMETER);

            // Assert
            Assert.AreEqual(
                91.44,
                centimeterQuantity.Value,
                Tolerance,
                "1 yd should convert to 91.44 cm"
            );
        }

        /// <summary>
        /// Tests conversion from centimeters to inches.
        /// </summary>
        [TestMethod]
        public void ConvertTo_CentimetersToInches_ReturnsCorrectValue()
        {
            // Arrange
            var centimeterQuantity = new Quantity(2.54, LengthUnit.CENTIMETER);

            // Act
            var inchQuantity = centimeterQuantity.ConvertTo(LengthUnit.INCH);

            // Assert
            Assert.AreEqual(1.0, inchQuantity.Value, Tolerance, "2.54 cm should convert to 1 in");
        }
    }
}
