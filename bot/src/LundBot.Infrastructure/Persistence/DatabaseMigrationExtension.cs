using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LundBot.Infrastructure.Persistence
{
    public static class DatabaseMigrationExtension
    {
        public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();

            LundBotDbContext dbContext = scope.ServiceProvider.GetRequiredService<LundBotDbContext>();

            try
            {
                Log.Information("Checking for pending database migrations...");

                await dbContext.Database.MigrateAsync();

                Log.Information("Database migrations applied successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while applying database migrations.");

                throw;
            }
        }
    }
}
