using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Members;

namespace LundBot.Application.Discord.Invites
{
    public interface IDiscordInviteService
    {
        Task<bool> RegisterWhoInvitedJoinedUser(DiscordGuildDto guild, DiscordMemberDto member);
    }
}
