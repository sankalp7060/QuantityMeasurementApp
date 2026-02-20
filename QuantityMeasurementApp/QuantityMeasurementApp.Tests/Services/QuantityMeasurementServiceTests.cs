using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Tests.Services
{
    // Test class for QuantityMeasurementService
    // Contains unit tests to verify the service layer functionality
    [TestClass]
    public class QuantityMeasurementServiceTests
    {
        // Service instance to be used in tests
        private QuantityMeasurementService _service = null!;

        // Setup method - runs before each test
        // Initializes a new service instance for each test
        [TestInitialize]
        public void Setup()
        {
            _service = new QuantityMeasurementService();
        }

        // Test: CompareFeetEquality with both non-null equal values
        // Verifies that equal values return true
        [TestMethod]
        public void CompareFeetEquality_BothNonNullEqualValues_ReturnsTrue()
        {
            // Arrange: Create two equal Feet objects
            var feet1 = new Feet(1.0);
            var feet2 = new Feet(1.0);

            // Act: Compare them using the service
            bool result = _service.CompareFeetEquality(feet1, feet2);

            // Assert: Equal values should return true
            Assert.IsTrue(result, "Equal values should return true");
        }

        // Test: CompareFeetEquality with both non-null different values
        // Verifies that different values return false
        [TestMethod]
        public void CompareFeetEquality_BothNonNullDifferentValues_ReturnsFalse()
        {
            // Arrange: Create two different Feet objects
            var feet1 = new Feet(1.0);
            var feet2 = new Feet(2.0);

            // Act: Compare them using the service
            bool result = _service.CompareFeetEquality(feet1, feet2);

            // Assert: Different values should return false
            Assert.IsFalse(result, "Different values should return false");
        }

        // Test: CompareFeetEquality with first parameter null
        // Verifies that comparison with null returns false
        [TestMethod]
        public void CompareFeetEquality_FirstParameterNull_ReturnsFalse()
        {
            // Arrange: First parameter null, second parameter valid
            Feet? feet1 = null;
            var feet2 = new Feet(1.0);

            // Act: Compare with null as first parameter
            bool result = _service.CompareFeetEquality(feet1, feet2);

            // Assert: Comparison with null should return false
            Assert.IsFalse(result, "Comparison with null should return false");
        }

        // Test: CompareFeetEquality with second parameter null
        // Verifies that comparison with null returns false
        [TestMethod]
        public void CompareFeetEquality_SecondParameterNull_ReturnsFalse()
        {
            // Arrange: First parameter valid, second parameter null
            var feet1 = new Feet(1.0);
            Feet? feet2 = null;

            // Act: Compare with null as second parameter
            bool result = _service.CompareFeetEquality(feet1, feet2);

            // Assert: Comparison with null should return false
            Assert.IsFalse(result, "Comparison with null should return false");
        }

        // Test: CompareFeetEquality with both parameters null
        // Verifies that comparison with both null returns false
        [TestMethod]
        public void CompareFeetEquality_BothParametersNull_ReturnsFalse()
        {
            // Arrange: Both parameters null
            Feet? feet1 = null;
            Feet? feet2 = null;

            // Act: Compare with both null
            bool result = _service.CompareFeetEquality(feet1, feet2);

            // Assert: Comparison with both null should return false
            Assert.IsFalse(result, "Comparison with both null should return false");
        }

        // Test: ParseFeetInput with valid numeric string
        // Verifies that valid input produces a Feet object
        [TestMethod]
        public void ParseFeetInput_ValidNumericString_ReturnsFeetObject()
        {
            // Arrange: Valid numeric input
            string input = "3.5";

            // Act: Parse the input
            Feet? result = _service.ParseFeetInput(input);

            // Assert: Should return non-null Feet object with correct value
            Assert.IsNotNull(result, "Should return non-null Feet object");
            Assert.AreEqual(3.5, result!.Value, 0.0001, "Value should match input");
        }

        // Test: ParseFeetInput with null input
        // Verifies that null input returns null
        [TestMethod]
        public void ParseFeetInput_NullInput_ReturnsNull()
        {
            // Arrange: Null input
            string? input = null;

            // Act: Parse null input
            Feet? result = _service.ParseFeetInput(input);

            // Assert: Null input should return null
            Assert.IsNull(result, "Null input should return null");
        }

        // Test: ParseFeetInput with empty string
        // Verifies that empty string returns null
        [TestMethod]
        public void ParseFeetInput_EmptyString_ReturnsNull()
        {
            // Arrange: Empty string input
            string input = "";

            // Act: Parse empty string
            Feet? result = _service.ParseFeetInput(input);

            // Assert: Empty string should return null
            Assert.IsNull(result, "Empty string should return null");
        }

        // Test: ParseFeetInput with whitespace
        // Verifies that whitespace returns null
        [TestMethod]
        public void ParseFeetInput_Whitespace_ReturnsNull()
        {
            // Arrange: Whitespace input
            string input = "   ";

            // Act: Parse whitespace
            Feet? result = _service.ParseFeetInput(input);

            // Assert: Whitespace should return null
            Assert.IsNull(result, "Whitespace should return null");
        }

        // Test: ParseFeetInput with non-numeric string
        // Verifies that non-numeric input returns null
        [TestMethod]
        public void ParseFeetInput_NonNumericString_ReturnsNull()
        {
            // Arrange: Non-numeric input
            string input = "abc";

            // Act: Parse non-numeric input
            Feet? result = _service.ParseFeetInput(input);

            // Assert: Non-numeric input should return null
            Assert.IsNull(result, "Non-numeric input should return null");
        }

        // Test: ParseFeetInput with negative number
        // Verifies that negative numbers are accepted
        [TestMethod]
        public void ParseFeetInput_NegativeNumber_ReturnsFeetObject()
        {
            // Arrange: Negative numeric input
            string input = "-2.5";

            // Act: Parse negative number
            Feet? result = _service.ParseFeetInput(input);

            // Assert: Should return non-null Feet object with negative value
            Assert.IsNotNull(result, "Should return non-null Feet object");
            Assert.AreEqual(-2.5, result!.Value, 0.0001, "Negative values should be accepted");
        }

        // Test: ParseFeetInput with zero
        // Verifies that zero is accepted
        [TestMethod]
        public void ParseFeetInput_Zero_ReturnsFeetObject()
        {
            // Arrange: Zero input
            string input = "0";

            // Act: Parse zero
            Feet? result = _service.ParseFeetInput(input);

            // Assert: Should return non-null Feet object with zero value
            Assert.IsNotNull(result, "Should return non-null Feet object");
            Assert.AreEqual(0, result!.Value, 0.0001, "Zero should be accepted");
        }
    }
}
