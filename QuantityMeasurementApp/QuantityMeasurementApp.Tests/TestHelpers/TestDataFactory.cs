using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.TestHelpers
{
    /// <summary>
    /// Factory class for creating test data with GenericQuantity.
    /// </summary>
    public static class TestDataFactory
    {
        /// <summary>
        /// Creates a generic quantity for testing.
        /// </summary>
        /// <typeparam name="T">The unit type (must implement IMeasurable).</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="unit">The unit.</param>
        /// <returns>A GenericQuantity instance.</returns>
        public static GenericQuantity<T> CreateQuantity<T>(double value, T unit)
            where T : class, IMeasurable
        {
            return new GenericQuantity<T>(value, unit);
        }

        /// <summary>
        /// Creates equivalent length quantities in different units.
        /// </summary>
        /// <param name="feetValue">The value in feet.</param>
        /// <returns>A tuple of equivalent length quantities.</returns>
        public static (
            GenericQuantity<LengthUnit> Feet,
            GenericQuantity<LengthUnit> Inches,
            GenericQuantity<LengthUnit> Yards,
            GenericQuantity<LengthUnit> Cm
        ) CreateEquivalentLengths(double feetValue)
        {
            return (
                Feet: new GenericQuantity<LengthUnit>(feetValue, LengthUnit.FEET),
                Inches: new GenericQuantity<LengthUnit>(feetValue * 12, LengthUnit.INCH),
                Yards: new GenericQuantity<LengthUnit>(feetValue / 3, LengthUnit.YARD),
                Cm: new GenericQuantity<LengthUnit>(feetValue * 30.48, LengthUnit.CENTIMETER)
            );
        }

        /// <summary>
        /// Creates equivalent weight quantities in different units.
        /// </summary>
        /// <param name="kgValue">The value in kilograms.</param>
        /// <returns>A tuple of equivalent weight quantities.</returns>
        public static (
            GenericQuantity<WeightUnit> Kg,
            GenericQuantity<WeightUnit> Grams,
            GenericQuantity<WeightUnit> Pounds
        ) CreateEquivalentWeights(double kgValue)
        {
            return (
                Kg: new GenericQuantity<WeightUnit>(kgValue, WeightUnit.KILOGRAM),
                Grams: new GenericQuantity<WeightUnit>(kgValue * 1000, WeightUnit.GRAM),
                Pounds: new GenericQuantity<WeightUnit>(kgValue * 2.20462262185, WeightUnit.POUND)
            );
        }
    }
}
