namespace LundBot.Entities
{
    /// <summary>
    /// Represents a upvoting leaderboard in the database. This entity is used to keep track of the upvoting leaderboards that are created in the discord server.
    /// </summary>
    public sealed class LeaderboardScoreSourceEntity : AbstractEntity
    {
        public int LeaderboardsId { get; set; }
        public string DiscordUserIdActor { get; set; } = null!;
        public string DiscordUserIdTarget { get; set; } = null!;

        public LeaderboardsEntity Leaderboard { get; set; } = null!;
    }
}
