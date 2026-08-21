using DSharpPlus.Entities;
using LundBot.Entities;

namespace LundBot.ValueObjects.Jobs
{
    public sealed class LeaderboardUpdateJob
    {
        public LeaderboardsEntity Leaderboard { get; init; } = null!;
        public DiscordChannel Channel { get; init; } = null!;
    }
}
