using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordGuildService
    {
        Task<DiscordGuild> GetGuildAsync(ulong guildId);
        Task<IReadOnlyList<DiscordInvite>> GetGuildInvitesAsync(DiscordGuild guild);
        bool BotIsInGuild(ulong guildId);
        Task<DiscordRole> GetRoleByIdAsync(DiscordGuild guild, ulong roleId);
    }
}
