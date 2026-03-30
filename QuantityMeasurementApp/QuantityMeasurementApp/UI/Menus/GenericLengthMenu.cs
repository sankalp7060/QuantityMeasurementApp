using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Menu for length measurement operations using generic Quantity class.
    /// UC10: Consolidated menu for all length operations.
    /// UC12: Updated to use GenericArithmeticMenu for arithmetic operations.
    /// </summary>
    public class GenericLengthMenu
    {
        private readonly GenericMeasurementService _measurementService;
        private readonly GenericArithmeticMenu _arithmeticMenu;

        /// <summary>
        /// Initializes a new instance of the GenericLengthMenu class.
        /// </summary>
        /// <param name="measurementService">The measurement service.</param>
        public GenericLengthMenu(GenericMeasurementService measurementService)
        {
            _measurementService = measurementService;
            _arithmeticMenu = new GenericArithmeticMenu(
                measurementService,
                "LENGTH",
                GenericUnitSelector.SelectLengthUnit,
                LengthUnit.GetAllUnits()
            );
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
                        _arithmeticMenu.Display();
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

        private void DisplayMainLengthMenu()
        {
            ConsoleHelper.DisplayAttributedHeader("LENGTH MEASUREMENTS", "ft, in, yd, cm");

            string[] menuOptions = new[]
            {
                "1.  Convert Length Units",
                "",
                "2.  Compare Lengths",
                "",
                "3.  Arithmetic Operations",
                "    (Add, Subtract, Divide)",
                "",
                "4.  Back to Main Menu",
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
    }
}
