using LundBot.Domain.Common;

namespace LundBot.Domain.MemberJoin
{
    /// <summary>
    /// Represents a member join message in the database.
    /// This entity is used to keep track of the messages sent when a member joins the discord server.
    /// </summary>
    public sealed class MemberJoinMessage : AbstractMessageEntity
    {
        public ulong DiscordUserId { get; set; }
    }
}
