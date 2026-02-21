using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for temperature measurements.
    /// UC14: Tests temperature workflows and cross-category behavior.
    /// </summary>
    [TestClass]
    public class TemperatureEndToEndTests
    {
        private GenericMeasurementService _measurementService = null!;
        private const double Tolerance = 0.000001;
        private const double FahrenheitTolerance = 0.01;

        [TestInitialize]
        public void Setup()
        {
            _measurementService = new GenericMeasurementService();
        }

        /// <summary>
        /// Tests a complete temperature workflow: create, convert, compare.
        /// </summary>
        [TestMethod]
        public void CompleteWorkflow_Temperature_AllOperations_WorkCorrectly()
        {
            // Step 1: Create quantities from string inputs
            var celsiusTemp = _measurementService.CreateQuantityFromString(
                "100",
                TemperatureUnit.CELSIUS
            );
            var fahrenheitTemp = _measurementService.CreateQuantityFromString(
                "212",
                TemperatureUnit.FAHRENHEIT
            );

            Assert.IsNotNull(celsiusTemp, "Celsius temperature should not be null");
            Assert.IsNotNull(fahrenheitTemp, "Fahrenheit temperature should not be null");

            // Step 2: Convert Celsius to Fahrenheit
            var convertedToFahrenheit = celsiusTemp!.ConvertTo(TemperatureUnit.FAHRENHEIT);
            Assert.AreEqual(
                212.0,
                convertedToFahrenheit.Value,
                FahrenheitTolerance,
                "100°C should convert to 212°F"
            );

            // Step 3: Compare with expected
            bool areEqual = _measurementService.AreQuantitiesEqual(
                convertedToFahrenheit,
                fahrenheitTemp!
            );
            Assert.IsTrue(areEqual, "Converted temperature should equal original Fahrenheit");

            // Step 4: Convert Fahrenheit to Celsius
            var convertedToCelsius = fahrenheitTemp!.ConvertTo(TemperatureUnit.CELSIUS);
            Assert.AreEqual(
                100.0,
                convertedToCelsius.Value,
                FahrenheitTolerance,
                "212°F should convert back to 100°C"
            );

            // Step 5: Compare original with converted
            areEqual = _measurementService.AreQuantitiesEqual(celsiusTemp, convertedToCelsius);
            Assert.IsTrue(areEqual, "Round-trip conversion should preserve value");
        }

        /// <summary>
        /// Tests cross-unit comparison across temperature units.
        /// </summary>
        [TestMethod]
        public void CompareAcrossUnits_Temperature_ReturnsCorrectResult()
        {
            // Test common reference points
            var freezingCelsius = new GenericQuantity<TemperatureUnit>(
                0.0,
                TemperatureUnit.CELSIUS
            );
            var freezingFahrenheit = new GenericQuantity<TemperatureUnit>(
                32.0,
                TemperatureUnit.FAHRENHEIT
            );
            var freezingKelvin = new GenericQuantity<TemperatureUnit>(
                273.15,
                TemperatureUnit.KELVIN
            );

            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(freezingCelsius, freezingFahrenheit),
                "0°C should equal 32°F"
            );
            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(freezingCelsius, freezingKelvin),
                "0°C should equal 273.15K"
            );

            var boilingCelsius = new GenericQuantity<TemperatureUnit>(
                100.0,
                TemperatureUnit.CELSIUS
            );
            var boilingFahrenheit = new GenericQuantity<TemperatureUnit>(
                212.0,
                TemperatureUnit.FAHRENHEIT
            );
            var boilingKelvin = new GenericQuantity<TemperatureUnit>(
                373.15,
                TemperatureUnit.KELVIN
            );

            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(boilingCelsius, boilingFahrenheit),
                "100°C should equal 212°F"
            );
            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(boilingCelsius, boilingKelvin),
                "100°C should equal 373.15K"
            );

            // Test special equality point
            var specialCelsius = new GenericQuantity<TemperatureUnit>(
                -40.0,
                TemperatureUnit.CELSIUS
            );
            var specialFahrenheit = new GenericQuantity<TemperatureUnit>(
                -40.0,
                TemperatureUnit.FAHRENHEIT
            );

            Assert.IsTrue(
                _measurementService.AreQuantitiesEqual(specialCelsius, specialFahrenheit),
                "-40°C should equal -40°F (unique equality point)"
            );
        }

        /// <summary>
        /// Tests that temperature cannot be compared with other categories.
        /// </summary>
        [TestMethod]
        public void Temperature_CrossCategory_ReturnsFalse()
        {
            var temp = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var length = new GenericQuantity<LengthUnit>(100.0, LengthUnit.FEET);
            var weight = new GenericQuantity<WeightUnit>(100.0, WeightUnit.KILOGRAM);
            var volume = new GenericQuantity<VolumeUnit>(100.0, VolumeUnit.LITRE);

            // These comparisons should return false (not throw exceptions)
            Assert.IsFalse(temp.Equals(length), "Temperature and length should not be equal");
            Assert.IsFalse(temp.Equals(weight), "Temperature and weight should not be equal");
            Assert.IsFalse(temp.Equals(volume), "Temperature and volume should not be equal");
        }

        /// <summary>
        /// Tests that arithmetic operations are not supported for temperature.
        /// </summary>
        [TestMethod]
        public void Temperature_ArithmeticOperations_NotSupported()
        {
            var temp1 = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var temp2 = new GenericQuantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            Assert.ThrowsException<NotSupportedException>(() =>
                _measurementService.AddQuantities(temp1, temp2)
            );
            Assert.ThrowsException<NotSupportedException>(() =>
                _measurementService.AddQuantitiesWithTarget(
                    temp1,
                    temp2,
                    TemperatureUnit.FAHRENHEIT
                )
            );
            Assert.ThrowsException<NotSupportedException>(() => temp1.Subtract(temp2));
            Assert.ThrowsException<NotSupportedException>(() => temp1.Divide(temp2));
        }

        /// <summary>
        /// Tests that temperature works with existing generic service methods.
        /// </summary>
        [TestMethod]
        public void Temperature_WorksWithGenericService()
        {
            // Create temperature
            var created = _measurementService.CreateQuantityFromString(
                "25",
                TemperatureUnit.CELSIUS
            );
            Assert.IsNotNull(created);
            Assert.AreEqual(25.0, created!.Value);
            Assert.AreEqual(TemperatureUnit.CELSIUS, created.Unit);

            // Convert via service
            var converted = _measurementService.ConvertValue(
                25.0,
                TemperatureUnit.CELSIUS,
                TemperatureUnit.FAHRENHEIT
            );
            Assert.AreEqual(77.0, converted, FahrenheitTolerance, "25°C should convert to 77°F");
        }
    }
}
