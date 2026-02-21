using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Main menu of the application.
    /// Provides access to all features using generic implementations.
    /// UC10: Simplified with only generic menus.
    /// </summary>
    public class MainMenu
    {
        private readonly GenericMeasurementService _measurementService;
        private readonly GenericLengthMenu _lengthMenu;
        private readonly GenericWeightMenu _weightMenu;

        /// <summary>
        /// Initializes a new instance of the MainMenu class.
        /// </summary>
        public MainMenu()
        {
            _measurementService = new GenericMeasurementService();
            _lengthMenu = new GenericLengthMenu(_measurementService);
            _weightMenu = new GenericWeightMenu(_measurementService);
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

                if (userChoice == "3")
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
            Console.WriteLine("║                    MAIN MENU                          ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    1.  Length Measurements (ft, in, yd, cm)           ║");
            Console.WriteLine("║    2.  Weight Measurements (kg, g, lb)                ║");
            Console.WriteLine("║    3.  Exit                                           ║");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        }

        private void ProcessUserChoice(string? userChoice)
        {
            switch (userChoice)
            {
                case "1":
                    _lengthMenu.Display();
                    break;
                case "2":
                    _weightMenu.Display();
                    break;
                default:
                    ConsoleHelper.DisplayError("Invalid choice!");
                    break;
            }
        }
    }
}
