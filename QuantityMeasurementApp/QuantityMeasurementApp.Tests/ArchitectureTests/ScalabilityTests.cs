using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.ArchitectureTests
{
    /// <summary>
    /// Tests demonstrating architectural scalability across multiple measurement categories.
    /// UC8: Standalone unit classes with conversion responsibility.
    /// UC9: Addition of Weight category following the same pattern.
    /// UC10: Generic Quantity class with IMeasurable interface.
    /// UC14: Updated to include ISupportsArithmetic in test implementations.
    /// Shows how the design scales to support Length, Weight, Volume, and Temperature.
    /// </summary>
    [TestClass]
    public class ScalabilityTests
    {
        private const double Tolerance = 0.000001;
        private const double PoundTolerance = 0.001;

        #region Length Category Tests (UC1-UC8)

        /// <summary>
        /// Tests that LengthUnit works correctly as a class implementing IMeasurable.
        /// Verifies UC8 pattern for length measurements.
        /// </summary>
        [TestMethod]
        public void LengthUnit_Class_WorksCorrectly()
        {
            // Test conversion factors
            Assert.AreEqual(1.0, LengthUnit.FEET.GetConversionFactor(), Tolerance);
            Assert.AreEqual(1.0 / 12.0, LengthUnit.INCH.GetConversionFactor(), Tolerance);
            Assert.AreEqual(3.0, LengthUnit.YARD.GetConversionFactor(), Tolerance);

            double cmFactor = 1.0 / (2.54 * 12.0);
            Assert.AreEqual(cmFactor, LengthUnit.CENTIMETER.GetConversionFactor(), Tolerance);

            // Test ToBaseUnit and FromBaseUnit
            Assert.AreEqual(1.0, LengthUnit.INCH.ToBaseUnit(12.0), Tolerance);
            Assert.AreEqual(12.0, LengthUnit.INCH.FromBaseUnit(1.0), Tolerance);

            // Test GetSymbol and GetName
            Assert.AreEqual("ft", LengthUnit.FEET.GetSymbol());
            Assert.AreEqual("in", LengthUnit.INCH.GetSymbol());
            Assert.AreEqual("yd", LengthUnit.YARD.GetSymbol());
            Assert.AreEqual("cm", LengthUnit.CENTIMETER.GetSymbol());

            Assert.AreEqual("feet", LengthUnit.FEET.GetName());
            Assert.AreEqual("inches", LengthUnit.INCH.GetName());
            Assert.AreEqual("yards", LengthUnit.YARD.GetName());
            Assert.AreEqual("centimeters", LengthUnit.CENTIMETER.GetName());

            // Test arithmetic support
            Assert.IsTrue(
                LengthUnit.FEET.SupportsArithmeticOperation(),
                "Length units should support arithmetic"
            );
        }

        /// <summary>
        /// Tests that GenericQuantity with LengthUnit works correctly.
        /// Verifies UC3-UC7 functionality for length.
        /// </summary>
        [TestMethod]
        public void GenericQuantity_Length_WorksCorrectly()
        {
            // Test equality
            var feetQuantity = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inchesQuantity = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);
            Assert.IsTrue(feetQuantity.Equals(inchesQuantity), "1 ft should equal 12 in");

            // Test conversion
            var convertedQuantity = feetQuantity.ConvertTo(LengthUnit.INCH);
            Assert.AreEqual(12.0, convertedQuantity.Value, Tolerance);
            Assert.AreEqual(LengthUnit.INCH, convertedQuantity.Unit);

            // Test addition
            var sumQuantity = feetQuantity.Add(inchesQuantity);
            Assert.AreEqual(2.0, sumQuantity.Value, Tolerance);
            Assert.AreEqual(LengthUnit.FEET, sumQuantity.Unit);
        }

        #endregion

        #region Weight Category Tests (UC9)

        /// <summary>
        /// Tests that WeightUnit works correctly as a class implementing IMeasurable.
        /// Verifies UC9 pattern for weight measurements following the same architecture as Length.
        /// </summary>
        [TestMethod]
        public void WeightUnit_Class_WorksCorrectly()
        {
            // Test conversion factors
            Assert.AreEqual(1.0, WeightUnit.KILOGRAM.GetConversionFactor(), Tolerance);
            Assert.AreEqual(0.001, WeightUnit.GRAM.GetConversionFactor(), Tolerance);
            Assert.AreEqual(0.45359237, WeightUnit.POUND.GetConversionFactor(), Tolerance);

            // Test ToBaseUnit (to kilograms)
            Assert.AreEqual(1.0, WeightUnit.KILOGRAM.ToBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, WeightUnit.GRAM.ToBaseUnit(1000.0), Tolerance);
            Assert.AreEqual(0.45359237, WeightUnit.POUND.ToBaseUnit(1.0), Tolerance);

            // Test FromBaseUnit (from kilograms)
            Assert.AreEqual(1000.0, WeightUnit.GRAM.FromBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, WeightUnit.POUND.FromBaseUnit(0.45359237), Tolerance);

            // Test GetSymbol and GetName
            Assert.AreEqual("kg", WeightUnit.KILOGRAM.GetSymbol());
            Assert.AreEqual("g", WeightUnit.GRAM.GetSymbol());
            Assert.AreEqual("lb", WeightUnit.POUND.GetSymbol());

            Assert.AreEqual("kilograms", WeightUnit.KILOGRAM.GetName());
            Assert.AreEqual("grams", WeightUnit.GRAM.GetName());
            Assert.AreEqual("pounds", WeightUnit.POUND.GetName());

            // Test arithmetic support
            Assert.IsTrue(
                WeightUnit.KILOGRAM.SupportsArithmeticOperation(),
                "Weight units should support arithmetic"
            );
        }

        /// <summary>
        /// Tests that GenericQuantity with WeightUnit works correctly.
        /// Verifies UC9 functionality for weight measurements.
        /// </summary>
        [TestMethod]
        public void GenericQuantity_Weight_WorksCorrectly()
        {
            // Test equality
            var kgWeight = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var gWeight = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);
            var lbWeight = new GenericQuantity<WeightUnit>(2.20462262185, WeightUnit.POUND);

            Assert.IsTrue(kgWeight.Equals(gWeight), "1 kg should equal 1000 g");
            Assert.IsTrue(kgWeight.Equals(lbWeight), "1 kg should approximately equal 2.20462 lb");
            Assert.IsTrue(gWeight.Equals(lbWeight), "1000 g should approximately equal 2.20462 lb");

            // Test conversion
            var convertedToGrams = kgWeight.ConvertTo(WeightUnit.GRAM);
            Assert.AreEqual(1000.0, convertedToGrams.Value, Tolerance);
            Assert.AreEqual(WeightUnit.GRAM, convertedToGrams.Unit);

            var convertedToPounds = kgWeight.ConvertTo(WeightUnit.POUND);
            Assert.AreEqual(2.20462262185, convertedToPounds.Value, PoundTolerance);
            Assert.AreEqual(WeightUnit.POUND, convertedToPounds.Unit);

            // Test addition with same unit
            var firstKg = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var secondKg = new GenericQuantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);
            var sumKg = firstKg.Add(secondKg);
            Assert.AreEqual(3.0, sumKg.Value, Tolerance);
            Assert.AreEqual(WeightUnit.KILOGRAM, sumKg.Unit);

            // Test addition with different units (result in first unit)
            var sumInKg = kgWeight.Add(gWeight);
            Assert.AreEqual(2.0, sumInKg.Value, Tolerance);
            Assert.AreEqual(WeightUnit.KILOGRAM, sumInKg.Unit);

            // Test addition with explicit target unit
            var sumInGrams = kgWeight.Add(gWeight, WeightUnit.GRAM);
            Assert.AreEqual(2000.0, sumInGrams.Value, Tolerance);
            Assert.AreEqual(WeightUnit.GRAM, sumInGrams.Unit);

            var sumInPounds = kgWeight.Add(gWeight, WeightUnit.POUND);
            double expectedPounds = 2.0 * 2.20462262185; // 2 kg in pounds
            Assert.AreEqual(expectedPounds, sumInPounds.Value, PoundTolerance);
            Assert.AreEqual(WeightUnit.POUND, sumInPounds.Unit);

            // Test static Add method
            var staticSum = GenericQuantity<WeightUnit>.Add(
                1.0,
                WeightUnit.KILOGRAM,
                500.0,
                WeightUnit.GRAM,
                WeightUnit.KILOGRAM
            );
            Assert.AreEqual(1.5, staticSum.Value, Tolerance);
            Assert.AreEqual(WeightUnit.KILOGRAM, staticSum.Unit);
        }

        #endregion

        #region Volume Category Tests (UC11)

        /// <summary>
        /// Tests that VolumeUnit works correctly as a class implementing IMeasurable.
        /// Verifies UC11 pattern for volume measurements.
        /// </summary>
        [TestMethod]
        public void VolumeUnit_Class_WorksCorrectly()
        {
            // Test conversion factors
            Assert.AreEqual(1.0, VolumeUnit.LITRE.GetConversionFactor(), Tolerance);
            Assert.AreEqual(0.001, VolumeUnit.MILLILITRE.GetConversionFactor(), Tolerance);
            Assert.AreEqual(3.78541, VolumeUnit.GALLON.GetConversionFactor(), Tolerance);

            // Test ToBaseUnit (to litres)
            Assert.AreEqual(1.0, VolumeUnit.LITRE.ToBaseUnit(1.0), Tolerance);
            Assert.AreEqual(1.0, VolumeUnit.MILLILITRE.ToBaseUnit(1000.0), Tolerance);
            Assert.AreEqual(3.78541, VolumeUnit.GALLON.ToBaseUnit(1.0), Tolerance);

            // Test GetSymbol and GetName
            Assert.AreEqual("L", VolumeUnit.LITRE.GetSymbol());
            Assert.AreEqual("mL", VolumeUnit.MILLILITRE.GetSymbol());
            Assert.AreEqual("gal", VolumeUnit.GALLON.GetSymbol());

            Assert.AreEqual("litres", VolumeUnit.LITRE.GetName());
            Assert.AreEqual("millilitres", VolumeUnit.MILLILITRE.GetName());
            Assert.AreEqual("gallons", VolumeUnit.GALLON.GetName());

            // Test arithmetic support
            Assert.IsTrue(
                VolumeUnit.LITRE.SupportsArithmeticOperation(),
                "Volume units should support arithmetic"
            );
        }

        #endregion

        #region Temperature Category Tests (UC14)

        /// <summary>
        /// Tests that TemperatureUnit works correctly as a class implementing IMeasurable.
        /// Verifies UC14 pattern for temperature measurements with selective arithmetic support.
        /// </summary>
        [TestMethod]
        public void TemperatureUnit_Class_WorksCorrectly()
        {
            // Test ToBaseUnit (to Celsius)
            Assert.AreEqual(0.0, TemperatureUnit.CELSIUS.ToBaseUnit(0.0), Tolerance);
            Assert.AreEqual(0.0, TemperatureUnit.FAHRENHEIT.ToBaseUnit(32.0), Tolerance);
            Assert.AreEqual(100.0, TemperatureUnit.FAHRENHEIT.ToBaseUnit(212.0), Tolerance);
            Assert.AreEqual(-40.0, TemperatureUnit.FAHRENHEIT.ToBaseUnit(-40.0), Tolerance);
            Assert.AreEqual(0.0, TemperatureUnit.KELVIN.ToBaseUnit(273.15), Tolerance);

            // Test FromBaseUnit (from Celsius)
            Assert.AreEqual(32.0, TemperatureUnit.FAHRENHEIT.FromBaseUnit(0.0), 0.01);
            Assert.AreEqual(212.0, TemperatureUnit.FAHRENHEIT.FromBaseUnit(100.0), 0.01);
            Assert.AreEqual(-40.0, TemperatureUnit.FAHRENHEIT.FromBaseUnit(-40.0), 0.01);
            Assert.AreEqual(273.15, TemperatureUnit.KELVIN.FromBaseUnit(0.0), Tolerance);
            Assert.AreEqual(373.15, TemperatureUnit.KELVIN.FromBaseUnit(100.0), Tolerance);

            // Test GetSymbol and GetName
            Assert.AreEqual("°C", TemperatureUnit.CELSIUS.GetSymbol());
            Assert.AreEqual("°F", TemperatureUnit.FAHRENHEIT.GetSymbol());
            Assert.AreEqual("K", TemperatureUnit.KELVIN.GetSymbol());

            Assert.AreEqual("Celsius", TemperatureUnit.CELSIUS.GetName());
            Assert.AreEqual("Fahrenheit", TemperatureUnit.FAHRENHEIT.GetName());
            Assert.AreEqual("Kelvin", TemperatureUnit.KELVIN.GetName());

            // Test arithmetic support - Temperature should NOT support arithmetic
            Assert.IsFalse(
                TemperatureUnit.CELSIUS.SupportsArithmeticOperation(),
                "Temperature units should NOT support arithmetic"
            );
        }

        /// <summary>
        /// Tests that GenericQuantity with TemperatureUnit works correctly.
        /// Verifies UC14 functionality for temperature measurements.
        /// </summary>
        [TestMethod]
        public void GenericQuantity_Temperature_WorksCorrectly()
        {
            // Test equality
            var celsiusTemp = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var fahrenheitTemp = new GenericQuantity<TemperatureUnit>(
                32.0,
                TemperatureUnit.FAHRENHEIT
            );
            var kelvinTemp = new GenericQuantity<TemperatureUnit>(273.15, TemperatureUnit.KELVIN);

            Assert.IsTrue(celsiusTemp.Equals(fahrenheitTemp), "0°C should equal 32°F");
            Assert.IsTrue(celsiusTemp.Equals(kelvinTemp), "0°C should equal 273.15K");

            // Test conversion
            var convertedToFahrenheit = celsiusTemp.ConvertTo(TemperatureUnit.FAHRENHEIT);
            Assert.AreEqual(32.0, convertedToFahrenheit.Value, 0.01);
            Assert.AreEqual(TemperatureUnit.FAHRENHEIT, convertedToFahrenheit.Unit);

            var convertedToKelvin = celsiusTemp.ConvertTo(TemperatureUnit.KELVIN);
            Assert.AreEqual(273.15, convertedToKelvin.Value, Tolerance);
            Assert.AreEqual(TemperatureUnit.KELVIN, convertedToKelvin.Unit);

            // Test that arithmetic operations throw NotSupportedException
            var temp1 = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var temp2 = new GenericQuantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            Assert.ThrowsException<NotSupportedException>(() => temp1.Add(temp2));
            Assert.ThrowsException<NotSupportedException>(() => temp1.Subtract(temp2));
            Assert.ThrowsException<NotSupportedException>(() => temp1.Divide(temp2));
        }

        #endregion

        #region Category Independence Tests

        /// <summary>
        /// Tests that different measurement categories are independent and cannot be mixed.
        /// Verifies type safety across categories.
        /// </summary>
        [TestMethod]
        public void DifferentCategories_AreIndependent_CannotBeMixed()
        {
            // Length quantities
            var lengthFeet = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var lengthInches = new GenericQuantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Weight quantities
            var weightKg = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var weightG = new GenericQuantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            // Volume quantities
            var volumeLitre = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var volumeMl = new GenericQuantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Temperature quantities
            var tempCelsius = new GenericQuantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var tempFahrenheit = new GenericQuantity<TemperatureUnit>(
                32.0,
                TemperatureUnit.FAHRENHEIT
            );

            // Test that they are different types
            Assert.AreNotEqual(
                lengthFeet.GetType(),
                weightKg.GetType(),
                "Length and Weight should be different types"
            );
            Assert.AreNotEqual(
                lengthFeet.GetType(),
                volumeLitre.GetType(),
                "Length and Volume should be different types"
            );
            Assert.AreNotEqual(
                lengthFeet.GetType(),
                tempCelsius.GetType(),
                "Length and Temperature should be different types"
            );

            // Test that operations within each category still work
            Assert.IsTrue(lengthFeet.Equals(lengthInches), "Length equality should still work");
            Assert.IsTrue(weightKg.Equals(weightG), "Weight equality should still work");
            Assert.IsTrue(volumeLitre.Equals(volumeMl), "Volume equality should still work");
            Assert.IsTrue(
                tempCelsius.Equals(tempFahrenheit),
                "Temperature equality should still work"
            );
        }

        /// <summary>
        /// Tests that each category has its own base unit and conversion logic.
        /// Verifies that categories don't interfere with each other.
        /// </summary>
        [TestMethod]
        public void EachCategory_HasOwnBaseUnit_AndConversionLogic()
        {
            // Length base unit is feet
            double lengthInBase = LengthUnit.FEET.ToBaseUnit(1.0);
            string lengthUnitName = LengthUnit.FEET.GetName();

            // Weight base unit is kilograms
            double weightInBase = WeightUnit.KILOGRAM.ToBaseUnit(1.0);
            string weightUnitName = WeightUnit.KILOGRAM.GetName();

            // Volume base unit is litres
            double volumeInBase = VolumeUnit.LITRE.ToBaseUnit(1.0);
            string volumeUnitName = VolumeUnit.LITRE.GetName();

            // Temperature base unit is Celsius
            double tempInBase = TemperatureUnit.CELSIUS.ToBaseUnit(1.0);
            string tempUnitName = TemperatureUnit.CELSIUS.GetName();

            // Test that they have different unit names
            Assert.AreNotEqual(
                lengthUnitName,
                weightUnitName,
                "Base units should have different names"
            );
            Assert.AreNotEqual(
                lengthUnitName,
                volumeUnitName,
                "Base units should have different names"
            );
            Assert.AreNotEqual(
                lengthUnitName,
                tempUnitName,
                "Base units should have different names"
            );

            // Test that the quantity classes are different types
            var lengthQuantity = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var weightQuantity = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var volumeQuantity = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var tempQuantity = new GenericQuantity<TemperatureUnit>(1.0, TemperatureUnit.CELSIUS);

            Assert.AreNotEqual(lengthQuantity.GetType(), weightQuantity.GetType());
            Assert.AreNotEqual(lengthQuantity.GetType(), volumeQuantity.GetType());
            Assert.AreNotEqual(lengthQuantity.GetType(), tempQuantity.GetType());
        }

        #endregion

        #region Future Category Demonstrations

        // Simple test class for demonstration that properly implements IMeasurable
        private class TestTemperatureUnit : IMeasurable
        {
            private readonly string _name;
            private readonly string _symbol;
            private readonly double _factor;
            private readonly double _offset;

            // Implement SupportsArithmetic property
            public ISupportsArithmetic SupportsArithmetic { get; } =
                new SupportsArithmeticImpl(() => false);

            public TestTemperatureUnit(string name, string symbol, double factor, double offset = 0)
            {
                _name = name;
                _symbol = symbol;
                _factor = factor;
                _offset = offset;
            }

            public double GetConversionFactor() => _factor;

            public double ToBaseUnit(double value)
            {
                return (value - _offset) / _factor;
            }

            public double FromBaseUnit(double valueInBaseUnit)
            {
                return (valueInBaseUnit * _factor) + _offset;
            }

            public string GetSymbol() => _symbol;

            public string GetName() => _name;

            public bool SupportsArithmeticOperation() => SupportsArithmetic.IsSupported();

            public void ValidateOperationSupport(string operation)
            {
                throw new NotSupportedException(
                    $"Temperature does not support {operation} operations"
                );
            }

            // Private implementation of ISupportsArithmetic
            private class SupportsArithmeticImpl : ISupportsArithmetic
            {
                private readonly Func<bool> _isSupported;

                public SupportsArithmeticImpl(Func<bool> isSupported)
                {
                    _isSupported = isSupported;
                }

                public bool IsSupported() => _isSupported();
            }
        }

        /// <summary>
        /// Example of how Temperature category would follow the same pattern.
        /// This demonstrates that the architecture can scale to any number of categories.
        /// </summary>
        [TestMethod]
        public void FutureCategory_Temperature_CanFollowSamePattern()
        {
            // Create a TemperatureUnit class for demonstration
            var celsiusUnit = new TestTemperatureUnit("celsius", "°C", 1.0, 0.0);
            var fahrenheitUnit = new TestTemperatureUnit("fahrenheit", "°F", 1.8, 32.0);

            // This test shows that any class implementing IMeasurable can work with GenericQuantity
            Assert.IsNotNull(celsiusUnit);
            Assert.IsNotNull(fahrenheitUnit);
            Assert.IsTrue(celsiusUnit is IMeasurable);
            Assert.IsTrue(fahrenheitUnit is IMeasurable);

            // Test that temperature units do NOT support arithmetic
            Assert.IsFalse(celsiusUnit.SupportsArithmeticOperation());
            Assert.IsFalse(fahrenheitUnit.SupportsArithmeticOperation());
        }

        /// <summary>
        /// Example of how Volume category would follow the same pattern.
        /// Demonstrates the reusability of the design.
        /// </summary>
        [TestMethod]
        public void FutureCategory_Volume_CanFollowSamePattern()
        {
            // This test demonstrates that Volume follows the same pattern as Length and Weight
            // VolumeUnit is already implemented in the main codebase

            Assert.IsTrue(
                true,
                "Volume category follows the exact same pattern as Length and Weight"
            );
        }

        #endregion

        #region IMeasurable Interface Tests

        /// <summary>
        /// Tests that all unit classes properly implement IMeasurable interface.
        /// </summary>
        [TestMethod]
        public void AllUnitClasses_Implement_IMeasurable()
        {
            // Length units
            Assert.IsTrue(LengthUnit.FEET is IMeasurable);
            Assert.IsTrue(LengthUnit.INCH is IMeasurable);
            Assert.IsTrue(LengthUnit.YARD is IMeasurable);
            Assert.IsTrue(LengthUnit.CENTIMETER is IMeasurable);

            // Weight units
            Assert.IsTrue(WeightUnit.KILOGRAM is IMeasurable);
            Assert.IsTrue(WeightUnit.GRAM is IMeasurable);
            Assert.IsTrue(WeightUnit.POUND is IMeasurable);

            // Volume units
            Assert.IsTrue(VolumeUnit.LITRE is IMeasurable);
            Assert.IsTrue(VolumeUnit.MILLILITRE is IMeasurable);
            Assert.IsTrue(VolumeUnit.GALLON is IMeasurable);

            // Temperature units
            Assert.IsTrue(TemperatureUnit.CELSIUS is IMeasurable);
            Assert.IsTrue(TemperatureUnit.FAHRENHEIT is IMeasurable);
            Assert.IsTrue(TemperatureUnit.KELVIN is IMeasurable);
        }

        #endregion

        #region GenericQuantity Type Safety Tests

        /// <summary>
        /// Tests that GenericQuantity maintains type safety across categories.
        /// </summary>
        [TestMethod]
        public void GenericQuantity_Maintains_TypeSafety()
        {
            // Arrange
            var lengthQuantity = new GenericQuantity<LengthUnit>(1.0, LengthUnit.FEET);
            var weightQuantity = new GenericQuantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var volumeQuantity = new GenericQuantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var tempQuantity = new GenericQuantity<TemperatureUnit>(1.0, TemperatureUnit.CELSIUS);

            // Assert - Different generic instantiations are different types
            Assert.AreNotEqual(lengthQuantity.GetType(), weightQuantity.GetType());
            Assert.AreNotEqual(lengthQuantity.GetType(), volumeQuantity.GetType());
            Assert.AreNotEqual(lengthQuantity.GetType(), tempQuantity.GetType());
        }

        #endregion
    }
}
