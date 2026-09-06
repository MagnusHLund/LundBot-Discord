namespace LundBot.Application.Discord.Users
{
    public record DiscordUserDto
    {
        public ulong UserId { get; }
        public string Username { get; }

        public DiscordUserDto(ulong userId, string username)
        {
            UserId = userId;
            Username = username;
        }
    }
}
