using LundBot.Domain.Common;

namespace LundBot.Domain.Leaderboards
{
    /// <summary>
    /// Used by all leaderboards to keep track of the messages that are sent to discord.
    /// This is used to be able to update the messages when the scores change.
    /// </summary>
    public sealed class LeaderboardMessage : AbstractMessageEntity
    {
        public int LeaderboardId { get; set; }
        public Leaderboard Leaderboard { get; set; } = null!;
    }
}
