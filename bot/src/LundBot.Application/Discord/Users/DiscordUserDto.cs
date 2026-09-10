namespace LundBot.Application.Discord.Users
{
    public record DiscordUserDto
    {
        public ulong UserId { get; }
        public string Username { get; }
        public string? GlobalName { get; }

        public DiscordUserDto(ulong userId, string username, string? globalName)
        {
            UserId = userId;
            Username = username;
            GlobalName = globalName;
        }
    }
}
