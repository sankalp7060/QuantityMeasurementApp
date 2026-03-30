using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Core.Exceptions;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.DomainTests.UnitTests
{
    /// <summary>
    /// Test class for TemperatureUnit implementing IMeasurable interface.
    /// UC14: Verifies that TemperatureUnit correctly implements IMeasurable with selective arithmetic support.
    /// </summary>
    [TestClass]
    public class TemperatureUnitTests
    {
        private const double Tolerance = 0.000001;
        private const double FahrenheitTolerance = 0.01;

        #region IMeasurable Implementation Tests

        /// <summary>
        /// Tests that TemperatureUnit implements IMeasurable interface.
        /// </summary>
        [TestMethod]
        public void TemperatureUnit_Implements_IMeasurable()
        {
            // Arrange
            var unit = TemperatureUnit.CELSIUS;

            // Assert
            Assert.IsTrue(unit is IMeasurable, "TemperatureUnit should implement IMeasurable");
        }

        /// <summary>
        /// Tests that TemperatureUnit does NOT support arithmetic operations.
        /// </summary>
        [TestMethod]
        public void TemperatureUnit_DoesNotSupportArithmetic()
        {
            // Assert
            Assert.IsFalse(
                TemperatureUnit.CELSIUS.SupportsArithmeticOperation(),
                "Celsius should not support arithmetic"
            );
            Assert.IsFalse(
                TemperatureUnit.FAHRENHEIT.SupportsArithmeticOperation(),
                "Fahrenheit should not support arithmetic"
            );
            Assert.IsFalse(
                TemperatureUnit.KELVIN.SupportsArithmeticOperation(),
                "Kelvin should not support arithmetic"
            );
        }

        /// <summary>
        /// Tests that LengthUnit DOES support arithmetic operations (for comparison).
        /// </summary>
        [TestMethod]
        public void LengthUnit_SupportsArithmetic()
        {
            // Assert
            Assert.IsTrue(
                LengthUnit.FEET.SupportsArithmeticOperation(),
                "Length units should support arithmetic"
            );
        }

        /// <summary>
        /// Tests GetConversionFactor throws NotSupportedException for temperature units.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void GetConversionFactor_Celsius_ThrowsNotSupportedException()
        {
            // Act - Should throw
            TemperatureUnit.CELSIUS.GetConversionFactor();
        }

        /// <summary>
        /// Tests ValidateOperationSupport throws NotSupportedException.
        /// </summary>
        [TestMethod]
        public void ValidateOperationSupport_ThrowsNotSupportedException()
        {
            // Arrange
            var celsius = TemperatureUnit.CELSIUS;

            // Act & Assert
            var ex = Assert.ThrowsException<NotSupportedException>(() =>
                celsius.ValidateOperationSupport("addition")
            );
            StringAssert.Contains(ex.Message, "do not support addition operations");
        }

        #endregion

        #region ToBaseUnit Tests (Celsius is base unit)

        /// <summary>
        /// Tests ToBaseUnit for Celsius (should return same value).
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_Celsius_ReturnsSameValue()
        {
            // Arrange
            double value = 100.0;

            // Act
            double result = TemperatureUnit.CELSIUS.ToBaseUnit(value);

            // Assert
            Assert.AreEqual(value, result, Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit for Fahrenheit to Celsius.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_FahrenheitToCelsius_ReturnsCorrectValue()
        {
            // Test 32°F -> 0°C
            Assert.AreEqual(0.0, TemperatureUnit.FAHRENHEIT.ToBaseUnit(32.0), Tolerance);

            // Test 212°F -> 100°C
            Assert.AreEqual(100.0, TemperatureUnit.FAHRENHEIT.ToBaseUnit(212.0), Tolerance);

            // Test -40°F -> -40°C (unique equality point)
            Assert.AreEqual(-40.0, TemperatureUnit.FAHRENHEIT.ToBaseUnit(-40.0), Tolerance);
        }

        /// <summary>
        /// Tests ToBaseUnit for Kelvin to Celsius.
        /// </summary>
        [TestMethod]
        public void ToBaseUnit_KelvinToCelsius_ReturnsCorrectValue()
        {
            // Test 273.15 K -> 0°C
            Assert.AreEqual(0.0, TemperatureUnit.KELVIN.ToBaseUnit(273.15), Tolerance);

            // Test 373.15 K -> 100°C
            Assert.AreEqual(100.0, TemperatureUnit.KELVIN.ToBaseUnit(373.15), Tolerance);

            // Test 0 K -> -273.15°C (absolute zero)
            Assert.AreEqual(-273.15, TemperatureUnit.KELVIN.ToBaseUnit(0.0), Tolerance);
        }

        #endregion

        #region FromBaseUnit Tests (from Celsius to other units)

        /// <summary>
        /// Tests FromBaseUnit for Celsius (should return same value).
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_Celsius_ReturnsSameValue()
        {
            // Arrange
            double value = 100.0;

            // Act
            double result = TemperatureUnit.CELSIUS.FromBaseUnit(value);

            // Assert
            Assert.AreEqual(value, result, Tolerance);
        }

        /// <summary>
        /// Tests FromBaseUnit for Celsius to Fahrenheit.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_CelsiusToFahrenheit_ReturnsCorrectValue()
        {
            // Test 0°C -> 32°F
            Assert.AreEqual(
                32.0,
                TemperatureUnit.FAHRENHEIT.FromBaseUnit(0.0),
                FahrenheitTolerance
            );

            // Test 100°C -> 212°F
            Assert.AreEqual(
                212.0,
                TemperatureUnit.FAHRENHEIT.FromBaseUnit(100.0),
                FahrenheitTolerance
            );

            // Test -40°C -> -40°F (unique equality point)
            Assert.AreEqual(
                -40.0,
                TemperatureUnit.FAHRENHEIT.FromBaseUnit(-40.0),
                FahrenheitTolerance
            );
        }

        /// <summary>
        /// Tests FromBaseUnit for Celsius to Kelvin.
        /// </summary>
        [TestMethod]
        public void FromBaseUnit_CelsiusToKelvin_ReturnsCorrectValue()
        {
            // Test 0°C -> 273.15 K
            Assert.AreEqual(273.15, TemperatureUnit.KELVIN.FromBaseUnit(0.0), Tolerance);

            // Test 100°C -> 373.15 K
            Assert.AreEqual(373.15, TemperatureUnit.KELVIN.FromBaseUnit(100.0), Tolerance);

            // Test -273.15°C -> 0 K (absolute zero)
            Assert.AreEqual(0.0, TemperatureUnit.KELVIN.FromBaseUnit(-273.15), Tolerance);
        }

        #endregion

        #region GetSymbol Tests

        /// <summary>
        /// Tests GetSymbol returns correct symbol for each temperature unit.
        /// </summary>
        [TestMethod]
        public void GetSymbol_ReturnsCorrectSymbol()
        {
            // Assert
            Assert.AreEqual("°C", TemperatureUnit.CELSIUS.GetSymbol());
            Assert.AreEqual("°F", TemperatureUnit.FAHRENHEIT.GetSymbol());
            Assert.AreEqual("K", TemperatureUnit.KELVIN.GetSymbol());
        }

        #endregion

        #region GetName Tests

        /// <summary>
        /// Tests GetName returns correct name for each temperature unit.
        /// </summary>
        [TestMethod]
        public void GetName_ReturnsCorrectName()
        {
            // Assert
            Assert.AreEqual("Celsius", TemperatureUnit.CELSIUS.GetName());
            Assert.AreEqual("Fahrenheit", TemperatureUnit.FAHRENHEIT.GetName());
            Assert.AreEqual("Kelvin", TemperatureUnit.KELVIN.GetName());
        }

        #endregion

        #region Validation Tests

        /// <summary>
        /// Tests ToBaseUnit with invalid value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void ToBaseUnit_NaNValue_ThrowsException()
        {
            // Act - Should throw
            TemperatureUnit.CELSIUS.ToBaseUnit(double.NaN);
        }

        /// <summary>
        /// Tests FromBaseUnit with invalid value throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void FromBaseUnit_NaNValue_ThrowsException()
        {
            // Act - Should throw
            TemperatureUnit.CELSIUS.FromBaseUnit(double.NaN);
        }

        #endregion

        #region Equality Tests

        /// <summary>
        /// Tests that the same unit instances are equal.
        /// </summary>
        [TestMethod]
        public void Equals_SameUnit_ReturnsTrue()
        {
            // Assert
            Assert.IsTrue(
                TemperatureUnit.CELSIUS.Equals(TemperatureUnit.CELSIUS),
                "Same unit should be equal"
            );
            Assert.IsTrue(
                TemperatureUnit.FAHRENHEIT.Equals(TemperatureUnit.FAHRENHEIT),
                "Same unit should be equal"
            );
            Assert.IsTrue(
                TemperatureUnit.KELVIN.Equals(TemperatureUnit.KELVIN),
                "Same unit should be equal"
            );
        }

        /// <summary>
        /// Tests that different unit instances are not equal.
        /// </summary>
        [TestMethod]
        public void Equals_DifferentUnit_ReturnsFalse()
        {
            // Assert
            Assert.IsFalse(
                TemperatureUnit.CELSIUS.Equals(TemperatureUnit.FAHRENHEIT),
                "Different units should not be equal"
            );
            Assert.IsFalse(
                TemperatureUnit.CELSIUS.Equals(TemperatureUnit.KELVIN),
                "Different units should not be equal"
            );
        }

        #endregion
    }
}
