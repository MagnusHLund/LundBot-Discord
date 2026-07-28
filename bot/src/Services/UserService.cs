using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Interfaces.Services;

namespace LundBot.Services
{
    public sealed class UserService : IUserService
    {
        public async Task<bool> IsUserAdminAsync(ulong userId, ulong guildId)
        {
            DiscordGuild guild = await BotService.DiscordClient.GetGuildAsync(guildId);

            var member = await guild.GetMemberAsync(userId);

            if (member.Permissions.HasFlag(Permissions.Administrator))
                return true;

            return member.Roles.Any(r => r.Permissions.HasFlag(Permissions.Administrator));
        }

        public async Task<bool> IsUserOwnerAsync(ulong userId, ulong guildId)
        {
            DiscordGuild guild = await BotService.DiscordClient.GetGuildAsync(guildId);

            if (guild.OwnerId == userId)
            {
                return true;
            }

            var member = await guild.GetMemberAsync(userId);

            return member.Roles.Any(r =>
                string.Equals(r.Name, "Owner", StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}
