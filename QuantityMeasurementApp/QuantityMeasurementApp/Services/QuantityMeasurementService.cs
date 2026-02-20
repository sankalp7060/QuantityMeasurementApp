using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Services
{
    // Service class for quantity measurement operations
    // This class handles the business logic for comparing measurements
    public class QuantityMeasurementService
    {
        // Compares two feet measurements for equality
        // Parameter: feet1 - First feet measurement (can be null)
        // Parameter: feet2 - Second feet measurement (can be null)
        // Returns: True if both measurements are equal and non-null; otherwise, false
        public bool CompareFeetEquality(Feet? feet1, Feet? feet2)
        {
            // Handle null cases - if either measurement is null, they cannot be equal
            if (feet1 is null || feet2 is null)
                return false;

            // Delegate the equality check to the Feet class's Equals method
            // This ensures consistent behavior with the Feet class's own equality logic
            return feet1.Equals(feet2);
        }

        // Creates a Feet object from a string input
        // Parameter: input - String input to parse (can be null or whitespace)
        // Returns: Feet object if parsing successful; otherwise, null
        public Feet? ParseFeetInput(string? input)
        {
            // Check for null or whitespace input
            // Empty strings or strings with only spaces are considered invalid
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Try to parse the string as a double
            // double.TryParse returns true if parsing succeeds, false otherwise
            if (double.TryParse(input, out double value))
            {
                // Successfully parsed, create and return a new Feet object
                return new Feet(value);
            }

            // Parsing failed (non-numeric input), return null
            return null;
        }
    }
}
