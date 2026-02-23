using System;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Generic Quantity class that represents any measurement with a value and unit
    /// This eliminates code duplication from separate Feet and Inch classes
    /// </summary>
    public class Quantity
    {
        // Private fields for value and unit
        private readonly double _value;
        private readonly LengthUnit _unit;

        // Create an instance of LengthUnitExtensions for non-static methods
        private readonly LengthUnitExtensions _unitExtensions;

        // Constructor to initialize a new Quantity object with a value and unit
        // Parameter: value - The measurement value
        // Parameter: unit - The unit of measurement (from LengthUnit enum)
        public Quantity(double value, LengthUnit unit)
        {
            _value = value;
            _unit = unit;
            _unitExtensions = new LengthUnitExtensions();
        }

        // Public property to access the measurement value
        public double Value => _value;

        // Public property to access the measurement unit
        public LengthUnit Unit => _unit;

        // Convert the current quantity to feet (base unit)
        // Returns: The value converted to feet
        private double ConvertToFeet()
        {
            return _value * _unitExtensions.GetConversionFactorToFeet(_unit);
        }

        // Determines whether the specified object is equal to the current Quantity object
        // Compares quantities by converting both to a common base unit (feet)
        // Parameter: obj - The object to compare with the current object
        // Returns: true if the specified object is equal to the current object; otherwise, false
        public override bool Equals(object? obj)
        {
            // Check if the object is the same reference (reflexive property)
            if (ReferenceEquals(this, obj))
                return true;

            // Check if the object is null
            if (obj is null)
                return false;

            // Check if the object is of different type (type safety)
            if (GetType() != obj.GetType())
                return false;

            // Safe cast after type check
            Quantity other = (Quantity)obj;

            // Convert both quantities to feet for comparison
            double thisInFeet = this.ConvertToFeet();
            double otherInFeet = other.ConvertToFeet();

            // Compare the converted values with tolerance for floating point precision
            return _unitExtensions.AreApproximatelyEqual(thisInFeet, otherInFeet);
        }

        // Serves as the default hash function
        // Returns a hash code for the current object based on converted value
        public override int GetHashCode()
        {
            // Use the converted value rounded to account for floating point precision
            double convertedValue = ConvertToFeet();
            // Round to 6 decimal places to handle floating point precision
            return Math.Round(convertedValue, 6).GetHashCode();
        }

        // Returns a string representation of the Quantity object
        // Format: "{value} {unitSymbol}" (e.g., "1.5 ft" or "12 in")
        public override string ToString()
        {
            return $"{_value} {LengthUnitExtensions.GetUnitSymbol(_unit)}";
        }
    }
}
