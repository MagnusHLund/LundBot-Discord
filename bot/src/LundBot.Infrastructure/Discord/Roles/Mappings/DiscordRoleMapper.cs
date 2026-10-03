using DSharpPlus.Entities;
using LundBot.Application.Discord.Roles;

namespace LundBot.Infrastructure.Discord.Roles.Mappings
{
    public static class DiscordRoleMapper
    {
        public static DiscordRoleDto Map(DiscordRole role)
        {
            return new DiscordRoleDto(role.Id, role.Name);
        }
    }
}
