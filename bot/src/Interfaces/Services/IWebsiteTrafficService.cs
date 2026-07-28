namespace LundBot.Interfaces.Services
{
    public interface IWebsiteTrafficService
    {
        Task<bool> RegisterWebsiteVisitAsync(string ipAddress);
        Task<bool> RegisterInviteLinkClickAsync(string ipAddress);
    }
}
