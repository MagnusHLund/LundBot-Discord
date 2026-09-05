using DSharpPlus.Entities;
using LundBot.Application.Discord.Permissions;

namespace LundBot.Presentation.Discord.Mappings
{
    public static class DiscordPermissionMapper
    {
        public static DiscordPermissionEnum Map(DiscordPermission permission)
        {
            return (DiscordPermissionEnum)(int)permission;
        }

        public static DiscordPermission Map(DiscordPermissionEnum permission)
        {
            return (DiscordPermission)(int)permission;
        }
    }
}
