using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordUserService : IDiscordUserService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordUserService>();

        public Task<DiscordUser> GetUserAsync(ulong userId)
        {
            _logger.Information("Fetching user with ID {UserId}...", userId);

            try
            {
                return BotService.DiscordClient.GetUserAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch user with ID {UserId}.", userId);
                throw;
            }
        }
    }
}
