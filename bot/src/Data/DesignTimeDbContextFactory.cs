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
            // Get the connection string from the migrations.sh script
            var envConn = Environment.GetEnvironmentVariable("EF_CONNECTION_STRING");

            string conn;

            if (!string.IsNullOrWhiteSpace(envConn))
            {
                conn = envConn;
            }
            else
            {
                // Fallback to JSON
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Development.json", optional: false)
                    .AddEnvironmentVariables()
                    .Build();

                conn = config.GetSection("Database")["ConnectionString"] ?? string.Empty;
            }

            var options = new DbContextOptionsBuilder<LundBotDiscordDbContext>()
                .UseMySql(conn, ServerVersion.AutoDetect(conn))
                .Options;

            return new LundBotDiscordDbContext(options);
        }
    }
}
