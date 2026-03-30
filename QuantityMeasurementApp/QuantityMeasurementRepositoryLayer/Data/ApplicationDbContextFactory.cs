using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace QuantityMeasurementRepositoryLayer.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Find the Web API project path
            string basePath = FindStartupProjectPath();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string not found");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly("QuantityMeasurementRepositoryLayer")
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        private string FindStartupProjectPath()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var parentDir = Directory.GetParent(currentDir)?.FullName;

            // Check parent directory for Web API project
            if (parentDir != null)
            {
                var webApiPath = Path.Combine(parentDir, "QuantityMeasurementWebAPI");
                if (
                    Directory.Exists(webApiPath)
                    && File.Exists(Path.Combine(webApiPath, "appsettings.json"))
                )
                    return webApiPath;
            }

            // Check current directory
            if (File.Exists(Path.Combine(currentDir, "appsettings.json")))
                return currentDir;

            throw new FileNotFoundException("appsettings.json not found");
        }
    }
}
