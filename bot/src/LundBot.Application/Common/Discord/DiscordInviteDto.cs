namespace LundBot.Application.Common.Discord
{
    public sealed record DiscordInviteDto
    {
        private string code;
        private ulong v;

        public string InviteCode { get; }
        public ushort Uses { get; }
        public DiscordMemberDto Inviter { get; }

        public DiscordInviteDto(string inviteCode, ushort uses, DiscordMemberDto inviter)
        {
            InviteCode = inviteCode;
            Uses = uses;
            Inviter = inviter;
        }

        public DiscordInviteDto(string code, ulong v)
        {
            this.code = code;
            this.v = v;
        }
    }
}
