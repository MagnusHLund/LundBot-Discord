using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Users;

namespace LundBot.Application.Features.Invites
{
    public interface IInviteService
    {
        Task<bool> RegisterUserJoinedWithInviteAsync(
            DiscordGuildDto guild,
            DiscordUserDto userJoined,
            DiscordUserDto invitedByUser
        );
    }
}
