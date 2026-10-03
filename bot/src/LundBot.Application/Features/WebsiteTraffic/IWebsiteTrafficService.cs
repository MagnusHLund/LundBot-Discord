namespace LundBot.Application.Features.WebsiteTraffic
{
    public interface IWebsiteTrafficService
    {
        Task<bool> CreateWebsiteTrafficChannelAsync(ulong channelId, ulong guildId);
        Task<bool> RemoveWebsiteTrafficChannelAsync(ulong guildId);
        Task<bool> RegisterWebsiteVisitAsync(string ipAddress, ulong guildId);
        Task<bool> RegisterInviteLinkClickAsync(string ipAddress, ulong guildId);
    }
}
