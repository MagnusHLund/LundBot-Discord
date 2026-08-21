using LundBot.Data;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Tests.Integration.Database;

internal static class MySqlDbContextFactory
{
    internal static LundBotDiscordDbContext Create()
    {
        var options = new DbContextOptionsBuilder<LundBotDiscordDbContext>()
            .UseMySql(
                "Server=localhost;Database=test;User=test;Password=test;",
                new MySqlServerVersion(new Version(8, 0, 0))
            )
            .Options;
        return new LundBotDiscordDbContext(options);
    }
}
