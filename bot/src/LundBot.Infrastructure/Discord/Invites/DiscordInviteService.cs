using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Invites;
using LundBot.Application.Discord.Members;

namespace LundBot.Infrastructure.Discord.Invites
{
    public class DiscordInviteService : IDiscordInviteService
    {
        public Task<bool> RegisterWhoInvitedJoinedUser(DiscordGuildDto guild, DiscordMemberDto member)
        {
            throw new NotImplementedException();
        }
    }
}
