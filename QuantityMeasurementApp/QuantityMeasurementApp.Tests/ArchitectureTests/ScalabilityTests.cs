using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.ArchitectureTests
{
    /// <summary>
    /// Tests demonstrating architectural scalability (UC8).
    /// Shows how new measurement categories can follow the same pattern.
    /// </summary>
    [TestClass]
    public class ScalabilityTests
    {
        private const double Tolerance = 0.000001;

        /// <summary>
        /// Tests that the pattern can be replicated for weight units.
        /// </summary>
        [TestMethod]
        public void Pattern_CanBeReplicated_ForWeightUnit()
        {
            // Test conversion
            double poundsToOunces = WeightUnit.POUND.ConvertTo(WeightUnit.OUNCE, 1.0);
            Assert.AreEqual(16.0, poundsToOunces, Tolerance);

            // Test quantity equality
            var w1 = new WeightQuantity(1.0, WeightUnit.POUND);
            var w2 = new WeightQuantity(16.0, WeightUnit.OUNCE);

            Assert.IsTrue(w1.Equals(w2), "1 lb should equal 16 oz");

            // Test conversion
            var converted = w1.ConvertTo(WeightUnit.OUNCE);
            Assert.AreEqual(16.0, converted.Value, Tolerance);
            Assert.AreEqual(WeightUnit.OUNCE, converted.Unit);
        }

        /// <summary>
        /// Tests that different categories are independent.
        /// </summary>
        [TestMethod]
        public void DifferentCategories_AreIndependent()
        {
            var length = new Quantity(1.0, LengthUnit.FEET);
            var weight = new WeightQuantity(1.0, WeightUnit.POUND);

            Assert.IsNotNull(length);
            Assert.IsNotNull(weight);
            Assert.AreEqual(1.0, length.Value);
            Assert.AreEqual(1.0, weight.Value);
        }
    }

    #region Example WeightUnit Implementation (Top-Level Classes)

    /// <summary>
    /// Example enum for weight units demonstrating the pattern.
    /// </summary>
    public enum WeightUnit
    {
        /// <summary>Pounds - base unit for weight.</summary>
        POUND,

        /// <summary>Ounces - 1 pound = 16 ounces.</summary>
        OUNCE,

        /// <summary>Kilograms - 1 kilogram = 2.20462 pounds.</summary>
        KILOGRAM,
    }

    /// <summary>
    /// Extension class for WeightUnit providing conversion functionality.
    /// This is a top-level static class as required for extension methods.
    /// </summary>
    public static class WeightUnitExtensions
    {
        // Conversion factors to pounds (base unit)
        private static readonly double[] ToPoundsFactors = new double[]
        {
            1.0, // POUND to POUND
            1.0 / 16.0, // OUNCE to POUND
            2.20462, // KILOGRAM to POUND
        };

        /// <summary>
        /// Gets the conversion factor for this unit to pounds.
        /// </summary>
        /// <param name="unit">The weight unit.</param>
        /// <returns>The conversion factor to pounds.</returns>
        public static double GetConversionFactor(this WeightUnit unit)
        {
            int index = (int)unit;
            return ToPoundsFactors[index];
        }

        /// <summary>
        /// Converts a value from this unit to the base unit (pounds).
        /// </summary>
        /// <param name="unit">The source unit.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value in pounds.</returns>
        public static double ToBaseUnit(this WeightUnit unit, double value)
        {
            return value * unit.GetConversionFactor();
        }

        /// <summary>
        /// Converts a value from the base unit (pounds) to this unit.
        /// </summary>
        /// <param name="unit">The target unit.</param>
        /// <param name="valueInPounds">The value in pounds.</param>
        /// <returns>The value in this unit.</returns>
        public static double FromBaseUnit(this WeightUnit unit, double valueInPounds)
        {
            return valueInPounds / unit.GetConversionFactor();
        }

        /// <summary>
        /// Converts a value from one unit to another.
        /// </summary>
        /// <param name="sourceUnit">The source unit.</param>
        /// <param name="targetUnit">The target unit.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double ConvertTo(
            this WeightUnit sourceUnit,
            WeightUnit targetUnit,
            double value
        )
        {
            double valueInPounds = sourceUnit.ToBaseUnit(value);
            return targetUnit.FromBaseUnit(valueInPounds);
        }

        /// <summary>
        /// Gets the symbol for the unit.
        /// </summary>
        /// <param name="unit">The weight unit.</param>
        /// <returns>The unit symbol.</returns>
        public static string GetSymbol(this WeightUnit unit)
        {
            return unit switch
            {
                WeightUnit.POUND => "lb",
                WeightUnit.OUNCE => "oz",
                WeightUnit.KILOGRAM => "kg",
                _ => unit.ToString().ToLower(),
            };
        }
    }

    /// <summary>
    /// Example Quantity class for weight measurements.
    /// </summary>
    public class WeightQuantity
    {
        private readonly double _value;
        private readonly WeightUnit _unit;

        /// <summary>
        /// Initializes a new instance of the WeightQuantity class.
        /// </summary>
        /// <param name="value">The weight value.</param>
        /// <param name="unit">The weight unit.</param>
        public WeightQuantity(double value, WeightUnit unit)
        {
            _value = value;
            _unit = unit;
        }

        /// <summary>
        /// Gets the weight value.
        /// </summary>
        public double Value => _value;

        /// <summary>
        /// Gets the weight unit.
        /// </summary>
        public WeightUnit Unit => _unit;

        /// <summary>
        /// Converts this weight to a target unit.
        /// </summary>
        /// <param name="targetUnit">The target unit.</param>
        /// <returns>A new WeightQuantity in the target unit.</returns>
        public WeightQuantity ConvertTo(WeightUnit targetUnit)
        {
            double inPounds = _unit.ToBaseUnit(_value);
            double converted = targetUnit.FromBaseUnit(inPounds);
            return new WeightQuantity(converted, targetUnit);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current WeightQuantity.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is null || GetType() != obj.GetType())
                return false;

            WeightQuantity other = (WeightQuantity)obj;
            double thisInPounds = _unit.ToBaseUnit(_value);
            double otherInPounds = other._unit.ToBaseUnit(other._value);

            return Math.Abs(thisInPounds - otherInPounds) < 0.000001;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            double valueInPounds = _unit.ToBaseUnit(_value);
            return Math.Round(valueInPounds, 6).GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the WeightQuantity.
        /// </summary>
        /// <returns>String in format "{value} {symbol}".</returns>
        public override string ToString()
        {
            return $"{_value} {_unit.GetSymbol()}";
        }
    }

    #endregion
}
