// Import required namespaces for file operations, LINQ, Entity Framework, and configuration
using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

// Define the namespace for database context factory
namespace QuantityMeasurementRepositoryLayer.Data
{
    /// <summary>
    /// Factory for creating ApplicationDbContext at design time (for migrations)
    /// This class is used by EF Core tools when running migrations (Add-Migration, Update-Database)
    /// It implements IDesignTimeDbContextFactory to tell EF Core how to create the DbContext at design time
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <summary>
        /// Creates a new instance of ApplicationDbContext for design-time tools
        /// This method is called by EF Core when running migrations
        /// </summary>
        /// <param name="args">Command line arguments (not used)</param>
        /// <returns>Configured ApplicationDbContext instance</returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Step 1: Find the startup project (Web API) path to read appsettings.json
            string basePath = FindStartupProjectPath();

            // Log the path being used (visible in Package Manager Console)
            Console.WriteLine($"Using base path: {basePath}");

            // Step 2: Build configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath) // Set base path for configuration files
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Required main config
                .AddJsonFile($"appsettings.{GetEnvironment()}.json", optional: true) // Optional environment-specific config
                .Build();

            // Step 3: Create DbContext options builder
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Step 4: Get connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Step 5: Validate connection string exists
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found. "
                        + $"Please ensure appsettings.json exists in {basePath}"
                );
            }

            // Log connection string (masked for security, but visible for debugging)
            Console.WriteLine($"Using connection string: {connectionString}");

            // Step 6: Configure SQL Server options
            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    // Specify which assembly contains the migrations
                    // This is important when migrations are in a different project than the startup project
                    sqlOptions.MigrationsAssembly("QuantityMeasurementRepositoryLayer");

                    // Set command timeout to 60 seconds (longer for migrations)
                    sqlOptions.CommandTimeout(60);
                }
            );

            // Step 7: Create and return the DbContext instance
            return new ApplicationDbContext(optionsBuilder.Options);
        }

        /// <summary>
        /// Finds the path to the startup project (Web API) that contains appsettings.json
        /// This is necessary because migrations run from the Repository layer but need
        /// configuration from the Web API project
        /// </summary>
        /// <returns>Full path to the startup project directory</returns>
        private string FindStartupProjectPath()
        {
            // Start from current directory (RepositoryLayer)
            string currentDirectory = Directory.GetCurrentDirectory();

            // Strategy 1: Look for Web API project in parent directory
            // Common structure: Solution/RepositoryLayer and Solution/WebAPI
            var parentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            if (parentDirectory != null)
            {
                var webApiPath = Path.Combine(parentDirectory, "QuantityMeasurementWebAPI");
                // Check if directory exists and contains appsettings.json
                if (
                    Directory.Exists(webApiPath)
                    && File.Exists(Path.Combine(webApiPath, "appsettings.json"))
                )
                {
                    return webApiPath;
                }
            }

            // Strategy 2: Find solution root and then Web API
            // Navigate up until we find .sln file, then look for Web API project
            var solutionRoot = FindSolutionRoot(currentDirectory);
            if (solutionRoot != null)
            {
                var webApiPath = Path.Combine(solutionRoot, "QuantityMeasurementWebAPI");
                if (
                    Directory.Exists(webApiPath)
                    && File.Exists(Path.Combine(webApiPath, "appsettings.json"))
                )
                {
                    return webApiPath;
                }
            }

            // Strategy 3: Try current directory (if running from Web API project)
            if (File.Exists(Path.Combine(currentDirectory, "appsettings.json")))
            {
                return currentDirectory;
            }

            // If all strategies fail, throw exception
            throw new FileNotFoundException(
                "Could not find appsettings.json. Please ensure it exists in the QuantityMeasurementWebAPI project."
            );
        }

        /// <summary>
        /// Finds the solution root directory by looking for .sln file
        /// Traverses up the directory tree until it finds a .sln file or reaches root
        /// </summary>
        /// <param name="startPath">Starting directory path</param>
        /// <returns>Solution root path or null if not found</returns>
        private string? FindSolutionRoot(string startPath)
        {
            var directory = new DirectoryInfo(startPath);
            while (directory != null)
            {
                // Check if current directory contains any .sln files
                if (directory.GetFiles("*.sln").Any())
                {
                    return directory.FullName;
                }
                // Move up to parent directory
                directory = directory.Parent;
            }
            return null; // No solution file found
        }

        /// <summary>
        /// Gets the current environment (Development, Staging, Production)
        /// Defaults to "Development" if not set
        /// </summary>
        /// <returns>Environment name</returns>
        private string GetEnvironment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        }
    }
}
