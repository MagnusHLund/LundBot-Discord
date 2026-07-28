namespace LundBot.Entities
{
    public abstract class AbstractMessageEntity : AbstractEntity
    {
        public string DiscordMessageId { get; set; } = null!;
    }
}
