using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Users;

namespace LundBot.Application.Features.Leaderboards.Contracts
{
    public interface IInviteLeaderboardService : IAbstractLeaderboardService
    {
        Task<bool> RegisterSuccessfullyInvitedUserAsync(
            DiscordGuildDto guild,
            DiscordUserDto userJoined,
            DiscordUserDto userInvitedBy
        );
    }
}
