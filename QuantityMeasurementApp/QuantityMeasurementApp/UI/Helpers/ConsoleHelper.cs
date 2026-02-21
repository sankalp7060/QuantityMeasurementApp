using QuantityMeasurementApp.Core.Abstractions;
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
            int width = 60;
            string border = new string('═', width - 2);

            Console.WriteLine($"╔{border}╗");
            Console.WriteLine($"║{CenterText(title, width - 2)}║");
            Console.WriteLine($"╚{border}╝\n");
        }

        /// <summary>
        /// Displays an attributed header with example.
        /// </summary>
        /// <param name="title">The header title.</param>
        /// <param name="example">The example text.</param>
        public static void DisplayAttributedHeader(string title, string example)
        {
            int width = 60;
            string border = new string('═', width - 2);

            Console.WriteLine($"╔{border}╗");
            Console.WriteLine($"║{CenterText(title, width - 2)}║");
            Console.WriteLine($"╠{new string('═', width - 2)}╣");
            Console.WriteLine($"║  Example: {example, -52} ║");
            Console.WriteLine($"╚{border}╝\n");
        }

        /// <summary>
        /// Displays a sub-header.
        /// </summary>
        /// <param name="title">The sub-header title.</param>
        public static void DisplaySubHeader(string title)
        {
            int width = 60;
            string border = new string('─', width - 2);

            Console.WriteLine($"┌{border}┐");
            Console.WriteLine($"│{CenterText(title, width - 2)}│");
            Console.WriteLine($"└{border}┘\n");
        }

        /// <summary>
        /// Displays a menu with options.
        /// </summary>
        /// <param name="options">Array of menu options.</param>
        public static void DisplayMenu(string[] options)
        {
            int width = 60;
            string border = new string('─', width - 2);

            Console.WriteLine($"┌{border}┐");
            foreach (var option in options)
            {
                Console.WriteLine($"│  {option, -56} │");
            }
            Console.WriteLine($"└{border}┘");
        }

        /// <summary>
        /// Displays an information box.
        /// </summary>
        /// <param name="lines">Array of text lines.</param>
        public static void DisplayInfoBox(string[] lines)
        {
            int width = 60;
            string border = new string('─', width - 2);

            Console.WriteLine($"┌{border}┐");
            foreach (var line in lines)
            {
                Console.WriteLine($"│  {line, -56} │");
            }
            Console.WriteLine($"└{border}┘");
        }

        /// <summary>
        /// Displays a result box with title and content.
        /// </summary>
        /// <param name="title">The box title.</param>
        /// <param name="content">Array of content lines.</param>
        public static void DisplayResultBox(string title, string[] content)
        {
            int width = 60;
            string border = new string('═', width - 2);

            Console.WriteLine($"╔{border}╗");
            Console.WriteLine($"║{CenterText(title, width - 2)}║");
            Console.WriteLine($"╠{border}╣");

            foreach (var line in content)
            {
                Console.WriteLine($"║  {line, -56} ║");
            }

            Console.WriteLine($"╚{border}╝");
        }

        /// <summary>
        /// Displays a comparison result.
        /// </summary>
        /// <param name="firstQuantity">First quantity string.</param>
        /// <param name="secondQuantity">Second quantity string.</param>
        /// <param name="areEqual">Whether they are equal.</param>
        /// <param name="firstInBase">First quantity in base unit.</param>
        /// <param name="secondInBase">Second quantity in base unit.</param>
        public static void DisplayComparisonResult(
            string firstQuantity,
            string secondQuantity,
            bool areEqual,
            double firstInBase,
            double secondInBase,
            string baseUnit
        )
        {
            int width = 60;
            string border = new string('═', width - 2);

            Console.WriteLine($"\n╔{border}╗");
            Console.WriteLine($"║{CenterText("COMPARISON RESULT", width - 2)}║");
            Console.WriteLine($"╠{border}╣");
            Console.WriteLine($"║  {firstQuantity, -20} vs {secondQuantity, -20}  ║");
            Console.WriteLine($"╠{border}╣");

            if (areEqual)
            {
                Console.WriteLine($"║  ✅ Measurements are EQUAL{new string(' ', 31)}║");
            }
            else
            {
                Console.WriteLine($"║  ❌ Measurements are NOT EQUAL{new string(' ', 28)}║");
            }

            Console.WriteLine($"╠{border}╣");
            Console.WriteLine($"║  In {baseUnit}:{new string(' ', 48 - baseUnit.Length)}║");
            Console.WriteLine(
                $"║    First:  {firstInBase, 12:F6} {baseUnit, -3}{new string(' ', 26)}║"
            );
            Console.WriteLine(
                $"║    Second: {secondInBase, 12:F6} {baseUnit, -3}{new string(' ', 26)}║"
            );
            Console.WriteLine($"╚{border}╝");
        }

        /// <summary>
        /// Displays a conversion result.
        /// </summary>
        /// <param name="inputValue">The input value.</param>
        /// <param name="sourceSymbol">Source unit symbol.</param>
        /// <param name="resultValue">The converted value.</param>
        /// <param name="targetSymbol">Target unit symbol.</param>
        public static void DisplayConversionResult(
            double inputValue,
            string sourceSymbol,
            double resultValue,
            string targetSymbol
        )
        {
            int width = 60;
            string border = new string('═', width - 2);

            Console.WriteLine($"\n╔{border}╗");
            Console.WriteLine($"║{CenterText("CONVERSION RESULT", width - 2)}║");
            Console.WriteLine($"╠{border}╣");
            Console.WriteLine(
                $"║  {inputValue, 8:F3} {sourceSymbol, -3} = {resultValue, 12:F6} {targetSymbol, -3}{new string(' ', 20)}║"
            );
            Console.WriteLine($"╚{border}╝");
        }

        /// <summary>
        /// Displays an addition result.
        /// </summary>
        /// <param name="firstQuantity">First quantity string.</param>
        /// <param name="secondQuantity">Second quantity string.</param>
        /// <param name="result">Result quantity.</param>
        public static void DisplayAdditionResult(
            string firstQuantity,
            string secondQuantity,
            double resultValue,
            string resultSymbol
        )
        {
            int width = 60;
            string border = new string('═', width - 2);

            Console.WriteLine($"\n╔{border}╗");
            Console.WriteLine($"║{CenterText("ADDITION RESULT", width - 2)}║");
            Console.WriteLine($"╠{border}╣");
            Console.WriteLine(
                $"║  {firstQuantity, -10} + {secondQuantity, -10}{new string(' ', 25)}║"
            );
            Console.WriteLine($"╠{border}╣");
            Console.WriteLine(
                $"║  = {resultValue, 12:F6} {resultSymbol, -3}{new string(' ', 34)}║"
            );
            Console.WriteLine($"╚{border}╝");
        }

        /// <summary>
        /// Centers text within a given width.
        /// </summary>
        /// <param name="text">The text to center.</param>
        /// <param name="width">The total width.</param>
        /// <returns>Centered text.</returns>
        private static string CenterText(string text, int width)
        {
            if (text.Length >= width)
                return text.Substring(0, width);

            int leftPadding = (width - text.Length) / 2;
            int rightPadding = width - text.Length - leftPadding;

            return new string(' ', leftPadding) + text + new string(' ', rightPadding);
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

        /// <summary>
        /// Displays a subtraction result.
        /// </summary>
        /// <param name="firstQuantity">First quantity string.</param>
        /// <param name="secondQuantity">Second quantity string.</param>
        /// <param name="resultValue">The result value.</param>
        /// <param name="resultSymbol">The result symbol.</param>
        public static void DisplaySubtractionResult(
            string firstQuantity,
            string secondQuantity,
            double resultValue,
            string resultSymbol
        )
        {
            int width = 60;
            string border = new string('═', width - 2);

            Console.WriteLine($"\n╔{border}╗");
            Console.WriteLine($"║{CenterText("SUBTRACTION RESULT", width - 2)}║");
            Console.WriteLine($"╠{border}╣");
            Console.WriteLine(
                $"║  {firstQuantity, -10} - {secondQuantity, -10}{new string(' ', 25)}║"
            );
            Console.WriteLine($"╠{border}╣");
            Console.WriteLine(
                $"║  = {resultValue, 12:F6} {resultSymbol, -3}{new string(' ', 34)}║"
            );
            Console.WriteLine($"╚{border}╝");
        }

        /// <summary>
        /// Displays a division result.
        /// </summary>
        /// <param name="firstQuantity">First quantity string.</param>
        /// <param name="secondQuantity">Second quantity string.</param>
        /// <param name="ratio">The division ratio.</param>
        public static void DisplayDivisionResult(
            string firstQuantity,
            string secondQuantity,
            double ratio
        )
        {
            int width = 60;
            string border = new string('═', width - 2);

            Console.WriteLine($"\n╔{border}╗");
            Console.WriteLine($"║{CenterText("DIVISION RESULT", width - 2)}║");
            Console.WriteLine($"╠{border}╣");
            Console.WriteLine(
                $"║  {firstQuantity, -10} ÷ {secondQuantity, -10}{new string(' ', 25)}║"
            );
            Console.WriteLine($"╠{border}╣");
            Console.WriteLine($"║  Ratio = {ratio, 12:F6}{new string(' ', 31)}║");
            Console.WriteLine($"╚{border}╝");
        }
    }
}
