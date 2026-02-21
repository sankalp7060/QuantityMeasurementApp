using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.UI.Helpers
{
    /// <summary>
    /// Helper class for unit selection.
    /// </summary>
    public static class UnitSelector
    {
        /// <summary>
        /// Allows user to select a unit from a menu.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <returns>The selected LengthUnit.</returns>
        public static LengthUnit SelectUnit(string prompt)
        {
            while (true)
            {
                Console.WriteLine($"\n{prompt}:");
                Console.WriteLine("  1. Feet (ft)");
                Console.WriteLine("  2. Inches (in)");
                Console.WriteLine("  3. Yards (yd)");
                Console.WriteLine("  4. Centimeters (cm)");

                string? choice = ConsoleHelper.GetInput("Enter choice (1-4)");

                switch (choice)
                {
                    case "1":
                        return LengthUnit.FEET;
                    case "2":
                        return LengthUnit.INCH;
                    case "3":
                        return LengthUnit.YARD;
                    case "4":
                        return LengthUnit.CENTIMETER;
                    default:
                        ConsoleHelper.DisplayError("Invalid choice!");
                        break;
                }
            }
        }
    }
}
