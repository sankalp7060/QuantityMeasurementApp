using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Menu for weight measurement operations using generic Quantity class.
    /// UC10: Consolidated menu for all weight operations.
    /// </summary>
    public class GenericWeightMenu
    {
        private readonly GenericMeasurementService _measurementService;

        /// <summary>
        /// Initializes a new instance of the GenericWeightMenu class.
        /// </summary>
        /// <param name="measurementService">The measurement service.</param>
        public GenericWeightMenu(GenericMeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        /// <summary>
        /// Displays the weight menu.
        /// </summary>
        public void Display()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                DisplayMainWeightMenu();

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        DisplayWeightConversion();
                        break;
                    case "2":
                        DisplayWeightComparison();
                        break;
                    case "3":
                        DisplayWeightAddition();
                        break;
                    case "4":
                        return;
                    default:
                        ConsoleHelper.DisplayError("Invalid choice! Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void DisplayMainWeightMenu()
        {
            ConsoleHelper.DisplayAttributedHeader("WEIGHT MEASUREMENTS", "kg, g, lb");

            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    WEIGHT OPTIONS                     ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    1.  Convert Weight Units                           ║");
            Console.WriteLine("║        (e.g., 1 kg = 1000 g)                          ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    2.  Compare Weights                                ║");
            Console.WriteLine("║        (e.g., 1 kg = 1000 g = 2.20462 lb)            ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    3.  Add Weights                                    ║");
            Console.WriteLine("║        (e.g., 1 kg + 500 g = 1.5 kg)                  ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    4.  Back to Main Menu                              ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        }

        private void DisplayWeightConversion()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "WEIGHT CONVERSION",
                "1 kg = 1000 g = 2.20462 lb"
            );

            try
            {
                WeightUnit sourceUnit = GenericUnitSelector.SelectWeightUnit("Select SOURCE unit");
                WeightUnit targetUnit = GenericUnitSelector.SelectWeightUnit("Select TARGET unit");

                string? userInput = ConsoleHelper.GetInput(
                    $"Enter value in {sourceUnit.GetName()}"
                );

                if (double.TryParse(userInput, out double inputValue))
                {
                    double convertedValue = _measurementService.ConvertValue(
                        inputValue,
                        sourceUnit,
                        targetUnit
                    );

                    Console.WriteLine("\n╔════════════════════════════════════════╗");
                    Console.WriteLine("║         CONVERSION RESULT             ║");
                    Console.WriteLine("╠════════════════════════════════════════╣");
                    Console.WriteLine(
                        $"║  {inputValue, 8:F3} {sourceUnit.GetSymbol(), -3} = {convertedValue, 10:F6} {targetUnit.GetSymbol(), -3} ║"
                    );
                    Console.WriteLine("╚════════════════════════════════════════╝");

                    ShowWeightConversionFormula(inputValue, sourceUnit, targetUnit, convertedValue);
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

        private void ShowWeightConversionFormula(
            double inputValue,
            WeightUnit sourceUnit,
            WeightUnit targetUnit,
            double convertedValue
        )
        {
            double sourceToKg = sourceUnit.GetConversionFactor();
            double targetToKg = targetUnit.GetConversionFactor();

            Console.WriteLine("\n📊 Conversion Formula:");
            Console.WriteLine(
                $"   {inputValue} {sourceUnit.GetSymbol()} × ({sourceToKg:F6} / {targetToKg:F6}) = {convertedValue:F6} {targetUnit.GetSymbol()}"
            );
        }

        private void DisplayWeightComparison()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "WEIGHT COMPARISON",
                "1 kg = 1000 g = 2.20462 lb"
            );

            try
            {
                // First weight
                Console.WriteLine("\n┌────────── FIRST WEIGHT ──────────┐");
                WeightUnit firstUnit = GenericUnitSelector.SelectWeightUnit(
                    "Select unit for first weight"
                );
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                // Second weight
                Console.WriteLine("\n┌────────── SECOND WEIGHT ─────────┐");
                WeightUnit secondUnit = GenericUnitSelector.SelectWeightUnit(
                    "Select unit for second weight"
                );
                Console.Write("│ Enter value: ");
                string? secondInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<WeightUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<WeightUnit>(secondValue, secondUnit);

                    bool areEqual = _measurementService.AreQuantitiesEqual(
                        firstQuantity,
                        secondQuantity
                    );

                    Console.WriteLine("\n╔════════════════════════════════════════╗");
                    Console.WriteLine("║         COMPARISON RESULT             ║");
                    Console.WriteLine("╠════════════════════════════════════════╣");
                    Console.WriteLine($"║  {firstQuantity, -8} vs {secondQuantity, -8}      ║");
                    Console.WriteLine("╠════════════════════════════════════════╣");

                    if (areEqual)
                    {
                        Console.WriteLine("║     ✅ Weights are EQUAL               ║");
                    }
                    else
                    {
                        Console.WriteLine("║     ❌ Weights are NOT EQUAL           ║");
                    }

                    Console.WriteLine("╚════════════════════════════════════════╝");

                    // Show in base unit for reference
                    var firstInKg = firstQuantity.ConvertTo(WeightUnit.KILOGRAM);
                    var secondInKg = secondQuantity.ConvertTo(WeightUnit.KILOGRAM);

                    Console.WriteLine($"\n📊 In kilograms:");
                    Console.WriteLine($"   First:  {firstInKg.Value:F6} kg");
                    Console.WriteLine($"   Second: {secondInKg.Value:F6} kg");
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

        private void DisplayWeightAddition()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                ConsoleHelper.DisplayAttributedHeader("WEIGHT ADDITION", "1 kg + 500 g = 1.5 kg");

                Console.WriteLine("╔════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                 ADDITION OPTIONS                      ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════╣");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("║    1.  Result in FIRST unit                           ║");
                Console.WriteLine("║        (e.g., 1 kg + 500 g = 1.5 kg)                  ║");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("║    2.  Result in SECOND unit                          ║");
                Console.WriteLine("║        (e.g., 1 kg + 500 g = 1500 g)                  ║");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("║    3.  Results in BOTH units                          ║");
                Console.WriteLine("║        (Compare both results)                         ║");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("║    4.  Back to Weight Menu                            ║");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════╝");

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        DisplayWeightAdditionInFirstUnit();
                        break;
                    case "2":
                        DisplayWeightAdditionInSecondUnit();
                        break;
                    case "3":
                        DisplayWeightAdditionInBothUnits();
                        break;
                    case "4":
                        return;
                    default:
                        ConsoleHelper.DisplayError("Invalid choice! Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void DisplayWeightAdditionInFirstUnit()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "WEIGHT ADDITION - RESULT IN FIRST UNIT",
                "1 kg + 500 g = 1.5 kg"
            );

            try
            {
                // First weight
                Console.WriteLine("\n┌────────── FIRST WEIGHT ──────────┐");
                WeightUnit firstUnit = GenericUnitSelector.SelectWeightUnit(
                    "Select unit for first weight"
                );
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                // Second weight
                Console.WriteLine("\n┌────────── SECOND WEIGHT ─────────┐");
                WeightUnit secondUnit = GenericUnitSelector.SelectWeightUnit(
                    "Select unit for second weight"
                );
                Console.Write("│ Enter value: ");
                string? secondInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<WeightUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<WeightUnit>(secondValue, secondUnit);

                    var sumInFirstUnit = _measurementService.AddQuantities(
                        firstQuantity,
                        secondQuantity
                    );

                    DisplayWeightResultBox(firstQuantity, secondQuantity, sumInFirstUnit);
                    ShowWeightCalculationDetails(
                        firstQuantity,
                        secondQuantity,
                        sumInFirstUnit.Unit,
                        sumInFirstUnit
                    );
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

        private void DisplayWeightAdditionInSecondUnit()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "WEIGHT ADDITION - RESULT IN SECOND UNIT",
                "1 kg + 500 g = 1500 g"
            );

            try
            {
                // First weight
                Console.WriteLine("\n┌────────── FIRST WEIGHT ──────────┐");
                WeightUnit firstUnit = GenericUnitSelector.SelectWeightUnit(
                    "Select unit for first weight"
                );
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                // Second weight
                Console.WriteLine("\n┌────────── SECOND WEIGHT ─────────┐");
                WeightUnit secondUnit = GenericUnitSelector.SelectWeightUnit(
                    "Select unit for second weight"
                );
                Console.Write("│ Enter value: ");
                string? secondInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<WeightUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<WeightUnit>(secondValue, secondUnit);

                    var sumInSecondUnit = _measurementService.AddQuantitiesWithTarget(
                        firstQuantity,
                        secondQuantity,
                        secondUnit
                    );

                    DisplayWeightResultBox(firstQuantity, secondQuantity, sumInSecondUnit);
                    ShowWeightCalculationDetails(
                        firstQuantity,
                        secondQuantity,
                        secondUnit,
                        sumInSecondUnit
                    );
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

        private void DisplayWeightAdditionInBothUnits()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "WEIGHT ADDITION - RESULTS IN BOTH UNITS",
                "Compare results"
            );

            try
            {
                // First weight
                Console.WriteLine("\n┌────────── FIRST WEIGHT ──────────┐");
                WeightUnit firstUnit = GenericUnitSelector.SelectWeightUnit(
                    "Select unit for first weight"
                );
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                // Second weight
                Console.WriteLine("\n┌────────── SECOND WEIGHT ─────────┐");
                WeightUnit secondUnit = GenericUnitSelector.SelectWeightUnit(
                    "Select unit for second weight"
                );
                Console.Write("│ Enter value: ");
                string? secondInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<WeightUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<WeightUnit>(secondValue, secondUnit);

                    var sumInFirstUnit = _measurementService.AddQuantitiesWithTarget(
                        firstQuantity,
                        secondQuantity,
                        firstUnit
                    );
                    var sumInSecondUnit = _measurementService.AddQuantitiesWithTarget(
                        firstQuantity,
                        secondQuantity,
                        secondUnit
                    );

                    DisplayWeightComparisonBox(
                        firstQuantity,
                        secondQuantity,
                        sumInFirstUnit,
                        sumInSecondUnit
                    );
                    ShowWeightCalculationDetails(
                        firstQuantity,
                        secondQuantity,
                        firstUnit,
                        sumInFirstUnit
                    );
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

        private void DisplayWeightResultBox(
            GenericQuantity<WeightUnit> firstQuantity,
            GenericQuantity<WeightUnit> secondQuantity,
            GenericQuantity<WeightUnit> sumQuantity
        )
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║           WEIGHT ADDITION RESULT      ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine($"║  {firstQuantity, -8} + {secondQuantity, -8}          ║");
            Console.WriteLine("║                                        ║");
            Console.WriteLine(
                $"║  = {sumQuantity.Value, 10:F6} {sumQuantity.Unit.GetSymbol(), -3}               ║"
            );
            Console.WriteLine("╚════════════════════════════════════════╝");
        }

        private void DisplayWeightComparisonBox(
            GenericQuantity<WeightUnit> firstQuantity,
            GenericQuantity<WeightUnit> secondQuantity,
            GenericQuantity<WeightUnit> sumInFirstUnit,
            GenericQuantity<WeightUnit> sumInSecondUnit
        )
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         COMPARISON RESULTS             ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine($"║  {firstQuantity, -8} + {secondQuantity, -8}          ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine(
                $"║  In {sumInFirstUnit.Unit.GetName(), -8}: {sumInFirstUnit.Value, 10:F6} {sumInFirstUnit.Unit.GetSymbol(), -3}  ║"
            );
            Console.WriteLine(
                $"║  In {sumInSecondUnit.Unit.GetName(), -7}: {sumInSecondUnit.Value, 10:F6} {sumInSecondUnit.Unit.GetSymbol(), -3}  ║"
            );
            Console.WriteLine("╚════════════════════════════════════════╝");
        }

        private void ShowWeightCalculationDetails(
            GenericQuantity<WeightUnit> firstQuantity,
            GenericQuantity<WeightUnit> secondQuantity,
            WeightUnit resultUnit,
            GenericQuantity<WeightUnit> sumQuantity
        )
        {
            var firstInKg = firstQuantity.ConvertTo(WeightUnit.KILOGRAM);
            var secondInKg = secondQuantity.ConvertTo(WeightUnit.KILOGRAM);
            double totalInKg = firstInKg.Value + secondInKg.Value;

            Console.WriteLine("\n┌────────── CALCULATION DETAILS ──────────┐");
            Console.WriteLine("│  Step 1: Convert to base unit (kg)     │");
            Console.WriteLine($"│    {firstQuantity} = {firstInKg.Value, 8:F6} kg           │");
            Console.WriteLine($"│    {secondQuantity} = {secondInKg.Value, 8:F6} kg           │");
            Console.WriteLine("│                                          │");
            Console.WriteLine("│  Step 2: Add in kilograms               │");
            Console.WriteLine(
                $"│    {firstInKg.Value:F6} + {secondInKg.Value:F6} = {totalInKg:F6} kg   │"
            );
            Console.WriteLine("│                                          │");
            Console.WriteLine("│  Step 3: Convert to target unit         │");
            Console.WriteLine(
                $"│    {totalInKg:F6} kg = {sumQuantity.Value:F6} {resultUnit.GetSymbol()}         │"
            );
            Console.WriteLine("└──────────────────────────────────────────┘");
        }
    }
}
