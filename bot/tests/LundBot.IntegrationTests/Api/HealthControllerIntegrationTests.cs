using System.Net;
using Xunit;

namespace LundBot.IntegrationTests.Api;

public sealed class HealthControllerIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task Get_requires_authentication()
    {
        using HttpResponseMessage response = await Host.Client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_returns_health_status_and_configured_version()
    {
        using HttpResponseMessage response = await SendAuthorizedAsync(HttpMethod.Get, "/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            """{"status":"Healthy","version":"integration-test"}""",
            await response.Content.ReadAsStringAsync()
        );
    }
}
