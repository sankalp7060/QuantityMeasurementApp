using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Tests.Services
{
    /// <summary>
    /// Contains unit tests for the QuantityMeasurementService class to verify
    /// quantity comparison logic, parsing functionality, cross-unit equality handling,
    /// static equality checks, and backward compatibility support for Feet and Inch models.
    /// </summary>
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

        // Tests QuantityMeasurementService.CompareQuantityEquality method with equal values and same unit
        [TestMethod]
        public void CompareQuantityEquality_BothNonNullEqualValues_SameUnit_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        // Tests QuantityMeasurementService.CompareQuantityEquality method with different values and same unit
        [TestMethod]
        public void CompareQuantityEquality_BothNonNullDifferentValues_SameUnit_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(2.0, LengthUnit.FEET);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsFalse(result);
        }

        // Tests QuantityMeasurementService.CompareQuantityEquality method with equivalent cross-unit values
        [TestMethod]
        public void CompareQuantityEquality_CrossUnitEquivalentValues_ReturnsTrue()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsTrue(result);
        }

        // Tests QuantityMeasurementService.CompareQuantityEquality method with non-equivalent cross-unit values
        [TestMethod]
        public void CompareQuantityEquality_CrossUnitNonEquivalentValues_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(13.0, LengthUnit.INCH);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsFalse(result);
        }

        // Tests QuantityMeasurementService.CompareQuantityEquality method when first parameter is null
        [TestMethod]
        public void CompareQuantityEquality_FirstParameterNull_ReturnsFalse()
        {
            Quantity? q1 = null;
            var q2 = new Quantity(1.0, LengthUnit.FEET);
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsFalse(result);
        }

        // Tests QuantityMeasurementService.CompareQuantityEquality method when second parameter is null
        [TestMethod]
        public void CompareQuantityEquality_SecondParameterNull_ReturnsFalse()
        {
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            Quantity? q2 = null;
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsFalse(result);
        }

        // Tests QuantityMeasurementService.CompareQuantityEquality method when both parameters are null
        [TestMethod]
        public void CompareQuantityEquality_BothParametersNull_ReturnsFalse()
        {
            Quantity? q1 = null;
            Quantity? q2 = null;
            bool result = _service.CompareQuantityEquality(q1, q2);
            Assert.IsFalse(result);
        }

        // Tests QuantityMeasurementService.ParseQuantityInput method with valid numeric string
        [TestMethod]
        public void ParseQuantityInput_ValidNumericString_ReturnsQuantityObject()
        {
            string input = "3.5";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNotNull(result);
            Assert.AreEqual(3.5, result!.Value);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        // Tests QuantityMeasurementService.ParseQuantityInput method with null input
        [TestMethod]
        public void ParseQuantityInput_NullInput_ReturnsNull()
        {
            string? input = null;
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNull(result);
        }

        // Tests QuantityMeasurementService.ParseQuantityInput method with empty string
        [TestMethod]
        public void ParseQuantityInput_EmptyString_ReturnsNull()
        {
            string input = "";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNull(result);
        }

        // Tests QuantityMeasurementService.ParseQuantityInput method with whitespace input
        [TestMethod]
        public void ParseQuantityInput_Whitespace_ReturnsNull()
        {
            string input = "   ";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNull(result);
        }

        // Tests QuantityMeasurementService.ParseQuantityInput method with non-numeric string
        [TestMethod]
        public void ParseQuantityInput_NonNumericString_ReturnsNull()
        {
            string input = "abc";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNull(result);
        }

        // Tests QuantityMeasurementService.ParseQuantityInput method with negative number
        [TestMethod]
        public void ParseQuantityInput_NegativeNumber_ReturnsQuantityObject()
        {
            string input = "-2.5";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNotNull(result);
            Assert.AreEqual(-2.5, result!.Value);
        }

        // Tests QuantityMeasurementService.ParseQuantityInput method with zero value
        [TestMethod]
        public void ParseQuantityInput_Zero_ReturnsQuantityObject()
        {
            string input = "0";
            Quantity? result = _service.ParseQuantityInput(input, LengthUnit.FEET);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result!.Value);
        }

        // Tests static QuantityMeasurementService.AreQuantitiesEqual method with equal values and same unit
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

        // Tests static QuantityMeasurementService.AreQuantitiesEqual method with different values and same unit
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

        // Tests static QuantityMeasurementService.AreQuantitiesEqual method with equivalent cross-unit values
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

        // Tests static QuantityMeasurementService.AreQuantitiesEqual method with non-equivalent cross-unit values
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

        // Tests QuantityMeasurementService.CompareFeetEquality method
        [TestMethod]
        public void CompareFeetEquality_BothNonNullEqualValues_ReturnsTrue()
        {
            var feet1 = new Feet(1.0);
            var feet2 = new Feet(1.0);
            bool result = _service.CompareFeetEquality(feet1, feet2);
            Assert.IsTrue(result);
        }

        // Tests QuantityMeasurementService.ParseFeetInput method
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

        // Tests QuantityMeasurementService.CompareInchEquality method
        [TestMethod]
        public void CompareInchEquality_BothNonNullEqualValues_ReturnsTrue()
        {
            var inch1 = new Inch(1.0);
            var inch2 = new Inch(1.0);
            bool result = _service.CompareInchEquality(inch1, inch2);
            Assert.IsTrue(result);
        }

        // Tests QuantityMeasurementService.ParseInchInput method
        [TestMethod]
        public void ParseInchInput_ValidNumericString_ReturnsInchObject()
        {
            string input = "3.5";
            Inch? result = _service.ParseInchInput(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(3.5, result!.Value, 0.0001);
        }

        #endregion
    }
}
