using LundBot.Application.Discord.Channels;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards.Shared
{
    public sealed class LeaderboardUpdateJob
    {
        public Leaderboard Leaderboard { get; init; } = null!;
        public DiscordChannelDto Channel { get; init; } = null!;
    }
}
