using System.ComponentModel.DataAnnotations;

namespace LundBot.Entities
{
    public sealed class LeaderboardMessagesEntity
    {
        public int LeaderboardMessagesId { get; set; }
        public int LeaderboardsId { get; set; }
        public string DiscordMessageId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public LeaderboardsEntity Leaderboard { get; set; } = null!;
    }
}
