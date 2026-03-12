using QuantityMeasurementApp.Core.Abstractions;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.UI.Helpers
{
    /// <summary>
    /// Helper class for unit selection across all measurement categories.
    /// UC14: Added temperature unit selection.
    /// </summary>
    public static class GenericUnitSelector
    {
        /// <summary>
        /// Allows user to select a length unit from a menu.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <returns>The selected LengthUnit.</returns>
        public static LengthUnit SelectLengthUnit(string prompt)
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
                        ConsoleHelper.DisplayError("Invalid choice! Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        /// <summary>
        /// Allows user to select a weight unit from a menu.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <returns>The selected WeightUnit.</returns>
        public static WeightUnit SelectWeightUnit(string prompt)
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
                        ConsoleHelper.DisplayError("Invalid choice! Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

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
