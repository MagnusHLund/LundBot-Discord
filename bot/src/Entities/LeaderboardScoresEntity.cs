namespace LundBot.Entities
{
    /// <summary>
    /// Used by all leaderboards to keep track of the scores of users. Like for example how many up votes that got or how many they invited to the server.
    /// </summary>
    public sealed class LeaderboardScoresEntity : AbstractEntity
    {
        public int LeaderboardsId { get; set; }
        public string DiscordUserId { get; set; } = null!;
        public int Score { get; set; }
        public DateTime UpdatedAt { get; set; }

        public LeaderboardsEntity Leaderboard { get; set; } = null!;
    }
}
