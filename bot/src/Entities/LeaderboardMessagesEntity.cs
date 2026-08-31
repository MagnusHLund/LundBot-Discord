namespace LundBot.Entities
{
    /// <summary>
    /// Used by all leaderboards to keep track of the messages that are sent to discord. This is used to be able to update the messages when the scores change.
    /// </summary>
    public sealed class LeaderboardMessagesEntity : AbstractMessageEntity
    {
        public int LeaderboardsId { get; set; }

        public LeaderboardsEntity Leaderboard { get; set; } = null!;
    }
}
