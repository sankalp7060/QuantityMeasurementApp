using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Menu for measurement comparison operations (UC1-UC4).
    /// </summary>
    public class ComparisonMenu
    {
        private readonly QuantityMeasurementService _measurementService;

        /// <summary>
        /// Initializes a new instance of the ComparisonMenu class.
        /// </summary>
        /// <param name="measurementService">The measurement service.</param>
        public ComparisonMenu(QuantityMeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        /// <summary>
        /// Displays the comparison menu.
        /// </summary>
        public void Display()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplaySubHeader("MEASUREMENT COMPARISON");

            try
            {
                // First measurement
                Console.WriteLine("\n--- FIRST MEASUREMENT ---");
                LengthUnit firstUnit = UnitSelector.SelectUnit("Select unit for first measurement");
                string? firstInput = ConsoleHelper.GetInput(
                    $"Enter value in {firstUnit.GetName()}"
                );

                // Second measurement
                Console.WriteLine("\n--- SECOND MEASUREMENT ---");
                LengthUnit secondUnit = UnitSelector.SelectUnit(
                    "Select unit for second measurement"
                );
                string? secondInput = ConsoleHelper.GetInput(
                    $"Enter value in {secondUnit.GetName()}"
                );

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new Quantity(firstValue, firstUnit);
                    var secondQuantity = new Quantity(secondValue, secondUnit);

                    bool areQuantitiesEqual = _measurementService.AreQuantitiesEqual(
                        firstQuantity,
                        secondQuantity
                    );

                    Console.WriteLine(
                        $"\n{firstQuantity} vs {secondQuantity}: {(areQuantitiesEqual ? "✅ EQUAL" : "❌ NOT EQUAL")}"
                    );

                    // Show in base unit for reference
                    Quantity firstQuantityInFeet = firstQuantity.ConvertTo(LengthUnit.FEET);
                    Quantity secondQuantityInFeet = secondQuantity.ConvertTo(LengthUnit.FEET);

                    Console.WriteLine($"\n📊 In base unit (feet):");
                    Console.WriteLine($"   First:  {firstQuantityInFeet.Value:F6} ft");
                    Console.WriteLine($"   Second: {secondQuantityInFeet.Value:F6} ft");

                    if (!areQuantitiesEqual)
                    {
                        double difference = Math.Abs(
                            firstQuantityInFeet.Value - secondQuantityInFeet.Value
                        );
                        Console.WriteLine($"   Difference: {difference:F6} ft");
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

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
