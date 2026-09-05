using LundBot.Application.Discord.Users;

namespace LundBot.Application.Discord.Guilds
{
    public sealed record DiscordInviteDto
    {
        public string InviteCode { get; }
        public ushort Uses { get; }
        public DiscordUserDto Inviter { get; }

        public DiscordInviteDto(string inviteCode, ushort uses, DiscordUserDto inviter)
        {
            InviteCode = inviteCode;
            Uses = uses;
            Inviter = inviter;
        }
    }
}
