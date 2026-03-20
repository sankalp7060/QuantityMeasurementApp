using QuantityMeasurementApp.UI.Menus;

namespace QuantityMeasurementApp
{
    /// <summary>
    /// Main program class - entry point of the application.
    /// Initializes and displays the main menu.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            // Create and display the main menu
            MainMenu menu = new MainMenu();
            menu.Display();
        }
    }
}
