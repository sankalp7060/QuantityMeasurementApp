using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Menu for arithmetic operations across all measurement categories.
    /// UC12: Consolidated menu for Addition, Subtraction, and Division.
    /// </summary>
    public class GenericArithmeticMenu
    {
        private readonly GenericMeasurementService _measurementService;
        private readonly string _categoryName;
        private readonly Func<string, object> _unitSelector;
        private readonly object[] _allUnits;

        /// <summary>
        /// Initializes a new instance of the GenericArithmeticMenu class.
        /// </summary>
        /// <param name="measurementService">The measurement service.</param>
        /// <param name="categoryName">Name of the measurement category.</param>
        /// <param name="unitSelector">Function to select a unit.</param>
        /// <param name="allUnits">Array of all units in the category.</param>
        public GenericArithmeticMenu(
            GenericMeasurementService measurementService,
            string categoryName,
            Func<string, object> unitSelector,
            object[] allUnits
        )
        {
            _measurementService = measurementService;
            _categoryName = categoryName;
            _unitSelector = unitSelector;
            _allUnits = allUnits;
        }

        /// <summary>
        /// Displays the arithmetic menu.
        /// </summary>
        public void Display()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                DisplayMainArithmeticMenu();

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        DisplayAdditionMenu();
                        break;
                    case "2":
                        DisplaySubtractionMenu();
                        break;
                    case "3":
                        DisplayDivisionMenu();
                        break;
                    case "4":
                        DisplayAllOperationsDemo();
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

        private void DisplayMainArithmeticMenu()
        {
            ConsoleHelper.DisplayAttributedHeader(
                $"{_categoryName} ARITHMETIC",
                "Add, Subtract, Divide"
            );

            string[] menuOptions = new[]
            {
                "1.  Addition",
                "",
                "2.  Subtraction",
                "",
                "3.  Division",
                "",
                "4.  All Operations Demo",
                "    (Try all operations with one pair)",
                "",
                "5.  Back to Main Menu",
            };

            ConsoleHelper.DisplayMenu(menuOptions);
        }

        private void DisplayAdditionMenu()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                ConsoleHelper.DisplayAttributedHeader("ADDITION", "a + b = c");

                string[] additionOptions = new[]
                {
                    "1.  Result in FIRST unit",
                    "",
                    "2.  Result in SECOND unit",
                    "",
                    "3.  Results in BOTH units",
                    "",
                    "4.  Back to Arithmetic Menu",
                };

                ConsoleHelper.DisplayMenu(additionOptions);

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        PerformAddition(AdditionMode.FirstUnit);
                        break;
                    case "2":
                        PerformAddition(AdditionMode.SecondUnit);
                        break;
                    case "3":
                        PerformAddition(AdditionMode.BothUnits);
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

        private void DisplaySubtractionMenu()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                ConsoleHelper.DisplayAttributedHeader("SUBTRACTION", "a - b = c");

                string[] subtractionOptions = new[]
                {
                    "1.  Result in FIRST unit",
                    "",
                    "2.  Result in SECOND unit",
                    "",
                    "3.  Results in BOTH units",
                    "",
                    "4.  Back to Arithmetic Menu",
                };

                ConsoleHelper.DisplayMenu(subtractionOptions);

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        PerformSubtraction(SubtractionMode.FirstUnit);
                        break;
                    case "2":
                        PerformSubtraction(SubtractionMode.SecondUnit);
                        break;
                    case "3":
                        PerformSubtraction(SubtractionMode.BothUnits);
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

        private void DisplayDivisionMenu()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                ConsoleHelper.DisplayAttributedHeader("DIVISION", "a ÷ b = ratio");

                string[] divisionOptions = new[]
                {
                    "1.  Show ratio only",
                    "",
                    "2.  Show in FIRST unit context",
                    "",
                    "3.  Show in SECOND unit context",
                    "",
                    "4.  Show in BOTH units",
                    "",
                    "5.  Back to Arithmetic Menu",
                };

                ConsoleHelper.DisplayMenu(divisionOptions);

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        PerformDivision(DivisionMode.RatioOnly);
                        break;
                    case "2":
                        PerformDivision(DivisionMode.FirstUnit);
                        break;
                    case "3":
                        PerformDivision(DivisionMode.SecondUnit);
                        break;
                    case "4":
                        PerformDivision(DivisionMode.BothUnits);
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

        private enum DivisionMode
        {
            RatioOnly,
            FirstUnit,
            SecondUnit,
            BothUnits,
        }

        private void PerformDivision(DivisionMode mode)
        {
            ConsoleHelper.ClearScreen();
            string modeText = mode switch
            {
                DivisionMode.RatioOnly => "RATIO ONLY",
                DivisionMode.FirstUnit => "FIRST UNIT CONTEXT",
                DivisionMode.SecondUnit => "SECOND UNIT CONTEXT",
                DivisionMode.BothUnits => "BOTH UNITS",
                _ => "DIVISION",
            };
            ConsoleHelper.DisplayAttributedHeader($"DIVISION - {modeText}", "a ÷ b = ratio");

            try
            {
                // First quantity
                ConsoleHelper.DisplaySubHeader("FIRST QUANTITY (Dividend)");
                object firstUnit = _unitSelector("Select unit for first quantity");
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second quantity
                ConsoleHelper.DisplaySubHeader("SECOND QUANTITY (Divisor)");
                object secondUnit = _unitSelector("Select unit for second quantity");
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    dynamic firstQuantity = CreateQuantity(firstValue, firstUnit);
                    dynamic secondQuantity = CreateQuantity(secondValue, secondUnit);

                    double ratio = firstQuantity.Divide(secondQuantity);

                    List<string> resultLines = new List<string>();

                    if (mode == DivisionMode.RatioOnly || mode == DivisionMode.BothUnits)
                    {
                        resultLines.Add($"{firstQuantity} ÷ {secondQuantity} = {ratio:F6}");
                        resultLines.Add("");
                        resultLines.Add(GetRatioInterpretation(ratio));
                    }

                    if (mode == DivisionMode.FirstUnit || mode == DivisionMode.BothUnits)
                    {
                        dynamic secondInFirstUnit = secondQuantity.ConvertTo(firstUnit);
                        resultLines.Add("");
                        resultLines.Add($"In {GetUnitName(firstUnit)} terms:");
                        resultLines.Add($"  {firstQuantity} ÷ {secondInFirstUnit} = {ratio:F6}");
                        resultLines.Add(
                            $"  Meaning: {firstValue} {GetUnitSymbol(firstUnit)} is {ratio:F2} times"
                        );
                        resultLines.Add(
                            $"           {secondInFirstUnit.Value:F2} {GetUnitSymbol(firstUnit)}"
                        );
                    }

                    if (mode == DivisionMode.SecondUnit || mode == DivisionMode.BothUnits)
                    {
                        dynamic firstInSecondUnit = firstQuantity.ConvertTo(secondUnit);
                        resultLines.Add("");
                        resultLines.Add($"In {GetUnitName(secondUnit)} terms:");
                        resultLines.Add($"  {firstInSecondUnit} ÷ {secondQuantity} = {ratio:F6}");
                        resultLines.Add(
                            $"  Meaning: {firstInSecondUnit.Value:F2} {GetUnitSymbol(secondUnit)} is {ratio:F2} times"
                        );
                        resultLines.Add($"           {secondValue} {GetUnitSymbol(secondUnit)}");
                    }

                    ConsoleHelper.DisplayResultBox("DIVISION RESULT", resultLines.ToArray());

                    // Show in base unit for reference
                    dynamic baseUnit = GetBaseUnit(firstUnit.GetType());
                    dynamic firstInBase = firstQuantity.ConvertTo(baseUnit);
                    dynamic secondInBase = secondQuantity.ConvertTo(baseUnit);

                    ConsoleHelper.DisplayInfoBox(
                        new[]
                        {
                            "In base units:",
                            $"  {firstInBase.Value:F6} {GetUnitSymbol(baseUnit)} ÷ {secondInBase.Value:F6} {GetUnitSymbol(baseUnit)} = {ratio:F6}",
                        }
                    );
                }
                else
                {
                    ConsoleHelper.DisplayError("Invalid numeric values!");
                }
            }
            catch (DivideByZeroException)
            {
                ConsoleHelper.DisplayError("Division by zero is not allowed!");
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
        }

        private void DisplayAllOperationsDemo()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ALL OPERATIONS DEMO",
                "Add, Subtract, Divide with one pair"
            );

            try
            {
                // First quantity
                ConsoleHelper.DisplaySubHeader("FIRST QUANTITY");
                object firstUnit = _unitSelector("Select unit for first quantity");
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second quantity
                ConsoleHelper.DisplaySubHeader("SECOND QUANTITY");
                object secondUnit = _unitSelector("Select unit for second quantity");
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                // Ask user which unit they want results in
                ConsoleHelper.DisplaySubHeader("RESULT UNIT OPTIONS");
                Console.WriteLine("\nResults can be displayed in:");
                Console.WriteLine("  1. First unit only");
                Console.WriteLine("  2. Second unit only");
                Console.WriteLine("  3. Both units (for comparison)");

                string? resultChoice = ConsoleHelper.GetInput("Enter your choice (1-3)");

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    dynamic firstQuantity = CreateQuantity(firstValue, firstUnit);
                    dynamic secondQuantity = CreateQuantity(secondValue, secondUnit);

                    // Convert second to first unit for display
                    dynamic secondInFirstUnit = secondQuantity.ConvertTo(firstUnit);
                    // Convert first to second unit for display
                    dynamic firstInSecondUnit = firstQuantity.ConvertTo(secondUnit);

                    List<string> resultLines = new List<string>();
                    resultLines.Add($"{firstQuantity} and {secondQuantity}");
                    resultLines.Add($"  = {firstQuantity} and {secondInFirstUnit} (in first unit)");
                    resultLines.Add(
                        $"  = {firstInSecondUnit} and {secondQuantity} (in second unit)"
                    );
                    resultLines.Add("");

                    // Perform operations
                    if (resultChoice == "1" || resultChoice == "3")
                    {
                        dynamic sumInFirst = firstQuantity.Add(secondQuantity, firstUnit);
                        dynamic diffInFirst = firstQuantity.Subtract(secondQuantity, firstUnit);
                        double ratio = firstQuantity.Divide(secondQuantity);

                        resultLines.Add($"In FIRST unit ({GetUnitSymbol(firstUnit)}):");
                        resultLines.Add(
                            $"  Addition:    {firstQuantity} + {secondQuantity} = {sumInFirst.Value:F6} {GetUnitSymbol(firstUnit)}"
                        );
                        resultLines.Add(
                            $"  Subtraction: {firstQuantity} - {secondQuantity} = {diffInFirst.Value:F6} {GetUnitSymbol(firstUnit)}"
                        );
                        resultLines.Add(
                            $"  Division:    {firstQuantity} ÷ {secondQuantity} = {ratio:F6} (dimensionless)"
                        );
                        resultLines.Add("");
                    }

                    if (resultChoice == "2" || resultChoice == "3")
                    {
                        dynamic sumInSecond = firstQuantity.Add(secondQuantity, secondUnit);
                        dynamic diffInSecond = firstQuantity.Subtract(secondQuantity, secondUnit);
                        double ratio = firstQuantity.Divide(secondQuantity);

                        resultLines.Add($"In SECOND unit ({GetUnitSymbol(secondUnit)}):");
                        resultLines.Add(
                            $"  Addition:    {firstQuantity} + {secondQuantity} = {sumInSecond.Value:F6} {GetUnitSymbol(secondUnit)}"
                        );
                        resultLines.Add(
                            $"  Subtraction: {firstQuantity} - {secondQuantity} = {diffInSecond.Value:F6} {GetUnitSymbol(secondUnit)}"
                        );
                        resultLines.Add(
                            $"  Division:    {firstQuantity} ÷ {secondQuantity} = {ratio:F6} (dimensionless)"
                        );
                    }

                    if (resultChoice == "3")
                    {
                        resultLines.Add("");
                        resultLines.Add("Note: Division result is the same regardless of unit");
                        resultLines.Add("      because it's a dimensionless ratio.");
                    }

                    ConsoleHelper.DisplayResultBox("ALL OPERATIONS RESULTS", resultLines.ToArray());
                }
                else
                {
                    ConsoleHelper.DisplayError("Invalid numeric values!");
                }
            }
            catch (DivideByZeroException)
            {
                ConsoleHelper.DisplayError("Division by zero is not allowed!");
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
        }

        private enum AdditionMode
        {
            FirstUnit,
            SecondUnit,
            BothUnits,
        }

        private enum SubtractionMode
        {
            FirstUnit,
            SecondUnit,
            BothUnits,
        }

        private void PerformAddition(AdditionMode mode)
        {
            ConsoleHelper.ClearScreen();
            string modeText =
                mode == AdditionMode.FirstUnit ? "FIRST UNIT"
                : mode == AdditionMode.SecondUnit ? "SECOND UNIT"
                : "BOTH UNITS";
            ConsoleHelper.DisplayAttributedHeader($"ADDITION - RESULT IN {modeText}", "a + b = c");

            try
            {
                // First quantity
                ConsoleHelper.DisplaySubHeader("FIRST QUANTITY");
                object firstUnit = _unitSelector("Select unit for first quantity");
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second quantity
                ConsoleHelper.DisplaySubHeader("SECOND QUANTITY");
                object secondUnit = _unitSelector("Select unit for second quantity");
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    dynamic firstQuantity = CreateQuantity(firstValue, firstUnit);
                    dynamic secondQuantity = CreateQuantity(secondValue, secondUnit);

                    // Convert second to first unit for display
                    dynamic secondInFirstUnit = secondQuantity.ConvertTo(firstUnit);

                    if (mode == AdditionMode.FirstUnit)
                    {
                        dynamic sum = firstQuantity.Add(secondQuantity);
                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} + {secondQuantity}",
                            $"  = {firstQuantity} + {secondInFirstUnit}",
                            "",
                            $"  = {sum.Value:F6} {GetUnitSymbol(firstUnit)}",
                        };
                        ConsoleHelper.DisplayResultBox("ADDITION RESULT", resultLines);
                    }
                    else if (mode == AdditionMode.SecondUnit)
                    {
                        dynamic sum = firstQuantity.Add(secondQuantity, secondUnit);
                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} + {secondQuantity}",
                            $"  = {firstQuantity.ConvertTo(secondUnit)} + {secondQuantity}",
                            "",
                            $"  = {sum.Value:F6} {GetUnitSymbol(secondUnit)}",
                        };
                        ConsoleHelper.DisplayResultBox("ADDITION RESULT", resultLines);
                    }
                    else // BothUnits
                    {
                        dynamic sumInFirst = firstQuantity.Add(secondQuantity, firstUnit);
                        dynamic sumInSecond = firstQuantity.Add(secondQuantity, secondUnit);

                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} + {secondQuantity}",
                            $"  = {firstQuantity} + {secondInFirstUnit}",
                            "",
                            $"In {GetUnitName(firstUnit)}: {sumInFirst.Value:F6} {GetUnitSymbol(firstUnit)}",
                            $"In {GetUnitName(secondUnit)}: {sumInSecond.Value:F6} {GetUnitSymbol(secondUnit)}",
                        };

                        ConsoleHelper.DisplayResultBox("ADDITION RESULTS", resultLines);
                    }

                    // Show calculation details
                    ShowArithmeticDetails(firstQuantity, secondQuantity, "addition", firstUnit);
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

        private void PerformSubtraction(SubtractionMode mode)
        {
            ConsoleHelper.ClearScreen();
            string modeText =
                mode == SubtractionMode.FirstUnit ? "FIRST UNIT"
                : mode == SubtractionMode.SecondUnit ? "SECOND UNIT"
                : "BOTH UNITS";
            ConsoleHelper.DisplayAttributedHeader(
                $"SUBTRACTION - RESULT IN {modeText}",
                "a - b = c"
            );

            try
            {
                // First quantity
                ConsoleHelper.DisplaySubHeader("FIRST QUANTITY (Minuend)");
                object firstUnit = _unitSelector("Select unit for first quantity");
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second quantity
                ConsoleHelper.DisplaySubHeader("SECOND QUANTITY (Subtrahend)");
                object secondUnit = _unitSelector("Select unit for second quantity");
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    dynamic firstQuantity = CreateQuantity(firstValue, firstUnit);
                    dynamic secondQuantity = CreateQuantity(secondValue, secondUnit);

                    // Convert second to first unit for display
                    dynamic secondInFirstUnit = secondQuantity.ConvertTo(firstUnit);

                    if (mode == SubtractionMode.FirstUnit)
                    {
                        dynamic difference = firstQuantity.Subtract(secondQuantity);
                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} - {secondQuantity}",
                            $"  = {firstQuantity} - {secondInFirstUnit}",
                            "",
                            $"  = {difference.Value:F6} {GetUnitSymbol(firstUnit)}",
                        };
                        ConsoleHelper.DisplayResultBox("SUBTRACTION RESULT", resultLines);
                    }
                    else if (mode == SubtractionMode.SecondUnit)
                    {
                        dynamic difference = firstQuantity.Subtract(secondQuantity, secondUnit);
                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} - {secondQuantity}",
                            $"  = {firstQuantity.ConvertTo(secondUnit)} - {secondQuantity}",
                            "",
                            $"  = {difference.Value:F6} {GetUnitSymbol(secondUnit)}",
                        };
                        ConsoleHelper.DisplayResultBox("SUBTRACTION RESULT", resultLines);
                    }
                    else // BothUnits
                    {
                        dynamic diffInFirst = firstQuantity.Subtract(secondQuantity, firstUnit);
                        dynamic diffInSecond = firstQuantity.Subtract(secondQuantity, secondUnit);

                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} - {secondQuantity}",
                            $"  = {firstQuantity} - {secondInFirstUnit}",
                            "",
                            $"In {GetUnitName(firstUnit)}: {diffInFirst.Value:F6} {GetUnitSymbol(firstUnit)}",
                            $"In {GetUnitName(secondUnit)}: {diffInSecond.Value:F6} {GetUnitSymbol(secondUnit)}",
                        };

                        ConsoleHelper.DisplayResultBox("SUBTRACTION RESULTS", resultLines);
                    }

                    // Show calculation details
                    ShowArithmeticDetails(firstQuantity, secondQuantity, "subtraction", firstUnit);
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

        private void ShowArithmeticDetails(
            dynamic firstQuantity,
            dynamic secondQuantity,
            string operation,
            object displayUnit
        )
        {
            dynamic baseUnit = GetBaseUnit(firstQuantity.Unit.GetType());
            dynamic firstInBase = firstQuantity.ConvertTo(baseUnit);
            dynamic secondInBase = secondQuantity.ConvertTo(baseUnit);

            double operationInBase =
                operation == "addition"
                    ? firstInBase.Value + secondInBase.Value
                    : firstInBase.Value - secondInBase.Value;

            dynamic resultInDisplayUnit =
                operation == "addition"
                    ? firstQuantity.Add(secondQuantity, displayUnit)
                    : firstQuantity.Subtract(secondQuantity, displayUnit);

            string[] calculationLines = new[]
            {
                "Step 1: Convert to base unit",
                $"  {firstQuantity} = {firstInBase.Value:F6} {baseUnit.GetSymbol()}",
                $"  {secondQuantity} = {secondInBase.Value:F6} {baseUnit.GetSymbol()}",
                "",
                $"Step 2: Perform {operation} in base unit",
                $"  {firstInBase.Value:F6} {GetOperator(operation)} {secondInBase.Value:F6} = {operationInBase:F6} {baseUnit.GetSymbol()}",
                "",
                "Step 3: Convert to target unit",
                $"  Result = {resultInDisplayUnit.Value:F6} {GetUnitSymbol(displayUnit)}",
            };

            ConsoleHelper.DisplayResultBox("CALCULATION DETAILS", calculationLines);
        }

        private string GetOperator(string operation)
        {
            return operation switch
            {
                "addition" => "+",
                "subtraction" => "-",
                "division" => "÷",
                _ => "?",
            };
        }

        private string GetRatioInterpretation(double ratio)
        {
            if (Math.Abs(ratio - 1.0) < 0.000001)
                return "The quantities are equal.";
            else if (ratio > 1.0)
                return $"First quantity is {ratio:F2} times larger than second.";
            else
                return $"First quantity is {(1 / ratio):F2} times smaller than second.";
        }

        private dynamic CreateQuantity(double value, object unit)
        {
            var type = typeof(GenericQuantity<>).MakeGenericType(unit.GetType());
            return Activator.CreateInstance(type, value, unit)!;
        }

        private dynamic GetBaseUnit(Type unitType)
        {
            if (unitType == typeof(LengthUnit))
                return LengthUnit.FEET;
            else if (unitType == typeof(WeightUnit))
                return WeightUnit.KILOGRAM;
            else if (unitType == typeof(VolumeUnit))
                return VolumeUnit.LITRE;
            else
                throw new ArgumentException($"Unknown unit type: {unitType}");
        }

        private string GetUnitSymbol(object unit)
        {
            if (unit is LengthUnit lengthUnit)
                return lengthUnit.GetSymbol();
            else if (unit is WeightUnit weightUnit)
                return weightUnit.GetSymbol();
            else if (unit is VolumeUnit volumeUnit)
                return volumeUnit.GetSymbol();
            else
                return "?";
        }

        private string GetUnitName(object unit)
        {
            if (unit is LengthUnit lengthUnit)
                return lengthUnit.GetName();
            else if (unit is WeightUnit weightUnit)
                return weightUnit.GetName();
            else if (unit is VolumeUnit volumeUnit)
                return volumeUnit.GetName();
            else
                return "?";
        }
    }
}
