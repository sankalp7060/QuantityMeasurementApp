using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.UI.Helpers
{
    /// <summary>
    /// Helper class for weight unit selection.
    /// UC9: Dedicated unit selector for weight measurements.
    /// </summary>
    public static class WeightUnitSelector
    {
        /// <summary>
        /// Allows user to select a weight unit from a menu.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <returns>The selected WeightUnit.</returns>
        public static WeightUnit SelectUnit(string prompt)
        {
            while (true)
            {
                Console.WriteLine($"\n{prompt}:");
                Console.WriteLine("  1. Kilograms (kg)");
                Console.WriteLine("  2. Grams (g)");
                Console.WriteLine("  3. Pounds (lb)");

                string? choice = ConsoleHelper.GetInput("Enter choice (1-3)");

                switch (choice)
                {
                    case "1":
                        return WeightUnit.KILOGRAM;
                    case "2":
                        return WeightUnit.GRAM;
                    case "3":
                        return WeightUnit.POUND;
                    default:
                        ConsoleHelper.DisplayError("Invalid choice!");
                        break;
                }
            }
        }
    }
}
