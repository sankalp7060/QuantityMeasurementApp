using System;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Views
{
    // View class responsible for all user interface interactions
    // Updated to use the generic Quantity class
    public class Menu
    {
        // Service instance to handle business logic
        private readonly QuantityMeasurementService _service;

        // Constructor initializes the service
        public Menu()
        {
            _service = new QuantityMeasurementService();
        }

        // Main menu display and handling
        public void Display()
        {
            // Display application header
            Console.WriteLine("=== Quantity Measurement Application ===");
            Console.WriteLine("UC3: Generic Quantity Class with DRY Principle\n");

            // Show static method examples with cross-unit comparison
            ShowStaticExamples();

            // Main application loop
            while (true)
            {
                ShowMainMenu();

                string? choice = Console.ReadLine();

                if (choice == "4" || choice?.ToLower() == "exit")
                    break;

                ProcessMainMenuChoice(choice);
            }

            // Farewell message
            Console.WriteLine("\nThank you for using Quantity Measurement Application!");
        }

        // Display static method examples including cross-unit comparisons
        private void ShowStaticExamples()
        {
            Console.WriteLine("--- Static Method Examples ---");
            Console.WriteLine(
                $"Feet to Feet: 1.0 ft vs 1.0 ft -> Equal? {QuantityMeasurementService.AreQuantitiesEqual(1.0, LengthUnit.FEET, 1.0, LengthUnit.FEET)}"
            );
            Console.WriteLine(
                $"Feet to Feet: 1.0 ft vs 2.0 ft -> Equal? {QuantityMeasurementService.AreQuantitiesEqual(1.0, LengthUnit.FEET, 2.0, LengthUnit.FEET)}"
            );
            Console.WriteLine(
                $"Inch to Inch: 1.0 in vs 1.0 in -> Equal? {QuantityMeasurementService.AreQuantitiesEqual(1.0, LengthUnit.INCH, 1.0, LengthUnit.INCH)}"
            );
            Console.WriteLine(
                $"Inch to Inch: 1.0 in vs 2.0 in -> Equal? {QuantityMeasurementService.AreQuantitiesEqual(1.0, LengthUnit.INCH, 2.0, LengthUnit.INCH)}"
            );
            Console.WriteLine(
                $"Cross-Unit: 1.0 ft vs 12.0 in -> Equal? {QuantityMeasurementService.AreQuantitiesEqual(1.0, LengthUnit.FEET, 12.0, LengthUnit.INCH)}"
            );
            Console.WriteLine(
                $"Cross-Unit: 12.0 in vs 1.0 ft -> Equal? {QuantityMeasurementService.AreQuantitiesEqual(12.0, LengthUnit.INCH, 1.0, LengthUnit.FEET)}\n"
            );
        }

        // Display main menu options
        private void ShowMainMenu()
        {
            Console.WriteLine("Choose comparison type:");
            Console.WriteLine("1. Same unit comparison (Feet or Inch)");
            Console.WriteLine("2. Cross-unit comparison (Feet vs Inch)");
            Console.WriteLine("3. Backward compatibility (Original Feet/Inch classes)");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice (1-4): ");
        }

        // Process user's main menu choice
        private void ProcessMainMenuChoice(string? choice)
        {
            switch (choice)
            {
                case "1":
                    ShowSameUnitComparisonScreen();
                    break;
                case "2":
                    ShowCrossUnitComparisonScreen();
                    break;
                case "3":
                    ShowBackwardCompatibilityScreen();
                    break;
                default:
                    Console.WriteLine("Invalid choice! Please try again.\n");
                    break;
            }
        }

        // Display same unit comparison screen
        private void ShowSameUnitComparisonScreen()
        {
            Console.WriteLine("\n--- Same Unit Comparison ---");

            Console.WriteLine("Choose unit:");
            Console.WriteLine("1. Feet");
            Console.WriteLine("2. Inch");
            Console.Write("Enter your choice (1-2): ");

            string? unitChoice = Console.ReadLine();
            LengthUnit selectedUnit = unitChoice == "1" ? LengthUnit.FEET : LengthUnit.INCH;
            string unitName = selectedUnit == LengthUnit.FEET ? "feet" : "inches";

            // Get first measurement
            Console.WriteLine($"\nEnter first measurement in {unitName}:");
            string? input1 = Console.ReadLine();

            // Get second measurement
            Console.WriteLine($"Enter second measurement in {unitName}:");
            string? input2 = Console.ReadLine();

            // Parse inputs
            Quantity? q1 = _service.ParseQuantityInput(input1, selectedUnit);
            Quantity? q2 = _service.ParseQuantityInput(input2, selectedUnit);

            // Validate and show result
            if (q1 is null || q2 is null)
            {
                ShowErrorMessage("Invalid input! Please enter valid numeric values.");
                return;
            }

            // Compare and display result
            bool areEqual = _service.CompareQuantityEquality(q1, q2);
            ShowComparisonResult(q1.ToString(), q2.ToString(), areEqual);
        }

        // Display cross-unit comparison screen (Feet vs Inch)
        private void ShowCrossUnitComparisonScreen()
        {
            Console.WriteLine("\n--- Cross-Unit Comparison (Feet vs Inch) ---");

            // Get feet measurement
            Console.WriteLine("Enter measurement in feet:");
            string? feetInput = Console.ReadLine();

            // Get inch measurement
            Console.WriteLine("Enter measurement in inches:");
            string? inchInput = Console.ReadLine();

            // Parse inputs with different units
            Quantity? feetQ = _service.ParseQuantityInput(feetInput, LengthUnit.FEET);
            Quantity? inchQ = _service.ParseQuantityInput(inchInput, LengthUnit.INCH);

            // Validate and show result
            if (feetQ is null || inchQ is null)
            {
                ShowErrorMessage("Invalid input! Please enter valid numeric values.");
                return;
            }

            // Compare and display result
            bool areEqual = _service.CompareQuantityEquality(feetQ, inchQ);
            ShowComparisonResult(feetQ.ToString(), inchQ.ToString(), areEqual);
        }

        // Display backward compatibility screen (using original Feet/Inch classes)
        private void ShowBackwardCompatibilityScreen()
        {
            Console.WriteLine("\n--- Backward Compatibility (Original Classes) ---");
            Console.WriteLine("Choose measurement type:");
            Console.WriteLine("1. Feet comparison");
            Console.WriteLine("2. Inch comparison");
            Console.Write("Enter your choice (1-2): ");

            string? choice = Console.ReadLine();

            if (choice == "1")
            {
                ShowFeetComparisonScreen();
            }
            else if (choice == "2")
            {
                ShowInchComparisonScreen();
            }
            else
            {
                Console.WriteLine("Invalid choice!\n");
            }
        }

        // Display feet comparison screen (backward compatibility)
        private void ShowFeetComparisonScreen()
        {
            Console.WriteLine("\n--- Feet Comparison (Original) ---");

            string? input1 = GetUserInput("Enter first measurement in feet:");
            string? input2 = GetUserInput("Enter second measurement in feet:");

            Feet? feet1 = _service.ParseFeetInput(input1);
            Feet? feet2 = _service.ParseFeetInput(input2);

            if (feet1 is null || feet2 is null)
            {
                ShowErrorMessage("Invalid input! Please enter valid numeric values.");
                return;
            }

            bool areEqual = _service.CompareFeetEquality(feet1, feet2);
            ShowComparisonResult(feet1.ToString(), feet2.ToString(), areEqual);
        }

        // Display inch comparison screen (backward compatibility)
        private void ShowInchComparisonScreen()
        {
            Console.WriteLine("\n--- Inch Comparison (Original) ---");

            string? input1 = GetUserInput("Enter first measurement in inches:");
            string? input2 = GetUserInput("Enter second measurement in inches:");

            Inch? inch1 = _service.ParseInchInput(input1);
            Inch? inch2 = _service.ParseInchInput(input2);

            if (inch1 is null || inch2 is null)
            {
                ShowErrorMessage("Invalid input! Please enter valid numeric values.");
                return;
            }

            bool areEqual = _service.CompareInchEquality(inch1, inch2);
            ShowComparisonResult(inch1.ToString(), inch2.ToString(), areEqual);
        }

        // Helper method to get user input with prompt
        private string? GetUserInput(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }

        // Helper method to display comparison result
        private void ShowComparisonResult(string measurement1, string measurement2, bool areEqual)
        {
            Console.WriteLine($"\nFirst measurement: {measurement1}");
            Console.WriteLine($"Second measurement: {measurement2}");
            Console.WriteLine(
                $"Are they equal? {areEqual} ({(areEqual ? "Equal" : "Not Equal")})\n"
            );
            Console.WriteLine("----------------------------------------\n");
        }

        // Helper method to display error messages
        private void ShowErrorMessage(string message)
        {
            Console.WriteLine($"{message}\n");
        }
    }
}
