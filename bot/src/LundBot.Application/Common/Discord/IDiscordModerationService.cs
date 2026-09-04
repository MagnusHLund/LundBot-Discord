namespace LundBot.Application.Common.Discord
{
    public interface IDiscordModerationService
    {
        Task KickMemberAsync(ulong memberId, ulong guildId);
    }
}
