using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Application.Discord.Users;
using LundBot.Infrastructure.Discord.Users.Mappings;
using Serilog;

namespace LundBot.Infrastructure.Discord.Users
{
    public sealed class DiscordUserService : IDiscordUserService
    {
        private readonly DiscordClient _discordClient;

        private readonly ILogger _logger = Log.ForContext<DiscordUserService>();

        public DiscordUserService(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task<DiscordUserDto?> GetUserAsync(ulong userId)
        {
            _logger.Information("Fetching user with ID {UserId}...", userId);

            try
            {
                DiscordUser user = await _discordClient.GetUserAsync(userId);
                return DiscordUserMapper.Map(user);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch user with ID {UserId}", userId);
                return null;
            }
        }
    }
}
