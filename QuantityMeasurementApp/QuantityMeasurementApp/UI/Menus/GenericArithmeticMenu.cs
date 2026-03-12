using System;
using System.Collections.Generic;
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
                "    (a + b = c)",
                "",
                "2.  Subtraction",
                "    (a - b = c)",
                "",
                "3.  Division",
                "    (a ÷ b = ratio)",
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
                    "    (e.g., 5 ft + 3 ft = 8 ft)",
                    "",
                    "2.  Result in SECOND unit",
                    "    (e.g., 5 ft + 3 ft = 8 ft)",
                    "",
                    "3.  Results in BOTH units",
                    "    (Compare both results)",
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

                string[] subtractionOptions = new[]
                {
                    "1.  Result in FIRST unit",
                    "    (e.g., 10 ft - 4 ft = 6 ft)",
                    "",
                    "2.  Result in SECOND unit",
                    "    (e.g., 10 ft - 4 ft = 6 ft)",
                    "",
                    "3.  Results in BOTH units",
                    "    (Compare both results)",
                    "",
                    "4.  Back to Arithmetic Menu",
                };

                // Display header and menu together
                ConsoleHelper.DisplayAttributedHeader("SUBTRACTION", "a - b = c");
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

                string[] divisionOptions = new[]
                {
                    "1.  Show ratio only",
                    "    (e.g., 10 ft ÷ 2 ft = 5.0)",
                    "",
                    "2.  Show in FIRST unit context",
                    "    (With conversion details)",
                    "",
                    "3.  Show in SECOND unit context",
                    "    (With conversion details)",
                    "",
                    "4.  Show in BOTH units",
                    "    (Compare interpretations)",
                    "",
                    "5.  Back to Arithmetic Menu",
                };

                // Display header and menu together
                ConsoleHelper.DisplayAttributedHeader("DIVISION", "a ÷ b = ratio");
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

        private void DisplayAllOperationsDemo()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ALL OPERATIONS DEMO",
                "Add, Subtract, Divide with one pair"
            );

            try
            {
                // FIRST: Get inputs
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
                    !double.TryParse(firstInput, out double firstValue)
                    || !double.TryParse(secondInput, out double secondValue)
                )
                {
                    ConsoleHelper.DisplayError("Invalid numeric values!");
                    ConsoleHelper.WaitForKeyPress();
                    return;
                }

                // SECOND: Now ask how they want results displayed
                ConsoleHelper.DisplaySubHeader("RESULT DISPLAY OPTIONS");
                Console.WriteLine("\nHow would you like to see the results?");
                Console.WriteLine("  1. Show results in first unit only");
                Console.WriteLine("  2. Show results in second unit only");
                Console.WriteLine("  3. Show results in both units (for comparison)");

                string? resultChoice = ConsoleHelper.GetInput("Enter your choice (1-3)");

                // Process based on unit type
                Type firstUnitType = firstUnit.GetType();
                Type secondUnitType = secondUnit.GetType();

                if (firstUnitType != secondUnitType)
                {
                    ConsoleHelper.DisplayError("Units must be of the same measurement category!");
                    ConsoleHelper.WaitForKeyPress();
                    return;
                }

                List<string> resultLines = new List<string>();

                if (firstUnitType == typeof(LengthUnit))
                {
                    // Length operations
                    var firstQuantity = new GenericQuantity<LengthUnit>(
                        firstValue,
                        (LengthUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<LengthUnit>(
                        secondValue,
                        (LengthUnit)secondUnit
                    );

                    var secondInFirstUnit = secondQuantity.ConvertTo((LengthUnit)firstUnit);
                    var firstInSecondUnit = firstQuantity.ConvertTo((LengthUnit)secondUnit);

                    resultLines.Add($"{firstQuantity} and {secondQuantity}");
                    resultLines.Add($"  = {firstQuantity} and {secondInFirstUnit} (in first unit)");
                    resultLines.Add(
                        $"  = {firstInSecondUnit} and {secondQuantity} (in second unit)"
                    );
                    resultLines.Add("");

                    // Perform operations
                    if (resultChoice == "1" || resultChoice == "3")
                    {
                        var sumInFirst = firstQuantity.Add(secondQuantity, (LengthUnit)firstUnit);
                        var diffInFirst = firstQuantity.Subtract(
                            secondQuantity,
                            (LengthUnit)firstUnit
                        );
                        double ratio = firstQuantity.Divide(secondQuantity);

                        resultLines.Add(
                            $"In FIRST unit ({((LengthUnit)firstUnit).GetSymbol()} - {((LengthUnit)firstUnit).GetName()}):"
                        );
                        resultLines.Add(
                            $"  Addition:    {firstQuantity} + {secondQuantity} = {sumInFirst.Value:F2} {((LengthUnit)firstUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Subtraction: {firstQuantity} - {secondQuantity} = {diffInFirst.Value:F2} {((LengthUnit)firstUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Division:    {firstQuantity} ÷ {secondQuantity} = {ratio:F4} (dimensionless)"
                        );
                        resultLines.Add("");
                    }

                    if (resultChoice == "2" || resultChoice == "3")
                    {
                        var sumInSecond = firstQuantity.Add(secondQuantity, (LengthUnit)secondUnit);
                        var diffInSecond = firstQuantity.Subtract(
                            secondQuantity,
                            (LengthUnit)secondUnit
                        );
                        double ratio = firstQuantity.Divide(secondQuantity);

                        resultLines.Add(
                            $"In SECOND unit ({((LengthUnit)secondUnit).GetSymbol()} - {((LengthUnit)secondUnit).GetName()}):"
                        );
                        resultLines.Add(
                            $"  Addition:    {firstQuantity} + {secondQuantity} = {sumInSecond.Value:F2} {((LengthUnit)secondUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Subtraction: {firstQuantity} - {secondQuantity} = {diffInSecond.Value:F2} {((LengthUnit)secondUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Division:    {firstQuantity} ÷ {secondQuantity} = {ratio:F4} (dimensionless)"
                        );
                    }
                }
                else if (firstUnitType == typeof(WeightUnit))
                {
                    // Weight operations
                    var firstQuantity = new GenericQuantity<WeightUnit>(
                        firstValue,
                        (WeightUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<WeightUnit>(
                        secondValue,
                        (WeightUnit)secondUnit
                    );

                    var secondInFirstUnit = secondQuantity.ConvertTo((WeightUnit)firstUnit);
                    var firstInSecondUnit = firstQuantity.ConvertTo((WeightUnit)secondUnit);

                    resultLines.Add($"{firstQuantity} and {secondQuantity}");
                    resultLines.Add($"  = {firstQuantity} and {secondInFirstUnit} (in first unit)");
                    resultLines.Add(
                        $"  = {firstInSecondUnit} and {secondQuantity} (in second unit)"
                    );
                    resultLines.Add("");

                    // Perform operations
                    if (resultChoice == "1" || resultChoice == "3")
                    {
                        var sumInFirst = firstQuantity.Add(secondQuantity, (WeightUnit)firstUnit);
                        var diffInFirst = firstQuantity.Subtract(
                            secondQuantity,
                            (WeightUnit)firstUnit
                        );
                        double ratio = firstQuantity.Divide(secondQuantity);

                        resultLines.Add(
                            $"In FIRST unit ({((WeightUnit)firstUnit).GetSymbol()} - {((WeightUnit)firstUnit).GetName()}):"
                        );
                        resultLines.Add(
                            $"  Addition:    {firstQuantity} + {secondQuantity} = {sumInFirst.Value:F2} {((WeightUnit)firstUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Subtraction: {firstQuantity} - {secondQuantity} = {diffInFirst.Value:F2} {((WeightUnit)firstUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Division:    {firstQuantity} ÷ {secondQuantity} = {ratio:F4} (dimensionless)"
                        );
                        resultLines.Add("");
                    }

                    if (resultChoice == "2" || resultChoice == "3")
                    {
                        var sumInSecond = firstQuantity.Add(secondQuantity, (WeightUnit)secondUnit);
                        var diffInSecond = firstQuantity.Subtract(
                            secondQuantity,
                            (WeightUnit)secondUnit
                        );
                        double ratio = firstQuantity.Divide(secondQuantity);

                        resultLines.Add(
                            $"In SECOND unit ({((WeightUnit)secondUnit).GetSymbol()} - {((WeightUnit)secondUnit).GetName()}):"
                        );
                        resultLines.Add(
                            $"  Addition:    {firstQuantity} + {secondQuantity} = {sumInSecond.Value:F2} {((WeightUnit)secondUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Subtraction: {firstQuantity} - {secondQuantity} = {diffInSecond.Value:F2} {((WeightUnit)secondUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Division:    {firstQuantity} ÷ {secondQuantity} = {ratio:F4} (dimensionless)"
                        );
                    }
                }
                else if (firstUnitType == typeof(VolumeUnit))
                {
                    // Volume operations
                    var firstQuantity = new GenericQuantity<VolumeUnit>(
                        firstValue,
                        (VolumeUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<VolumeUnit>(
                        secondValue,
                        (VolumeUnit)secondUnit
                    );

                    var secondInFirstUnit = secondQuantity.ConvertTo((VolumeUnit)firstUnit);
                    var firstInSecondUnit = firstQuantity.ConvertTo((VolumeUnit)secondUnit);

                    resultLines.Add($"{firstQuantity} and {secondQuantity}");
                    resultLines.Add($"  = {firstQuantity} and {secondInFirstUnit} (in first unit)");
                    resultLines.Add(
                        $"  = {firstInSecondUnit} and {secondQuantity} (in second unit)"
                    );
                    resultLines.Add("");

                    // Perform operations
                    if (resultChoice == "1" || resultChoice == "3")
                    {
                        var sumInFirst = firstQuantity.Add(secondQuantity, (VolumeUnit)firstUnit);
                        var diffInFirst = firstQuantity.Subtract(
                            secondQuantity,
                            (VolumeUnit)firstUnit
                        );
                        double ratio = firstQuantity.Divide(secondQuantity);

                        resultLines.Add(
                            $"In FIRST unit ({((VolumeUnit)firstUnit).GetSymbol()} - {((VolumeUnit)firstUnit).GetName()}):"
                        );
                        resultLines.Add(
                            $"  Addition:    {firstQuantity} + {secondQuantity} = {sumInFirst.Value:F2} {((VolumeUnit)firstUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Subtraction: {firstQuantity} - {secondQuantity} = {diffInFirst.Value:F2} {((VolumeUnit)firstUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Division:    {firstQuantity} ÷ {secondQuantity} = {ratio:F4} (dimensionless)"
                        );
                        resultLines.Add("");
                    }

                    if (resultChoice == "2" || resultChoice == "3")
                    {
                        var sumInSecond = firstQuantity.Add(secondQuantity, (VolumeUnit)secondUnit);
                        var diffInSecond = firstQuantity.Subtract(
                            secondQuantity,
                            (VolumeUnit)secondUnit
                        );
                        double ratio = firstQuantity.Divide(secondQuantity);

                        resultLines.Add(
                            $"In SECOND unit ({((VolumeUnit)secondUnit).GetSymbol()} - {((VolumeUnit)secondUnit).GetName()}):"
                        );
                        resultLines.Add(
                            $"  Addition:    {firstQuantity} + {secondQuantity} = {sumInSecond.Value:F2} {((VolumeUnit)secondUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Subtraction: {firstQuantity} - {secondQuantity} = {diffInSecond.Value:F2} {((VolumeUnit)secondUnit).GetSymbol()}"
                        );
                        resultLines.Add(
                            $"  Division:    {firstQuantity} ÷ {secondQuantity} = {ratio:F4} (dimensionless)"
                        );
                    }
                }
                else if (firstUnitType == typeof(TemperatureUnit))
                {
                    // Temperature operations - only equality and conversion supported
                    var firstQuantity = new GenericQuantity<TemperatureUnit>(
                        firstValue,
                        (TemperatureUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<TemperatureUnit>(
                        secondValue,
                        (TemperatureUnit)secondUnit
                    );

                    var secondInFirstUnit = secondQuantity.ConvertTo((TemperatureUnit)firstUnit);
                    var firstInSecondUnit = firstQuantity.ConvertTo((TemperatureUnit)secondUnit);

                    resultLines.Add($"{firstQuantity} and {secondQuantity}");
                    resultLines.Add($"  = {firstQuantity} and {secondInFirstUnit} (in first unit)");
                    resultLines.Add(
                        $"  = {firstInSecondUnit} and {secondQuantity} (in second unit)"
                    );
                    resultLines.Add("");
                    resultLines.Add(
                        "⚠️  NOTE: Temperature does NOT support arithmetic operations!"
                    );
                    resultLines.Add(
                        "   Only equality comparison and unit conversion are supported."
                    );
                }

                if (resultChoice == "3" && firstUnitType != typeof(TemperatureUnit))
                {
                    resultLines.Add("");
                    resultLines.Add("Note: Division result is the same regardless of unit");
                    resultLines.Add("      because it's a dimensionless ratio.");
                }

                ConsoleHelper.DisplayResultBox("ALL OPERATIONS RESULTS", resultLines.ToArray());
            }
            catch (DivideByZeroException)
            {
                ConsoleHelper.DisplayError("Division by zero is not allowed!");
            }
            catch (NotSupportedException ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
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

        private enum DivisionMode
        {
            RatioOnly,
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
                object firstUnit = _unitSelector(
                    $"Select unit for first {_categoryName.ToLower()}"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second quantity
                ConsoleHelper.DisplaySubHeader("SECOND QUANTITY");
                object secondUnit = _unitSelector(
                    $"Select unit for second {_categoryName.ToLower()}"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    !double.TryParse(firstInput, out double firstValue)
                    || !double.TryParse(secondInput, out double secondValue)
                )
                {
                    ConsoleHelper.DisplayError("Invalid numeric values!");
                    ConsoleHelper.WaitForKeyPress();
                    return;
                }

                Type unitType = firstUnit.GetType();

                if (unitType == typeof(LengthUnit))
                {
                    var firstQuantity = new GenericQuantity<LengthUnit>(
                        firstValue,
                        (LengthUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<LengthUnit>(
                        secondValue,
                        (LengthUnit)secondUnit
                    );

                    if (mode == AdditionMode.FirstUnit)
                    {
                        var sum = firstQuantity.Add(secondQuantity);
                        ConsoleHelper.DisplayAdditionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            sum.Value,
                            sum.Unit.GetSymbol()
                        );
                    }
                    else if (mode == AdditionMode.SecondUnit)
                    {
                        var sum = firstQuantity.Add(secondQuantity, (LengthUnit)secondUnit);
                        ConsoleHelper.DisplayAdditionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            sum.Value,
                            sum.Unit.GetSymbol()
                        );
                    }
                    else
                    {
                        var sumInFirst = firstQuantity.Add(secondQuantity, (LengthUnit)firstUnit);
                        var sumInSecond = firstQuantity.Add(secondQuantity, (LengthUnit)secondUnit);

                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} + {secondQuantity}",
                            "",
                            $"In {((LengthUnit)firstUnit).GetName()}: {sumInFirst.Value:F2} {((LengthUnit)firstUnit).GetSymbol()}",
                            $"In {((LengthUnit)secondUnit).GetName()}: {sumInSecond.Value:F2} {((LengthUnit)secondUnit).GetSymbol()}",
                        };
                        ConsoleHelper.DisplayResultBox("ADDITION RESULTS", resultLines);
                    }
                }
                else if (unitType == typeof(WeightUnit))
                {
                    var firstQuantity = new GenericQuantity<WeightUnit>(
                        firstValue,
                        (WeightUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<WeightUnit>(
                        secondValue,
                        (WeightUnit)secondUnit
                    );

                    if (mode == AdditionMode.FirstUnit)
                    {
                        var sum = firstQuantity.Add(secondQuantity);
                        ConsoleHelper.DisplayAdditionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            sum.Value,
                            sum.Unit.GetSymbol()
                        );
                    }
                    else if (mode == AdditionMode.SecondUnit)
                    {
                        var sum = firstQuantity.Add(secondQuantity, (WeightUnit)secondUnit);
                        ConsoleHelper.DisplayAdditionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            sum.Value,
                            sum.Unit.GetSymbol()
                        );
                    }
                    else
                    {
                        var sumInFirst = firstQuantity.Add(secondQuantity, (WeightUnit)firstUnit);
                        var sumInSecond = firstQuantity.Add(secondQuantity, (WeightUnit)secondUnit);

                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} + {secondQuantity}",
                            "",
                            $"In {((WeightUnit)firstUnit).GetName()}: {sumInFirst.Value:F2} {((WeightUnit)firstUnit).GetSymbol()}",
                            $"In {((WeightUnit)secondUnit).GetName()}: {sumInSecond.Value:F2} {((WeightUnit)secondUnit).GetSymbol()}",
                        };
                        ConsoleHelper.DisplayResultBox("ADDITION RESULTS", resultLines);
                    }
                }
                else if (unitType == typeof(VolumeUnit))
                {
                    var firstQuantity = new GenericQuantity<VolumeUnit>(
                        firstValue,
                        (VolumeUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<VolumeUnit>(
                        secondValue,
                        (VolumeUnit)secondUnit
                    );

                    if (mode == AdditionMode.FirstUnit)
                    {
                        var sum = firstQuantity.Add(secondQuantity);
                        ConsoleHelper.DisplayAdditionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            sum.Value,
                            sum.Unit.GetSymbol()
                        );
                    }
                    else if (mode == AdditionMode.SecondUnit)
                    {
                        var sum = firstQuantity.Add(secondQuantity, (VolumeUnit)secondUnit);
                        ConsoleHelper.DisplayAdditionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            sum.Value,
                            sum.Unit.GetSymbol()
                        );
                    }
                    else
                    {
                        var sumInFirst = firstQuantity.Add(secondQuantity, (VolumeUnit)firstUnit);
                        var sumInSecond = firstQuantity.Add(secondQuantity, (VolumeUnit)secondUnit);

                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} + {secondQuantity}",
                            "",
                            $"In {((VolumeUnit)firstUnit).GetName()}: {sumInFirst.Value:F2} {((VolumeUnit)firstUnit).GetSymbol()}",
                            $"In {((VolumeUnit)secondUnit).GetName()}: {sumInSecond.Value:F2} {((VolumeUnit)secondUnit).GetSymbol()}",
                        };
                        ConsoleHelper.DisplayResultBox("ADDITION RESULTS", resultLines);
                    }
                }
                else if (unitType == typeof(TemperatureUnit))
                {
                    ConsoleHelper.DisplayError("Temperature does not support addition operations.");
                }
                else
                {
                    ConsoleHelper.DisplayError($"Unsupported unit type: {unitType.Name}");
                }
            }
            catch (NotSupportedException ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
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
                object firstUnit = _unitSelector(
                    $"Select unit for first {_categoryName.ToLower()}"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second quantity
                ConsoleHelper.DisplaySubHeader("SECOND QUANTITY (Subtrahend)");
                object secondUnit = _unitSelector(
                    $"Select unit for second {_categoryName.ToLower()}"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    !double.TryParse(firstInput, out double firstValue)
                    || !double.TryParse(secondInput, out double secondValue)
                )
                {
                    ConsoleHelper.DisplayError("Invalid numeric values!");
                    ConsoleHelper.WaitForKeyPress();
                    return;
                }

                Type unitType = firstUnit.GetType();

                if (unitType == typeof(LengthUnit))
                {
                    var firstQuantity = new GenericQuantity<LengthUnit>(
                        firstValue,
                        (LengthUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<LengthUnit>(
                        secondValue,
                        (LengthUnit)secondUnit
                    );

                    if (mode == SubtractionMode.FirstUnit)
                    {
                        var diff = firstQuantity.Subtract(secondQuantity);
                        ConsoleHelper.DisplaySubtractionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            diff.Value,
                            diff.Unit.GetSymbol()
                        );
                    }
                    else if (mode == SubtractionMode.SecondUnit)
                    {
                        var diff = firstQuantity.Subtract(secondQuantity, (LengthUnit)secondUnit);
                        ConsoleHelper.DisplaySubtractionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            diff.Value,
                            diff.Unit.GetSymbol()
                        );
                    }
                    else
                    {
                        var diffInFirst = firstQuantity.Subtract(
                            secondQuantity,
                            (LengthUnit)firstUnit
                        );
                        var diffInSecond = firstQuantity.Subtract(
                            secondQuantity,
                            (LengthUnit)secondUnit
                        );

                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} - {secondQuantity}",
                            "",
                            $"In {((LengthUnit)firstUnit).GetName()}: {diffInFirst.Value:F2} {((LengthUnit)firstUnit).GetSymbol()}",
                            $"In {((LengthUnit)secondUnit).GetName()}: {diffInSecond.Value:F2} {((LengthUnit)secondUnit).GetSymbol()}",
                        };
                        ConsoleHelper.DisplayResultBox("SUBTRACTION RESULTS", resultLines);
                    }
                }
                else if (unitType == typeof(WeightUnit))
                {
                    var firstQuantity = new GenericQuantity<WeightUnit>(
                        firstValue,
                        (WeightUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<WeightUnit>(
                        secondValue,
                        (WeightUnit)secondUnit
                    );

                    if (mode == SubtractionMode.FirstUnit)
                    {
                        var diff = firstQuantity.Subtract(secondQuantity);
                        ConsoleHelper.DisplaySubtractionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            diff.Value,
                            diff.Unit.GetSymbol()
                        );
                    }
                    else if (mode == SubtractionMode.SecondUnit)
                    {
                        var diff = firstQuantity.Subtract(secondQuantity, (WeightUnit)secondUnit);
                        ConsoleHelper.DisplaySubtractionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            diff.Value,
                            diff.Unit.GetSymbol()
                        );
                    }
                    else
                    {
                        var diffInFirst = firstQuantity.Subtract(
                            secondQuantity,
                            (WeightUnit)firstUnit
                        );
                        var diffInSecond = firstQuantity.Subtract(
                            secondQuantity,
                            (WeightUnit)secondUnit
                        );

                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} - {secondQuantity}",
                            "",
                            $"In {((WeightUnit)firstUnit).GetName()}: {diffInFirst.Value:F2} {((WeightUnit)firstUnit).GetSymbol()}",
                            $"In {((WeightUnit)secondUnit).GetName()}: {diffInSecond.Value:F2} {((WeightUnit)secondUnit).GetSymbol()}",
                        };
                        ConsoleHelper.DisplayResultBox("SUBTRACTION RESULTS", resultLines);
                    }
                }
                else if (unitType == typeof(VolumeUnit))
                {
                    var firstQuantity = new GenericQuantity<VolumeUnit>(
                        firstValue,
                        (VolumeUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<VolumeUnit>(
                        secondValue,
                        (VolumeUnit)secondUnit
                    );

                    if (mode == SubtractionMode.FirstUnit)
                    {
                        var diff = firstQuantity.Subtract(secondQuantity);
                        ConsoleHelper.DisplaySubtractionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            diff.Value,
                            diff.Unit.GetSymbol()
                        );
                    }
                    else if (mode == SubtractionMode.SecondUnit)
                    {
                        var diff = firstQuantity.Subtract(secondQuantity, (VolumeUnit)secondUnit);
                        ConsoleHelper.DisplaySubtractionResult(
                            firstQuantity.ToString()!,
                            secondQuantity.ToString()!,
                            diff.Value,
                            diff.Unit.GetSymbol()
                        );
                    }
                    else
                    {
                        var diffInFirst = firstQuantity.Subtract(
                            secondQuantity,
                            (VolumeUnit)firstUnit
                        );
                        var diffInSecond = firstQuantity.Subtract(
                            secondQuantity,
                            (VolumeUnit)secondUnit
                        );

                        string[] resultLines = new[]
                        {
                            $"{firstQuantity} - {secondQuantity}",
                            "",
                            $"In {((VolumeUnit)firstUnit).GetName()}: {diffInFirst.Value:F2} {((VolumeUnit)firstUnit).GetSymbol()}",
                            $"In {((VolumeUnit)secondUnit).GetName()}: {diffInSecond.Value:F2} {((VolumeUnit)secondUnit).GetSymbol()}",
                        };
                        ConsoleHelper.DisplayResultBox("SUBTRACTION RESULTS", resultLines);
                    }
                }
                else if (unitType == typeof(TemperatureUnit))
                {
                    ConsoleHelper.DisplayError(
                        "Temperature does not support subtraction operations."
                    );
                }
                else
                {
                    ConsoleHelper.DisplayError($"Unsupported unit type: {unitType.Name}");
                }
            }
            catch (NotSupportedException ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
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
                object firstUnit = _unitSelector(
                    $"Select unit for first {_categoryName.ToLower()}"
                );
                Console.Write("Enter value: ");
                string? firstInput = Console.ReadLine();

                // Second quantity
                ConsoleHelper.DisplaySubHeader("SECOND QUANTITY (Divisor)");
                object secondUnit = _unitSelector(
                    $"Select unit for second {_categoryName.ToLower()}"
                );
                Console.Write("Enter value: ");
                string? secondInput = Console.ReadLine();

                if (
                    !double.TryParse(firstInput, out double firstValue)
                    || !double.TryParse(secondInput, out double secondValue)
                )
                {
                    ConsoleHelper.DisplayError("Invalid numeric values!");
                    ConsoleHelper.WaitForKeyPress();
                    return;
                }

                Type unitType = firstUnit.GetType();

                if (unitType == typeof(LengthUnit))
                {
                    var firstQuantity = new GenericQuantity<LengthUnit>(
                        firstValue,
                        (LengthUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<LengthUnit>(
                        secondValue,
                        (LengthUnit)secondUnit
                    );

                    double ratio = firstQuantity.Divide(secondQuantity);
                    var secondInFirstUnit = secondQuantity.ConvertTo((LengthUnit)firstUnit);
                    var firstInSecondUnit = firstQuantity.ConvertTo((LengthUnit)secondUnit);

                    List<string> resultLines = new List<string>();

                    if (mode == DivisionMode.RatioOnly || mode == DivisionMode.BothUnits)
                    {
                        resultLines.Add($"{firstQuantity} ÷ {secondQuantity} = {ratio:F4}");
                        resultLines.Add("");
                        resultLines.Add(GetRatioInterpretation(ratio));
                    }

                    if (mode == DivisionMode.FirstUnit || mode == DivisionMode.BothUnits)
                    {
                        resultLines.Add("");
                        resultLines.Add($"In {((LengthUnit)firstUnit).GetName()} terms:");
                        resultLines.Add($"  {firstQuantity} ÷ {secondInFirstUnit} = {ratio:F4}");
                        resultLines.Add(
                            $"  → {firstValue:F2} {((LengthUnit)firstUnit).GetSymbol()} is {ratio:F2} times"
                        );
                        resultLines.Add(
                            $"    {secondInFirstUnit.Value:F2} {((LengthUnit)firstUnit).GetSymbol()}"
                        );
                    }

                    if (mode == DivisionMode.SecondUnit || mode == DivisionMode.BothUnits)
                    {
                        resultLines.Add("");
                        resultLines.Add($"In {((LengthUnit)secondUnit).GetName()} terms:");
                        resultLines.Add($"  {firstInSecondUnit} ÷ {secondQuantity} = {ratio:F4}");
                        resultLines.Add(
                            $"  → {firstInSecondUnit.Value:F2} {((LengthUnit)secondUnit).GetSymbol()} is {ratio:F2} times"
                        );
                        resultLines.Add(
                            $"    {secondValue:F2} {((LengthUnit)secondUnit).GetSymbol()}"
                        );
                    }

                    ConsoleHelper.DisplayResultBox("DIVISION RESULT", resultLines.ToArray());
                }
                else if (unitType == typeof(WeightUnit))
                {
                    var firstQuantity = new GenericQuantity<WeightUnit>(
                        firstValue,
                        (WeightUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<WeightUnit>(
                        secondValue,
                        (WeightUnit)secondUnit
                    );

                    double ratio = firstQuantity.Divide(secondQuantity);
                    var secondInFirstUnit = secondQuantity.ConvertTo((WeightUnit)firstUnit);
                    var firstInSecondUnit = firstQuantity.ConvertTo((WeightUnit)secondUnit);

                    List<string> resultLines = new List<string>();

                    if (mode == DivisionMode.RatioOnly || mode == DivisionMode.BothUnits)
                    {
                        resultLines.Add($"{firstQuantity} ÷ {secondQuantity} = {ratio:F4}");
                        resultLines.Add("");
                        resultLines.Add(GetRatioInterpretation(ratio));
                    }

                    if (mode == DivisionMode.FirstUnit || mode == DivisionMode.BothUnits)
                    {
                        resultLines.Add("");
                        resultLines.Add($"In {((WeightUnit)firstUnit).GetName()} terms:");
                        resultLines.Add($"  {firstQuantity} ÷ {secondInFirstUnit} = {ratio:F4}");
                        resultLines.Add(
                            $"  → {firstValue:F2} {((WeightUnit)firstUnit).GetSymbol()} is {ratio:F2} times"
                        );
                        resultLines.Add(
                            $"    {secondInFirstUnit.Value:F2} {((WeightUnit)firstUnit).GetSymbol()}"
                        );
                    }

                    if (mode == DivisionMode.SecondUnit || mode == DivisionMode.BothUnits)
                    {
                        resultLines.Add("");
                        resultLines.Add($"In {((WeightUnit)secondUnit).GetName()} terms:");
                        resultLines.Add($"  {firstInSecondUnit} ÷ {secondQuantity} = {ratio:F4}");
                        resultLines.Add(
                            $"  → {firstInSecondUnit.Value:F2} {((WeightUnit)secondUnit).GetSymbol()} is {ratio:F2} times"
                        );
                        resultLines.Add(
                            $"    {secondValue:F2} {((WeightUnit)secondUnit).GetSymbol()}"
                        );
                    }

                    ConsoleHelper.DisplayResultBox("DIVISION RESULT", resultLines.ToArray());
                }
                else if (unitType == typeof(VolumeUnit))
                {
                    var firstQuantity = new GenericQuantity<VolumeUnit>(
                        firstValue,
                        (VolumeUnit)firstUnit
                    );
                    var secondQuantity = new GenericQuantity<VolumeUnit>(
                        secondValue,
                        (VolumeUnit)secondUnit
                    );

                    double ratio = firstQuantity.Divide(secondQuantity);
                    var secondInFirstUnit = secondQuantity.ConvertTo((VolumeUnit)firstUnit);
                    var firstInSecondUnit = firstQuantity.ConvertTo((VolumeUnit)secondUnit);

                    List<string> resultLines = new List<string>();

                    if (mode == DivisionMode.RatioOnly || mode == DivisionMode.BothUnits)
                    {
                        resultLines.Add($"{firstQuantity} ÷ {secondQuantity} = {ratio:F4}");
                        resultLines.Add("");
                        resultLines.Add(GetRatioInterpretation(ratio));
                    }

                    if (mode == DivisionMode.FirstUnit || mode == DivisionMode.BothUnits)
                    {
                        resultLines.Add("");
                        resultLines.Add($"In {((VolumeUnit)firstUnit).GetName()} terms:");
                        resultLines.Add($"  {firstQuantity} ÷ {secondInFirstUnit} = {ratio:F4}");
                        resultLines.Add(
                            $"  → {firstValue:F2} {((VolumeUnit)firstUnit).GetSymbol()} is {ratio:F2} times"
                        );
                        resultLines.Add(
                            $"    {secondInFirstUnit.Value:F2} {((VolumeUnit)firstUnit).GetSymbol()}"
                        );
                    }

                    if (mode == DivisionMode.SecondUnit || mode == DivisionMode.BothUnits)
                    {
                        resultLines.Add("");
                        resultLines.Add($"In {((VolumeUnit)secondUnit).GetName()} terms:");
                        resultLines.Add($"  {firstInSecondUnit} ÷ {secondQuantity} = {ratio:F4}");
                        resultLines.Add(
                            $"  → {firstInSecondUnit.Value:F2} {((VolumeUnit)secondUnit).GetSymbol()} is {ratio:F2} times"
                        );
                        resultLines.Add(
                            $"    {secondValue:F2} {((VolumeUnit)secondUnit).GetSymbol()}"
                        );
                    }

                    ConsoleHelper.DisplayResultBox("DIVISION RESULT", resultLines.ToArray());
                }
                else if (unitType == typeof(TemperatureUnit))
                {
                    ConsoleHelper.DisplayError("Temperature does not support division operations.");
                }
                else
                {
                    ConsoleHelper.DisplayError($"Unsupported unit type: {unitType.Name}");
                }
            }
            catch (DivideByZeroException)
            {
                ConsoleHelper.DisplayError("Division by zero is not allowed!");
            }
            catch (NotSupportedException ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
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
    }
}
