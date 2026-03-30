using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.UI.Helpers
{
    /// <summary>
    /// Helper class for temperature unit selection.
    /// UC14: Dedicated selector for temperature measurements.
    /// </summary>
    public static class TemperatureUnitSelector
    {
        /// <summary>
        /// Allows user to select a temperature unit from a menu.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <returns>The selected TemperatureUnit.</returns>
        public static TemperatureUnit SelectTemperatureUnit(string prompt)
        {
            while (true)
            {
                Console.WriteLine($"\n{prompt}:");
                Console.WriteLine("  1. Celsius (°C)");
                Console.WriteLine("  2. Fahrenheit (°F)");
                Console.WriteLine("  3. Kelvin (K)");

                string? choice = ConsoleHelper.GetInput("Enter choice (1-3)");

                switch (choice)
                {
                    case "1":
                        return TemperatureUnit.CELSIUS;
                    case "2":
                        return TemperatureUnit.FAHRENHEIT;
                    case "3":
                        return TemperatureUnit.KELVIN;
                    default:
                        ConsoleHelper.DisplayError("Invalid choice! Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
