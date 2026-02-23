using System;
using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp.Views
{
    /// <summary>
    /// View class responsible for all user interface interactions.
    /// Handles displaying menus, getting user input, and showing results.
    /// </summary>
    public class Menu
    {
        private readonly QuantityMeasurementService _service;
        private readonly UnitConverter _unitConverter;

        /// <summary>
        /// Initializes a new instance of the Menu class.
        /// </summary>
        public Menu()
        {
            _service = new QuantityMeasurementService();
            _unitConverter = new UnitConverter();
        }

        /// <summary>
        /// Displays the main menu and handles user interaction.
        /// </summary>
        public void Display()
        {
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║     QUANTITY MEASUREMENT APP           ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();

            while (true)
            {
                ShowMainMenu();

                string? choice = Console.ReadLine();

                if (choice == "7")
                    break;

                ProcessMainMenuChoice(choice);
            }

            Console.WriteLine("\nThank you for using Quantity Measurement Application!");
        }

        /// <summary>
        /// Displays the main menu options.
        /// </summary>
        private void ShowMainMenu()
        {
            Console.WriteLine("┌────────────────────────────────────┐");
            Console.WriteLine("│            MAIN MENU               │");
            Console.WriteLine("├────────────────────────────────────┤");
            Console.WriteLine("│ 1. Convert Units                   │");
            Console.WriteLine("│ 2. Compare Measurements            │");
            Console.WriteLine("│ 3. Add Measurements                │");
            Console.WriteLine("│ 4. View Commutativity Demo         │");
            Console.WriteLine("│ 5. Batch Operations                │");
            Console.WriteLine("│ 6. Legacy Mode (Original Classes)  │");
            Console.WriteLine("│ 7. Exit                            │");
            Console.WriteLine("└────────────────────────────────────┘");
            Console.Write("Enter your choice (1-7): ");
        }

        /// <summary>
        /// Processes the user's main menu choice.
        /// </summary>
        /// <param name="choice">The user's menu choice.</param>
        private void ProcessMainMenuChoice(string? choice)
        {
            switch (choice)
            {
                case "1":
                    ShowConversionScreen();
                    break;
                case "2":
                    ShowEqualityComparisonScreen();
                    break;
                case "3":
                    ShowAdditionScreen();
                    break;
                case "4":
                    ShowCommutativityDemo();
                    break;
                case "5":
                    ShowBatchOperationsScreen();
                    break;
                case "6":
                    ShowLegacyModeScreen();
                    break;
                default:
                    Console.WriteLine("❌ Invalid choice! Please try again.\n");
                    break;
            }
        }

        /// <summary>
        /// Displays the unit conversion screen.
        /// </summary>
        private void ShowConversionScreen()
        {
            Console.WriteLine("\n┌────────────────────────────────────┐");
            Console.WriteLine("│         UNIT CONVERSION            │");
            Console.WriteLine("└────────────────────────────────────┘\n");

            try
            {
                // Get source unit
                LengthUnit sourceUnit = SelectUnit("Select SOURCE unit:");

                // Get target unit
                LengthUnit targetUnit = SelectUnit("Select TARGET unit:");

                // Get value to convert
                Console.Write($"\nEnter value in {sourceUnit.GetUnitName()}: ");
                string? valueInput = Console.ReadLine();

                if (double.TryParse(valueInput, out double value))
                {
                    double result = Quantity.Convert(value, sourceUnit, targetUnit);

                    Console.WriteLine("\n┌────────────────────────────────────┐");
                    Console.WriteLine("│          CONVERSION RESULT         │");
                    Console.WriteLine("├────────────────────────────────────┤");
                    Console.WriteLine(
                        $"│ {value} {sourceUnit.GetUnitSymbol(), -3} = {result, 10:F6} {targetUnit.GetUnitSymbol(), -3} │"
                    );
                    Console.WriteLine("└────────────────────────────────────┘\n");
                }
                else
                {
                    Console.WriteLine("❌ Invalid numeric value!\n");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Displays the equality comparison screen.
        /// </summary>
        private void ShowEqualityComparisonScreen()
        {
            Console.WriteLine("\n┌────────────────────────────────────┐");
            Console.WriteLine("│      MEASUREMENT COMPARISON        │");
            Console.WriteLine("└────────────────────────────────────┘\n");

            try
            {
                // Get first measurement
                Console.WriteLine("--- FIRST MEASUREMENT ---");
                LengthUnit unit1 = SelectUnit("Select unit:");
                Console.Write($"Enter value in {unit1.GetUnitName()}: ");
                string? value1Input = Console.ReadLine();

                // Get second measurement
                Console.WriteLine("\n--- SECOND MEASUREMENT ---");
                LengthUnit unit2 = SelectUnit("Select unit:");
                Console.Write($"Enter value in {unit2.GetUnitName()}: ");
                string? value2Input = Console.ReadLine();

                if (
                    double.TryParse(value1Input, out double value1)
                    && double.TryParse(value2Input, out double value2)
                )
                {
                    var q1 = new Quantity(value1, unit1);
                    var q2 = new Quantity(value2, unit2);

                    bool areEqual = q1.Equals(q2);

                    Console.WriteLine("\n┌────────────────────────────────────┐");
                    Console.WriteLine("│         COMPARISON RESULT          │");
                    Console.WriteLine("├────────────────────────────────────┤");
                    Console.WriteLine($"│ {q1, -12} vs {q2, -12} │");
                    Console.WriteLine($"│                                     │");

                    if (areEqual)
                    {
                        Console.WriteLine("│        ✅ Measurements are EQUAL        │");
                    }
                    else
                    {
                        Console.WriteLine("│      ❌ Measurements are NOT EQUAL      │");
                    }

                    Console.WriteLine("├────────────────────────────────────┤");
                    Console.WriteLine($"│ Both in feet:                         │");
                    Console.WriteLine(
                        $"│   First:  {q1.ConvertTo(LengthUnit.FEET).Value, 10:F6} ft    │"
                    );
                    Console.WriteLine(
                        $"│   Second: {q2.ConvertTo(LengthUnit.FEET).Value, 10:F6} ft    │"
                    );
                    Console.WriteLine("└────────────────────────────────────┘\n");
                }
                else
                {
                    Console.WriteLine("❌ Invalid numeric value(s)!\n");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Displays the addition operation screen.
        /// </summary>
        private void ShowAdditionScreen()
        {
            Console.WriteLine("\n┌────────────────────────────────────┐");
            Console.WriteLine("│         MEASUREMENT ADDITION       │");
            Console.WriteLine("└────────────────────────────────────┘\n");

            try
            {
                // Get first measurement
                Console.WriteLine("--- FIRST MEASUREMENT ---");
                LengthUnit unit1 = SelectUnit("Select unit:");
                Console.Write($"Enter value in {unit1.GetUnitName()}: ");
                string? value1Input = Console.ReadLine();

                // Get second measurement
                Console.WriteLine("\n--- SECOND MEASUREMENT ---");
                LengthUnit unit2 = SelectUnit("Select unit:");
                Console.Write($"Enter value in {unit2.GetUnitName()}: ");
                string? value2Input = Console.ReadLine();

                // Get result unit
                Console.WriteLine("\n--- RESULT UNIT ---");
                LengthUnit resultUnit = SelectUnit("Select unit for result:");

                if (
                    double.TryParse(value1Input, out double value1)
                    && double.TryParse(value2Input, out double value2)
                )
                {
                    var q1 = new Quantity(value1, unit1);
                    var q2 = new Quantity(value2, unit2);

                    var sum = Quantity.Add(q1, q2, resultUnit);

                    Console.WriteLine("\n┌────────────────────────────────────┐");
                    Console.WriteLine("│           ADDITION RESULT          │");
                    Console.WriteLine("├────────────────────────────────────┤");
                    Console.WriteLine($"│ {q1, -8} + {q2, -8}                 │");
                    Console.WriteLine($"│                                     │");
                    Console.WriteLine(
                        $"│ = {sum.Value, 10:F6} {sum.Unit.GetUnitSymbol(), -3}                │"
                    );
                    Console.WriteLine("├────────────────────────────────────┤");

                    // Show calculation details
                    Quantity q1InFeet = q1.ConvertTo(LengthUnit.FEET);
                    Quantity q2InFeet = q2.ConvertTo(LengthUnit.FEET);
                    double sumInFeet = q1InFeet.Value + q2InFeet.Value;

                    Console.WriteLine($"│ Calculation:                         │");
                    Console.WriteLine(
                        $"│   {q1InFeet.Value, 8:F6} ft + {q2InFeet.Value, 8:F6} ft = {sumInFeet, 8:F6} ft │"
                    );
                    Console.WriteLine("└────────────────────────────────────┘\n");
                }
                else
                {
                    Console.WriteLine("❌ Invalid numeric value(s)!\n");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Displays commutativity demonstration with user input.
        /// </summary>
        private void ShowCommutativityDemo()
        {
            Console.WriteLine("\n┌────────────────────────────────────┐");
            Console.WriteLine("│     COMMUTATIVITY DEMONSTRATION    │");
            Console.WriteLine("│        (a + b = b + a)             │");
            Console.WriteLine("└────────────────────────────────────┘\n");

            try
            {
                // Get first measurement
                Console.WriteLine("--- FIRST MEASUREMENT ---");
                LengthUnit unit1 = SelectUnit("Select unit:");
                Console.Write($"Enter value in {unit1.GetUnitName()}: ");
                string? value1Input = Console.ReadLine();

                // Get second measurement
                Console.WriteLine("\n--- SECOND MEASUREMENT ---");
                LengthUnit unit2 = SelectUnit("Select unit:");
                Console.Write($"Enter value in {unit2.GetUnitName()}: ");
                string? value2Input = Console.ReadLine();

                if (
                    double.TryParse(value1Input, out double value1)
                    && double.TryParse(value2Input, out double value2)
                )
                {
                    var a = new Quantity(value1, unit1);
                    var b = new Quantity(value2, unit2);

                    // Add in both orders
                    var aPlusB = a.Add(b); // Result in unit1
                    var bPlusA = b.Add(a); // Result in unit2

                    Console.WriteLine("\n┌────────────────────────────────────┐");
                    Console.WriteLine("│         COMMUTATIVITY CHECK        │");
                    Console.WriteLine("├────────────────────────────────────┤");
                    Console.WriteLine($"│ a = {a, -10} b = {b, -10} │");
                    Console.WriteLine($"│                                     │");
                    Console.WriteLine(
                        $"│ a + b = {aPlusB.Value, 10:F6} {aPlusB.Unit.GetUnitSymbol(), -3}            │"
                    );
                    Console.WriteLine(
                        $"│ b + a = {bPlusA.Value, 10:F6} {bPlusA.Unit.GetUnitSymbol(), -3}            │"
                    );
                    Console.WriteLine("├────────────────────────────────────┤");

                    // Check if they represent the same physical quantity
                    bool areEqual = aPlusB
                        .ConvertTo(LengthUnit.FEET)
                        .Equals(bPlusA.ConvertTo(LengthUnit.FEET));

                    if (areEqual)
                    {
                        Console.WriteLine("│ ✅ Addition is COMMUTATIVE!            │");
                        Console.WriteLine("│    a + b = b + a in physical terms    │");
                    }
                    else
                    {
                        Console.WriteLine("│ ❌ Addition is NOT commutative!        │");
                    }
                    Console.WriteLine("└────────────────────────────────────┘\n");
                }
                else
                {
                    Console.WriteLine("❌ Invalid numeric value(s)!\n");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Displays batch operations screen.
        /// </summary>
        private void ShowBatchOperationsScreen()
        {
            Console.WriteLine("\n┌────────────────────────────────────┐");
            Console.WriteLine("│         BATCH OPERATIONS           │");
            Console.WriteLine("├────────────────────────────────────┤");
            Console.WriteLine("│ 1. Batch Unit Conversion           │");
            Console.WriteLine("│ 2. Batch Addition Demo             │");
            Console.WriteLine("│ 3. Back to Main Menu               │");
            Console.WriteLine("└────────────────────────────────────┘");
            Console.Write("Enter your choice (1-3): ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowBatchConversion();
                    break;
                case "2":
                    ShowBatchAdditionDemo();
                    break;
                default:
                    Console.WriteLine("Returning to main menu...\n");
                    break;
            }
        }

        /// <summary>
        /// Displays batch conversion with user-defined values.
        /// </summary>
        private void ShowBatchConversion()
        {
            Console.WriteLine("\n┌────────────────────────────────────┐");
            Console.WriteLine("│        BATCH UNIT CONVERSION       │");
            Console.WriteLine("└────────────────────────────────────┘\n");

            try
            {
                Console.Write("Enter values to convert (comma-separated, e.g., 1,2.5,3.7): ");
                string? valuesInput = Console.ReadLine();

                LengthUnit sourceUnit = SelectUnit("Select SOURCE unit:");
                LengthUnit targetUnit = SelectUnit("Select TARGET unit:");

                if (!string.IsNullOrWhiteSpace(valuesInput))
                {
                    string[] valueStrings = valuesInput.Split(
                        ',',
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    Console.WriteLine("\n┌────────────────────────────────────┐");
                    Console.WriteLine("│         CONVERSION RESULTS         │");
                    Console.WriteLine("├────────────────────────────────────┤");

                    foreach (string valueStr in valueStrings)
                    {
                        if (double.TryParse(valueStr.Trim(), out double value))
                        {
                            double result = Quantity.Convert(value, sourceUnit, targetUnit);
                            Console.WriteLine(
                                $"│ {value, 8:F3} {sourceUnit.GetUnitSymbol(), -3} = {result, 10:F6} {targetUnit.GetUnitSymbol(), -3} │"
                            );
                        }
                    }

                    Console.WriteLine("└────────────────────────────────────┘\n");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Displays batch addition demonstration with predefined combinations.
        /// </summary>
        private void ShowBatchAdditionDemo()
        {
            Console.WriteLine("\n┌────────────────────────────────────┐");
            Console.WriteLine("│        BATCH ADDITION DEMO         │");
            Console.WriteLine("└────────────────────────────────────┘\n");

            try
            {
                Console.Write("Enter a base value (e.g., 2): ");
                string? baseValueInput = Console.ReadLine();

                if (!double.TryParse(baseValueInput, out double baseValue))
                {
                    Console.WriteLine("❌ Invalid base value!\n");
                    return;
                }

                LengthUnit[] units =
                {
                    LengthUnit.FEET,
                    LengthUnit.INCH,
                    LengthUnit.YARD,
                    LengthUnit.CENTIMETER,
                };

                Console.WriteLine($"\nAdding {baseValue} to various units:");
                Console.WriteLine("┌────────────────────────────────────┐");

                foreach (var unit in units)
                {
                    var q = new Quantity(baseValue, unit);
                    var oneUnit = new Quantity(1.0, unit);
                    var sum = q.Add(oneUnit);

                    Console.WriteLine(
                        $"│ {q, -8} + 1 {unit.GetUnitSymbol(), -2} = {sum.Value, 10:F6} {unit.GetUnitSymbol(), -3} │"
                    );
                }

                Console.WriteLine("└────────────────────────────────────┘\n");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Displays legacy mode screen (original classes).
        /// </summary>
        private void ShowLegacyModeScreen()
        {
            Console.WriteLine("\n┌────────────────────────────────────┐");
            Console.WriteLine("│          LEGACY MODE                │");
            Console.WriteLine("│   (Original Feet/Inch Classes)     │");
            Console.WriteLine("├────────────────────────────────────┤");
            Console.WriteLine("│ 1. Compare Feet                    │");
            Console.WriteLine("│ 2. Compare Inches                  │");
            Console.WriteLine("│ 3. Back to Main Menu               │");
            Console.WriteLine("└────────────────────────────────────┘");
            Console.Write("Enter your choice (1-3): ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowLegacyFeetComparison();
                    break;
                case "2":
                    ShowLegacyInchComparison();
                    break;
                default:
                    Console.WriteLine("Returning to main menu...\n");
                    break;
            }
        }

        /// <summary>
        /// Displays legacy feet comparison screen.
        /// </summary>
        private void ShowLegacyFeetComparison()
        {
            Console.WriteLine("\n--- Feet Comparison (Legacy) ---\n");

            Console.Write("Enter first measurement in feet: ");
            string? input1 = Console.ReadLine();

            Console.Write("Enter second measurement in feet: ");
            string? input2 = Console.ReadLine();

            Feet? feet1 = _service.ParseFeetInput(input1);
            Feet? feet2 = _service.ParseFeetInput(input2);

            if (feet1 is null || feet2 is null)
            {
                Console.WriteLine("❌ Invalid input! Please enter valid numeric values.\n");
                return;
            }

            bool areEqual = _service.CompareFeetEquality(feet1, feet2);

            Console.WriteLine($"\n{feet1} vs {feet2}: {(areEqual ? "✅ Equal" : "❌ Not Equal")}\n");
        }

        /// <summary>
        /// Displays legacy inch comparison screen.
        /// </summary>
        private void ShowLegacyInchComparison()
        {
            Console.WriteLine("\n--- Inch Comparison (Legacy) ---\n");

            Console.Write("Enter first measurement in inches: ");
            string? input1 = Console.ReadLine();

            Console.Write("Enter second measurement in inches: ");
            string? input2 = Console.ReadLine();

            Inch? inch1 = _service.ParseInchInput(input1);
            Inch? inch2 = _service.ParseInchInput(input2);

            if (inch1 is null || inch2 is null)
            {
                Console.WriteLine("❌ Invalid input! Please enter valid numeric values.\n");
                return;
            }

            bool areEqual = _service.CompareInchEquality(inch1, inch2);

            Console.WriteLine($"\n{inch1} vs {inch2}: {(areEqual ? "✅ Equal" : "❌ Not Equal")}\n");
        }

        /// <summary>
        /// Helper method to let user select a unit from a menu.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <returns>The selected LengthUnit.</returns>
        private LengthUnit SelectUnit(string prompt)
        {
            Console.WriteLine($"\n{prompt}");
            Console.WriteLine("  1. Feet (ft)");
            Console.WriteLine("  2. Inches (in)");
            Console.WriteLine("  3. Yards (yd)");
            Console.WriteLine("  4. Centimeters (cm)");
            Console.Write("Enter choice (1-4): ");

            string? choice = Console.ReadLine();

            return choice switch
            {
                "1" => LengthUnit.FEET,
                "2" => LengthUnit.INCH,
                "3" => LengthUnit.YARD,
                "4" => LengthUnit.CENTIMETER,
                _ => throw new ArgumentException("Invalid unit choice"),
            };
        }
    }
}
