using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LundBot.Infrastructure.Persistence
{
    public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LundBotDbContext>
    {
        public LundBotDbContext CreateDbContext(string[] args)
        {
            // Get the connection string from the migrations.sh script
            string? envConnectionString = Environment.GetEnvironmentVariable("EF_CONNECTION_STRING");
            string connectionString;

            if (!string.IsNullOrWhiteSpace(envConnectionString))
            {
                connectionString = envConnectionString;
            }
            else
            {
                // Fallback to JSON
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Development.json", optional: false)
                    .AddEnvironmentVariables()
                    .Build();

                connectionString = config.GetSection("Database")["ConnectionString"] ?? string.Empty;
            }

            var options = new DbContextOptionsBuilder<LundBotDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            return new LundBotDbContext(options);
        }
    }
}
