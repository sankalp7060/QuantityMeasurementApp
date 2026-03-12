using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.UI.Helpers;
using QuantityMeasurementApp.UI.Menus;
using QuantityMeasurementConsole.Factory;

namespace QuantityMeasurementConsole.Menus
{
    public class MainConsoleMenu
    {
        private readonly ILogger<MainConsoleMenu> _logger;
        private readonly MeasurementMenu _measurementMenu;
        private readonly MainMenu _originalMainMenu;

        public MainConsoleMenu(ILoggerFactory loggerFactory, ServiceFactory serviceFactory)
        {
            _logger = loggerFactory.CreateLogger<MainConsoleMenu>();
            _measurementMenu = new MeasurementMenu(loggerFactory, serviceFactory);
            _originalMainMenu = new MainMenu();
        }

        public void Display()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();

                Console.WriteLine("╔════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                      MAIN MENU                         ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════╣");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("║    1.  Basic Calculator                                ║");
                Console.WriteLine("║    2.  Advanced Operations                             ║");
                Console.WriteLine("║    3.  Exit                                            ║");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════╝");

                Console.Write("\nYour choice: ");
                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        _originalMainMenu.Display();
                        break;
                    case "2":
                        _measurementMenu.Display();
                        break;
                    case "3":
                        Console.WriteLine("\nThank you for using Quantity Measurement System!");
                        return;
                    default:
                        Console.WriteLine("\n Invalid option. Please enter 1, 2, or 3.");
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
            Console.WriteLine("║             QUANTITY MEASUREMENT SYSTEM                ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
