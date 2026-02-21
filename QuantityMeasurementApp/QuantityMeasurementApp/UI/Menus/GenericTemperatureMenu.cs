using System;
using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Menu for temperature measurement operations.
    /// UC14: Temperature menu showing equality, conversion, and unsupported arithmetic operations.
    /// </summary>
    public class GenericTemperatureMenu
    {
        private readonly GenericMeasurementService _measurementService;

        /// <summary>
        /// Initializes a new instance of the GenericTemperatureMenu class.
        /// </summary>
        /// <param name="measurementService">The measurement service.</param>
        public GenericTemperatureMenu(GenericMeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        /// <summary>
        /// Displays the temperature menu.
        /// </summary>
        public void Display()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                DisplayMainTemperatureMenu();

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        DisplayTemperatureConversion();
                        break;
                    case "2":
                        DisplayTemperatureComparison();
                        break;
                    case "3":
                        DisplayTemperatureUnsupportedOperations();
                        break;
                    case "4":
                        DisplayTemperatureInfo();
                        break;
                    case "5":
                        return;
                    default:
                        ConsoleHelper.DisplayError("Invalid choice! Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void DisplayMainTemperatureMenu()
        {
            ConsoleHelper.DisplayAttributedHeader("TEMPERATURE MEASUREMENTS", "°C, °F, K");

            string[] menuOptions = new[]
            {
                "1.  Convert Temperature Units",
                "    (e.g., 0°C = 32°F = 273.15K)",
                "",
                "2.  Compare Temperatures",
                "    (e.g., 100°C = 212°F = 373.15K)",
                "",
                "3.  Arithmetic Operations",
                "    (Shows why temperature arithmetic is not supported)",
                "",
                "4.  Temperature Info",
                "    (About temperature scales)",
                "",
                "5.  Back to Main Menu",
            };

            ConsoleHelper.DisplayMenu(menuOptions);
        }

        private void DisplayTemperatureConversion()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader("TEMPERATURE CONVERSION", "°C, °F, K");

            try
            {
                TemperatureUnit sourceUnit = TemperatureUnitSelector.SelectTemperatureUnit(
                    "Select SOURCE unit"
                );
                TemperatureUnit targetUnit = TemperatureUnitSelector.SelectTemperatureUnit(
                    "Select TARGET unit"
                );

                Console.Write($"\nEnter value in {sourceUnit.GetName()}: ");
                string? userInput = Console.ReadLine();

                if (double.TryParse(userInput, out double inputValue))
                {
                    // Create temperature quantity
                    var temperature = new GenericQuantity<TemperatureUnit>(inputValue, sourceUnit);
                    var converted = temperature.ConvertTo(targetUnit);

                    ConsoleHelper.DisplayConversionResult(
                        inputValue,
                        sourceUnit.GetSymbol(),
                        converted.Value,
                        targetUnit.GetSymbol()
                    );

                    // Show conversion formula
                    ShowTemperatureFormula(inputValue, sourceUnit, targetUnit, converted.Value);
                }
                else
                {
                    ConsoleHelper.DisplayError("Invalid numeric value!");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
        }

        private void ShowTemperatureFormula(
            double inputValue,
            TemperatureUnit sourceUnit,
            TemperatureUnit targetUnit,
            double result
        )
        {
            string formula = "";

            if (sourceUnit == TemperatureUnit.CELSIUS && targetUnit == TemperatureUnit.FAHRENHEIT)
                formula = $"°F = (°C × 9/5) + 32 = ({inputValue} × 1.8) + 32 = {result:F2}°F";
            else if (
                sourceUnit == TemperatureUnit.FAHRENHEIT
                && targetUnit == TemperatureUnit.CELSIUS
            )
                formula = $"°C = (°F - 32) × 5/9 = ({inputValue} - 32) × 0.5556 = {result:F2}°C";
            else if (sourceUnit == TemperatureUnit.CELSIUS && targetUnit == TemperatureUnit.KELVIN)
                formula = $"K = °C + 273.15 = {inputValue} + 273.15 = {result:F2}K";
            else if (sourceUnit == TemperatureUnit.KELVIN && targetUnit == TemperatureUnit.CELSIUS)
                formula = $"°C = K - 273.15 = {inputValue} - 273.15 = {result:F2}°C";
            else if (
                sourceUnit == TemperatureUnit.FAHRENHEIT
                && targetUnit == TemperatureUnit.KELVIN
            )
                formula =
                    $"K = (°F - 32) × 5/9 + 273.15 = ({inputValue} - 32) × 0.5556 + 273.15 = {result:F2}K";
            else if (
                sourceUnit == TemperatureUnit.KELVIN
                && targetUnit == TemperatureUnit.FAHRENHEIT
            )
                formula =
                    $"°F = (K - 273.15) × 9/5 + 32 = ({inputValue} - 273.15) × 1.8 + 32 = {result:F2}°F";
            else
                formula =
                    $"Same unit conversion: {inputValue} {sourceUnit.GetSymbol()} = {result:F2} {targetUnit.GetSymbol()}";

            ConsoleHelper.DisplayInfoBox(new[] { "Conversion Formula:", formula });
        }

        private void DisplayTemperatureComparison()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader("TEMPERATURE COMPARISON", "0°C = 32°F = 273.15K");

            try
            {
                // First temperature
                ConsoleHelper.DisplaySubHeader("FIRST TEMPERATURE");
                TemperatureUnit firstUnit = TemperatureUnitSelector.SelectTemperatureUnit(
                    "Select unit for first temperature"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second temperature
                ConsoleHelper.DisplaySubHeader("SECOND TEMPERATURE");
                TemperatureUnit secondUnit = TemperatureUnitSelector.SelectTemperatureUnit(
                    "Select unit for second temperature"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstTemp = new GenericQuantity<TemperatureUnit>(firstValue, firstUnit);
                    var secondTemp = new GenericQuantity<TemperatureUnit>(secondValue, secondUnit);

                    bool areEqual = _measurementService.AreQuantitiesEqual(firstTemp, secondTemp);

                    // Show in base unit (Celsius) for reference
                    var firstInCelsius = firstTemp.ConvertTo(TemperatureUnit.CELSIUS);
                    var secondInCelsius = secondTemp.ConvertTo(TemperatureUnit.CELSIUS);

                    ConsoleHelper.DisplayComparisonResult(
                        firstTemp.ToString()!,
                        secondTemp.ToString()!,
                        areEqual,
                        firstInCelsius.Value,
                        secondInCelsius.Value,
                        "°C"
                    );

                    // Show special temperature points
                    if (
                        Math.Abs(firstValue - (-40)) < 0.1
                        && firstUnit == TemperatureUnit.CELSIUS
                        && Math.Abs(secondValue - (-40)) < 0.1
                        && secondUnit == TemperatureUnit.FAHRENHEIT
                    )
                    {
                        ConsoleHelper.DisplaySuccess(
                            "✓ Special point: -40°C = -40°F (unique equality point)"
                        );
                    }
                }
                else
                {
                    ConsoleHelper.DisplayError("Invalid numeric values!");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
        }

        private void DisplayTemperatureUnsupportedOperations()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "TEMPERATURE ARITHMETIC",
                "Why operations are not supported"
            );

            // Show explanation box
            string[] explanationLines = new[]
            {
                "╔══════════════════════════════════════════════════════════════╗",
                "║          WHY TEMPERATURE DOESN'T SUPPORT                     ║",
                "║               ARITHMETIC OPERATIONS                          ║",
                "╠══════════════════════════════════════════════════════════════╣",
                "║                                                              ║",
                "║  • Adding absolute temperatures:                             ║",
                "║    10°C + 20°C ≠ 30°C (not meaningful)                       ║",
                "║                                                              ║",
                "║  • Subtracting gives a temperature difference,               ║",
                "║    not a temperature                                         ║",
                "║                                                              ║",
                "║  • Dividing yields a meaningless ratio                       ║",
                "║    (e.g., What does 50°C / 25°C mean?)                       ║",
                "║                                                              ║",
                "║  • Only supported operations:                                ║",
                "║    ✓ Equality comparison                                     ║",
                "║    ✓ Unit conversion                                         ║",
                "║                                                              ║",
                "╚══════════════════════════════════════════════════════════════╝",
            };

            foreach (var line in explanationLines)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine();

            // Demo with actual attempt - using a clean table format
            var temp1 = new GenericQuantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var temp2 = new GenericQuantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            Console.WriteLine($"  Temperature 1: {temp1}");
            Console.WriteLine($"  Temperature 2: {temp2}\n");

            // Create a clean table for the results
            Console.WriteLine(
                "  ┌──────────────────────┬────────────────────────────────────────┐"
            );
            Console.WriteLine(
                "  │ Operation             │ Result                                 │"
            );
            Console.WriteLine(
                "  ├──────────────────────┼────────────────────────────────────────┤"
            );

            try
            {
                var sum = temp1.Add(temp2);
                Console.WriteLine(
                    $"  │ Add (temp1 + temp2)   │ {sum.Value} {sum.Unit.GetSymbol(), -32} │"
                );
            }
            catch (NotSupportedException)
            {
                Console.WriteLine(
                    "  │ Add (temp1 + temp2)   │ ❌ NOT SUPPORTED                      │"
                );
            }

            try
            {
                var difference = temp1.Subtract(temp2);
                Console.WriteLine(
                    $"  │ Subtract (temp1 - temp2)│ {difference.Value} {difference.Unit.GetSymbol(), -29} │"
                );
            }
            catch (NotSupportedException)
            {
                Console.WriteLine(
                    "  │ Subtract (temp1 - temp2)│ ❌ NOT SUPPORTED                      │"
                );
            }

            try
            {
                var ratio = temp1.Divide(temp2);
                Console.WriteLine($"  │ Divide (temp1 ÷ temp2) │ {ratio:F6,-36} │");
            }
            catch (NotSupportedException)
            {
                Console.WriteLine(
                    "  │ Divide (temp1 ÷ temp2) │ ❌ NOT SUPPORTED                      │"
                );
            }

            Console.WriteLine(
                "  └──────────────────────┴────────────────────────────────────────┘"
            );
            Console.WriteLine("\n  ✅ Temperature supports:");
            Console.WriteLine("     • Equality comparison");
            Console.WriteLine("     • Unit conversion\n");

            ConsoleHelper.WaitForKeyPress();
        }

        private void DisplayTemperatureInfo()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "TEMPERATURE INFORMATION",
                "About temperature scales"
            );

            string[] infoLines = new[]
            {
                "╔══════════════════════════════════════════════════════════════╗",
                "║                      TEMPERATURE SCALES                      ║",
                "╠══════════════════════════════════════════════════════════════╣",
                "║                                                              ║",
                "║   ┌─────────────┬──────────────┬──────────────┐              ║",
                "║   │   Scale     │   Freezing   │   Boiling    │              ║",
                "║   ├─────────────┼──────────────┼──────────────┤              ║",
                "║   │ Celsius (°C)│     0°C      │    100°C     │              ║",
                "║   │ Fahrenheit  │     32°F     │    212°F     │              ║",
                "║   │ Kelvin (K)  │   273.15 K   │   373.15 K   │              ║",
                "║   └─────────────┴──────────────┴──────────────┘              ║",
                "║                                                              ║",
                "║   ┌──────────────────────────────────────────────────────┐   ║",
                "║   │ Conversion Formulas                                  │   ║",
                "║   ├──────────────────────────────────────────────────────┤   ║",
                "║   │ °F = (°C × 9/5) + 32                                 │   ║",
                "║   │ °C = (°F - 32) × 5/9                                 │   ║",
                "║   │ K  = °C + 273.15                                     │   ║",
                "║   │ °C = K - 273.15                                      │   ║",
                "║   └──────────────────────────────────────────────────────┘   ║",
                "║                                                              ║",
                "║   ┌──────────────────────────────────────────────────────┐   ║",
                "║   │ Special Points                                       │   ║",
                "║   ├──────────────────────────────────────────────────────┤   ║",
                "║   │ • -40°C = -40°F (unique equality point)              │   ║",
                "║   │ • 0 K = -273.15°C (absolute zero)                    │   ║",
                "║   └──────────────────────────────────────────────────────┘   ║",
                "║                                                              ║",
                "║   ┌──────────────────────────────────────────────────────┐   ║",
                "║   │ Supported Operations                                 │   ║",
                "║   ├──────────────────────────────────────────────────────┤   ║",
                "║   │ ✓ Equality comparison                                │   ║",
                "║   │ ✓ Unit conversion                                    │   ║",
                "║   │ ✗ Addition (not meaningful)                          │   ║",
                "║   │ ✗ Subtraction (not meaningful)                       │   ║",
                "║   │ ✗ Division (not meaningful)                          │   ║",
                "║   └──────────────────────────────────────────────────────┘   ║",
                "║                                                              ║",
                "╚══════════════════════════════════════════════════════════════╝",
            };

            foreach (var line in infoLines)
            {
                Console.WriteLine(line);
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}
