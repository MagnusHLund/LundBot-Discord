namespace LundBot.Entities
{
    public sealed class UpvotingLeaderBoardEntity
    {
        public int UpvotingLeaderboardId { get; set; }
        public int LeaderboardsId { get; set; }
        public string DiscordUserIdVoter { get; set; } = null!;
        public string DiscordUserIdTarget { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public LeaderboardsEntity Leaderboard { get; set; } = null!;
    }
}
