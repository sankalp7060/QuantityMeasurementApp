using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using QuantityMeasurementConsole.Factory;
using QuantityMeasurementConsole.Menus;

namespace QuantityMeasurementConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Quantity Measurement System";

            // Welcome Header
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║               QUANTITY MEASUREMENT SYSTEM              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            try
            {
                // Setup logging (hidden from user)
                using var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                    builder.SetMinimumLevel(LogLevel.Warning); // Only show warnings and errors
                });

                // Initialize system
                Console.WriteLine("Initializing system...");
                var serviceFactory = new ServiceFactory(loggerFactory);

                // Quick check (hidden message)
                var stats = await serviceFactory.GetRepository().GetRepositoryStatisticsAsync();

                Console.Clear();

                // Show main menu
                var menu = new MainConsoleMenu(loggerFactory, serviceFactory);
                menu.Display();

                // Cleanup
                var repo = serviceFactory.GetRepository();
                await repo.ReleaseResourcesAsync();
            }
            catch (Exception ex)
            {
                Console.Clear();
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
