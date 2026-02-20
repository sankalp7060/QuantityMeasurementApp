using System;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Views
{
    // View class responsible for all user interface interactions
    // Handles displaying menus, getting user input, and showing results
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
            Console.WriteLine("UC1 & UC2: Feet and Inch Measurement Equality\n");

            // Show static method examples
            ShowStaticExamples();

            // Main application loop
            while (true)
            {
                ShowMainMenu();

                string? choice = Console.ReadLine();

                if (choice == "3" || choice?.ToLower() == "exit")
                    break;

                ProcessMainMenuChoice(choice);
            }

            // Farewell message
            Console.WriteLine("\nThank you for using Quantity Measurement Application!");
        }

        // Display static method examples
        private void ShowStaticExamples()
        {
            Console.WriteLine("--- Static Method Examples ---");
            Console.WriteLine(
                $"Feet: 1.0 ft vs 1.0 ft -> Equal? {QuantityMeasurementService.AreFeetEqual(1.0, 1.0)}"
            );
            Console.WriteLine(
                $"Feet: 1.0 ft vs 2.0 ft -> Equal? {QuantityMeasurementService.AreFeetEqual(1.0, 2.0)}"
            );
            Console.WriteLine(
                $"Inch: 1.0 in vs 1.0 in -> Equal? {QuantityMeasurementService.AreInchEqual(1.0, 1.0)}"
            );
            Console.WriteLine(
                $"Inch: 1.0 in vs 2.0 in -> Equal? {QuantityMeasurementService.AreInchEqual(1.0, 2.0)}\n"
            );
        }

        // Display main menu options
        private void ShowMainMenu()
        {
            Console.WriteLine("Choose measurement type:");
            Console.WriteLine("1. Feet comparison");
            Console.WriteLine("2. Inch comparison");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice (1-3): ");
        }

        // Process user's main menu choice
        private void ProcessMainMenuChoice(string? choice)
        {
            switch (choice)
            {
                case "1":
                    ShowFeetComparisonScreen();
                    break;
                case "2":
                    ShowInchComparisonScreen();
                    break;
                default:
                    Console.WriteLine("Invalid choice! Please try again.\n");
                    break;
            }
        }

        // Display feet comparison screen
        private void ShowFeetComparisonScreen()
        {
            Console.WriteLine("\n--- Feet Comparison ---");

            // Get first measurement
            string? input1 = GetUserInput("Enter first measurement in feet:");

            // Get second measurement
            string? input2 = GetUserInput("Enter second measurement in feet:");

            // Parse inputs
            Feet? feet1 = _service.ParseFeetInput(input1);
            Feet? feet2 = _service.ParseFeetInput(input2);

            // Validate and show result
            if (feet1 is null || feet2 is null)
            {
                ShowErrorMessage("Invalid input! Please enter valid numeric values.");
                return;
            }

            // Compare and display result
            bool areEqual = _service.CompareFeetEquality(feet1, feet2);
            ShowComparisonResult(feet1.ToString(), feet2.ToString(), areEqual);
        }

        // Display inch comparison screen
        private void ShowInchComparisonScreen()
        {
            Console.WriteLine("\n--- Inch Comparison ---");

            // Get first measurement
            string? input1 = GetUserInput("Enter first measurement in inches:");

            // Get second measurement
            string? input2 = GetUserInput("Enter second measurement in inches:");

            // Parse inputs
            Inch? inch1 = _service.ParseInchInput(input1);
            Inch? inch2 = _service.ParseInchInput(input2);

            // Validate and show result
            if (inch1 is null || inch2 is null)
            {
                ShowErrorMessage("Invalid input! Please enter valid numeric values.");
                return;
            }

            // Compare and display result
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
