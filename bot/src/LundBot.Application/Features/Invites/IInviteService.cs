using LundBot.Application.Discord.Users;

namespace LundBot.Application.Features.Invites
{
    public interface IInviteService
    {
        Task<bool> RegisterUserJoinedWithInviteAsync(
            ulong guildId,
            DiscordUserDto userJoined,
            DiscordUserDto invitedByUser
        );
    }
}
