using System.Text;
using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementConsole.Factory;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementRepositoryLayer.Interface;
using QuantityMeasurementRepositoryLayer.Services;

namespace QuantityMeasurementConsole.Menus
{
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

                Console.WriteLine("┌────────────────────────────────────────────────────────┐");
                Console.WriteLine("│                  MEASUREMENT OPERATIONS                │");
                Console.WriteLine("├────────────────────────────────────────────────────────┤");
                Console.WriteLine("│                                                        │");
                Console.WriteLine("│    1.  Compare Measurements                            │");
                Console.WriteLine("│    2.  Convert Units                                   │");
                Console.WriteLine("│    3.  Add Measurements                                │");
                Console.WriteLine("│    4.  Subtract Measurements                           │");
                Console.WriteLine("│    5.  Divide Measurements                             │");
                Console.WriteLine("│    6.  View History                                    │");
                Console.WriteLine("│    7.  Clear History                                   │");
                Console.WriteLine("│    8.  System Statistics                               │");
                Console.WriteLine("│    9.  Back to Main Menu                               │");
                Console.WriteLine("│                                                        │");
                Console.WriteLine("│    (Press ESC at any time to cancel current operation) │");
                Console.WriteLine("└────────────────────────────────────────────────────────┘");

                Console.Write("\nYour choice: ");
                var choice = ReadLineWithCancel();
                if (choice == null)
                    continue; // ESC pressed

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
                        ShowStatistics();
                        break;
                    case "9":
                        return;
                    default:
                        Console.WriteLine(
                            "\nInvalid option. Please enter a number between 1 and 9."
                        );
                        Pause();
                        break;
                }
            }
        }

        private void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("┌────────────────────────────────────────────────────────┐");
            Console.WriteLine("│              ADVANCED MEASUREMENT TOOL                │");
            Console.WriteLine("└────────────────────────────────────────────────────────┘");
            Console.ResetColor();
            Console.WriteLine();
        }

        private string? ReadLineWithCancel()
        {
            var input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\nOperation cancelled.");
                    return null;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
            }
            return input.ToString();
        }

        private string? ReadLineWithCancel(string prompt)
        {
            Console.Write(prompt);
            return ReadLineWithCancel();
        }

        #region Input Helper Methods

        private (string?, bool) GetValidCategoryWithCancel()
        {
            while (true)
            {
                Console.WriteLine("Select category:");
                Console.WriteLine("  1. Length");
                Console.WriteLine("  2. Weight");
                Console.WriteLine("  3. Volume");
                Console.WriteLine("  4. Temperature");
                Console.Write("Choice (ESC to cancel): ");

                var input = ReadLineWithCancel();
                if (input == null)
                    return (null, true);

                string? category = input switch
                {
                    "1" => "LENGTH",
                    "2" => "WEIGHT",
                    "3" => "VOLUME",
                    "4" => "TEMPERATURE",
                    _ => null,
                };

                if (category != null)
                    return (category, false);

                Console.WriteLine("Invalid category. Please select 1, 2, 3, or 4.");
            }
        }

        private (string?, bool) GetValidUnitWithCancel(string prompt, string category)
        {
            while (true)
            {
                Console.WriteLine($"\n{prompt} (ESC to cancel):");

                int maxOption = 3;
                switch (category)
                {
                    case "LENGTH":
                        Console.WriteLine("  1. Feet (ft)");
                        Console.WriteLine("  2. Inches (in)");
                        Console.WriteLine("  3. Yards (yd)");
                        Console.WriteLine("  4. Centimeters (cm)");
                        maxOption = 4;
                        break;
                    case "WEIGHT":
                        Console.WriteLine("  1. Kilograms (kg)");
                        Console.WriteLine("  2. Grams (g)");
                        Console.WriteLine("  3. Pounds (lb)");
                        maxOption = 3;
                        break;
                    case "VOLUME":
                        Console.WriteLine("  1. Litres (L)");
                        Console.WriteLine("  2. Millilitres (mL)");
                        Console.WriteLine("  3. Gallons (gal)");
                        maxOption = 3;
                        break;
                    case "TEMPERATURE":
                        Console.WriteLine("  1. Celsius (C)");
                        Console.WriteLine("  2. Fahrenheit (F)");
                        Console.WriteLine("  3. Kelvin (K)");
                        maxOption = 3;
                        break;
                }

                Console.Write($"Choose unit (1-{maxOption}): ");

                var input = ReadLineWithCancel();
                if (input == null)
                    return (null, true);

                string? unit = category switch
                {
                    "LENGTH" => input switch
                    {
                        "1" => "FEET",
                        "2" => "INCH",
                        "3" => "YARD",
                        "4" => "CENTIMETER",
                        _ => null,
                    },
                    "WEIGHT" => input switch
                    {
                        "1" => "KILOGRAM",
                        "2" => "GRAM",
                        "3" => "POUND",
                        _ => null,
                    },
                    "VOLUME" => input switch
                    {
                        "1" => "LITRE",
                        "2" => "MILLILITRE",
                        "3" => "GALLON",
                        _ => null,
                    },
                    "TEMPERATURE" => input switch
                    {
                        "1" => "CELSIUS",
                        "2" => "FAHRENHEIT",
                        "3" => "KELVIN",
                        _ => null,
                    },
                    _ => null,
                };

                if (unit != null)
                    return (unit, false);

                Console.WriteLine($"Invalid choice. Please select a valid option (1-{maxOption}).");
            }
        }

        private (double, bool) GetValidValueWithCancel(string prompt)
        {
            while (true)
            {
                Console.Write($"\n{prompt} (ESC to cancel): ");

                var input = ReadLineWithCancel();
                if (input == null)
                    return (0, true);

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Value cannot be empty.");
                    continue;
                }

                if (!double.TryParse(input, out double value))
                {
                    Console.WriteLine("Invalid number. Please enter a valid number.");
                    continue;
                }

                return (value, false);
            }
        }

        private QuantityDTO? GetQuantityInput(string prompt, string? fixedCategory = null)
        {
            Console.WriteLine($"\n{prompt}");

            string? category;
            bool cancelled;

            if (fixedCategory == null)
            {
                (category, cancelled) = GetValidCategoryWithCancel();
                if (cancelled)
                    return null;
            }
            else
            {
                category = fixedCategory;
                cancelled = false;
                Console.WriteLine($"Category: {GetCategoryDisplayName(category)}");
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

        private bool GetYesNoInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine()?.Trim().ToLower();

                if (input == "y" || input == "yes")
                    return true;
                if (input == "n" || input == "no" || string.IsNullOrEmpty(input))
                    return false;

                Console.WriteLine("Please enter 'y' or 'n'.");
            }
        }

        #endregion

        #region Operation Methods

        private void CompareMeasurements()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                Console.WriteLine("\n--- COMPARE MEASUREMENTS ---\n");

                try
                {
                    var q1 = GetQuantityInput("First measurement");
                    if (q1 == null)
                        return;

                    var q2 = GetQuantityInput("Second measurement", q1.Category);
                    if (q2 == null)
                        return;

                    var request = new BinaryQuantityRequest { Quantity1 = q1, Quantity2 = q2 };

                    var result = Task.Run(async () =>
                        await _service.CompareQuantitiesAsync(request)
                    ).Result;
                    DisplayResult(result);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
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

                try
                {
                    var source = GetQuantityInput("Source measurement");
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

                    var result = Task.Run(async () =>
                        await _service.ConvertQuantityAsync(request)
                    ).Result;
                    DisplayResult(result);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
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

                try
                {
                    var q1 = GetQuantityInput("First measurement");
                    if (q1 == null)
                        return;

                    var q2 = GetQuantityInput("Second measurement", q1.Category);
                    if (q2 == null)
                        return;

                    string? targetUnit = null;
                    if (GetYesNoInput("\nSpecify target unit? (y/n): "))
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

                    var result = Task.Run(async () =>
                        await _service.AddQuantitiesAsync(request)
                    ).Result;
                    DisplayResult(result);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
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

                try
                {
                    var q1 = GetQuantityInput("First measurement (minuend)");
                    if (q1 == null)
                        return;

                    var q2 = GetQuantityInput("Second measurement (subtrahend)", q1.Category);
                    if (q2 == null)
                        return;

                    string? targetUnit = null;
                    if (GetYesNoInput("\nSpecify target unit? (y/n): "))
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

                    var result = Task.Run(async () =>
                        await _service.SubtractQuantitiesAsync(request)
                    ).Result;
                    DisplayResult(result);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
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

                try
                {
                    var q1 = GetQuantityInput("First measurement (dividend)");
                    if (q1 == null)
                        return;

                    var q2 = GetQuantityInput("Second measurement (divisor)", q1.Category);
                    if (q2 == null)
                        return;

                    var request = new BinaryQuantityRequest { Quantity1 = q1, Quantity2 = q2 };

                    var result = Task.Run(async () =>
                        await _service.DivideQuantitiesAsync(request)
                    ).Result;
                    DisplayDivisionResult(result);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
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
                var allEntities = Task.Run(async () =>
                    await _repository.GetAllQuantitiesAsync()
                ).Result;

                if (allEntities.Count == 0)
                {
                    Console.WriteLine("No history available.");
                }
                else
                {
                    Console.WriteLine($"Found {allEntities.Count} records:\n");

                    int count = 0;
                    foreach (var entity in allEntities.OrderByDescending(e => e.CreatedAt).Take(20))
                    {
                        count++;
                        Console.WriteLine($"Record #{count}");
                        Console.WriteLine($"  When: {entity.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                        Console.WriteLine($"  Operation: {entity.OperationType}");
                        if (!string.IsNullOrEmpty(entity.FormattedResult))
                        {
                            string formatted = FormatResultString(entity.FormattedResult);
                            Console.WriteLine($"  Result: {formatted}");
                        }
                        if (!string.IsNullOrEmpty(entity.ErrorDetails))
                        {
                            Console.WriteLine($"  Error: {entity.ErrorDetails}");
                        }
                        Console.WriteLine(new string('-', 50));
                    }

                    if (allEntities.Count > 20)
                    {
                        Console.WriteLine($"... and {allEntities.Count - 20} more records");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
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
                var count = Task.Run(async () =>
                    await _repository.GetTotalQuantityCountAsync()
                ).Result;

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

                Task.Run(async () => await _repository.ClearAllQuantitiesAsync()).Wait();
                Console.WriteLine($"\nSuccessfully cleared {count} records.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Pause();
        }

        private void ShowStatistics()
        {
            Console.Clear();
            DisplayHeader();
            Console.WriteLine("\n--- SYSTEM STATISTICS ---\n");

            try
            {
                var stats = Task.Run(async () =>
                    await _repository.GetRepositoryStatisticsAsync()
                ).Result;

                // Check if database is actually available
                bool isDatabaseAvailable = false;
                if (stats.ContainsKey("DatabaseAvailable"))
                {
                    isDatabaseAvailable = Convert.ToBoolean(stats["DatabaseAvailable"]);
                }
                else if (stats.ContainsKey("DatabaseStatus"))
                {
                    isDatabaseAvailable = stats["DatabaseStatus"]?.ToString() == "ONLINE";
                }

                // Display sync status with color
                if (stats.ContainsKey("SyncStatus"))
                {
                    string syncStatus = stats["SyncStatus"].ToString() ?? "";
                    if (syncStatus.Contains("✅"))
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (syncStatus.Contains("⚠️"))
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else if (syncStatus.Contains("❌") || syncStatus.Contains("🔴"))
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine($"Sync Status: {syncStatus}");
                    Console.ResetColor();
                }

                Console.WriteLine(
                    $"Total Measurements: {stats.GetValueOrDefault("CacheRecords", 0)}"
                );
                Console.WriteLine($"Storage Mode: {stats.GetValueOrDefault("Mode", "Standard")}");

                if (stats.ContainsKey("Note"))
                {
                    string note = stats["Note"].ToString() ?? "";
                    if (note.Contains("✅"))
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (note.Contains("⚠️"))
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else if (note.Contains("🔴"))
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine($"Note: {note}");
                    Console.ResetColor();
                }

                Console.WriteLine(
                    $"\n📦 Cache Records: {stats.GetValueOrDefault("CacheRecords", 0)}"
                );

                // Handle Database Records display
                if (stats.ContainsKey("DatabaseRecords"))
                {
                    var dbRecords = stats["DatabaseRecords"];
                    if (dbRecords is string && dbRecords.ToString() == "🔴 OFFLINE")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"💾 Database Records: {dbRecords}");
                        Console.ResetColor();
                    }
                    else if (dbRecords is int dbCount)
                    {
                        int cacheCount = Convert.ToInt32(stats["CacheRecords"]);
                        int pending = Convert.ToInt32(stats.GetValueOrDefault("PendingWrites", 0));

                        if (pending > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(
                                $"💾 Database Records: {dbCount} (⚠️ {pending} writes pending)"
                            );
                            Console.ResetColor();
                        }
                        else if (cacheCount != dbCount)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"💾 Database Records: {dbCount} (⚠️ out of sync)");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine($"💾 Database Records: {dbCount}");
                        }
                    }
                }

                if (stats.ContainsKey("PendingWrites"))
                {
                    int pending = Convert.ToInt32(stats["PendingWrites"]);
                    if (pending > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"⏳ Pending Writes: {pending} (waiting to sync)");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"Pending Writes: 0");
                    }
                }

                if (stats.ContainsKey("DatabaseStatus"))
                {
                    string status = stats["DatabaseStatus"].ToString() ?? "";
                    if (status == "OFFLINE")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Database Status: {status}");
                        Console.ResetColor();
                    }
                }

                // Show flush stats
                if (stats.ContainsKey("TotalWritesAttempted"))
                {
                    int total = Convert.ToInt32(stats["TotalWritesAttempted"]);
                    int success = Convert.ToInt32(stats.GetValueOrDefault("SuccessfulWrites", 0));
                    int failed = Convert.ToInt32(stats.GetValueOrDefault("FailedWrites", 0));

                    if (total > 0)
                    {
                        Console.WriteLine($"\nWrite Statistics:");
                        Console.WriteLine($"  Total Attempts: {total}");
                        Console.WriteLine($"  Successful: {success}");

                        if (failed > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"  Failed/Pending: {failed}");
                            Console.ResetColor();
                        }
                    }
                }

                if (
                    stats.ContainsKey("CountsByOperation")
                    && stats["CountsByOperation"] is Dictionary<string, int> counts
                )
                {
                    Console.WriteLine("\nOperations Summary:");
                    foreach (var item in counts)
                    {
                        Console.WriteLine($"  {item.Key}: {item.Value}");
                    }
                }

                // Show recent activity
                var recent = Task.Run(async () => await _repository.GetAllQuantitiesAsync()).Result;
                if (recent.Any())
                {
                    Console.WriteLine("\nRecent Activity:");
                    foreach (var item in recent.OrderByDescending(x => x.CreatedAt).Take(3))
                    {
                        string display = item.FormattedResult ?? "Operation";
                        display = FormatResultString(display);
                        var parts = display.Split('=');
                        if (parts.Length > 1)
                        {
                            display = parts[1].Trim();
                        }
                        else
                        {
                            display = display.Trim();
                        }
                        Console.WriteLine($"  {item.CreatedAt:HH:mm} - {display}");
                    }
                }

                // SYNC OPTIONS - Only show if database is available
                if (isDatabaseAvailable)
                {
                    bool isOutOfSync = false;
                    if (
                        stats.ContainsKey("SyncStatus")
                        && stats["SyncStatus"].ToString()?.Contains("OUT OF SYNC") == true
                    )
                    {
                        isOutOfSync = true;
                    }
                    else if (
                        stats.ContainsKey("Note")
                        && stats["Note"].ToString()?.Contains("OUT OF SYNC") == true
                    )
                    {
                        isOutOfSync = true;
                    }
                    else if (
                        stats.ContainsKey("DatabaseRecords")
                        && stats["DatabaseRecords"] is int dbCount
                    )
                    {
                        int cacheCount = Convert.ToInt32(stats["CacheRecords"]);
                        if (cacheCount != dbCount)
                        {
                            isOutOfSync = true;
                        }
                    }

                    if (isOutOfSync && _repository is AutoHybridRepository autoRepo)
                    {
                        Console.WriteLine($"\n⚠️ Cache and database are out of sync!");
                        if (GetYesNoInput("Fix sync now? (y/n): "))
                        {
                            Console.WriteLine("\n⏳ Syncing...");
                            var success = Task.Run(async () =>
                                await autoRepo.ForceFullSyncAsync()
                            ).Result;

                            if (success)
                            {
                                Console.WriteLine("✅ Sync completed successfully!");

                                // Show updated stats
                                Console.WriteLine("\nRefreshing statistics...");
                                stats = Task.Run(async () =>
                                    await _repository.GetRepositoryStatisticsAsync()
                                ).Result;

                                // Display the updated sync status
                                if (stats.ContainsKey("SyncStatus"))
                                {
                                    string newStatus = stats["SyncStatus"].ToString() ?? "";
                                    if (newStatus.Contains("✅"))
                                        Console.ForegroundColor = ConsoleColor.Green;
                                    else if (newStatus.Contains("⚠️"))
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                    else if (newStatus.Contains("❌") || newStatus.Contains("🔴"))
                                        Console.ForegroundColor = ConsoleColor.Red;

                                    Console.WriteLine($"New Status: {newStatus}");
                                    Console.ResetColor();
                                }

                                // Show the updated record counts
                                int newCacheCount = Convert.ToInt32(
                                    stats.GetValueOrDefault("CacheRecords", 0)
                                );
                                Console.WriteLine($"\n📦 Cache Records: {newCacheCount}");

                                if (
                                    stats.ContainsKey("DatabaseRecords")
                                    && stats["DatabaseRecords"] is int newDbCount
                                )
                                {
                                    Console.WriteLine($"💾 Database Records: {newDbCount}");
                                }
                            }
                            else
                            {
                                Console.WriteLine(
                                    "❌ Sync failed. Please check database connection."
                                );
                            }
                        }
                    }
                    else if (
                        _repository is AutoHybridRepository autoRepo2
                        && GetYesNoInput("\nForce full sync anyway? (y/n): ")
                    )
                    {
                        Console.WriteLine("\n⏳ Syncing...");
                        var success = Task.Run(async () =>
                            await autoRepo2.ForceFullSyncAsync()
                        ).Result;

                        if (success)
                        {
                            Console.WriteLine("✅ Sync completed successfully!");
                        }
                        else
                        {
                            Console.WriteLine("❌ Sync failed. Please check database connection.");
                        }
                    }
                }
                else
                {
                    // Database is offline - show message but no sync option
                    Console.WriteLine("\nℹ️ Database is offline. Sync options are disabled.");
                    Console.WriteLine("   To enable database sync:");
                    Console.WriteLine("   1. Start SQL Server");
                    Console.WriteLine("   2. Ensure database 'QuantityMeasurementDB' exists");
                    Console.WriteLine("   3. Restart the application");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
            }

            Pause();
        }
        #endregion

        #region UI Helper Methods

        private void DisplayResult(QuantityResponse result)
        {
            Console.WriteLine("\n" + new string('=', 50));

            if (result.Success)
            {
                Console.WriteLine(result.Message);
                if (result.FormattedResult != null)
                {
                    // Format the result to show consistent units and 2 decimal places
                    string formatted = FormatResultString(result.FormattedResult);
                    Console.WriteLine($"\n{formatted}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {result.Message}");
            }

            Console.WriteLine(new string('=', 50));
        }

        private void DisplayDivisionResult(DivisionResponse result)
        {
            Console.WriteLine("\n" + new string('=', 50));

            if (result.Success)
            {
                Console.WriteLine(result.Message);
                Console.WriteLine($"\nRatio: {result.Ratio:F2}");
                Console.WriteLine($"\n{result.Interpretation}");
            }
            else
            {
                Console.WriteLine($"Error: {result.Message}");
            }

            Console.WriteLine(new string('=', 50));
        }

        private string FormatResultString(string result)
        {
            if (string.IsNullOrEmpty(result))
                return result;

            // Replace INCH with in
            result = result
                .Replace(" INCH", " in")
                .Replace(" INCHES", " in")
                .Replace(" FEET", " ft")
                .Replace(" YARD", " yd")
                .Replace(" YARDS", " yd")
                .Replace(" CENTIMETER", " cm")
                .Replace(" CENTIMETERS", " cm")
                .Replace(" KILOGRAM", " kg")
                .Replace(" KILOGRAMS", " kg")
                .Replace(" GRAM", " g")
                .Replace(" GRAMS", " g")
                .Replace(" POUND", " lb")
                .Replace(" POUNDS", " lb")
                .Replace(" LITRE", " L")
                .Replace(" LITRES", " L")
                .Replace(" MILLILITRE", " mL")
                .Replace(" MILLILITRES", " mL")
                .Replace(" GALLON", " gal")
                .Replace(" GALLONS", " gal")
                .Replace(" CELSIUS", "°C")
                .Replace(" FAHRENHEIT", "°F")
                .Replace(" KELVIN", " K");

            // Format numbers to 2 decimal places
            return System.Text.RegularExpressions.Regex.Replace(
                result,
                @"\d+\.?\d*",
                match =>
                {
                    if (double.TryParse(match.Value, out double num))
                    {
                        return num.ToString("F2");
                    }
                    return match.Value;
                }
            );
        }

        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private bool AskRetry()
        {
            Console.Write("\nPress 'R' to retry or any other key to return to menu: ");
            var key = Console.ReadKey(intercept: true);
            Console.WriteLine();
            return key.Key == ConsoleKey.R;
        }
        #endregion
    }
}
