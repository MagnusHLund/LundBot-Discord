using LundBot.Application.Discord.Members;

namespace LundBot.Application.Discord.Guilds
{
    public sealed record DiscordInviteDto
    {
        public string InviteCode { get; }
        public ushort Uses { get; }
        public DiscordMemberDto Inviter { get; }

        public DiscordInviteDto(string inviteCode, ushort uses, DiscordMemberDto inviter)
        {
            InviteCode = inviteCode;
            Uses = uses;
            Inviter = inviter;
        }
    }
}
