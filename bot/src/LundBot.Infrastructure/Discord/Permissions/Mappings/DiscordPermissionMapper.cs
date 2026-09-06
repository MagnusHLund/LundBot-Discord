using DSharpPlus.Entities;
using LundBot.Application.Discord.Permissions;

namespace LundBot.Infrastructure.Discord.Permissions.Mappings
{
    public static class DiscordPermissionMapper
    {
        public static DiscordPermissionEnum Map(this DiscordPermission permission)
        {
            return (DiscordPermissionEnum)(int)permission;
        }

        public static DiscordPermission Map(this DiscordPermissionEnum permission)
        {
            return (DiscordPermission)(int)permission;
        }
    }
}
