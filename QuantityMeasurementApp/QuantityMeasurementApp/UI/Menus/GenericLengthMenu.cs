using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Menu for length measurement operations using generic Quantity class.
    /// UC10: Consolidated menu for all length operations with proper UI.
    /// </summary>
    public class GenericLengthMenu
    {
        private readonly GenericMeasurementService _measurementService;

        /// <summary>
        /// Initializes a new instance of the GenericLengthMenu class.
        /// </summary>
        /// <param name="measurementService">The measurement service.</param>
        public GenericLengthMenu(GenericMeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        /// <summary>
        /// Displays the length menu.
        /// </summary>
        public void Display()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                DisplayMainLengthMenu();

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        DisplayLengthConversion();
                        break;
                    case "2":
                        DisplayLengthComparison();
                        break;
                    case "3":
                        DisplayLengthAddition();
                        break;
                    case "4":
                        DisplayCommutativityDemo();
                        break;
                    case "5":
                        DisplayBatchOperations();
                        break;
                    case "6":
                        return;
                    default:
                        ConsoleHelper.DisplayError("Invalid choice! Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void DisplayMainLengthMenu()
        {
            ConsoleHelper.DisplayAttributedHeader("LENGTH MEASUREMENTS", "ft, in, yd, cm");

            string[] menuOptions = new[]
            {
                "1.  Convert Length Units",
                "    (e.g., 1 ft = 12 in)",
                "",
                "2.  Compare Lengths",
                "    (e.g., 1 ft = 12 in = 0.333 yd)",
                "",
                "3.  Add Lengths",
                "    (e.g., 1 ft + 12 in = 2 ft)",
                "",
                "4.  Commutativity Demo",
                "    (Shows that a + b = b + a)",
                "",
                "5.  Batch Operations",
                "    (Convert/Add multiple values)",
                "",
                "6.  Back to Main Menu",
            };

            ConsoleHelper.DisplayMenu(menuOptions);
        }

        private void DisplayLengthConversion()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "LENGTH CONVERSION",
                "1 ft = 12 in = 0.333 yd = 30.48 cm"
            );

            try
            {
                LengthUnit sourceUnit = GenericUnitSelector.SelectLengthUnit("Select SOURCE unit");
                LengthUnit targetUnit = GenericUnitSelector.SelectLengthUnit("Select TARGET unit");

                Console.Write($"\nEnter value in {sourceUnit.GetName()}: ");
                string? userInput = Console.ReadLine();

                if (double.TryParse(userInput, out double inputValue))
                {
                    double convertedValue = _measurementService.ConvertValue(
                        inputValue,
                        sourceUnit,
                        targetUnit
                    );
                    ConsoleHelper.DisplayConversionResult(
                        inputValue,
                        sourceUnit.GetSymbol(),
                        convertedValue,
                        targetUnit.GetSymbol()
                    );

                    // Show formula
                    double sourceToBase = sourceUnit.GetConversionFactor();
                    double targetToBase = targetUnit.GetConversionFactor();

                    ConsoleHelper.DisplayInfoBox(
                        new[]
                        {
                            "Conversion Formula:",
                            $"  {inputValue} {sourceUnit.GetSymbol()} × ({sourceToBase:F6} / {targetToBase:F6}) = {convertedValue:F6} {targetUnit.GetSymbol()}",
                        }
                    );
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

        private void DisplayLengthComparison()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "LENGTH COMPARISON",
                "1 ft = 12 in = 0.333 yd = 30.48 cm"
            );

            try
            {
                // First length
                ConsoleHelper.DisplaySubHeader("FIRST LENGTH");
                LengthUnit firstUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for first length"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second length
                ConsoleHelper.DisplaySubHeader("SECOND LENGTH");
                LengthUnit secondUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for second length"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<LengthUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<LengthUnit>(secondValue, secondUnit);

                    bool areEqual = _measurementService.AreQuantitiesEqual(
                        firstQuantity,
                        secondQuantity
                    );

                    // Show in base unit for reference
                    var firstInFeet = firstQuantity.ConvertTo(LengthUnit.FEET);
                    var secondInFeet = secondQuantity.ConvertTo(LengthUnit.FEET);

                    ConsoleHelper.DisplayComparisonResult(
                        firstQuantity.ToString()!,
                        secondQuantity.ToString()!,
                        areEqual,
                        firstInFeet.Value,
                        secondInFeet.Value,
                        "ft"
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

        private void DisplayLengthAddition()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                ConsoleHelper.DisplayAttributedHeader("LENGTH ADDITION", "1 ft + 12 in = 2 ft");

                string[] additionOptions = new[]
                {
                    "1.  Result in FIRST unit",
                    "    (e.g., 1 ft + 12 in = 2 ft)",
                    "",
                    "2.  Result in SECOND unit",
                    "    (e.g., 1 ft + 12 in = 24 in)",
                    "",
                    "3.  Results in BOTH units",
                    "    (Compare both results)",
                    "",
                    "4.  Back to Length Menu",
                };

                ConsoleHelper.DisplayMenu(additionOptions);

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        DisplayLengthAdditionInFirstUnit();
                        break;
                    case "2":
                        DisplayLengthAdditionInSecondUnit();
                        break;
                    case "3":
                        DisplayLengthAdditionInBothUnits();
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

        private void DisplayLengthAdditionInFirstUnit()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ADDITION - RESULT IN FIRST UNIT",
                "1 ft + 12 in = 2 ft"
            );

            try
            {
                // First length
                ConsoleHelper.DisplaySubHeader("FIRST LENGTH");
                LengthUnit firstUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for first length"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second length
                ConsoleHelper.DisplaySubHeader("SECOND LENGTH");
                LengthUnit secondUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for second length"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<LengthUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<LengthUnit>(secondValue, secondUnit);

                    var sumInFirstUnit = _measurementService.AddQuantities(
                        firstQuantity,
                        secondQuantity
                    );

                    ConsoleHelper.DisplayAdditionResult(
                        firstQuantity.ToString()!,
                        secondQuantity.ToString()!,
                        sumInFirstUnit.Value,
                        sumInFirstUnit.Unit.GetSymbol()
                    );

                    ShowLengthCalculationDetails(
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

        private void DisplayLengthAdditionInSecondUnit()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ADDITION - RESULT IN SECOND UNIT",
                "1 ft + 12 in = 24 in"
            );

            try
            {
                // First length
                ConsoleHelper.DisplaySubHeader("FIRST LENGTH");
                LengthUnit firstUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for first length"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second length
                ConsoleHelper.DisplaySubHeader("SECOND LENGTH");
                LengthUnit secondUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for second length"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<LengthUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<LengthUnit>(secondValue, secondUnit);

                    var sumInSecondUnit = _measurementService.AddQuantitiesWithTarget(
                        firstQuantity,
                        secondQuantity,
                        secondUnit
                    );

                    ConsoleHelper.DisplayAdditionResult(
                        firstQuantity.ToString()!,
                        secondQuantity.ToString()!,
                        sumInSecondUnit.Value,
                        sumInSecondUnit.Unit.GetSymbol()
                    );

                    ShowLengthCalculationDetails(
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

        private void DisplayLengthAdditionInBothUnits()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ADDITION - RESULTS IN BOTH UNITS",
                "Compare results"
            );

            try
            {
                // First length
                ConsoleHelper.DisplaySubHeader("FIRST LENGTH");
                LengthUnit firstUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for first length"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second length
                ConsoleHelper.DisplaySubHeader("SECOND LENGTH");
                LengthUnit secondUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for second length"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<LengthUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<LengthUnit>(secondValue, secondUnit);

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

                    string[] resultLines = new[]
                    {
                        $"{firstQuantity} + {secondQuantity}",
                        "",
                        $"In {firstUnit.GetName()}: {sumInFirstUnit.Value:F6} {firstUnit.GetSymbol()}",
                        $"In {secondUnit.GetName()}: {sumInSecondUnit.Value:F6} {secondUnit.GetSymbol()}",
                    };

                    ConsoleHelper.DisplayResultBox("COMPARISON RESULTS", resultLines);
                    ShowLengthCalculationDetails(
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

        private void ShowLengthCalculationDetails(
            GenericQuantity<LengthUnit> firstQuantity,
            GenericQuantity<LengthUnit> secondQuantity,
            LengthUnit resultUnit,
            GenericQuantity<LengthUnit> sumQuantity
        )
        {
            var firstInFeet = firstQuantity.ConvertTo(LengthUnit.FEET);
            var secondInFeet = secondQuantity.ConvertTo(LengthUnit.FEET);
            double totalInFeet = firstInFeet.Value + secondInFeet.Value;

            string[] calculationLines = new[]
            {
                "Step 1: Convert to base unit (feet)",
                $"  {firstQuantity} = {firstInFeet.Value:F6} ft",
                $"  {secondQuantity} = {secondInFeet.Value:F6} ft",
                "",
                "Step 2: Add in feet",
                $"  {firstInFeet.Value:F6} + {secondInFeet.Value:F6} = {totalInFeet:F6} ft",
                "",
                "Step 3: Convert to target unit",
                $"  {totalInFeet:F6} ft = {sumQuantity.Value:F6} {resultUnit.GetSymbol()}",
            };

            ConsoleHelper.DisplayResultBox("CALCULATION DETAILS", calculationLines);
        }

        private void DisplayCommutativityDemo()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader("COMMUTATIVITY DEMO", "a + b = b + a");

            try
            {
                ConsoleHelper.DisplaySubHeader("FIRST LENGTH (a)");
                LengthUnit firstUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for first length"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                ConsoleHelper.DisplaySubHeader("SECOND LENGTH (b)");
                LengthUnit secondUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for second length"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var a = new GenericQuantity<LengthUnit>(firstValue, firstUnit);
                    var b = new GenericQuantity<LengthUnit>(secondValue, secondUnit);

                    // Add in both orders
                    var aPlusB = a.Add(b, LengthUnit.FEET);
                    var bPlusA = b.Add(a, LengthUnit.FEET);

                    string[] resultLines = new[]
                    {
                        $"a = {a}",
                        $"b = {b}",
                        "",
                        $"a + b (in feet) = {aPlusB.Value:F6} ft",
                        $"b + a (in feet) = {bPlusA.Value:F6} ft",
                        "",
                        $"Difference: {Math.Abs(aPlusB.Value - bPlusA.Value):E8} ft",
                    };

                    ConsoleHelper.DisplayResultBox("COMMUTATIVITY CHECK", resultLines);

                    if (Math.Abs(aPlusB.Value - bPlusA.Value) < 0.000001)
                    {
                        ConsoleHelper.DisplaySuccess("✅ Addition is COMMUTATIVE! (a + b = b + a)");
                    }
                    else
                    {
                        ConsoleHelper.DisplayError("❌ Addition is NOT commutative!");
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
        }

        private void DisplayBatchOperations()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader("BATCH OPERATIONS", "Multiple values at once");

            string[] batchOptions = new[]
            {
                "1. Batch Conversion",
                "   (Convert multiple values)",
                "",
                "2. Batch Addition",
                "   (Add multiple pairs)",
                "",
                "3. Back to Length Menu",
            };

            ConsoleHelper.DisplayMenu(batchOptions);

            string? choice = ConsoleHelper.GetInput("Enter your choice");

            switch (choice)
            {
                case "1":
                    DisplayBatchConversion();
                    break;
                case "2":
                    DisplayBatchAddition();
                    break;
            }
        }

        private void DisplayBatchConversion()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader("BATCH CONVERSION", "Convert multiple values");

            try
            {
                LengthUnit sourceUnit = GenericUnitSelector.SelectLengthUnit("Select SOURCE unit");
                LengthUnit targetUnit = GenericUnitSelector.SelectLengthUnit("Select TARGET unit");

                Console.Write("\nEnter values (comma-separated, e.g., 1,2.5,3.7): ");
                string? valuesInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(valuesInput))
                {
                    string[] valueStrings = valuesInput.Split(
                        ',',
                        StringSplitOptions.RemoveEmptyEntries
                    );
                    List<string> results = new List<string>();

                    foreach (string valueStr in valueStrings)
                    {
                        if (double.TryParse(valueStr.Trim(), out double value))
                        {
                            double result = _measurementService.ConvertValue(
                                value,
                                sourceUnit,
                                targetUnit
                            );
                            results.Add(
                                $"{value, 8:F3} {sourceUnit.GetSymbol()} = {result, 12:F6} {targetUnit.GetSymbol()}"
                            );
                        }
                    }

                    if (results.Any())
                    {
                        ConsoleHelper.DisplayResultBox("CONVERSION RESULTS", results.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
        }

        private void DisplayBatchAddition()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader("BATCH ADDITION", "Add multiple pairs");

            try
            {
                LengthUnit unit1 = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for first column"
                );
                LengthUnit unit2 = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for second column"
                );
                LengthUnit resultUnit = GenericUnitSelector.SelectLengthUnit(
                    "Select unit for results"
                );

                Console.WriteLine(
                    "\nEnter pairs (format: value1,value2 per line, empty line to finish):"
                );
                Console.WriteLine("Example: 1,12  (means 1 ft + 12 in)");

                List<string> results = new List<string>();

                while (true)
                {
                    Console.Write($"Pair {results.Count + 1}: ");
                    string? line = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                        break;

                    string[] parts = line.Split(',');
                    if (
                        parts.Length == 2
                        && double.TryParse(parts[0].Trim(), out double val1)
                        && double.TryParse(parts[1].Trim(), out double val2)
                    )
                    {
                        var q1 = new GenericQuantity<LengthUnit>(val1, unit1);
                        var q2 = new GenericQuantity<LengthUnit>(val2, unit2);
                        var sum = _measurementService.AddQuantitiesWithTarget(q1, q2, resultUnit);
                        results.Add(
                            $"{val1} {unit1.GetSymbol()} + {val2} {unit2.GetSymbol()} = {sum.Value:F6} {resultUnit.GetSymbol()}"
                        );
                    }
                    else
                    {
                        ConsoleHelper.DisplayError("Invalid format! Use: value1,value2");
                    }
                }

                if (results.Any())
                {
                    ConsoleHelper.DisplayResultBox("ADDITION RESULTS", results.ToArray());
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
        }
    }
}
