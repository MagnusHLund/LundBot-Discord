using LundBot.Presentation.Api.Common.Validation;

namespace LundBot.Presentation.Api.Moderation.Dtos
{
    public class UnassignRoleToKickRequestDto
    {
        [DiscordId]
        public required ulong GuildId { get; set; }

        [DiscordId]
        public required ulong RoleId { get; set; }
    }
}
