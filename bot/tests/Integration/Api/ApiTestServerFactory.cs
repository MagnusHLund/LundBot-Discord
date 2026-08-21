using LundBot.Config;
using LundBot.Controllers;
using LundBot.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LundBot.Tests.Integration.Api;

internal static class ApiTestServerFactory
{
    internal static async Task<(
        HttpClient Client,
        SpyCommandsService Commands,
        SpyLeaderboardService Leaderboards,
        SpyWebsiteTrafficService Traffic
    )> CreateAsync()
    {
        var commands = new SpyCommandsService();
        var leaderboards = new SpyLeaderboardService();
        var traffic = new SpyWebsiteTrafficService();

        var builder = new HostBuilder().ConfigureWebHost(webBuilder =>
        {
            webBuilder.UseTestServer();
            webBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<ICommandsService>(commands);
                services.AddSingleton<ILeaderboardService>(leaderboards);
                services.AddSingleton<IWebsiteTrafficService>(traffic);
                services.Configure<DeveloperEnvironmentConfig>(_ => { });
                services.Configure<ServerConfig>(config => config.Version = "9.9.9");
                services.AddControllers().AddApplicationPart(typeof(HealthController).Assembly);
            });

            webBuilder.Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints => endpoints.MapControllers());
            });
        });

        IHost host = await builder.StartAsync();
        return (host.GetTestClient(), commands, leaderboards, traffic);
    }
}
