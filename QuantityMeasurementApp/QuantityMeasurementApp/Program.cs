using QuantityMeasurementApp.Views;

namespace QuantityMeasurementApp
{
    // Main program class - now only responsible for launching the application
    class Program
    {
        // Entry point of the application
        static void Main(string[] args)
        {
            // Create and display the menu view
            Menu menu = new Menu();
            menu.Display();
        }
    }
}
