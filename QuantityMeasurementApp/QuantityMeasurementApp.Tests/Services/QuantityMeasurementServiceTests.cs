using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Tests.Services
{
    [TestClass]
    public class QuantityMeasurementServiceTests
    {
        private QuantityMeasurementService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _service = new QuantityMeasurementService();
        }

        #region Generic Quantity Tests

        [TestMethod]
        public void CompareQuantityEquality_BothNonNullEqualValues_SameUnit_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_BothNonNullDifferentValues_SameUnit_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(2.0, LengthUnit.FEET);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_CrossUnitEquivalentValues_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_CrossUnitNonEquivalentValues_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(13.0, LengthUnit.INCH);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_FirstParameterNull_ReturnsFalse()
        {
            Quantity? q1 = null;
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_SecondParameterNull_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            Quantity? q2 = null;
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_BothParametersNull_ReturnsFalse()
        {
            Quantity? q1 = null;
            Quantity? q2 = null;
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ParseQuantityInput_ValidNumericString_ReturnsQuantityObject()
        {
            string input = "3.5";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNotNull(result);
            Assert.AreEqual(3.5, result!.Value);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        [TestMethod]
        public void ParseQuantityInput_NullInput_ReturnsNull()
        {
            string? input = null;
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseQuantityInput_EmptyString_ReturnsNull()
        {
            string input = "";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseQuantityInput_Whitespace_ReturnsNull()
        {
            string input = "   ";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseQuantityInput_NonNumericString_ReturnsNull()
        {
            string input = "abc";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseQuantityInput_NegativeNumber_ReturnsQuantityObject()
        {
            string input = "-2.5";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNotNull(result);
            Assert.AreEqual(-2.5, result!.Value);
        }

        [TestMethod]
        public void ParseQuantityInput_Zero_ReturnsQuantityObject()
        {
            string input = "0";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result!.Value);
        }

        [TestMethod]
        public void AreQuantitiesEqual_EqualValues_SameUnit_ReturnsTrue()
        {
            bool result = QuantityMeasurementService.AreQuantitiesEqual(
                1.0,
                LengthUnit.FEET,
                1.0,
                LengthUnit.FEET
            );
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreQuantitiesEqual_DifferentValues_SameUnit_ReturnsFalse()
        {
            bool result = QuantityMeasurementService.AreQuantitiesEqual(
                1.0,
                LengthUnit.FEET,
                2.0,
                LengthUnit.FEET
            );
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AreQuantitiesEqual_CrossUnitEquivalentValues_ReturnsTrue()
        {
            bool result = QuantityMeasurementService.AreQuantitiesEqual(
                1.0,
                LengthUnit.FEET,
                12.0,
                LengthUnit.INCH
            );
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreQuantitiesEqual_CrossUnitNonEquivalentValues_ReturnsFalse()
        {
            bool result = QuantityMeasurementService.AreQuantitiesEqual(
                1.0,
                LengthUnit.FEET,
                13.0,
                LengthUnit.INCH
            );
            Assert.IsFalse(result);
        }

        #endregion

        #region Backward Compatibility Tests (Feet)

        [TestMethod]
        public void CompareFeetEquality_BothNonNullEqualValues_ReturnsTrue()
        {
            var feet1 = new Feet(1.0);
            var feet2 = new Feet(1.0);
            bool result = _service.CompareFeetEquality(feet1, feet2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseFeetInput_ValidNumericString_ReturnsFeetObject()
        {
            string input = "3.5";
            Feet? result = _service.ParseFeetInput(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(3.5, result!.Value, 0.0001);
        }

        #endregion

        #region Backward Compatibility Tests (Inch)

        [TestMethod]
        public void CompareInchEquality_BothNonNullEqualValues_ReturnsTrue()
        {
            var inch1 = new Inch(1.0);
            var inch2 = new Inch(1.0);
            bool result = _service.CompareInchEquality(inch1, inch2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseInchInput_ValidNumericString_ReturnsInchObject()
        {
            string input = "3.5";
            Inch? result = _service.ParseInchInput(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(3.5, result!.Value, 0.0001);
        }

        [TestMethod]
        public void CompareQuantityEquality_YardToYardEqualValues_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(1.0, LengthUnit.YARD);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_YardToFeetEquivalentValues_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(3.0, LengthUnit.FEET);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_YardToInchesEquivalentValues_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.YARD);
            var q2 = new Quantity(36.0, LengthUnit.INCH);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_CentimeterToCentimeterEqualValues_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.CENTIMETER);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_CentimeterToInchesEquivalentValues_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(0.393700787, LengthUnit.INCH);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_CentimeterToFeetEquivalentValues_ReturnsTrue()
        {
            var q1 = new Quantity(30.48, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_CentimeterToYardEquivalentValues_ReturnsTrue()
        {
            var q1 = new Quantity(91.44, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.YARD);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreQuantitiesEqual_YardToFeetEquivalentValues_ReturnsTrue()
        {
            bool result = QuantityMeasurementService.AreQuantitiesEqual(
                1.0,
                LengthUnit.YARD,
                3.0,
                LengthUnit.FEET
            );
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreQuantitiesEqual_YardToInchesEquivalentValues_ReturnsTrue()
        {
            bool result = QuantityMeasurementService.AreQuantitiesEqual(
                1.0,
                LengthUnit.YARD,
                36.0,
                LengthUnit.INCH
            );
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreQuantitiesEqual_CentimeterToInchesEquivalentValues_ReturnsTrue()
        {
            bool result = QuantityMeasurementService.AreQuantitiesEqual(
                1.0,
                LengthUnit.CENTIMETER,
                0.393700787,
                LengthUnit.INCH
            );
            Assert.IsTrue(result, "Static method: 1 cm should equal 0.393700787 inches");
        }

        [TestMethod]
        public void AreQuantitiesEqual_CentimeterToFeetEquivalentValues_ReturnsTrue()
        {
            bool result = QuantityMeasurementService.AreQuantitiesEqual(
                30.48,
                LengthUnit.CENTIMETER,
                1.0,
                LengthUnit.FEET
            );
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreQuantitiesEqual_CentimeterToYardEquivalentValues_ReturnsTrue()
        {
            bool result = QuantityMeasurementService.AreQuantitiesEqual(
                91.44,
                LengthUnit.CENTIMETER,
                1.0,
                LengthUnit.YARD
            );
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompareQuantityEquality_CentimeterToInches_RoundedValues_ReturnsTrue()
        {
            // Test with rounded values that should still be considered equal within tolerance
            var q1 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q2 = new Quantity(0.393701, LengthUnit.INCH); // Rounded to 6 decimal places
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result, "1 cm should approximately equal 0.393701 inches");
        }

        [TestMethod]
        public void CompareQuantityEquality_CentimeterToFeet_PrecisionTest()
        {
            // Test that 30.48 cm exactly equals 1 foot
            var q1 = new Quantity(30.48, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result, "30.48 cm should exactly equal 1 foot");
        }

        [TestMethod]
        public void CompareQuantityEquality_CentimeterToYard_PrecisionTest()
        {
            // Test that 91.44 cm exactly equals 1 yard
            var q1 = new Quantity(91.44, LengthUnit.CENTIMETER);
            var q2 = new Quantity(1.0, LengthUnit.YARD);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result, "91.44 cm should exactly equal 1 yard");
        }

        #endregion
    }
}
