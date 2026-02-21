using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;
using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Menu for arithmetic operations (UC6-UC7).
    /// </summary>
    public class ArithmeticMenu
    {
        private readonly QuantityMeasurementService _measurementService;

        /// <summary>
        /// Initializes a new instance of the ArithmeticMenu class.
        /// </summary>
        /// <param name="measurementService">The measurement service.</param>
        public ArithmeticMenu(QuantityMeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        /// <summary>
        /// Displays the arithmetic menu.
        /// </summary>
        public void Display()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                ConsoleHelper.DisplaySubHeader("MEASUREMENT ADDITION");

                DisplayAttractiveMenu();

                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                switch (userChoice)
                {
                    case "1":
                        DisplayAdditionInFirstUnit();
                        break;
                    case "2":
                        DisplayAdditionInSecondUnit();
                        break;
                    case "3":
                        DisplayAdditionInBothUnits();
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

        private void DisplayAttractiveMenu()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                   ADDITION OPTIONS                    ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    1.  Result in FIRST unit                           ║");
            Console.WriteLine("║        (e.g., 1 ft + 12 in = 2 ft)                    ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    2.  Result in SECOND unit                          ║");
            Console.WriteLine("║        (e.g., 1 ft + 12 in = 24 in)                   ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    3.  Results in BOTH units                          ║");
            Console.WriteLine("║        (Compare both results)                         ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    4.  Back to Main Menu                              ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        }

        private void DisplayAdditionInFirstUnit()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ADDITION - RESULT IN FIRST UNIT",
                "1 ft + 12 in = 2 ft"
            );

            try
            {
                // Get first measurement
                Console.WriteLine("\n┌────────── FIRST MEASUREMENT ──────────┐");
                LengthUnit firstUnit = UnitSelector.SelectUnit("Select unit for first measurement");
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└────────────────────────────────────────┘");

                // Get second measurement
                Console.WriteLine("\n┌────────── SECOND MEASUREMENT ─────────┐");
                LengthUnit secondUnit = UnitSelector.SelectUnit(
                    "Select unit for second measurement"
                );
                Console.Write("│ Enter value: ");
                string? secondInput = Console.ReadLine();
                Console.WriteLine("└────────────────────────────────────────┘");

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new Quantity(firstValue, firstUnit);
                    var secondQuantity = new Quantity(secondValue, secondUnit);

                    var sumInFirstUnit = _measurementService.AddQuantities(
                        firstQuantity,
                        secondQuantity
                    );

                    DisplayResultBox(firstQuantity, secondQuantity, sumInFirstUnit);
                    ShowDetailedCalculation(
                        firstQuantity,
                        secondQuantity,
                        sumInFirstUnit.Unit,
                        sumInFirstUnit
                    );
                }
                else
                {
                    ConsoleHelper.DisplayError("❌ Invalid numeric values!");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"❌ Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
        }

        private void DisplayAdditionInSecondUnit()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ADDITION - RESULT IN SECOND UNIT",
                "1 ft + 12 in = 24 in"
            );

            try
            {
                // Get first measurement
                Console.WriteLine("\n┌────────── FIRST MEASUREMENT ──────────┐");
                LengthUnit firstUnit = UnitSelector.SelectUnit("Select unit for first measurement");
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└────────────────────────────────────────┘");

                // Get second measurement
                Console.WriteLine("\n┌────────── SECOND MEASUREMENT ─────────┐");
                LengthUnit secondUnit = UnitSelector.SelectUnit(
                    "Select unit for second measurement"
                );
                Console.Write("│ Enter value: ");
                string? secondInput = Console.ReadLine();
                Console.WriteLine("└────────────────────────────────────────┘");

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new Quantity(firstValue, firstUnit);
                    var secondQuantity = new Quantity(secondValue, secondUnit);

                    // Add with result in second unit
                    var sumInSecondUnit = _measurementService.AddQuantitiesWithTarget(
                        firstQuantity,
                        secondQuantity,
                        secondUnit
                    );

                    DisplayResultBox(firstQuantity, secondQuantity, sumInSecondUnit);
                    ShowDetailedCalculation(
                        firstQuantity,
                        secondQuantity,
                        secondUnit,
                        sumInSecondUnit
                    );
                }
                else
                {
                    ConsoleHelper.DisplayError("❌ Invalid numeric values!");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"❌ Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
        }

        private void DisplayAdditionInBothUnits()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplayAttributedHeader(
                "ADDITION - RESULTS IN BOTH UNITS",
                "Compare results"
            );

            try
            {
                // Get first measurement
                Console.WriteLine("\n┌────────── FIRST MEASUREMENT ──────────┐");
                LengthUnit firstUnit = UnitSelector.SelectUnit("Select unit for first measurement");
                Console.Write("│ Enter value: ");
                string? firstInput = Console.ReadLine();
                Console.WriteLine("└────────────────────────────────────────┘");

                // Get second measurement
                Console.WriteLine("\n┌────────── SECOND MEASUREMENT ─────────┐");
                LengthUnit secondUnit = UnitSelector.SelectUnit(
                    "Select unit for second measurement"
                );
                Console.Write("│ Enter value: ");
                string? secondInput = Console.ReadLine();
                Console.WriteLine("└────────────────────────────────────────┘");

                if (
                    double.TryParse(firstInput, out double firstValue)
                    && double.TryParse(secondInput, out double secondValue)
                )
                {
                    var firstQuantity = new Quantity(firstValue, firstUnit);
                    var secondQuantity = new Quantity(secondValue, secondUnit);

                    // Calculate in both units
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

                    DisplayComparisonBox(
                        firstQuantity,
                        secondQuantity,
                        sumInFirstUnit,
                        sumInSecondUnit
                    );
                    ShowDetailedCalculation(
                        firstQuantity,
                        secondQuantity,
                        firstUnit,
                        sumInFirstUnit
                    );
                }
                else
                {
                    ConsoleHelper.DisplayError("❌ Invalid numeric values!");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError($"❌ Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKeyPress();
        }

        private void DisplayResultBox(
            Quantity firstQuantity,
            Quantity secondQuantity,
            Quantity sumQuantity
        )
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║           ADDITION RESULT             ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine($"║  {firstQuantity, -8} + {secondQuantity, -8}          ║");
            Console.WriteLine("║                                        ║");
            Console.WriteLine(
                $"║  = {sumQuantity.Value, 10:F6} {sumQuantity.Unit.GetSymbol(), -3}               ║"
            );
            Console.WriteLine("╚════════════════════════════════════════╝");
        }

        private void DisplayComparisonBox(
            Quantity firstQuantity,
            Quantity secondQuantity,
            Quantity sumInFirstUnit,
            Quantity sumInSecondUnit
        )
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         COMPARISON RESULTS             ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine($"║  {firstQuantity, -8} + {secondQuantity, -8}          ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine(
                $"║  In {sumInFirstUnit.Unit.GetName(), -7}: {sumInFirstUnit.Value, 10:F6} {sumInFirstUnit.Unit.GetSymbol(), -3}  ║"
            );
            Console.WriteLine(
                $"║  In {sumInSecondUnit.Unit.GetName(), -6}: {sumInSecondUnit.Value, 10:F6} {sumInSecondUnit.Unit.GetSymbol(), -3}  ║"
            );
            Console.WriteLine("╚════════════════════════════════════════╝");
        }

        private void ShowDetailedCalculation(
            Quantity firstQuantity,
            Quantity secondQuantity,
            LengthUnit resultUnit,
            Quantity sumQuantity
        )
        {
            Quantity firstInFeet = firstQuantity.ConvertTo(LengthUnit.FEET);
            Quantity secondInFeet = secondQuantity.ConvertTo(LengthUnit.FEET);
            double totalInFeet = firstInFeet.Value + secondInFeet.Value;

            Console.WriteLine("\n┌────────── CALCULATION DETAILS ──────────┐");
            Console.WriteLine("│  Step 1: Convert to base unit (feet)   │");
            Console.WriteLine($"│    {firstQuantity} = {firstInFeet.Value, 8:F6} ft           │");
            Console.WriteLine($"│    {secondQuantity} = {secondInFeet.Value, 8:F6} ft           │");
            Console.WriteLine("│                                          │");
            Console.WriteLine("│  Step 2: Add in feet                    │");
            Console.WriteLine(
                $"│    {firstInFeet.Value:F6} + {secondInFeet.Value:F6} = {totalInFeet:F6} ft   │"
            );
            Console.WriteLine("│                                          │");
            Console.WriteLine("│  Step 3: Convert to target unit         │");
            Console.WriteLine(
                $"│    {totalInFeet:F6} ft = {sumQuantity.Value:F6} {resultUnit.GetSymbol()}         │"
            );
            Console.WriteLine("└──────────────────────────────────────────┘");
        }
    }
}
