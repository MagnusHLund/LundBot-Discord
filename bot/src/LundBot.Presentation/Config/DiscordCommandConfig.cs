namespace LundBot.Presentation.Config
{
    public sealed class DiscordCommandConfig
    {
        public List<ulong> FastUpdateGuildIds { get; set; } = new List<ulong>();
        public bool ShouldRegisterGlobalCommands { get; set; } = false;
    }
}
