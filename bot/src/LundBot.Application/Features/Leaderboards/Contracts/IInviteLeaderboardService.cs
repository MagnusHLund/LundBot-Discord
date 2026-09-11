using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Users;

namespace LundBot.Application.Features.Leaderboards.Contracts
{
    public interface IInviteLeaderboardService : ILeaderboardService
    {
        Task<bool> RegisterSuccessfullyInvitedUserAsync(
            DiscordGuildDto guild,
            DiscordUserDto userJoined,
            DiscordUserDto userInvitedBy
        );
    }
}
