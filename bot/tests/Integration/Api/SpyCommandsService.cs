using LundBot.Interfaces.Services;

namespace LundBot.Tests.Integration.Api;

internal sealed class SpyCommandsService : ICommandsService
{
    internal bool RefreshCommandsCalled { get; private set; }
    internal bool UnregisterAllShouldSucceed { get; set; } = true;

    public Task RegisterCommandsAsync() => Task.CompletedTask;

    public Task LogRegisteredCommandsForGuildsAsync() => Task.CompletedTask;

    public Task RefreshCommandsAsync()
    {
        RefreshCommandsCalled = true;
        return Task.CompletedTask;
    }

    public Task<bool> UnregisterCommand(string commandId, bool global = false) =>
        Task.FromResult(true);

    public Task<bool> UnregisterAllCommands(bool global = false) =>
        Task.FromResult(UnregisterAllShouldSucceed);
}
