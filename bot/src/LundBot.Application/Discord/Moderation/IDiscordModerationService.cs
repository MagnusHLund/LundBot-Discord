namespace LundBot.Application.Discord.Moderation
{
    public interface IDiscordModerationService
    {
        Task<bool> KickMemberAsync(ulong memberId, ulong guildId);
    }
}
