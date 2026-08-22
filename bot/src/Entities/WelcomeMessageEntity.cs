namespace LundBot.Entities
{
    public sealed class WelcomeMessageEntity : AbstractMessageEntity
    {
        public string DiscordUserId { get; set; } = null!;
    }
}
