using LundBot.Application.Common.Discord;

namespace LundBot.Infrastructure.Discord.Services
{
    public class DiscordModerationService : IDiscordModerationService
    {
        public Task KickMemberAsync(ulong memberId, ulong guildId)
        {
            throw new NotImplementedException();
        }
    }
}
