using System;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Views
{
    // View class responsible for all user interface interactions
    // Updated to include YARD and CENTIMETER units
    public class Menu
    {
        // Service instance to handle business logic
        private readonly QuantityMeasurementService _service;

        // Extension instance for unit operations
        private readonly LengthUnitExtensions _unitExtensions;

        // Constructor initializes the service and extensions
        public Menu()
        {
            _service = new QuantityMeasurementService();
            _unitExtensions = new LengthUnitExtensions();
        }

        // Main menu display and handling
        public void Display()
        {
            // Display application header
            Console.WriteLine("=== Quantity Measurement Application ===");
            Console.WriteLine("UC4: Extended Unit Support (Yards & Centimeters)\n");

            // Show static method examples with all units
            ShowStaticExamples();

            // Main application loop
            while (true)
            {
                ShowMainMenu();

                string? choice = Console.ReadLine();

                if (choice == "5" || choice?.ToLower() == "exit")
                    break;

                ProcessMainMenuChoice(choice);
            }

            // Farewell message
            Console.WriteLine("\nThank you for using Quantity Measurement Application!");
        }

        // Display static method examples including all unit combinations
        private void ShowStaticExamples()
        {
            Console.WriteLine("--- Static Method Examples (All Units) ---");
            Console.WriteLine(
                $"Feet to Feet: 1.0 ft vs 1.0 ft -> Equal? {_service.AreQuantitiesEqual(1.0, LengthUnit.FEET, 1.0, LengthUnit.FEET)}"
            );
            Console.WriteLine(
                $"Inch to Inch: 1.0 in vs 1.0 in -> Equal? {_service.AreQuantitiesEqual(1.0, LengthUnit.INCH, 1.0, LengthUnit.INCH)}"
            );
            Console.WriteLine(
                $"Yard to Yard: 1.0 yd vs 1.0 yd -> Equal? {_service.AreQuantitiesEqual(1.0, LengthUnit.YARD, 1.0, LengthUnit.YARD)}"
            );
            Console.WriteLine(
                $"CM to CM: 1.0 cm vs 1.0 cm -> Equal? {_service.AreQuantitiesEqual(1.0, LengthUnit.CENTIMETER, 1.0, LengthUnit.CENTIMETER)}"
            );
            Console.WriteLine();

            Console.WriteLine("--- Cross-Unit Examples ---");
            Console.WriteLine(
                $"Yard to Feet: 1.0 yd vs 3.0 ft -> Equal? {_service.AreQuantitiesEqual(1.0, LengthUnit.YARD, 3.0, LengthUnit.FEET)}"
            );
            Console.WriteLine(
                $"Yard to Inches: 1.0 yd vs 36.0 in -> Equal? {_service.AreQuantitiesEqual(1.0, LengthUnit.YARD, 36.0, LengthUnit.INCH)}"
            );
            Console.WriteLine(
                $"CM to Inches: 1.0 cm vs 0.393701 in -> Equal? {_service.AreQuantitiesEqual(1.0, LengthUnit.CENTIMETER, 0.393701, LengthUnit.INCH)}"
            );
            Console.WriteLine(
                $"CM to Feet: 30.48 cm vs 1.0 ft -> Equal? {_service.AreQuantitiesEqual(30.48, LengthUnit.CENTIMETER, 1.0, LengthUnit.FEET)}"
            );
            Console.WriteLine(
                $"Yard to CM: 1.0 yd vs 91.44 cm -> Equal? {_service.AreQuantitiesEqual(1.0, LengthUnit.YARD, 91.44, LengthUnit.CENTIMETER)}"
            );
            Console.WriteLine();

            Console.WriteLine("--- Complex Examples ---");
            Console.WriteLine(
                $"2 Yards to 6 Feet to 72 Inches: {_service.AreQuantitiesEqual(2.0, LengthUnit.YARD, 6.0, LengthUnit.FEET)} and {_service.AreQuantitiesEqual(6.0, LengthUnit.FEET, 72.0, LengthUnit.INCH)}"
            );
            Console.WriteLine(
                $"Therefore, 2 Yards equals 72 Inches: {_service.AreQuantitiesEqual(2.0, LengthUnit.YARD, 72.0, LengthUnit.INCH)}\n"
            );
        }

        // Display main menu options
        private void ShowMainMenu()
        {
            Console.WriteLine("Choose comparison type:");
            Console.WriteLine("1. Same unit comparison");
            Console.WriteLine("2. Cross-unit comparison");
            Console.WriteLine("3. Advanced multi-unit comparison");
            Console.WriteLine("4. Backward compatibility (Original Feet/Inch classes)");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice (1-5): ");
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
                    ShowAdvancedComparisonScreen();
                    break;
                case "4":
                    ShowBackwardCompatibilityScreen();
                    break;
                default:
                    Console.WriteLine("Invalid choice! Please try again.\n");
                    break;
            }
        }

        // Display same unit comparison screen with all units
        private void ShowSameUnitComparisonScreen()
        {
            Console.WriteLine("\n--- Same Unit Comparison ---");

            Console.WriteLine("Choose unit:");
            Console.WriteLine("1. Feet");
            Console.WriteLine("2. Inches");
            Console.WriteLine("3. Yards");
            Console.WriteLine("4. Centimeters");
            Console.Write("Enter your choice (1-4): ");

            string? unitChoice = Console.ReadLine();
            LengthUnit selectedUnit = GetUnitFromChoice(unitChoice);

            if (selectedUnit == LengthUnit.FEET && unitChoice != "1")
            {
                ShowErrorMessage("Invalid unit choice!");
                return;
            }

            string unitName = LengthUnitExtensions.GetUnitName(selectedUnit);

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

        // Display cross-unit comparison screen with all unit combinations
        private void ShowCrossUnitComparisonScreen()
        {
            Console.WriteLine("\n--- Cross-Unit Comparison ---");

            // Get first unit
            Console.WriteLine("Select first unit:");
            Console.WriteLine("1. Feet");
            Console.WriteLine("2. Inches");
            Console.WriteLine("3. Yards");
            Console.WriteLine("4. Centimeters");
            Console.Write("Enter choice (1-4): ");
            string? unit1Choice = Console.ReadLine();
            LengthUnit unit1 = GetUnitFromChoice(unit1Choice);

            if (unit1 == LengthUnit.FEET && unit1Choice != "1")
            {
                ShowErrorMessage("Invalid first unit choice!");
                return;
            }

            // Get second unit
            Console.WriteLine("\nSelect second unit:");
            Console.WriteLine("1. Feet");
            Console.WriteLine("2. Inches");
            Console.WriteLine("3. Yards");
            Console.WriteLine("4. Centimeters");
            Console.Write("Enter choice (1-4): ");
            string? unit2Choice = Console.ReadLine();
            LengthUnit unit2 = GetUnitFromChoice(unit2Choice);

            if (unit2 == LengthUnit.FEET && unit2Choice != "1")
            {
                ShowErrorMessage("Invalid second unit choice!");
                return;
            }

            // Get first measurement
            Console.WriteLine($"\nEnter measurement in {LengthUnitExtensions.GetUnitName(unit1)}:");
            string? input1 = Console.ReadLine();

            // Get second measurement
            Console.WriteLine($"Enter measurement in {LengthUnitExtensions.GetUnitName(unit2)}:");
            string? input2 = Console.ReadLine();

            // Parse inputs
            Quantity? q1 = _service.ParseQuantityInput(input1, unit1);
            Quantity? q2 = _service.ParseQuantityInput(input2, unit2);

            // Validate and show result
            if (q1 is null || q2 is null)
            {
                ShowErrorMessage("Invalid input! Please enter valid numeric values.");
                return;
            }

            // Compare and display result
            bool areEqual = _service.CompareQuantityEquality(q1, q2);
            ShowComparisonResult(q1.ToString(), q2.ToString(), areEqual);

            // Show conversion explanation
            ShowConversionExplanation(q1, q2, areEqual);
        }

        // Display advanced multi-unit comparison screen
        private void ShowAdvancedComparisonScreen()
        {
            Console.WriteLine("\n--- Advanced Multi-Unit Comparison ---");
            Console.WriteLine("This demonstrates transitive property across units.\n");

            // Example 1: Yards to Feet to Inches
            Console.WriteLine("Example 1: 2 Yards = 6 Feet = 72 Inches");
            bool yardToFeet = _service.AreQuantitiesEqual(
                2.0,
                LengthUnit.YARD,
                6.0,
                LengthUnit.FEET
            );
            bool feetToInches = _service.AreQuantitiesEqual(
                6.0,
                LengthUnit.FEET,
                72.0,
                LengthUnit.INCH
            );
            bool yardToInches = _service.AreQuantitiesEqual(
                2.0,
                LengthUnit.YARD,
                72.0,
                LengthUnit.INCH
            );

            Console.WriteLine($"2 yd vs 6 ft: {yardToFeet}");
            Console.WriteLine($"6 ft vs 72 in: {feetToInches}");
            Console.WriteLine($"Therefore, 2 yd vs 72 in: {yardToInches}\n");

            // Example 2: Centimeters to Inches to Feet
            Console.WriteLine("Example 2: 30.48 cm = 12 inches = 1 foot");
            bool cmToInches = _service.AreQuantitiesEqual(
                30.48,
                LengthUnit.CENTIMETER,
                12.0,
                LengthUnit.INCH
            );
            bool inchesToFeet = _service.AreQuantitiesEqual(
                12.0,
                LengthUnit.INCH,
                1.0,
                LengthUnit.FEET
            );
            bool cmToFeet = _service.AreQuantitiesEqual(
                30.48,
                LengthUnit.CENTIMETER,
                1.0,
                LengthUnit.FEET
            );

            Console.WriteLine($"30.48 cm vs 12 in: {cmToInches}");
            Console.WriteLine($"12 in vs 1 ft: {inchesToFeet}");
            Console.WriteLine($"Therefore, 30.48 cm vs 1 ft: {cmToFeet}\n");

            // Example 3: Yards to Centimeters
            Console.WriteLine("Example 3: 1 Yard = 91.44 Centimeters");
            bool yardToCm = _service.AreQuantitiesEqual(
                1.0,
                LengthUnit.YARD,
                91.44,
                LengthUnit.CENTIMETER
            );
            Console.WriteLine($"1 yd vs 91.44 cm: {yardToCm}\n");

            // Let user try their own
            Console.WriteLine("Try your own multi-unit comparison:");
            Console.WriteLine("Enter value in yards:");
            string? yardsInput = Console.ReadLine();
            Console.WriteLine("Enter value in centimeters:");
            string? cmInput = Console.ReadLine();

            if (
                double.TryParse(yardsInput, out double yards)
                && double.TryParse(cmInput, out double cm)
            )
            {
                bool result = _service.AreQuantitiesEqual(
                    yards,
                    LengthUnit.YARD,
                    cm,
                    LengthUnit.CENTIMETER
                );
                Console.WriteLine($"{yards} yd vs {cm} cm: {(result ? "Equal" : "Not Equal")}");

                if (!result)
                {
                    double expectedCm = yards * 91.44;
                    Console.WriteLine($"Note: {yards} yard(s) should equal {expectedCm} cm");
                }
            }

            Console.WriteLine("----------------------------------------\n");
        }

        // Helper method to get unit from menu choice
        private LengthUnit GetUnitFromChoice(string? choice)
        {
            return choice switch
            {
                "1" => LengthUnit.FEET,
                "2" => LengthUnit.INCH,
                "3" => LengthUnit.YARD,
                "4" => LengthUnit.CENTIMETER,
                _ => LengthUnit.FEET, // Default, will be caught by validation
            };
        }

        // Helper method to show conversion explanation
        private void ShowConversionExplanation(Quantity q1, Quantity q2, bool areEqual)
        {
            double q1InFeet = q1.Value * _unitExtensions.GetConversionFactorToFeet(q1.Unit);
            double q2InFeet = q2.Value * _unitExtensions.GetConversionFactorToFeet(q2.Unit);

            Console.WriteLine("\n--- Conversion Details ---");
            Console.WriteLine($"{q1} = {q1InFeet:F6} feet");
            Console.WriteLine($"{q2} = {q2InFeet:F6} feet");
            Console.WriteLine($"Difference: {Math.Abs(q1InFeet - q2InFeet):F6} feet");

            if (areEqual)
            {
                Console.WriteLine("These measurements are equivalent!");
            }
            else
            {
                Console.WriteLine("These measurements are NOT equivalent.");
            }
            Console.WriteLine();
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
