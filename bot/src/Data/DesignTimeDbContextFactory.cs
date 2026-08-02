using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LundBot.Data
{
    public sealed class LundBotDiscordDbContextFactory
        : IDesignTimeDbContextFactory<LundBotDiscordDbContext>
    {
        public LundBotDiscordDbContext CreateDbContext(string[] args)
        {
            string environment =
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? "Development";
            string basePath = ResolveConfigurationBasePath();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            string? connectionString = configuration.GetSection("Database")["ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Database:ConnectionString is not configured for EF design-time. Ensure appsettings.Development.json (or the file matching DOTNET_ENVIRONMENT/ASPNETCORE_ENVIRONMENT) contains Database.ConnectionString."
                );
            }

            var optionsBuilder = new DbContextOptionsBuilder<LundBotDiscordDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new LundBotDiscordDbContext(optionsBuilder.Options);
        }

        private static string ResolveConfigurationBasePath()
        {
            var candidateDirectories = new[]
            {
                Directory.GetCurrentDirectory(),
                AppContext.BaseDirectory,
            }
                .Select(Path.GetFullPath)
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (string candidateDirectory in candidateDirectories)
            {
                var current = new DirectoryInfo(candidateDirectory);
                while (current is not null)
                {
                    string appSettingsPath = Path.Combine(current.FullName, "appsettings.json");
                    string projectPath = Path.Combine(current.FullName, "LundBot.csproj");
                    if (File.Exists(appSettingsPath) && File.Exists(projectPath))
                    {
                        return current.FullName;
                    }

                    current = current.Parent;
                }
            }

            throw new InvalidOperationException(
                "Could not locate the project directory containing LundBot.csproj and appsettings.json for EF design-time configuration."
            );
        }
    }
}
