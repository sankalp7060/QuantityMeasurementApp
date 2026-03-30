using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.GenericQuantityTests
{
    /// <summary>
    /// Test class for GenericQuantity with TemperatureUnit.
    /// UC14: Tests temperature measurements equality, conversion, and unsupported operations.
    /// </summary>
    [TestClass]
    public class GenericTemperatureQuantityTests
    {
        private const double Tolerance = 0.000001;
        private const double FahrenheitTolerance = 0.01;

        #region Equality Tests

        /// <summary>
        /// Tests that two temperatures in Celsius with same value are equal.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_CelsiusSameValue_ReturnsTrue()
        {
            // Arrange
            var firstTemp = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var secondTemp = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);

            // Act
            bool areEqual = firstTemp.Equals(secondTemp);

            // Assert
            Assert.IsTrue(areEqual, "0°C should equal 0°C");
        }

        /// <summary>
        /// Tests that two temperatures in Celsius with different values are not equal.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_CelsiusDifferentValue_ReturnsFalse()
        {
            // Arrange
            var firstTemp = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var secondTemp = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            // Act
            bool areEqual = firstTemp.Equals(secondTemp);

            // Assert
            Assert.IsFalse(areEqual, "0°C should not equal 100°C");
        }

        /// <summary>
        /// Tests cross-unit equality: 0°C = 32°F.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_CelsiusToFahrenheit_0Celsius32Fahrenheit_ReturnsTrue()
        {
            // Arrange
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var fahrenheitTemp = new GenericQuantity<TemperatureUnit>(
                32.0,
                TemperatureUnit.FAHRENHEIT
            );

            // Act
            bool areEqual = celsiusTemp.Equals(fahrenheitTemp);

            // Assert
            Assert.IsTrue(areEqual, "0°C should equal 32°F");
        }

        /// <summary>
        /// Tests cross-unit equality: 100°C = 212°F.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_CelsiusToFahrenheit_100Celsius212Fahrenheit_ReturnsTrue()
        {
            // Arrange
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var fahrenheitTemp = new GenericQuantity<TemperatureUnit>(
                212.0,
                TemperatureUnit.FAHRENHEIT
            );

            // Act
            bool areEqual = celsiusTemp.Equals(fahrenheitTemp);

            // Assert
            Assert.IsTrue(areEqual, "100°C should equal 212°F");
        }

        /// <summary>
        /// Tests special equality point: -40°C = -40°F.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_CelsiusToFahrenheit_Negative40Equal_ReturnsTrue()
        {
            // Arrange
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(-40.0, TemperatureUnit.CELSIUS);
            var fahrenheitTemp = new GenericQuantity<TemperatureUnit>(
                -40.0,
                TemperatureUnit.FAHRENHEIT
            );

            // Act
            bool areEqual = celsiusTemp.Equals(fahrenheitTemp);

            // Assert
            Assert.IsTrue(areEqual, "-40°C should equal -40°F (unique equality point)");
        }

        /// <summary>
        /// Tests cross-unit equality: 0°C = 273.15K.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_CelsiusToKelvin_0Celsius273_15K_ReturnsTrue()
        {
            // Arrange
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var kelvinTemp = new GenericQuantity<TemperatureUnit>(273.15, TemperatureUnit.KELVIN);

            // Act
            bool areEqual = celsiusTemp.Equals(kelvinTemp);

            // Assert
            Assert.IsTrue(areEqual, "0°C should equal 273.15K");
        }

        /// <summary>
        /// Tests cross-unit equality: 100°C = 373.15K.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_CelsiusToKelvin_100Celsius373_15K_ReturnsTrue()
        {
            // Arrange
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var kelvinTemp = new GenericQuantity<TemperatureUnit>(373.15, TemperatureUnit.KELVIN);

            // Act
            bool areEqual = celsiusTemp.Equals(kelvinTemp);

            // Assert
            Assert.IsTrue(areEqual, "100°C should equal 373.15K");
        }

        /// <summary>
        /// Tests absolute zero: -273.15°C = 0K.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_AbsoluteZero_ReturnsTrue()
        {
            // Arrange
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(
                -273.15,
                TemperatureUnit.CELSIUS
            );
            var kelvinTemp = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.KELVIN);

            // Act
            bool areEqual = celsiusTemp.Equals(kelvinTemp);

            // Assert
            Assert.IsTrue(areEqual, "-273.15°C should equal 0K (absolute zero)");
        }

        /// <summary>
        /// Tests symmetric property.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_Symmetric_ReturnsTrue()
        {
            // Arrange
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var fahrenheitTemp = new GenericQuantity<TemperatureUnit>(
                32.0,
                TemperatureUnit.FAHRENHEIT
            );

            // Act
            bool celsiusEqualsFahrenheit = celsiusTemp.Equals(fahrenheitTemp);
            bool fahrenheitEqualsCelsius = fahrenheitTemp.Equals(celsiusTemp);

            // Assert
            Assert.IsTrue(
                celsiusEqualsFahrenheit && fahrenheitEqualsCelsius,
                "Equality should be symmetric"
            );
        }

        /// <summary>
        /// Tests reflexive property.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_Reflexive_ReturnsTrue()
        {
            // Arrange
            var temp = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            // Act
            bool isEqualToItself = temp.Equals(temp);

            // Assert
            Assert.IsTrue(isEqualToItself, "Object should equal itself");
        }

        /// <summary>
        /// Tests null comparison.
        /// </summary>
        [TestMethod]
        public void Equals_Temperature_NullComparison_ReturnsFalse()
        {
            // Arrange
            var temp = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            // Act
            bool isEqualToNull = temp.Equals(null);

            // Assert
            Assert.IsFalse(isEqualToNull, "Object should not equal null");
        }

        /// <summary>
        /// Tests that temperature and length are not equal (different categories).
        /// </summary>
        [TestMethod]
        public void Equals_TemperatureVsLength_ReturnsFalse()
        {
            // Arrange
            var temp = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var length = new GenericQuantity<LengthUnit>(100.0, LengthUnit.FEET);

            // Act
            bool areEqual = temp.Equals(length);

            // Assert
            Assert.IsFalse(areEqual, "Temperature and length should not be equal");
        }

        #endregion

        #region Conversion Tests

        /// <summary>
        /// Tests ConvertTo for Celsius to Fahrenheit.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Temperature_CelsiusToFahrenheit_ReturnsCorrectQuantity()
        {
            // Arrange
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            // Act
            var fahrenheitTemp = celsiusTemp.ConvertTo(TemperatureUnit.FAHRENHEIT);

            // Assert
            Assert.AreEqual(
                212.0,
                fahrenheitTemp.Value,
                FahrenheitTolerance,
                "100°C should convert to 212°F"
            );
            Assert.AreEqual(
                TemperatureUnit.FAHRENHEIT,
                fahrenheitTemp.Unit,
                "Unit should be Fahrenheit"
            );
        }

        /// <summary>
        /// Tests ConvertTo for Fahrenheit to Celsius.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Temperature_FahrenheitToCelsius_ReturnsCorrectQuantity()
        {
            // Arrange
            var fahrenheitTemp = new GenericQuantity<TemperatureUnit>(
                32.0,
                TemperatureUnit.FAHRENHEIT
            );

            // Act
            var celsiusTemp = fahrenheitTemp.ConvertTo(TemperatureUnit.CELSIUS);

            // Assert
            Assert.AreEqual(0.0, celsiusTemp.Value, Tolerance, "32°F should convert to 0°C");
            Assert.AreEqual(TemperatureUnit.CELSIUS, celsiusTemp.Unit, "Unit should be Celsius");
        }

        /// <summary>
        /// Tests ConvertTo for Celsius to Kelvin.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Temperature_CelsiusToKelvin_ReturnsCorrectQuantity()
        {
            // Arrange
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);

            // Act
            var kelvinTemp = celsiusTemp.ConvertTo(TemperatureUnit.KELVIN);

            // Assert
            Assert.AreEqual(273.15, kelvinTemp.Value, Tolerance, "0°C should convert to 273.15K");
            Assert.AreEqual(TemperatureUnit.KELVIN, kelvinTemp.Unit, "Unit should be Kelvin");
        }

        /// <summary>
        /// Tests ConvertTo for Kelvin to Celsius.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Temperature_KelvinToCelsius_ReturnsCorrectQuantity()
        {
            // Arrange
            var kelvinTemp = new GenericQuantity<TemperatureUnit>(273.15, TemperatureUnit.KELVIN);

            // Act
            var celsiusTemp = kelvinTemp.ConvertTo(TemperatureUnit.CELSIUS);

            // Assert
            Assert.AreEqual(0.0, celsiusTemp.Value, Tolerance, "273.15K should convert to 0°C");
            Assert.AreEqual(TemperatureUnit.CELSIUS, celsiusTemp.Unit, "Unit should be Celsius");
        }

        /// <summary>
        /// Tests ConvertTo for Fahrenheit to Kelvin.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Temperature_FahrenheitToKelvin_ReturnsCorrectQuantity()
        {
            // Arrange
            var fahrenheitTemp = new GenericQuantity<TemperatureUnit>(
                32.0,
                TemperatureUnit.FAHRENHEIT
            );

            // Act
            var kelvinTemp = fahrenheitTemp.ConvertTo(TemperatureUnit.KELVIN);

            // Assert
            Assert.AreEqual(
                273.15,
                kelvinTemp.Value,
                0.1,
                "32°F should convert to approximately 273.15K"
            );
            Assert.AreEqual(TemperatureUnit.KELVIN, kelvinTemp.Unit, "Unit should be Kelvin");
        }

        /// <summary>
        /// Tests ConvertTo for Kelvin to Fahrenheit.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Temperature_KelvinToFahrenheit_ReturnsCorrectQuantity()
        {
            // Arrange
            var kelvinTemp = new GenericQuantity<TemperatureUnit>(273.15, TemperatureUnit.KELVIN);

            // Act
            var fahrenheitTemp = kelvinTemp.ConvertTo(TemperatureUnit.FAHRENHEIT);

            // Assert
            Assert.AreEqual(
                32.0,
                fahrenheitTemp.Value,
                0.1,
                "273.15K should convert to approximately 32°F"
            );
            Assert.AreEqual(
                TemperatureUnit.FAHRENHEIT,
                fahrenheitTemp.Unit,
                "Unit should be Fahrenheit"
            );
        }

        /// <summary>
        /// Tests round-trip conversion.
        /// </summary>
        [TestMethod]
        public void ConvertTo_Temperature_RoundTrip_ReturnsOriginalValue()
        {
            // Arrange
            double originalValue = 25.0;
            var originalTemp = new GenericQuantity<TemperatureUnit>(
                originalValue,
                TemperatureUnit.CELSIUS
            );

            // Act
            var inFahrenheit = originalTemp.ConvertTo(TemperatureUnit.FAHRENHEIT);
            var backToCelsius = inFahrenheit.ConvertTo(TemperatureUnit.CELSIUS);

            // Assert
            Assert.AreEqual(
                originalValue,
                backToCelsius.Value,
                0.1,
                "Round-trip °C->°F->°C should return original"
            );
        }

        #endregion

        #region Unsupported Arithmetic Operations Tests

        /// <summary>
        /// Tests that Add throws NotSupportedException for temperature.
        /// FIXED: Updated to check for the actual error message format.
        /// </summary>
        [TestMethod]
        public void Add_Temperature_ThrowsNotSupportedException()
        {
            // Arrange
            var temp1 = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var temp2 = new GenericQuantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            // Act & Assert
            var ex = Assert.ThrowsException<NotSupportedException>(() => temp1.Add(temp2));
            StringAssert.Contains(ex.Message, "ADD operations");
        }

        /// <summary>
        /// Tests that Subtract throws NotSupportedException for temperature.
        /// FIXED: Updated to check for the actual error message format.
        /// </summary>
        [TestMethod]
        public void Subtract_Temperature_ThrowsNotSupportedException()
        {
            // Arrange
            var temp1 = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var temp2 = new GenericQuantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            // Act & Assert
            var ex = Assert.ThrowsException<NotSupportedException>(() => temp1.Subtract(temp2));
            StringAssert.Contains(ex.Message, "SUBTRACT operations");
        }

        /// <summary>
        /// Tests that Divide throws NotSupportedException for temperature.
        /// FIXED: Updated to check for the actual error message format.
        /// </summary>
        [TestMethod]
        public void Divide_Temperature_ThrowsNotSupportedException()
        {
            // Arrange
            var temp1 = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var temp2 = new GenericQuantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            // Act & Assert
            var ex = Assert.ThrowsException<NotSupportedException>(() => temp1.Divide(temp2));
            StringAssert.Contains(ex.Message, "DIVIDE operations");
        }

        /// <summary>
        /// Tests that Add with different temperature units throws NotSupportedException.
        /// </summary>
        [TestMethod]
        public void Add_Temperature_DifferentUnits_ThrowsNotSupportedException()
        {
            // Arrange
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var fahrenheitTemp = new GenericQuantity<TemperatureUnit>(
                212.0,
                TemperatureUnit.FAHRENHEIT
            );

            // Act & Assert
            Assert.ThrowsException<NotSupportedException>(() => celsiusTemp.Add(fahrenheitTemp));
        }

        #endregion

        #region Edge Cases

        /// <summary>
        /// Tests that zero values work correctly.
        /// </summary>
        [TestMethod]
        public void Temperature_ZeroValues_WorkCorrectly()
        {
            // Arrange
            var zeroCelsius = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var zeroFahrenheit = new GenericQuantity<TemperatureUnit>(
                0.0,
                TemperatureUnit.FAHRENHEIT
            );
            var zeroKelvin = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.KELVIN);

            // Assert - 0°C is not equal to 0°F
            Assert.IsFalse(zeroCelsius.Equals(zeroFahrenheit), "0°C should not equal 0°F");

            // 0°C = 32°F, not 0°F
            Assert.IsTrue(
                new GenericQuantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT).Equals(
                    zeroCelsius
                )
            );

            // 0K = -273.15°C
            Assert.IsTrue(
                new GenericQuantity<TemperatureUnit>(-273.15, TemperatureUnit.CELSIUS).Equals(
                    zeroKelvin
                )
            );
        }

        /// <summary>
        /// Tests that constructor rejects invalid values.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void Constructor_Temperature_NaNValue_ThrowsException()
        {
            // Act - Should throw
            var invalidTemp = new GenericQuantity<TemperatureUnit>(
                double.NaN,
                TemperatureUnit.CELSIUS
            );
        }

        /// <summary>
        /// Tests that null unit throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Temperature_NullUnit_ThrowsException()
        {
            // Act - Should throw
            var invalidTemp = new GenericQuantity<TemperatureUnit>(100.0, null!);
        }

        #endregion
    }
}
