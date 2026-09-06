using DSharpPlus;
using LundBot.Application.Discord.Moderation;
using Serilog;

namespace LundBot.Infrastructure.Discord.Moderation
{
    public class DiscordModerationService : IDiscordModerationService
    {
        private readonly DiscordClient _discordClient;

        private readonly ILogger _logger = Log.ForContext<DiscordModerationService>();

        public DiscordModerationService(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task<bool> KickMemberAsync(ulong memberId, ulong guildId)
        {
            _logger.Information("Kicking member {MemberId} from guild {GuildId}...", memberId, guildId);

            try
            {
                var guild = await _discordClient.GetGuildAsync(guildId);
                var member = await guild.GetMemberAsync(memberId);

                await member.RemoveAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to kick member {MemberId} from guild {GuildId}", memberId, guildId);
                return false;
            }
        }
    }
}
