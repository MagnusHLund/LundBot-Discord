using LundBot.Enums;

namespace LundBot.Entities
{
    /// <summary>
    /// Represents a leaderboard in the database. This entity is used to keep track of the leaderboards that are created in the discord server.
    /// </summary>
    public sealed class LeaderboardsEntity : AbstractEntity
    {
        public string DiscordServerId { get; set; } = null!;
        public string DiscordChannelId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public LeaderboardType LeaderboardType { get; set; }

        public ICollection<LeaderboardScoresEntity> LeaderboardScores { get; set; } =
            new List<LeaderboardScoresEntity>();
        public ICollection<LeaderboardMessagesEntity> LeaderboardMessages { get; set; } =
            new List<LeaderboardMessagesEntity>();
        public ICollection<LeaderboardScoreSourceEntity> LeaderboardScoreSources { get; set; } =
            new List<LeaderboardScoreSourceEntity>();
    }
}
