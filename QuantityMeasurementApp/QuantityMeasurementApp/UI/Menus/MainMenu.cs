using QuantityMeasurementApp.Services;
using QuantityMeasurementApp.UI.Helpers;
using QuantityMeasurementApp.UI.Menus;

namespace QuantityMeasurementApp.UI.Menus
{
    /// <summary>
    /// Main menu of the application.
    /// Provides access to all features using generic implementations.
    /// UC11: Added Volume measurements to the menu.
    /// </summary>
    public class MainMenu
    {
        private readonly GenericMeasurementService _measurementService;
        private readonly GenericLengthMenu _lengthMenu;
        private readonly GenericWeightMenu _weightMenu;
        private readonly GenericVolumeMenu _volumeMenu;

        /// <summary>
        /// Initializes a new instance of the MainMenu class.
        /// </summary>
        public MainMenu()
        {
            _measurementService = new GenericMeasurementService();
            _lengthMenu = new GenericLengthMenu(_measurementService);
            _weightMenu = new GenericWeightMenu(_measurementService);
            _volumeMenu = new GenericVolumeMenu(_measurementService);
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

                if (userChoice == "4")
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
            Console.WriteLine("║                         MAIN MENU                      ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                        ║");
            Console.WriteLine("║    1.  Length Measurements (ft, in, yd, cm)            ║");
            Console.WriteLine("║    2.  Weight Measurements (kg, g, lb)                 ║");
            Console.WriteLine("║    3.  Volume Measurements (L, mL, gal)                ║");
            Console.WriteLine("║    4.  Exit                                            ║");
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
                case "3":
                    _volumeMenu.Display();
                    break;
                default:
                    ConsoleHelper.DisplayError("Invalid choice!");
                    break;
            }
        }
    }
}
