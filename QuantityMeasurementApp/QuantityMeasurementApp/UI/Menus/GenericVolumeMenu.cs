using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Menu for volume measurement operations using generic Quantity class.
    /// UC11: Menu for all volume operations (litres, millilitres, gallons).
    /// </summary>
    public class GenericVolumeMenu
    {
        private readonly GenericMeasurementService _measurementService;

        /// <summary>
        /// Initializes a new instance of the GenericVolumeMenu class.
        /// </summary>
        /// <param name="measurementService">The measurement service.</param>
        public GenericVolumeMenu(GenericMeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        /// <summary>
        /// Displays the volume menu.
        /// </summary>
        public void Display()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                DisplayMainVolumeMenu();

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        DisplayVolumeConversion();
                        break;
                    case "2":
                        DisplayVolumeComparison();
                        break;
                    case "3":
                        DisplayVolumeAddition();
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

        private void DisplayMainVolumeMenu()
        {
            ConsoleHelper.DisplayAttributedHeader("VOLUME MEASUREMENTS", "L, mL, gal");

            string[] menuOptions = new[]
            {
                "1.  Convert Volume Units",
                "",
                "2.  Compare Volumes",
                "",
                "3.  Add Volumes",
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

        private void DisplayVolumeConversion()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader("VOLUME CONVERSION", "1 L = 1000 mL = 0.264 gal");

            try
            {
                VolumeUnit sourceUnit = VolumeUnitSelector.SelectVolumeUnit("Select SOURCE unit");
                VolumeUnit targetUnit = VolumeUnitSelector.SelectVolumeUnit("Select TARGET unit");

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

        private void DisplayVolumeComparison()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader("VOLUME COMPARISON", "1 L = 1000 mL = 0.264 gal");

            try
            {
                // First volume
                ConsoleHelper.DisplaySubHeader("FIRST VOLUME");
                VolumeUnit firstUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for first volume"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second volume
                ConsoleHelper.DisplaySubHeader("SECOND VOLUME");
                VolumeUnit secondUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for second volume"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<VolumeUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<VolumeUnit>(secondValue, secondUnit);

                    bool areEqual = _measurementService.AreQuantitiesEqual(
                        firstQuantity,
                        secondQuantity
                    );

                    // Show in base unit for reference
                    var firstInLitre = firstQuantity.ConvertTo(VolumeUnit.LITRE);
                    var secondInLitre = secondQuantity.ConvertTo(VolumeUnit.LITRE);

                    ConsoleHelper.DisplayComparisonResult(
                        firstQuantity.ToString()!,
                        secondQuantity.ToString()!,
                        areEqual,
                        firstInLitre.Value,
                        secondInLitre.Value,
                        "L"
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

        private void DisplayVolumeAddition()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                ConsoleHelper.DisplayAttributedHeader("VOLUME ADDITION", "1 L + 500 mL = 1.5 L");

                string[] additionOptions = new[]
                {
                    "1.  Result in FIRST unit",
                    "",
                    "2.  Result in SECOND unit",
                    "",
                    "3.  Results in BOTH units",
                    "",
                    "4.  Back to Volume Menu",
                };

                ConsoleHelper.DisplayMenu(additionOptions);

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        DisplayVolumeAdditionInFirstUnit();
                        break;
                    case "2":
                        DisplayVolumeAdditionInSecondUnit();
                        break;
                    case "3":
                        DisplayVolumeAdditionInBothUnits();
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

        private void DisplayVolumeAdditionInFirstUnit()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ADDITION - RESULT IN FIRST UNIT",
                "1 L + 500 mL = 1.5 L"
            );

            try
            {
                // First volume
                ConsoleHelper.DisplaySubHeader("FIRST VOLUME");
                VolumeUnit firstUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for first volume"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second volume
                ConsoleHelper.DisplaySubHeader("SECOND VOLUME");
                VolumeUnit secondUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for second volume"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<VolumeUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<VolumeUnit>(secondValue, secondUnit);

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

                    ShowVolumeCalculationDetails(
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

        private void DisplayVolumeAdditionInSecondUnit()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ADDITION - RESULT IN SECOND UNIT",
                "1 L + 500 mL = 1500 mL"
            );

            try
            {
                // First volume
                ConsoleHelper.DisplaySubHeader("FIRST VOLUME");
                VolumeUnit firstUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for first volume"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second volume
                ConsoleHelper.DisplaySubHeader("SECOND VOLUME");
                VolumeUnit secondUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for second volume"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<VolumeUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<VolumeUnit>(secondValue, secondUnit);

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

                    ShowVolumeCalculationDetails(
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

        private void DisplayVolumeAdditionInBothUnits()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ADDITION - RESULTS IN BOTH UNITS",
                "Compare results"
            );

            try
            {
                // First volume
                ConsoleHelper.DisplaySubHeader("FIRST VOLUME");
                VolumeUnit firstUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for first volume"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second volume
                ConsoleHelper.DisplaySubHeader("SECOND VOLUME");
                VolumeUnit secondUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for second volume"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new GenericQuantity<VolumeUnit>(firstValue, firstUnit);
                    var secondQuantity = new GenericQuantity<VolumeUnit>(secondValue, secondUnit);

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
                    ShowVolumeCalculationDetails(
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

        private void ShowVolumeCalculationDetails(
            GenericQuantity<VolumeUnit> firstQuantity,
            GenericQuantity<VolumeUnit> secondQuantity,
            VolumeUnit resultUnit,
            GenericQuantity<VolumeUnit> sumQuantity
        )
        {
            var firstInLitre = firstQuantity.ConvertTo(VolumeUnit.LITRE);
            var secondInLitre = secondQuantity.ConvertTo(VolumeUnit.LITRE);
            double totalInLitre = firstInLitre.Value + secondInLitre.Value;

            string[] calculationLines = new[]
            {
                "Step 1: Convert to base unit (litres)",
                $"  {firstQuantity} = {firstInLitre.Value:F6} L",
                $"  {secondQuantity} = {secondInLitre.Value:F6} L",
                "",
                "Step 2: Add in litres",
                $"  {firstInLitre.Value:F6} + {secondInLitre.Value:F6} = {totalInLitre:F6} L",
                "",
                "Step 3: Convert to target unit",
                $"  {totalInLitre:F6} L = {sumQuantity.Value:F6} {resultUnit.GetSymbol()}",
            };

            ConsoleHelper.DisplayResultBox("CALCULATION DETAILS", calculationLines);
        }

        private void DisplayCommutativityDemo()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader("COMMUTATIVITY DEMO", "a + b = b + a");

            try
            {
                ConsoleHelper.DisplaySubHeader("FIRST VOLUME (a)");
                VolumeUnit firstUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for first volume"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                ConsoleHelper.DisplaySubHeader("SECOND VOLUME (b)");
                VolumeUnit secondUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for second volume"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var a = new GenericQuantity<VolumeUnit>(firstValue, firstUnit);
                    var b = new GenericQuantity<VolumeUnit>(secondValue, secondUnit);

                    // Add in both orders
                    var aPlusB = a.Add(b, VolumeUnit.LITRE);
                    var bPlusA = b.Add(a, VolumeUnit.LITRE);

                    string[] resultLines = new[]
                    {
                        $"a = {a}",
                        $"b = {b}",
                        "",
                        $"a + b (in litres) = {aPlusB.Value:F6} L",
                        $"b + a (in litres) = {bPlusA.Value:F6} L",
                        "",
                        $"Difference: {Math.Abs(aPlusB.Value - bPlusA.Value):E8} L",
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
                "3. Back to Volume Menu",
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
                VolumeUnit sourceUnit = VolumeUnitSelector.SelectVolumeUnit("Select SOURCE unit");
                VolumeUnit targetUnit = VolumeUnitSelector.SelectVolumeUnit("Select TARGET unit");

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
                VolumeUnit unit1 = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for first column"
                );
                VolumeUnit unit2 = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for second column"
                );
                VolumeUnit resultUnit = VolumeUnitSelector.SelectVolumeUnit(
                    "Select unit for results"
                );

                Console.WriteLine(
                    "\nEnter pairs (format: value1,value2 per line, empty line to finish):"
                );
                Console.WriteLine("Example: 1,500  (means 1 L + 500 mL)");

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
                        var q1 = new GenericQuantity<VolumeUnit>(val1, unit1);
                        var q2 = new GenericQuantity<VolumeUnit>(val2, unit2);
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
