namespace LundBot.Application.Discord.Guilds
{
    public interface IDiscordGuildService
    {
        Task<DiscordGuildDto?> GetGuildAsync(ulong guildId);
        Task<IReadOnlyList<DiscordInviteDto>> GetGuildInvitesAsync(ulong guildId);
        bool IsBotInGuild(ulong guildId);
    }
}
