namespace LundBot.Entities
{
    public sealed class LeaderboardsEntity
    {
        public int LeaderboardsId { get; set; }
        public string DiscordServerId { get; set; } = null!;
        public string DiscordChannelId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public ICollection<LeaderboardScoresEntity> LeaderboardScores { get; set; } =
            new List<LeaderboardScoresEntity>();
        public ICollection<LeaderboardMessagesEntity> LeaderboardMessages { get; set; } =
            new List<LeaderboardMessagesEntity>();
        public ICollection<UpvotingLeaderBoardEntity> UpvotingLeaderboard { get; set; } =
            new List<UpvotingLeaderBoardEntity>();
    }
}
