using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using QuantityMeasurementConsole.Factory;
using QuantityMeasurementConsole.Menus;

namespace QuantityMeasurementConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Quantity Measurement System";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         QUANTITY MEASUREMENT SYSTEM                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            try
            {
                // Setup logging (only once)
                using var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                    builder.SetMinimumLevel(LogLevel.Information);
                });

                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogInformation("Starting Quantity Measurement Console Application");

                // Create factory instance (non-static)
                var serviceFactory = new ServiceFactory(loggerFactory);

                // Create and display main menu (menu is static UI, so we pass the factory)
                var menu = new MainConsoleMenu(loggerFactory, serviceFactory);
                menu.Display();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("╔════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    SYSTEM ERROR                       ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════╣");
                Console.WriteLine($"║ {ex.Message, -52} ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
