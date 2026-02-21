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

        // Constructor to initialize a new Quantity object with a value and unit
        // Parameter: value - The measurement value
        // Parameter: unit - The unit of measurement (from LengthUnit enum)
        public Quantity(double value, LengthUnit unit)
        {
            _value = value;
            _unit = unit;
        }

        // Public property to access the measurement value
        public double Value => _value;

        // Public property to access the measurement unit
        public LengthUnit Unit => _unit;

        // Convert the current quantity to feet (base unit)
        // Returns: The value converted to feet
        private double ConvertToFeet()
        {
            return _value * _unit.GetConversionFactorToFeet();
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

            // Compare the converted values with a small tolerance for floating point precision
            // Using exact equality for strict comparison
            return thisInFeet == otherInFeet;
        }

        // Serves as the default hash function
        // Returns a hash code for the current object based on converted value
        public override int GetHashCode()
        {
            // Use the converted value to ensure consistent hash codes
            return ConvertToFeet().GetHashCode();
        }

        // Returns a string representation of the Quantity object
        // Format: "{value} {unitSymbol}" (e.g., "1.5 ft" or "12 in")
        public override string ToString()
        {
            return $"{_value} {_unit.GetUnitSymbol()}";
        }
    }
}
