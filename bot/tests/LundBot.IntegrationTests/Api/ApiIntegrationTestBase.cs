using System.Net.Http.Json;
using Xunit;

namespace LundBot.IntegrationTests.Api;

public abstract class ApiIntegrationTestBase : IAsyncLifetime
{
    protected ApiTestHost Host { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Host = new ApiTestHost();
        await Host.InitializeAsync();
    }

    public Task DisposeAsync() => Host.DisposeAsync();

    protected Task<HttpResponseMessage> SendAuthorizedAsync(HttpMethod method, string path) =>
        Host.Client.SendAsync(Host.AuthorizedRequest(method, path));

    protected Task<HttpResponseMessage> SendAuthorizedJsonAsync(HttpMethod method, string path, object body) =>
        Host.Client.SendAsync(Host.AuthorizedRequest(method, path, JsonContent.Create(body)));

    protected Task<HttpResponseMessage> SendJsonAsync(HttpMethod method, string path, object body) =>
        Host.Client.SendAsync(new HttpRequestMessage(method, path) { Content = JsonContent.Create(body) });
}
