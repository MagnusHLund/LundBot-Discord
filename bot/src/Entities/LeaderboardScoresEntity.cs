namespace LundBot.Entities
{
    public sealed class LeaderboardScoresEntity
    {
        public int LeaderboardScoresId { get; set; }
        public int LeaderboardsId { get; set; }
        public string DiscordUserId { get; set; } = null!;
        public int Score { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public LeaderboardsEntity Leaderboard { get; set; } = null!;
    }
}
