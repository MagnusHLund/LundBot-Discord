namespace LundBot.Config
{
    public sealed class DiscordConfig
    {
        public string Token { get; set; } = string.Empty;
        public List<ulong> FastUpdateGuildIds { get; set; } = new List<ulong>();
        public string WebTrafficChannelId { get; set; } = string.Empty;
        public bool ShouldClearGlobalCommands { get; set; } = false;
        public bool ShouldRegisterGlobalCommands { get; set; } = false;
    }
}
