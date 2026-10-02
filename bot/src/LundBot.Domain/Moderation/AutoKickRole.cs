using LundBot.Domain.Common;

namespace LundBot.Domain.Moderation
{
    public sealed class AutoKickRole : AbstractEntity
    {
        public ulong RoleId { get; set; }
        public ulong GuildId { get; set; }
        public string Reason { get; set; } = null!;

        public AutoKickRole(ulong roleId, ulong guildId, string reason)
        {
            RoleId = roleId;
            GuildId = guildId;
            Reason = reason;
        }

        private AutoKickRole() { }
    }
}
