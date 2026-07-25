namespace LundBot.Entities
{
    public sealed class WebsiteTrafficMessagesEntity
    {
        public int WebsiteTrafficMessagesId { get; set; }
        public string DiscordMessageId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
