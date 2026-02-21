using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.UI.Helpers
{
    /// <summary>
    /// Helper class for volume unit selection.
    /// UC11: Dedicated selector for volume measurements.
    /// </summary>
    public static class VolumeUnitSelector
    {
        /// <summary>
        /// Allows user to select a volume unit from a menu.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <returns>The selected VolumeUnit.</returns>
        public static VolumeUnit SelectVolumeUnit(string prompt)
        {
            while (true)
            {
                Console.WriteLine($"\n{prompt}:");
                Console.WriteLine("  1. Litres (L)");
                Console.WriteLine("  2. Millilitres (mL)");
                Console.WriteLine("  3. Gallons (gal)");

                string? choice = ConsoleHelper.GetInput("Enter choice (1-3)");

                switch (choice)
                {
                    case "1":
                        return VolumeUnit.LITRE;
                    case "2":
                        return VolumeUnit.MILLILITRE;
                    case "3":
                        return VolumeUnit.GALLON;
                    default:
                        ConsoleHelper.DisplayError("Invalid choice! Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
