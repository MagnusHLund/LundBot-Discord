using LundBot.Domain.Common;

namespace LundBot.Domain.Leaderboards
{
    /// <summary>
    /// Represents a leaderboard in the database.
    /// This entity is used to keep track of the leaderboards that are created in the discord server.
    /// </summary>
    public sealed class Leaderboard : AbstractEntity
    {
        public ulong DiscordServerId { get; set; }
        public ulong DiscordChannelId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public LeaderboardType LeaderboardType { get; set; }

        public ICollection<LeaderboardScore> LeaderboardScores { get; set; } = new List<LeaderboardScore>();
        public ICollection<LeaderboardMessage> LeaderboardMessages { get; set; } = new List<LeaderboardMessage>();
        public ICollection<LeaderboardScoreSource> LeaderboardScoreSources { get; set; } =
            new List<LeaderboardScoreSource>();
    }
}
