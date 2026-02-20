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

        #region Feet Tests

        [TestMethod]
        public void CompareFeetEquality_BothNonNullEqualValues_ReturnsTrue()
        {
            var feet1 = new Feet(1.0);
            var feet2 = new Feet(1.0);
            bool result = _service.CompareFeetEquality(feet1, feet2);
            Assert.IsTrue(result, "Equal values should return true");
        }

        [TestMethod]
        public void CompareFeetEquality_BothNonNullDifferentValues_ReturnsFalse()
        {
            var feet1 = new Feet(1.0);
            var feet2 = new Feet(2.0);
            bool result = _service.CompareFeetEquality(feet1, feet2);
            Assert.IsFalse(result, "Different values should return false");
        }

        [TestMethod]
        public void CompareFeetEquality_FirstParameterNull_ReturnsFalse()
        {
            Feet? feet1 = null;
            var feet2 = new Feet(1.0);
            bool result = _service.CompareFeetEquality(feet1, feet2);
            Assert.IsFalse(result, "Comparison with null should return false");
        }

        [TestMethod]
        public void CompareFeetEquality_SecondParameterNull_ReturnsFalse()
        {
            var feet1 = new Feet(1.0);
            Feet? feet2 = null;
            bool result = _service.CompareFeetEquality(feet1, feet2);
            Assert.IsFalse(result, "Comparison with null should return false");
        }

        [TestMethod]
        public void CompareFeetEquality_BothParametersNull_ReturnsFalse()
        {
            Feet? feet1 = null;
            Feet? feet2 = null;
            bool result = _service.CompareFeetEquality(feet1, feet2);
            Assert.IsFalse(result, "Comparison with both null should return false");
        }

        [TestMethod]
        public void ParseFeetInput_ValidNumericString_ReturnsFeetObject()
        {
            string input = "3.5";
            Feet? result = _service.ParseFeetInput(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(3.5, result!.Value, 0.0001);
        }

        [TestMethod]
        public void ParseFeetInput_NullInput_ReturnsNull()
        {
            string? input = null;
            Feet? result = _service.ParseFeetInput(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseFeetInput_EmptyString_ReturnsNull()
        {
            string input = "";
            Feet? result = _service.ParseFeetInput(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseFeetInput_Whitespace_ReturnsNull()
        {
            string input = "   ";
            Feet? result = _service.ParseFeetInput(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseFeetInput_NonNumericString_ReturnsNull()
        {
            string input = "abc";
            Feet? result = _service.ParseFeetInput(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseFeetInput_NegativeNumber_ReturnsFeetObject()
        {
            string input = "-2.5";
            Feet? result = _service.ParseFeetInput(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(-2.5, result!.Value, 0.0001);
        }

        [TestMethod]
        public void ParseFeetInput_Zero_ReturnsFeetObject()
        {
            string input = "0";
            Feet? result = _service.ParseFeetInput(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result!.Value, 0.0001);
        }

        [TestMethod]
        public void AreFeetEqual_EqualValues_ReturnsTrue()
        {
            bool result = QuantityMeasurementService.AreFeetEqual(1.0, 1.0);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreFeetEqual_DifferentValues_ReturnsFalse()
        {
            bool result = QuantityMeasurementService.AreFeetEqual(1.0, 2.0);
            Assert.IsFalse(result);
        }

        #endregion

        #region Inch Tests

        [TestMethod]
        public void CompareInchEquality_BothNonNullEqualValues_ReturnsTrue()
        {
            var inch1 = new Inch(1.0);
            var inch2 = new Inch(1.0);
            bool result = _service.CompareInchEquality(inch1, inch2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompareInchEquality_BothNonNullDifferentValues_ReturnsFalse()
        {
            var inch1 = new Inch(1.0);
            var inch2 = new Inch(2.0);
            bool result = _service.CompareInchEquality(inch1, inch2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CompareInchEquality_FirstParameterNull_ReturnsFalse()
        {
            Inch? inch1 = null;
            var inch2 = new Inch(1.0);
            bool result = _service.CompareInchEquality(inch1, inch2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CompareInchEquality_SecondParameterNull_ReturnsFalse()
        {
            var inch1 = new Inch(1.0);
            Inch? inch2 = null;
            bool result = _service.CompareInchEquality(inch1, inch2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CompareInchEquality_BothParametersNull_ReturnsFalse()
        {
            Inch? inch1 = null;
            Inch? inch2 = null;
            bool result = _service.CompareInchEquality(inch1, inch2);
            Assert.IsFalse(result);
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
        public void ParseInchInput_NullInput_ReturnsNull()
        {
            string? input = null;
            Inch? result = _service.ParseInchInput(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseInchInput_EmptyString_ReturnsNull()
        {
            string input = "";
            Inch? result = _service.ParseInchInput(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseInchInput_Whitespace_ReturnsNull()
        {
            string input = "   ";
            Inch? result = _service.ParseInchInput(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseInchInput_NonNumericString_ReturnsNull()
        {
            string input = "abc";
            Inch? result = _service.ParseInchInput(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseInchInput_NegativeNumber_ReturnsInchObject()
        {
            string input = "-2.5";
            Inch? result = _service.ParseInchInput(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(-2.5, result!.Value, 0.0001);
        }

        [TestMethod]
        public void ParseInchInput_Zero_ReturnsInchObject()
        {
            string input = "0";
            Inch? result = _service.ParseInchInput(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result!.Value, 0.0001);
        }

        [TestMethod]
        public void AreInchEqual_EqualValues_ReturnsTrue()
        {
            bool result = QuantityMeasurementService.AreInchEqual(1.0, 1.0);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreInchEqual_DifferentValues_ReturnsFalse()
        {
            bool result = QuantityMeasurementService.AreInchEqual(1.0, 2.0);
            Assert.IsFalse(result);
        }

        #endregion
    }
}
