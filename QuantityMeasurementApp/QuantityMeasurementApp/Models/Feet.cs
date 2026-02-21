using System;

namespace QuantityMeasurementApp.Models
{
    /// <summary>
    /// Represents a feet measurement with value-based equality
    /// This class is immutable - once created, its value cannot be changed
    /// </summary>
    public class Feet
    {
        // Private field to store the measurement value
        // Made readonly to ensure immutability
        private readonly double _value;

        // Constructor to initialize a new Feet object with a given value
        // Parameter: value - The measurement value in feet
        public Feet(double value)
        {
            _value = value;
        }

        // Public property to access the measurement value
        // Since the field is readonly, this provides read-only access
        public double Value => _value;

        // Determines whether the specified object is equal to the current Feet object
        // Implements value-based equality with proper null checking and type safety
        // Parameter: obj - The object to compare with the current object
        // Returns: true if the specified object is equal to the current object; otherwise, false
        public override bool Equals(object? obj)
        {
            // Check if the object is the same reference (reflexive property)
            // If both references point to the same object, they are equal
            if (ReferenceEquals(this, obj))
                return true;

            // Check if the object is null
            // By contract, no object should equal null
            if (obj is null)
                return false;

            // Check if the object is of different type (type safety)
            // We only want to compare Feet objects with other Feet objects
            if (GetType() != obj.GetType())
                return false;

            // Safe cast after type check - this will never fail due to above check
            Feet other = (Feet)obj;

            // Compare double values using exact equality
            // This ensures that 1.000001 and 1.000002 are considered different
            // which is important for the test cases
            return _value == other._value;
        }

        // Serves as the default hash function
        // Returns a hash code for the current object
        // Important: Equal objects must have equal hash codes
        public override int GetHashCode()
        {
            // Use the built-in hash code of the double value
            // This ensures that equal values produce the same hash code
            return _value.GetHashCode();
        }

        // Returns a string representation of the Feet object
        // Format: "{value} ft" (e.g., "1.5 ft")
        public override string ToString()
        {
            return $"{_value} ft";
        }
    }
}
