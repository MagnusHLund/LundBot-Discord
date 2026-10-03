namespace LundBot.Domain.Common
{
    public abstract class AbstractMessageEntity : AbstractEntity
    {
        public ulong DiscordMessageId { get; set; }
    }
}
