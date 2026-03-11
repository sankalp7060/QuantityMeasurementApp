using System.Text;
using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Exceptions;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementConsole.Factory;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementConsole.Menus
{
    /// <summary>
    /// Advanced measurement operations menu with comprehensive input validation
    /// </summary>
    public class MeasurementMenu
    {
        private readonly IQuantityMeasurementService _service;
        private readonly IQuantityMeasurementRepository _repository;
        private readonly ILogger<MeasurementMenu> _logger;

        public MeasurementMenu(ILoggerFactory loggerFactory, ServiceFactory serviceFactory)
        {
            _service = serviceFactory.GetService();
            _repository = serviceFactory.GetRepository();
            _logger = loggerFactory.CreateLogger<MeasurementMenu>();
        }

        public void Display()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();

                Console.WriteLine("╔════════════════════════════════════════════════════════╗");
                Console.WriteLine("║              MEASUREMENT OPERATIONS                   ║");
                Console.WriteLine("╠════════════════════════════════════════════════════════╣");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("║    1.  Compare Measurements                           ║");
                Console.WriteLine("║    2.  Convert Units                                  ║");
                Console.WriteLine("║    3.  Add Measurements                               ║");
                Console.WriteLine("║    4.  Subtract Measurements                          ║");
                Console.WriteLine("║    5.  Divide Measurements                            ║");
                Console.WriteLine("║    6.  View History                                   ║");
                Console.WriteLine("║    7.  Clear History                                  ║");
                Console.WriteLine("║    8.  Back to Main Menu                              ║");
                Console.WriteLine("║                                                        ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════╝");

                Console.Write("\nSelect an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CompareMeasurements();
                        break;
                    case "2":
                        ConvertUnits();
                        break;
                    case "3":
                        AddMeasurements();
                        break;
                    case "4":
                        SubtractMeasurements();
                        break;
                    case "5":
                        DivideMeasurements();
                        break;
                    case "6":
                        ViewHistory();
                        break;
                    case "7":
                        ClearHistory();
                        break;
                    case "8":
                        return;
                    default:
                        DisplayError("Invalid option. Please enter a number between 1 and 8.");
                        Pause();
                        break;
                }
            }
        }

        private void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           ADVANCED MEASUREMENT SYSTEM                 ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        private bool TryGetValidCategory(out string? category)
        {
            category = string.Empty;
            Console.WriteLine("Select category:");
            Console.WriteLine("  1. Length");
            Console.WriteLine("  2. Weight");
            Console.WriteLine("  3. Volume");
            Console.WriteLine("  4. Temperature");
            Console.Write("Choice: ");
            var catChoice = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(catChoice))
            {
                DisplayError("Category cannot be empty.");
                return false;
            }

            category = catChoice switch
            {
                "1" => "LENGTH",
                "2" => "WEIGHT",
                "3" => "VOLUME",
                "4" => "TEMPERATURE",
                _ => null,
            };

            if (category == null)
            {
                DisplayError("Invalid category. Please select 1, 2, 3, or 4.");
                return false;
            }

            return true;
        }

        private bool TryGetValidValue(string prompt, out double value)
        {
            value = 0;
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayError("Value cannot be empty.");
                return false;
            }

            if (!double.TryParse(input, out value))
            {
                DisplayError("Invalid numeric value. Please enter a valid number.");
                return false;
            }

            // Check for extreme values
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                DisplayError("Value cannot be infinite or NaN.");
                return false;
            }

            return true;
        }

        private QuantityDTO? GetQuantityInput(string prompt)
        {
            Console.WriteLine($"\n{prompt}");

            var (category, cancelled1) = GetValidCategoryWithCancel();
            if (cancelled1)
                return null;

            var (unit, cancelled2) = GetValidUnitWithCancel("Select unit", category!);
            if (cancelled2)
                return null;

            var (value, cancelled3) = GetValidValueWithCancel("Enter value");
            if (cancelled3)
                return null;

            return new QuantityDTO
            {
                Value = value,
                Unit = unit!,
                Category = category!,
            };
        }

        private void CompareMeasurements()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                Console.WriteLine("\n--- COMPARE MEASUREMENTS ---\n");
                Console.WriteLine("(Press ESC at any prompt to cancel, or ENTER to continue)\n");

                try
                {
                    // First measurement - gets category
                    var q1 = GetQuantityInputWithCancel("First measurement");
                    if (q1 == null)
                        return;

                    // Second measurement - uses same category
                    var q2 = GetQuantityInputWithCancel("Second measurement", q1.Category);
                    if (q2 == null)
                        return;

                    var request = new BinaryQuantityRequest { Quantity1 = q1, Quantity2 = q2 };

                    var result = _service.CompareQuantitiesAsync(request).GetAwaiter().GetResult();
                    DisplayResult(result);
                    break;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (QuantityMeasurementException ex)
                {
                    DisplayError($"Measurement Error: {ex.Message}");
                    if (!AskRetry())
                        return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in compare operation");
                    DisplayError($"System Error: {ex.Message}");
                    if (!AskRetry())
                        return;
                }
            }

            Pause();
        }

        private void ConvertUnits()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                Console.WriteLine("\n--- CONVERT UNITS ---\n");
                Console.WriteLine("(Press ESC at any prompt to cancel, or ENTER to continue)\n");

                try
                {
                    var source = GetQuantityInputWithCancel("Source measurement");
                    if (source == null)
                        return;

                    var (targetUnit, cancelled) = GetValidUnitWithCancel(
                        "Select target unit",
                        source.Category
                    );
                    if (cancelled)
                        return;

                    var request = new ConversionRequest
                    {
                        Source = source,
                        TargetUnit = targetUnit!,
                    };

                    var result = _service.ConvertQuantityAsync(request).GetAwaiter().GetResult();
                    DisplayResult(result);
                    break;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (QuantityMeasurementException ex)
                {
                    DisplayError($"Measurement Error: {ex.Message}");
                    if (!AskRetry())
                        return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in convert operation");
                    DisplayError($"System Error: {ex.Message}");
                    if (!AskRetry())
                        return;
                }
            }

            Pause();
        }

        private void AddMeasurements()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                Console.WriteLine("\n--- ADD MEASUREMENTS ---\n");
                Console.WriteLine("(Press ESC at any prompt to cancel, or ENTER to continue)\n");

                try
                {
                    // First measurement - gets category
                    var q1 = GetQuantityInputWithCancel("First measurement");
                    if (q1 == null)
                        return;

                    // Second measurement - uses same category
                    var q2 = GetQuantityInputWithCancel("Second measurement", q1.Category);
                    if (q2 == null)
                        return;

                    string? targetUnit = null;
                    if (GetYesNoInput("\nDo you want to specify a target unit? (y/n): "))
                    {
                        var (tu, cancelled) = GetValidUnitWithCancel(
                            "Select target unit",
                            q1.Category
                        );
                        if (cancelled)
                            return;
                        targetUnit = tu;
                    }

                    var request = new BinaryQuantityRequest
                    {
                        Quantity1 = q1,
                        Quantity2 = q2,
                        TargetUnit = targetUnit,
                    };

                    var result = _service.AddQuantitiesAsync(request).GetAwaiter().GetResult();
                    DisplayResult(result);
                    break;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (QuantityMeasurementException ex)
                {
                    DisplayError($"Measurement Error: {ex.Message}");
                    if (!AskRetry())
                        return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in add operation");
                    DisplayError($"System Error: {ex.Message}");
                    if (!AskRetry())
                        return;
                }
            }

            Pause();
        }

        private void SubtractMeasurements()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                Console.WriteLine("\n--- SUBTRACT MEASUREMENTS ---\n");
                Console.WriteLine("(Press ESC at any prompt to cancel, or ENTER to continue)\n");

                try
                {
                    // First measurement - gets category
                    var q1 = GetQuantityInputWithCancel("First measurement (minuend)");
                    if (q1 == null)
                        return;

                    // Second measurement - uses same category
                    var q2 = GetQuantityInputWithCancel(
                        "Second measurement (subtrahend)",
                        q1.Category
                    );
                    if (q2 == null)
                        return;

                    string? targetUnit = null;
                    if (GetYesNoInput("\nDo you want to specify a target unit? (y/n): "))
                    {
                        var (tu, cancelled) = GetValidUnitWithCancel(
                            "Select target unit",
                            q1.Category
                        );
                        if (cancelled)
                            return;
                        targetUnit = tu;
                    }

                    var request = new BinaryQuantityRequest
                    {
                        Quantity1 = q1,
                        Quantity2 = q2,
                        TargetUnit = targetUnit,
                    };

                    var result = _service.SubtractQuantitiesAsync(request).GetAwaiter().GetResult();
                    DisplayResult(result);
                    break;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (QuantityMeasurementException ex)
                {
                    DisplayError($"Measurement Error: {ex.Message}");
                    if (!AskRetry())
                        return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in subtract operation");
                    DisplayError($"System Error: {ex.Message}");
                    if (!AskRetry())
                        return;
                }
            }

            Pause();
        }

        private void DivideMeasurements()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                Console.WriteLine("\n--- DIVIDE MEASUREMENTS ---\n");
                Console.WriteLine("(Press ESC at any prompt to cancel, or ENTER to continue)\n");

                try
                {
                    // First measurement - gets category
                    var q1 = GetQuantityInputWithCancel("First measurement (dividend)");
                    if (q1 == null)
                        return;

                    // Second measurement - uses same category
                    var q2 = GetQuantityInputWithCancel(
                        "Second measurement (divisor)",
                        q1.Category
                    );
                    if (q2 == null)
                        return;

                    // Check for zero divisor
                    if (Math.Abs(q2.Value) < 0.000001)
                    {
                        DisplayError("Division by zero is not allowed.");
                        if (!AskRetry())
                            return;
                        continue;
                    }

                    var request = new BinaryQuantityRequest { Quantity1 = q1, Quantity2 = q2 };

                    var result = _service.DivideQuantitiesAsync(request).GetAwaiter().GetResult();
                    DisplayDivisionResult(result);
                    break;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (QuantityMeasurementException ex)
                {
                    DisplayError($"Measurement Error: {ex.Message}");
                    if (!AskRetry())
                        return;
                }
                catch (DivideByZeroException)
                {
                    DisplayError("Division by zero is not allowed.");
                    if (!AskRetry())
                        return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in divide operation");
                    DisplayError($"System Error: {ex.Message}");
                    if (!AskRetry())
                        return;
                }
            }

            Pause();
        }

        private void ViewHistory()
        {
            Console.Clear();
            DisplayHeader();
            Console.WriteLine("\n--- MEASUREMENT HISTORY ---\n");

            try
            {
                var allEntities = _repository.GetAll();

                if (allEntities.Count == 0)
                {
                    Console.WriteLine("No history available.");
                }
                else
                {
                    Console.WriteLine($"Found {allEntities.Count} records:\n");

                    int count = 0;
                    foreach (var entity in allEntities.OrderByDescending(e => e.Timestamp))
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Record #{count}");
                        Console.ResetColor();
                        Console.WriteLine($"  When: {entity.Timestamp:HH:mm:ss dd-MMM-yyyy}");
                        Console.WriteLine($"  Date: {entity.Timestamp:yyyy-MM-dd HH:mm:ss}");
                        Console.WriteLine($"  Operation: {entity.OperationType}");
                        if (!string.IsNullOrEmpty(entity.FormattedResult))
                        {
                            Console.WriteLine($"  Result: {entity.FormattedResult}");
                        }
                        if (!string.IsNullOrEmpty(entity.ErrorMessage))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"  Error: {entity.ErrorMessage}");
                            Console.ResetColor();
                        }
                        Console.WriteLine(new string('-', 50));

                        if (count >= 20)
                        {
                            Console.WriteLine($"... and {allEntities.Count - 20} more records");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Error viewing history: {ex.Message}");
            }

            Pause();
        }

        private void ClearHistory()
        {
            Console.Clear();
            DisplayHeader();
            Console.WriteLine("\n--- CLEAR HISTORY ---\n");

            try
            {
                var count = _repository.GetAll().Count;

                if (count == 0)
                {
                    Console.WriteLine("History is already empty.");
                    Pause();
                    return;
                }

                Console.WriteLine($"Are you sure you want to clear {count} records?");
                if (!GetYesNoInput("This action cannot be undone. Continue? (y/n): "))
                {
                    Console.WriteLine("Operation cancelled.");
                    Pause();
                    return;
                }

                _repository.Clear();
                Console.WriteLine($"\n✅ Successfully cleared {count} records.");
            }
            catch (Exception ex)
            {
                DisplayError($"Error clearing history: {ex.Message}");
            }

            Pause();
        }

        private bool GetYesNoInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine()?.Trim().ToLower();

                if (input == "y" || input == "yes")
                    return true;
                if (input == "n" || input == "no")
                    return false;

                DisplayError("Please enter 'y' or 'n'.");
            }
        }

        private void DisplayResult(QuantityResponse result)
        {
            Console.WriteLine("\n" + new string('═', 50));

            if (result.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ {result.Message}");
                if (result.FormattedResult != null)
                {
                    Console.WriteLine($"\n{result.FormattedResult}");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ {result.Message}");
                Console.ResetColor();
            }

            Console.WriteLine(new string('═', 50));
        }

        private void DisplayDivisionResult(DivisionResponse result)
        {
            Console.WriteLine("\n" + new string('═', 50));

            if (result.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ {result.Message}");
                Console.WriteLine($"\nRatio: {result.Ratio:F6}");
                Console.WriteLine($"\n{result.Interpretation}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ {result.Message}");
                Console.ResetColor();
            }

            Console.WriteLine(new string('═', 50));
        }

        private void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ {message}");
            Console.ResetColor();
        }

        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private QuantityDTO? GetQuantityInputWithCancel(string prompt, string? fixedCategory = null)
        {
            Console.WriteLine($"\n{prompt}");

            string? category;
            bool cancelled1;

            if (fixedCategory == null)
            {
                // First measurement - ask for category
                (category, cancelled1) = GetValidCategoryWithCancel();
                if (cancelled1)
                    return null;
            }
            else
            {
                // Second measurement - use same category as first
                category = fixedCategory;
                cancelled1 = false;
                Console.WriteLine(
                    $"Category: {GetCategoryDisplayName(category)} (same as first measurement)"
                );
            }

            var (unit, cancelled2) = GetValidUnitWithCancel("Select unit", category!);
            if (cancelled2)
                return null;

            var (value, cancelled3) = GetValidValueWithCancel("Enter value");
            if (cancelled3)
                return null;

            return new QuantityDTO
            {
                Value = value,
                Unit = unit!,
                Category = category!,
            };
        }

        // Helper method to display category name
        private string GetCategoryDisplayName(string category)
        {
            return category switch
            {
                "LENGTH" => "Length",
                "WEIGHT" => "Weight",
                "VOLUME" => "Volume",
                "TEMPERATURE" => "Temperature",
                _ => category,
            };
        }

        private (string?, bool) GetValidCategoryWithCancel()
        {
            while (true)
            {
                Console.WriteLine("Select category:");
                Console.WriteLine("  1. Length");
                Console.WriteLine("  2. Weight");
                Console.WriteLine("  3. Volume");
                Console.WriteLine("  4. Temperature");
                Console.WriteLine("  ESC - Cancel operation");
                Console.Write("Choice: ");

                var keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\n⏹ Operation cancelled.");
                    return (null, true);
                }

                var catChoice = keyInfo.KeyChar.ToString();
                Console.WriteLine(catChoice); // Show the pressed key

                string? category = catChoice switch
                {
                    "1" => "LENGTH",
                    "2" => "WEIGHT",
                    "3" => "VOLUME",
                    "4" => "TEMPERATURE",
                    _ => null,
                };

                if (category != null)
                    return (category!, false);

                DisplayError("Invalid category. Please select 1, 2, 3, or 4.");
                // Continue loop
            }
        }

        private (string?, bool) GetValidUnitWithCancel(string prompt, string category)
        {
            while (true)
            {
                Console.WriteLine($"\n{prompt} (ESC to cancel):");

                // Show appropriate unit options based on category
                switch (category)
                {
                    case "LENGTH":
                        Console.WriteLine("  1. Feet (ft)");
                        Console.WriteLine("  2. Inches (in)");
                        Console.WriteLine("  3. Yards (yd)");
                        Console.WriteLine("  4. Centimeters (cm)");
                        break;
                    case "WEIGHT":
                        Console.WriteLine("  1. Kilograms (kg)");
                        Console.WriteLine("  2. Grams (g)");
                        Console.WriteLine("  3. Pounds (lb)");
                        break;
                    case "VOLUME":
                        Console.WriteLine("  1. Litres (L)");
                        Console.WriteLine("  2. Millilitres (mL)");
                        Console.WriteLine("  3. Gallons (gal)");
                        break;
                    case "TEMPERATURE":
                        Console.WriteLine("  1. Celsius (°C)");
                        Console.WriteLine("  2. Fahrenheit (°F)");
                        Console.WriteLine("  3. Kelvin (K)");
                        break;
                }

                Console.Write("Choose unit (1-4): ");

                // Check for ESC key
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\n⏹ Operation cancelled.");
                    return (null, true);
                }

                var choice = key.KeyChar.ToString();
                Console.WriteLine(choice); // Show the pressed key

                string? unit = category switch
                {
                    "LENGTH" => choice switch
                    {
                        "1" => "FEET",
                        "2" => "INCH",
                        "3" => "YARD",
                        "4" => "CENTIMETER",
                        _ => null,
                    },
                    "WEIGHT" => choice switch
                    {
                        "1" => "KILOGRAM",
                        "2" => "GRAM",
                        "3" => "POUND",
                        _ => null,
                    },
                    "VOLUME" => choice switch
                    {
                        "1" => "LITRE",
                        "2" => "MILLILITRE",
                        "3" => "GALLON",
                        _ => null,
                    },
                    "TEMPERATURE" => choice switch
                    {
                        "1" => "CELSIUS",
                        "2" => "FAHRENHEIT",
                        "3" => "KELVIN",
                        _ => null,
                    },
                    _ => null,
                };

                if (unit != null)
                    return (unit!, false);

                DisplayError(
                    $"Invalid choice. Please select a valid option (1-{(category == "LENGTH" ? "4" : "3")})."
                );
                // Continue loop
            }
        }

        private (double, bool) GetValidValueWithCancel(string prompt)
        {
            while (true)
            {
                Console.Write($"\n{prompt} (ESC to cancel): ");

                // Read key properly
                var input = new StringBuilder();
                while (true)
                {
                    var keyInfo = Console.ReadKey(intercept: true);

                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("\n⏹ Operation cancelled.");
                        return (0, true);
                    }

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }

                    if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input.Remove(input.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                    else if (!char.IsControl(keyInfo.KeyChar))
                    {
                        input.Append(keyInfo.KeyChar);
                        Console.Write(keyInfo.KeyChar);
                    }
                }

                var valueStr = input.ToString();

                if (string.IsNullOrWhiteSpace(valueStr))
                {
                    DisplayError("Value cannot be empty.");
                    continue;
                }

                if (!double.TryParse(valueStr, out double value))
                {
                    DisplayError("Invalid number. Please enter a valid number (e.g., 12.5).");
                    continue;
                }

                if (double.IsInfinity(value) || double.IsNaN(value))
                {
                    DisplayError("Value cannot be infinite or NaN.");
                    continue;
                }

                return (value, false);
            }
        }

        private bool AskRetry()
        {
            Console.Write("\nPress 'R' to retry or any other key to return to menu: ");
            var key = Console.ReadKey(intercept: true);
            Console.WriteLine();
            return key.Key == ConsoleKey.R;
        }
    }
}
