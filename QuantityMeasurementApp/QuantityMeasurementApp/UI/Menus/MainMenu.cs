using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Main menu of the application.
    /// Provides access to all features including new weight measurements.
    /// </summary>
    public class MainMenu
    {
        private readonly QuantityMeasurementService _measurementService;
        private readonly ConversionMenu _conversionMenu;
        private readonly ComparisonMenu _comparisonMenu;
        private readonly ArithmeticMenu _arithmeticMenu;
        private readonly WeightMenu _weightMenu;

        /// <summary>
        /// Initializes a new instance of the MainMenu class.
        /// </summary>
        public MainMenu()
        {
            _measurementService = new QuantityMeasurementService();
            _conversionMenu = new ConversionMenu(_measurementService);
            _comparisonMenu = new ComparisonMenu(_measurementService);
            _arithmeticMenu = new ArithmeticMenu(_measurementService);
            _weightMenu = new WeightMenu();
        }

        /// <summary>
        /// Displays the main menu and handles user interaction.
        /// </summary>
        public void Display()
        {
            ConsoleHelper.DisplayHeader("QUANTITY MEASUREMENT APPLICATION");

            while (true)
            {
                DisplayOptions();
                string? userChoice = ConsoleHelper.GetInput("Enter your choice");

                if (userChoice == "6")
                    break;

                ProcessUserChoice(userChoice);
            }

            ConsoleHelper.DisplayMessage(
                "Thank you for using Quantity Measurement Application!",
                ConsoleColor.Green
            );
        }

        private void DisplayOptions()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                       MAIN MENU                        ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    1.  Length Measurements (ft, in, yd, cm)            ║");
            Console.WriteLine("║    2.  Weight Measurements (kg, g, lb)                 ║");
            Console.WriteLine("║    3.  Compare Measurements                            ║");
            Console.WriteLine("║    4.  Add Measurements                                ║");
            Console.WriteLine("║    5.  Legacy Mode (Original Classes)                  ║");
            Console.WriteLine("║    6.  Exit                                            ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        }

        private void ProcessUserChoice(string? userChoice)
        {
            switch (userChoice)
            {
                case "1":
                    _conversionMenu.Display();
                    break;
                case "2":
                    _weightMenu.Display();
                    break;
                case "3":
                    _comparisonMenu.Display();
                    break;
                case "4":
                    _arithmeticMenu.Display();
                    break;
                case "5":
                    DisplayLegacyMenu();
                    break;
                default:
                    ConsoleHelper.DisplayError("Invalid choice!");
                    break;
            }
        }

        private void DisplayLegacyMenu()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplaySubHeader("LEGACY MODE");

            var options = new[] { "1. Compare Feet", "2. Compare Inches", "3. Back" };
            ConsoleHelper.DisplayMenu(options);

            string? userChoice = ConsoleHelper.GetInput("Enter your choice");

            if (userChoice == "1")
                CompareLegacyFeet();
            else if (userChoice == "2")
                CompareLegacyInches();
        }

        private void CompareLegacyFeet()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplaySubHeader("COMPARE FEET (LEGACY)");

            string? firstInput = ConsoleHelper.GetInput("Enter first measurement in feet");
            string? secondInput = ConsoleHelper.GetInput("Enter second measurement in feet");

            var firstFeet = _measurementService.CreateFeetFromString(firstInput);
            var secondFeet = _measurementService.CreateFeetFromString(secondInput);

            if (firstFeet == null || secondFeet == null)
            {
                ConsoleHelper.DisplayError("Invalid input! Press any key to continue...");
                Console.ReadKey();
                return;
            }

            bool areEqual = _measurementService.AreFeetEqual(firstFeet, secondFeet);

            Console.WriteLine(
                $"\n{firstFeet} vs {secondFeet}: {(areEqual ? "✅ EQUAL" : "❌ NOT EQUAL")}"
            );

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void CompareLegacyInches()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.DisplaySubHeader("COMPARE INCHES (LEGACY)");

            string? firstInput = ConsoleHelper.GetInput("Enter first measurement in inches");
            string? secondInput = ConsoleHelper.GetInput("Enter second measurement in inches");

            var firstInch = _measurementService.CreateInchFromString(firstInput);
            var secondInch = _measurementService.CreateInchFromString(secondInput);

            if (firstInch == null || secondInch == null)
            {
                ConsoleHelper.DisplayError("Invalid input! Press any key to continue...");
                Console.ReadKey();
                return;
            }

            bool areEqual = _measurementService.AreInchesEqual(firstInch, secondInch);

            Console.WriteLine(
                $"\n{firstInch} vs {secondInch}: {(areEqual ? "✅ EQUAL" : "❌ NOT EQUAL")}"
            );

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
