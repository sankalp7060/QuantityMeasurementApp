using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Menu for unit conversion operations (UC5).
    /// </summary>
    public class ConversionMenu
    {
        private readonly QuantityMeasurementService _measurementService;

        /// <summary>
        /// Initializes a new instance of the ConversionMenu class.
        /// </summary>
        /// <param name="measurementService">The measurement service.</param>
        public ConversionMenu(QuantityMeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        /// <summary>
        /// Displays the conversion menu.
        /// </summary>
        public void Display()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplaySubHeader("UNIT CONVERSION");

            try
            {
                LengthUnit sourceUnit = UnitSelector.SelectUnit("Select SOURCE unit");
                LengthUnit targetUnit = UnitSelector.SelectUnit("Select TARGET unit");

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

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(
                        $"\n✅ {inputValue} {sourceUnit.GetSymbol()} = {convertedValue:F6} {targetUnit.GetSymbol()}"
                    );
                    Console.ResetColor();

                    ShowConversionFormula(inputValue, sourceUnit, targetUnit, convertedValue);
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

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ShowConversionFormula(
            double inputValue,
            LengthUnit sourceUnit,
            LengthUnit targetUnit,
            double convertedValue
        )
        {
            double sourceToFeet = sourceUnit.GetConversionFactor();
            double targetToFeet = targetUnit.GetConversionFactor();

            Console.WriteLine("\n📊 Conversion Formula:");
            Console.WriteLine(
                $"   {inputValue} {sourceUnit.GetSymbol()} × ({sourceToFeet:F6} / {targetToFeet:F6}) = {convertedValue:F6} {targetUnit.GetSymbol()}"
            );
        }
    }
}
