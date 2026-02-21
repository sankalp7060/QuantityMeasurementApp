using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Menu for weight measurement operations (UC9).
    /// </summary>
    public class WeightMenu
    {
        private readonly WeightMeasurementService _weightService;

        /// <summary>
        /// Initializes a new instance of the WeightMenu class.
        /// </summary>
        public WeightMenu()
        {
            _weightService = new WeightMeasurementService();
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
                        DisplayWeightVsLengthDemo();
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
            Console.WriteLine("║    4.  Weight vs Length (Incompatible Demo)           ║");
            Console.WriteLine("║        (Shows that weight and length cannot mix)      ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    5.  Back to Main Menu                              ║");
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
                WeightUnit sourceUnit = WeightUnitSelector.SelectUnit("Select SOURCE unit");
                WeightUnit targetUnit = WeightUnitSelector.SelectUnit("Select TARGET unit");

                string? userInput = ConsoleHelper.GetInput(
                    $"Enter value in {sourceUnit.GetName()}"
                );

                if (double.TryParse(userInput, out double inputValue))
                {
                    double convertedValue = _weightService.ConvertWeightValue(
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
                WeightUnit firstUnit = WeightUnitSelector.SelectUnit(
                    "Select unit for first weight"
                );
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                // Second weight
                Console.WriteLine("\n┌────────── SECOND WEIGHT ─────────┐");
                WeightUnit secondUnit = WeightUnitSelector.SelectUnit(
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
                    var firstWeight = new WeightQuantity(firstValue, firstUnit);
                    var secondWeight = new WeightQuantity(secondValue, secondUnit);

                    bool areWeightsEqual = _weightService.AreWeightsEqual(
                        firstWeight,
                        secondWeight
                    );

                    Console.WriteLine("\n╔════════════════════════════════════════╗");
                    Console.WriteLine("║         COMPARISON RESULT             ║");
                    Console.WriteLine("╠════════════════════════════════════════╣");
                    Console.WriteLine($"║  {firstWeight, -8} vs {secondWeight, -8}      ║");
                    Console.WriteLine("╠════════════════════════════════════════╣");

                    if (areWeightsEqual)
                    {
                        Console.WriteLine("║     ✅ Weights are EQUAL              ║");
                    }
                    else
                    {
                        Console.WriteLine("║     ❌ Weights are NOT EQUAL          ║");
                    }

                    Console.WriteLine("╚════════════════════════════════════════╝");

                    // Show in base unit for reference
                    WeightQuantity firstInKg = firstWeight.ConvertTo(WeightUnit.KILOGRAM);
                    WeightQuantity secondInKg = secondWeight.ConvertTo(WeightUnit.KILOGRAM);

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
                WeightUnit firstUnit = WeightUnitSelector.SelectUnit(
                    "Select unit for first weight"
                );
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                // Second weight
                Console.WriteLine("\n┌────────── SECOND WEIGHT ─────────┐");
                WeightUnit secondUnit = WeightUnitSelector.SelectUnit(
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
                    var firstWeight = new WeightQuantity(firstValue, firstUnit);
                    var secondWeight = new WeightQuantity(secondValue, secondUnit);

                    var sumInFirstUnit = _weightService.AddWeights(firstWeight, secondWeight);

                    DisplayWeightResultBox(firstWeight, secondWeight, sumInFirstUnit);
                    ShowWeightCalculationDetails(
                        firstWeight,
                        secondWeight,
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
                WeightUnit firstUnit = WeightUnitSelector.SelectUnit(
                    "Select unit for first weight"
                );
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                // Second weight
                Console.WriteLine("\n┌────────── SECOND WEIGHT ─────────┐");
                WeightUnit secondUnit = WeightUnitSelector.SelectUnit(
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
                    var firstWeight = new WeightQuantity(firstValue, firstUnit);
                    var secondWeight = new WeightQuantity(secondValue, secondUnit);

                    var sumInSecondUnit = _weightService.AddWeightsWithTarget(
                        firstWeight,
                        secondWeight,
                        secondUnit
                    );

                    DisplayWeightResultBox(firstWeight, secondWeight, sumInSecondUnit);
                    ShowWeightCalculationDetails(
                        firstWeight,
                        secondWeight,
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
                WeightUnit firstUnit = WeightUnitSelector.SelectUnit(
                    "Select unit for first weight"
                );
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└───────────────────────────────────┘");

                // Second weight
                Console.WriteLine("\n┌────────── SECOND WEIGHT ─────────┐");
                WeightUnit secondUnit = WeightUnitSelector.SelectUnit(
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
                    var firstWeight = new WeightQuantity(firstValue, firstUnit);
                    var secondWeight = new WeightQuantity(secondValue, secondUnit);

                    var sumInFirstUnit = _weightService.AddWeightsWithTarget(
                        firstWeight,
                        secondWeight,
                        firstUnit
                    );
                    var sumInSecondUnit = _weightService.AddWeightsWithTarget(
                        firstWeight,
                        secondWeight,
                        secondUnit
                    );

                    DisplayWeightComparisonBox(
                        firstWeight,
                        secondWeight,
                        sumInFirstUnit,
                        sumInSecondUnit
                    );
                    ShowWeightCalculationDetails(
                        firstWeight,
                        secondWeight,
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
            WeightQuantity firstWeight,
            WeightQuantity secondWeight,
            WeightQuantity sumQuantity
        )
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║           WEIGHT ADDITION RESULT      ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine($"║  {firstWeight, -8} + {secondWeight, -8}          ║");
            Console.WriteLine("║                                        ║");
            Console.WriteLine(
                $"║  = {sumQuantity.Value, 10:F6} {sumQuantity.Unit.GetSymbol(), -3}               ║"
            );
            Console.WriteLine("╚════════════════════════════════════════╝");
        }

        private void DisplayWeightComparisonBox(
            WeightQuantity firstWeight,
            WeightQuantity secondWeight,
            WeightQuantity sumInFirstUnit,
            WeightQuantity sumInSecondUnit
        )
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         COMPARISON RESULTS             ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine($"║  {firstWeight, -8} + {secondWeight, -8}          ║");
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
            WeightQuantity firstWeight,
            WeightQuantity secondWeight,
            WeightUnit resultUnit,
            WeightQuantity sumQuantity
        )
        {
            WeightQuantity firstInKg = firstWeight.ConvertTo(WeightUnit.KILOGRAM);
            WeightQuantity secondInKg = secondWeight.ConvertTo(WeightUnit.KILOGRAM);
            double totalInKg = firstInKg.Value + secondInKg.Value;

            Console.WriteLine("\n┌────────── CALCULATION DETAILS ──────────┐");
            Console.WriteLine("│  Step 1: Convert to base unit (kg)     │");
            Console.WriteLine($"│    {firstWeight} = {firstInKg.Value, 8:F6} kg           │");
            Console.WriteLine($"│    {secondWeight} = {secondInKg.Value, 8:F6} kg           │");
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

        private void DisplayWeightVsLengthDemo()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "WEIGHT VS LENGTH",
                "Demonstrating Category Incompatibility"
            );

            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     WEIGHT AND LENGTH ARE DIFFERENT CATEGORIES        ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║  • 1 kilogram is NOT equal to 1 foot                  ║");
            Console.WriteLine("║  • 500 grams is NOT equal to 12 inches                ║");
            Console.WriteLine("║  • Weight and length cannot be compared               ║");
            Console.WriteLine("║  • They cannot be added or converted                  ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            // Demo with actual objects - FIXED: Changed FOOT to FEET
            var weight = new WeightQuantity(1.0, WeightUnit.KILOGRAM);
            var length = new Quantity(1.0, LengthUnit.FEET);

            Console.WriteLine($"  Weight: {weight}");
            Console.WriteLine($"  Length: {length}");
            Console.WriteLine($"  Are they equal? {weight.Equals(length)} (Always false)");
            Console.WriteLine($"  Same type check: {weight.GetType() == length.GetType()}");

            Console.WriteLine("\n📌 Key Takeaway:");
            Console.WriteLine("   Different measurement categories are type-safe and");
            Console.WriteLine("   cannot be mixed. This prevents logical errors.");

            ConsoleHelper.WaitForKeyPress();
        }
    }
}
