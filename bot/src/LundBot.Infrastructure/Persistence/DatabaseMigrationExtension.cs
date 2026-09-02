using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LundBot.Infrastructure.Persistence
{
    public static class DatabaseMigrationExtension
    {
        public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();

            ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<LundBotDbContext>>();

            LundBotDbContext dbContext = scope.ServiceProvider.GetRequiredService<LundBotDbContext>();

            try
            {
                logger.LogInformation("Checking for pending database migrations...");

                await dbContext.Database.MigrateAsync();

                logger.LogInformation("Database migrations applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying database migrations.");

                throw;
            }
        }
    }
}
