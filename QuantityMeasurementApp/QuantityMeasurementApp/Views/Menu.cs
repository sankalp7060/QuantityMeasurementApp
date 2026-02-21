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

        /// <summary>
        /// Initializes a new instance of the Menu class.
        /// </summary>
        public Menu()
        {
            _service = new QuantityMeasurementService();
        }

        /// <summary>
        /// Displays the main menu and handles user interaction.
        /// </summary>
        public void Display()
        {
            Console.WriteLine("=== Quantity Measurement Application ===");
            Console.WriteLine("UC5: Unit-to-Unit Conversion (Same Measurement Type)\n");

            ShowStaticExamples();

            while (true)
            {
                ShowMainMenu();

                string? choice = Console.ReadLine();

                if (choice == "6" || choice?.ToLower() == "exit")
                    break;

                ProcessMainMenuChoice(choice);
            }

            Console.WriteLine("\nThank you for using Quantity Measurement Application!");
        }

        /// <summary>
        /// Displays static method examples for conversion and equality.
        /// </summary>
        private void ShowStaticExamples()
        {
            Console.WriteLine("--- Conversion Examples ---");
            Console.WriteLine(
                $"1.0 FEET to INCHES: {Quantity.Convert(1.0, LengthUnit.FEET, LengthUnit.INCH):F6} inches"
            );
            Console.WriteLine(
                $"3.0 YARDS to FEET: {Quantity.Convert(3.0, LengthUnit.YARD, LengthUnit.FEET):F6} feet"
            );
            Console.WriteLine(
                $"36.0 INCHES to YARDS: {Quantity.Convert(36.0, LengthUnit.INCH, LengthUnit.YARD):F6} yards"
            );
            Console.WriteLine(
                $"1.0 CENTIMETERS to INCHES: {Quantity.Convert(1.0, LengthUnit.CENTIMETER, LengthUnit.INCH):F6} inches"
            );
            Console.WriteLine(
                $"30.48 CENTIMETERS to FEET: {Quantity.Convert(30.48, LengthUnit.CENTIMETER, LengthUnit.FEET):F6} feet"
            );
            Console.WriteLine();

            Console.WriteLine("--- Equality Examples ---");
            var q1 = new Quantity(1.0, LengthUnit.FEET);
            var q2 = new Quantity(12.0, LengthUnit.INCH);
            Console.WriteLine($"1 ft vs 12 in: {q1.Equals(q2)}");

            var q3 = new Quantity(1.0, LengthUnit.YARD);
            var q4 = new Quantity(36.0, LengthUnit.INCH);
            Console.WriteLine($"1 yd vs 36 in: {q3.Equals(q4)}");

            var q5 = new Quantity(1.0, LengthUnit.CENTIMETER);
            var q6 = new Quantity(0.393701, LengthUnit.INCH);
            Console.WriteLine($"1 cm vs 0.393701 in: {q5.Equals(q6)}\n");
        }

        /// <summary>
        /// Displays the main menu options.
        /// </summary>
        private void ShowMainMenu()
        {
            Console.WriteLine("Choose operation:");
            Console.WriteLine("1. Unit Conversion");
            Console.WriteLine("2. Equality Comparison");
            Console.WriteLine("3. Round-trip Conversion Demo");
            Console.WriteLine("4. Batch Conversion Demo");
            Console.WriteLine("5. Backward compatibility (Original Feet/Inch classes)");
            Console.WriteLine("6. Exit");
            Console.Write("Enter your choice (1-6): ");
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
                    ShowRoundTripConversionDemo();
                    break;
                case "4":
                    ShowBatchConversionDemo();
                    break;
                case "5":
                    ShowBackwardCompatibilityScreen();
                    break;
                default:
                    Console.WriteLine("Invalid choice! Please try again.\n");
                    break;
            }
        }

        /// <summary>
        /// Displays the unit conversion screen.
        /// </summary>
        private void ShowConversionScreen()
        {
            Console.WriteLine("\n--- Unit Conversion ---");

            Console.WriteLine("Select source unit:");
            Console.WriteLine("1. Feet");
            Console.WriteLine("2. Inches");
            Console.WriteLine("3. Yards");
            Console.WriteLine("4. Centimeters");
            Console.Write("Enter choice (1-4): ");

            string? sourceChoice = Console.ReadLine();
            LengthUnit sourceUnit = GetUnitFromChoice(sourceChoice);

            if (sourceUnit == LengthUnit.FEET && sourceChoice != "1")
            {
                ShowErrorMessage("Invalid source unit choice!");
                return;
            }

            Console.WriteLine("\nSelect target unit:");
            Console.WriteLine("1. Feet");
            Console.WriteLine("2. Inches");
            Console.WriteLine("3. Yards");
            Console.WriteLine("4. Centimeters");
            Console.Write("Enter choice (1-4): ");

            string? targetChoice = Console.ReadLine();
            LengthUnit targetUnit = GetUnitFromChoice(targetChoice);

            if (targetUnit == LengthUnit.FEET && targetChoice != "1")
            {
                ShowErrorMessage("Invalid target unit choice!");
                return;
            }

            Console.WriteLine($"\nEnter value in {sourceUnit.GetUnitName()}:");
            string? valueInput = Console.ReadLine();

            if (double.TryParse(valueInput, out double value))
            {
                try
                {
                    double result = Quantity.Convert(value, sourceUnit, targetUnit);
                    Console.WriteLine(
                        $"\n{value} {sourceUnit.GetUnitSymbol()} = {result:F6} {targetUnit.GetUnitSymbol()}"
                    );

                    // Show conversion formula
                    ShowConversionFormula(value, sourceUnit, targetUnit, result);
                }
                catch (ArgumentException ex)
                {
                    ShowErrorMessage($"Conversion error: {ex.Message}");
                }
            }
            else
            {
                ShowErrorMessage("Invalid numeric value!");
            }

            Console.WriteLine("----------------------------------------\n");
        }

        /// <summary>
        /// Displays the conversion formula.
        /// </summary>
        private void ShowConversionFormula(
            double value,
            LengthUnit source,
            LengthUnit target,
            double result
        )
        {
            double sourceToFeet = source.GetConversionFactorToFeet();
            double targetToFeet = target.GetConversionFactorToFeet();

            Console.WriteLine($"\nConversion formula:");
            Console.WriteLine(
                $"{value} {source.GetUnitSymbol()} × ({sourceToFeet:F6} / {targetToFeet:F6}) = {result:F6} {target.GetUnitSymbol()}"
            );
        }

        /// <summary>
        /// Displays the equality comparison screen.
        /// </summary>
        private void ShowEqualityComparisonScreen()
        {
            Console.WriteLine("\n--- Equality Comparison ---");

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

            Console.WriteLine($"\nEnter value in {unit1.GetUnitName()}:");
            string? value1Input = Console.ReadLine();

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

            Console.WriteLine($"\nEnter value in {unit2.GetUnitName()}:");
            string? value2Input = Console.ReadLine();

            if (
                double.TryParse(value1Input, out double value1)
                && double.TryParse(value2Input, out double value2)
            )
            {
                try
                {
                    var q1 = new Quantity(value1, unit1);
                    var q2 = new Quantity(value2, unit2);

                    bool areEqual = q1.Equals(q2);

                    Console.WriteLine($"\nFirst: {q1}");
                    Console.WriteLine($"Second: {q2}");
                    Console.WriteLine(
                        $"Are they equal? {areEqual} ({(areEqual ? "Equal" : "Not Equal")})"
                    );

                    // Show converted values
                    Console.WriteLine($"\nBoth in feet:");
                    Console.WriteLine(
                        $"First in feet: {q1.ConvertTo(LengthUnit.FEET).Value:F6} ft"
                    );
                    Console.WriteLine(
                        $"Second in feet: {q2.ConvertTo(LengthUnit.FEET).Value:F6} ft"
                    );
                }
                catch (ArgumentException ex)
                {
                    ShowErrorMessage($"Error: {ex.Message}");
                }
            }
            else
            {
                ShowErrorMessage("Invalid numeric values!");
            }

            Console.WriteLine("----------------------------------------\n");
        }

        /// <summary>
        /// Displays round-trip conversion demonstration.
        /// </summary>
        private void ShowRoundTripConversionDemo()
        {
            Console.WriteLine("\n--- Round-trip Conversion Demo ---");
            Console.WriteLine(
                "This demonstrates that converting A→B→A returns the original value.\n"
            );

            double[] testValues = { 1.0, 2.5, 10.0, -3.0 };
            LengthUnit[] units =
            {
                LengthUnit.FEET,
                LengthUnit.INCH,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER,
            };

            foreach (double originalValue in testValues)
            {
                foreach (LengthUnit source in units)
                {
                    foreach (LengthUnit target in units)
                    {
                        if (source != target)
                        {
                            try
                            {
                                double converted = Quantity.Convert(originalValue, source, target);
                                double roundTrip = Quantity.Convert(converted, target, source);

                                bool preservesValue =
                                    Math.Abs(originalValue - roundTrip) < 0.000001;

                                Console.WriteLine(
                                    $"{originalValue} {source.GetUnitSymbol()} → {target.GetUnitSymbol()} → {source.GetUnitSymbol()}: "
                                        + $"{converted:F6} → {roundTrip:F6} {(preservesValue ? "✓" : "✗")}"
                                );
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                    }
                }
            }

            Console.WriteLine(
                "\nAll round-trip conversions preserve the original value within tolerance.\n"
            );
            Console.WriteLine("----------------------------------------\n");
        }

        /// <summary>
        /// Displays batch conversion demonstration.
        /// </summary>
        private void ShowBatchConversionDemo()
        {
            Console.WriteLine("\n--- Batch Conversion Demo ---");
            Console.WriteLine("Converting common measurements between all units:\n");

            double[] testValues = { 1, 12, 36, 2.54, 30.48, 91.44 };
            LengthUnit[] units =
            {
                LengthUnit.FEET,
                LengthUnit.INCH,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER,
            };

            foreach (double value in testValues)
            {
                foreach (LengthUnit source in units)
                {
                    Console.WriteLine($"\n{value} {source.GetUnitSymbol()} equals:");
                    foreach (LengthUnit target in units)
                    {
                        if (source != target)
                        {
                            try
                            {
                                double result = Quantity.Convert(value, source, target);
                                Console.WriteLine($"  {result:F6} {target.GetUnitSymbol()}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(
                                    $"  Error converting to {target.GetUnitSymbol()}: {ex.Message}"
                                );
                            }
                        }
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("----------------------------------------\n");
        }

        /// <summary>
        /// Displays backward compatibility screen using original classes.
        /// </summary>
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

        /// <summary>
        /// Displays feet comparison screen.
        /// </summary>
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

        /// <summary>
        /// Displays inch comparison screen.
        /// </summary>
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

        /// <summary>
        /// Gets unit from menu choice.
        /// </summary>
        /// <param name="choice">The menu choice.</param>
        /// <returns>The corresponding LengthUnit.</returns>
        private LengthUnit GetUnitFromChoice(string? choice)
        {
            return choice switch
            {
                "1" => LengthUnit.FEET,
                "2" => LengthUnit.INCH,
                "3" => LengthUnit.YARD,
                "4" => LengthUnit.CENTIMETER,
                _ => LengthUnit.FEET,
            };
        }

        /// <summary>
        /// Gets user input with prompt.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <returns>The user's input.</returns>
        private string? GetUserInput(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }

        /// <summary>
        /// Displays comparison result.
        /// </summary>
        private void ShowComparisonResult(string measurement1, string measurement2, bool areEqual)
        {
            Console.WriteLine($"\nFirst measurement: {measurement1}");
            Console.WriteLine($"Second measurement: {measurement2}");
            Console.WriteLine(
                $"Are they equal? {areEqual} ({(areEqual ? "Equal" : "Not Equal")})\n"
            );
            Console.WriteLine("----------------------------------------\n");
        }

        /// <summary>
        /// Displays error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        private void ShowErrorMessage(string message)
        {
            Console.WriteLine($"{message}\n");
        }
    }
}
