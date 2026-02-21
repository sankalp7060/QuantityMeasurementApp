using QuantityMeasurementApp.Domain.Quantities;
using QuantityMeasurementApp.Domain.Units;

namespace QuantityMeasurementApp.UI.Helpers
{
    /// <summary>
    /// Helper class for console operations.
    /// Provides consistent UI formatting.
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Clears the console screen.
        /// </summary>
        public static void ClearScreen()
        {
            Console.Clear();
        }

        /// <summary>
        /// Waits for user to press any key.
        /// </summary>
        public static void WaitForKeyPress()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays a header with box formatting.
        /// </summary>
        /// <param name="title">The header title.</param>
        public static void DisplayHeader(string title)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine($"║     {title, -28} ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");
        }

        /// <summary>
        /// Displays an attributed header with example.
        /// </summary>
        /// <param name="title">The header title.</param>
        /// <param name="example">The example text.</param>
        public static void DisplayAttributedHeader(string title, string example)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine($"║  {title, -34} ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine($"║  Example: {example, -30} ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");
        }

        /// <summary>
        /// Displays a sub-header.
        /// </summary>
        /// <param name="title">The sub-header title.</param>
        public static void DisplaySubHeader(string title)
        {
            Console.WriteLine($"\n┌───── {title} ─────┐\n");
        }

        /// <summary>
        /// Displays a menu with options.
        /// </summary>
        /// <param name="options">Array of menu options.</param>
        public static void DisplayMenu(string[] options)
        {
            Console.WriteLine("┌────────────────────────────────────┐");
            foreach (var option in options)
            {
                Console.WriteLine($"│ {option, -35} │");
            }
            Console.WriteLine("└────────────────────────────────────┘");
        }

        /// <summary>
        /// Gets user input with a prompt.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <returns>The user's input.</returns>
        public static string? GetInput(string prompt)
        {
            Console.Write($"{prompt}: ");
            return Console.ReadLine();
        }

        /// <summary>
        /// Displays a success message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void DisplaySuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Displays a generic message with color.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="color">The color to use.</param>
        public static void DisplayMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
