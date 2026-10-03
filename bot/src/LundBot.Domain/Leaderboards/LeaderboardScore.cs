using LundBot.Domain.Common;

namespace LundBot.Domain.Leaderboards
{
    /// <summary>
    /// Used by all leaderboards to keep track of the scores of users.
    /// Like for example how many up votes that got or how many they invited to the server.
    /// </summary>
    public sealed class LeaderboardScore : AbstractEntity
    {
        public int LeaderboardId { get; set; }
        public ulong DiscordUserId { get; set; }
        public int Score { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Leaderboard Leaderboard { get; set; } = null!;
    }
}
