using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.UI.Helpers;
using QuantityMeasurementApp.UI.Menus;
using QuantityMeasurementConsole.Factory;

namespace QuantityMeasurementConsole.Menus
{
    /// <summary>
    /// Main console menu for the Quantity Measurement System
    /// Menu itself is static UI, but uses non-static factory
    /// </summary>
    public class MainConsoleMenu
    {
        private readonly ILogger<MainConsoleMenu> _logger;
        private readonly MeasurementMenu _measurementMenu;
        private readonly MainMenu _originalMainMenu;

        public MainConsoleMenu(ILoggerFactory loggerFactory, ServiceFactory serviceFactory)
        {
            _logger = loggerFactory.CreateLogger<MainConsoleMenu>();
            _measurementMenu = new MeasurementMenu(loggerFactory, serviceFactory);
            _originalMainMenu = new MainMenu(); // This is the original UC1-14 menu
        }

        public void Display()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();

                Console.WriteLine("╔════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    MAIN MENU                          ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════╣");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("║    1.  Classic Measurement System                     ║");
                Console.WriteLine("║    2.  Advanced Measurement System                    ║");
                Console.WriteLine("║    3.  Exit                                           ║");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════╝");

                Console.Write("\nSelect an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _logger.LogInformation("Launching classic measurement system");
                        _originalMainMenu.Display();
                        break;
                    case "2":
                        _logger.LogInformation("Launching advanced measurement system");
                        _measurementMenu.Display();
                        break;
                    case "3":
                        _logger.LogInformation("Exiting application");
                        Console.WriteLine("\nThank you for using Quantity Measurement System!");
                        return;
                    default:
                        Console.WriteLine("\n❌ Invalid option. Please try again.");
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         QUANTITY MEASUREMENT SYSTEM                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
