using LundBot.Presentation.Api.Common.Validation;

namespace LundBot.Presentation.Api.Moderation.Dtos
{
    public class AssignRoleToKickRequestDto
    {
        [DiscordId]
        public required ulong GuildId { get; set; }

        [DiscordId]
        public required ulong RoleId { get; set; }

        public required string KickReason { get; set; }
    }
}
