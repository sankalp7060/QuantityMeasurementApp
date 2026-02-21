using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.Tests.TestHelpers
{
    /// <summary>
    /// Factory class for creating test data.
    /// </summary>
    public static class TestDataFactory
    {
        /// <summary>
        /// Creates a quantity for testing.
        /// </summary>
        public static Quantity CreateQuantity(double value, LengthUnit unit)
        {
            return new Quantity(value, unit);
        }

        /// <summary>
        /// Creates equivalent quantities in different units.
        /// </summary>
        public static (
            Quantity Feet,
            Quantity Inches,
            Quantity Yards,
            Quantity Cm
        ) CreateEquivalentQuantities(double feetValue)
        {
            return (
                Feet: new Quantity(feetValue, LengthUnit.FEET),
                Inches: new Quantity(feetValue * 12, LengthUnit.INCH),
                Yards: new Quantity(feetValue / 3, LengthUnit.YARD),
                Cm: new Quantity(feetValue * 30.48, LengthUnit.CENTIMETER)
            );
        }
    }
}
