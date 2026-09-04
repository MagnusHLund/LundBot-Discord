namespace LundBot.Application.Common.Discord
{
    public interface IDiscordGuildService
    {
        Task<DiscordGuildDto?> GetGuildAsync(ulong guildId);
        Task<IReadOnlyList<DiscordInviteDto>> GetGuildInvitesAsync(ulong guildId);
        bool IsBotInGuild(ulong guildId);
    }
}
