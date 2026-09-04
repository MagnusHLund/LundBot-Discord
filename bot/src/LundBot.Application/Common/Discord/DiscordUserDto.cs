namespace LundBot.Application.Common.Discord
{
    public record DiscordUserDto
    {
        public ulong UserId { get; }
        public string Username { get; }
        public string DisplayName { get; }

        public DiscordUserDto(ulong userId, string username, string displayName)
        {
            UserId = userId;
            Username = username;
            DisplayName = displayName;
        }
    }
}
