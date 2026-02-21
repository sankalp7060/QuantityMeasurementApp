using QuantityMeasurementApp.Views;

namespace QuantityMeasurementApp
{
    /// <summary>
    /// Main program class - entry point of the application.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            // Create and display the menu
            Menu menu = new Menu();
            menu.Display();
        }
    }
}
