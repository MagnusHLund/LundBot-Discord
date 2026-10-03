using LundBot.Application.Discord.Users;

namespace LundBot.Application.Discord.Members
{
    public sealed record DiscordMemberDto : DiscordUserDto
    {
        public string DisplayName { get; }

        public DiscordMemberDto(ulong userId, string username, string displayName, string? globalName = null)
            : base(userId, username, globalName)
        {
            DisplayName = displayName;
        }
    }
}
