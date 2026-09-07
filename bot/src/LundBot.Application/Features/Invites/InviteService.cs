using LundBot.Application.Discord.Users;

namespace LundBot.Application.Features.Invites
{
    public class InviteService : IInviteService
    {
        public Task<bool> RegisterUserJoinedWithInviteAsync(
            ulong guildId,
            DiscordUserDto userJoined,
            DiscordUserDto invitedByUser
        )
        {
            throw new NotImplementedException();
        }
    }
}
