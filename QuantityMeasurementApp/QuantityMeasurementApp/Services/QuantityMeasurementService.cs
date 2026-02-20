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
            return feet1.Equals(feet2);
        }

        // Creates a Feet object from a string input
        // Parameter: input - String input to parse (can be null or whitespace)
        // Returns: Feet object if parsing successful; otherwise, null
        public Feet? ParseFeetInput(string? input)
        {
            // Check for null or whitespace input
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Try to parse the string as a double
            if (double.TryParse(input, out double value))
            {
                // Successfully parsed, create and return a new Feet object
                return new Feet(value);
            }

            // Parsing failed (non-numeric input), return null
            return null;
        }

        // Compares two inch measurements for equality
        // Parameter: inch1 - First inch measurement (can be null)
        // Parameter: inch2 - Second inch measurement (can be null)
        // Returns: True if both measurements are equal and non-null; otherwise, false
        public bool CompareInchEquality(Inch? inch1, Inch? inch2)
        {
            // Handle null cases - if either measurement is null, they cannot be equal
            if (inch1 is null || inch2 is null)
                return false;

            // Delegate the equality check to the Inch class's Equals method
            return inch1.Equals(inch2);
        }

        // Creates an Inch object from a string input
        // Parameter: input - String input to parse (can be null or whitespace)
        // Returns: Inch object if parsing successful; otherwise, null
        public Inch? ParseInchInput(string? input)
        {
            // Check for null or whitespace input
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Try to parse the string as a double
            if (double.TryParse(input, out double value))
            {
                // Successfully parsed, create and return a new Inch object
                return new Inch(value);
            }

            // Parsing failed (non-numeric input), return null
            return null;
        }

        // Static method for feet equality check - reduces dependency on main method
        // Parameter: value1 - First feet value as double
        // Parameter: value2 - Second feet value as double
        // Returns: True if both feet measurements are equal
        public static bool AreFeetEqual(double value1, double value2)
        {
            Feet feet1 = new Feet(value1);
            Feet feet2 = new Feet(value2);
            return feet1.Equals(feet2);
        }

        // Static method for inch equality check - reduces dependency on main method
        // Parameter: value1 - First inch value as double
        // Parameter: value2 - Second inch value as double
        // Returns: True if both inch measurements are equal
        public static bool AreInchEqual(double value1, double value2)
        {
            Inch inch1 = new Inch(value1);
            Inch inch2 = new Inch(value2);
            return inch1.Equals(inch2);
        }
    }
}
