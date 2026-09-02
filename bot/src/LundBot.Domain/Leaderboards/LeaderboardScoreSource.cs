using LundBot.Domain.Common;

namespace LundBot.Domain.Leaderboards
{
    /// <summary>
    /// Represents a upvoting leaderboard in the database.
    /// This entity is used to keep track of the upvoting leaderboards that are created in the discord server.
    /// </summary>
    public sealed class LeaderboardScoreSource : AbstractEntity
    {
        public int LeaderboardId { get; set; }
        public ulong DiscordUserIdActor { get; set; }
        public ulong DiscordUserIdTarget { get; set; }

        public Leaderboard Leaderboard { get; set; } = null!;
    }
}
