namespace LundBot.Entities
{
    /// <summary>
    /// Represents a upvoting leaderboard in the database. This entity is used to keep track of the upvoting leaderboards that are created in the discord server.
    /// </summary>
    public sealed class UpvotingLeaderBoardEntity : AbstractEntity
    {
        public int LeaderboardsId { get; set; }
        public string DiscordUserIdVoter { get; set; } = null!;
        public string DiscordUserIdTarget { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }

        public LeaderboardsEntity Leaderboard { get; set; } = null!;
    }
}
