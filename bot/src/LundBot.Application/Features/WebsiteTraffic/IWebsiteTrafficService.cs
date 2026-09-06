namespace LundBot.Application.Features.WebsiteTraffic
{
    public interface IWebsiteTrafficService
    {
        Task<bool> RegisterWebsiteVisitAsync(string ipAddress);
        Task<bool> RegisterInviteLinkClickAsync(string ipAddress);
    }
}
