using QuantityMeasurementApp.Models;
using QuantityMeasurementApp.Services;

namespace QuantityMeasurementApp
{
    // Main program class for the Quantity Measurement Application
    // This class handles user interaction through the console
    class Program
    {
        // Entry point of the application
        static void Main(string[] args)
        {
            // Display application header
            Console.WriteLine("=== Quantity Measurement Application ===");
            Console.WriteLine("UC1: Feet Measurement Equality\n");

            // Create an instance of the service to handle business logic
            var service = new QuantityMeasurementService();

            // Main application loop - continues until user types 'exit'
            while (true)
            {
                // Prompt user for first measurement
                Console.WriteLine("Enter first measurement in feet (or 'exit' to quit):");
                string? input1 = Console.ReadLine();

                // Check if user wants to exit
                if (input1?.ToLower() == "exit")
                    break;

                // Prompt user for second measurement
                Console.WriteLine("Enter second measurement in feet:");
                string? input2 = Console.ReadLine();

                // Check if user wants to exit
                if (input2?.ToLower() == "exit")
                    break;

                // Parse the input strings into Feet objects
                Feet? feet1 = service.ParseFeetInput(input1);
                Feet? feet2 = service.ParseFeetInput(input2);

                // Validate that both inputs were successfully parsed
                if (feet1 is null || feet2 is null)
                {
                    Console.WriteLine("Invalid input! Please enter valid numeric values.\n");
                    continue; // Go back to the beginning of the loop
                }

                // Compare the two measurements
                bool areEqual = service.CompareFeetEquality(feet1, feet2);

                // Display the results
                Console.WriteLine($"\nFirst measurement: {feet1}");
                Console.WriteLine($"Second measurement: {feet2}");
                Console.WriteLine(
                    $"Are they equal? {areEqual} ({(areEqual ? "Equal" : "Not Equal")})\n"
                );
                Console.WriteLine("----------------------------------------\n");
            }

            // Farewell message when user exits
            Console.WriteLine("Thank you for using Quantity Measurement Application!");
        }
    }
}
