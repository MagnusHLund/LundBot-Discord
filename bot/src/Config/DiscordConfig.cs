namespace LundBot.Config
{
    public sealed class DiscordConfig
    {
        public string Token { get; set; } = string.Empty;
        public List<ulong> FastUpdateGuildIds { get; set; } = new List<ulong>();
        public ulong WebTrafficChannelId { get; set; } = 0;
        public ulong RoleIdToAutoKick { get; set; } = 0;
        public bool ShouldRegisterGlobalCommands { get; set; } = false;
        public Dictionary<string, ulong> Roles { get; set; } = new();
    }
}
