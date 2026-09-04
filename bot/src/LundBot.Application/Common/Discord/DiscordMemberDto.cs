namespace LundBot.Application.Common.Discord
{
    public sealed record DiscordMemberDto : DiscordUserDto
    {
        public DiscordMemberDto(ulong userId, string username, string displayName)
            : base(userId, username, displayName) { }
    }
}
