using LundBot.Interfaces.Services;

namespace LundBot.Tests.Integration.Api;

internal sealed class SpyWebsiteTrafficService : IWebsiteTrafficService
{
    internal bool RegisterVisitResult { get; set; } = true;
    internal bool RegisterInviteClickResult { get; set; } = true;

    public Task<bool> RegisterWebsiteVisitAsync(string ipAddress) =>
        Task.FromResult(RegisterVisitResult);

    public Task<bool> RegisterInviteLinkClickAsync(string ipAddress) =>
        Task.FromResult(RegisterInviteClickResult);
}
