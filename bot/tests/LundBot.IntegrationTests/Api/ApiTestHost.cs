using LundBot.Application.Common.Bot;
using LundBot.Application.Common.Messaging;
using LundBot.Application.Discord.Moderation;
using LundBot.Application.Discord.Roles;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Application.Features.Moderation;
using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Domain.WebsiteTraffic;
using LundBot.Infrastructure.Persistence;
using LundBot.Infrastructure.Persistence.Repositories.Moderation;
using LundBot.Infrastructure.Persistence.Repositories.WebsiteTraffic;
using LundBot.Presentation.Api.Authentication;
using LundBot.Presentation.Api.Middleware;
using LundBot.Presentation.Api.Server;
using LundBot.Presentation.Config;
using LundBot.Presentation.Discord.Interactions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace LundBot.IntegrationTests.Api;

public sealed class ApiTestHost
{
    public const string ApiKey = "test-api-key";

    private SqliteConnection _connection = null!;
    private IHost _host = null!;
    private TestServer _server = null!;

    public HttpClient Client { get; private set; } = null!;
    public ICommandService CommandService { get; private set; } = null!;
    public ILeaderboardService LeaderboardService { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        CommandService = Substitute.For<ICommandService>();
        LeaderboardService = Substitute.For<ILeaderboardService>();

        var trafficMessageService = Substitute.For<
            IMessageService<
                WebsiteTrafficAnalyticsMessage,
                IWebsiteTrafficMessageRepository,
                WebsiteTrafficMessageFactory
            >
        >();
        trafficMessageService.MessageFactory.Returns(new WebsiteTrafficMessageFactory());
        trafficMessageService
            .SynchronizeDiscordMessagesAsync(
                Arg.Any<string>(),
                Arg.Any<IEnumerable<WebsiteTrafficAnalyticsMessage>>(),
                Arg.Any<ulong>()
            )
            .Returns(Task.FromResult(true));

        var roleService = Substitute.For<IDiscordRoleService>();
        var moderationService = Substitute.For<IDiscordModerationService>();

        _host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
                webHost
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddControllers().AddApplicationPart(typeof(HealthController).Assembly);

                        services.Configure<ServerConfig>(options =>
                        {
                            options.ApiKey = ApiKey;
                            options.Version = "integration-test";
                        });
                        services.Configure<DeveloperEnvironmentConfig>(_ => { });

                        services
                            .AddAuthentication("ApiKey")
                            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", _ => { });
                        services.AddAuthorization();

                        services.AddSingleton(_connection);
                        services.AddDbContext<LundBotDbContext>(
                            (serviceProvider, options) =>
                                options.UseSqlite(serviceProvider.GetRequiredService<SqliteConnection>())
                        );

                        services.AddScoped<IAutoKickRolesRepository, AutoKickRolesRepository>();
                        services.AddScoped<IWebsiteTrafficChannelRepository, WebsiteTrafficChannelRepository>();
                        services.AddScoped<IWebsiteTrafficRepository, WebsiteTrafficRepository>();
                        services.AddScoped<IWebsiteTrafficMessageRepository, WebsiteTrafficMessageRepository>();
                        services.AddScoped<WebsiteTrafficMessageFactory>();
                        services.AddScoped<IWebsiteTrafficService, WebsiteTrafficService>();
                        services.AddScoped<IModerationActionService, ModerationActionService>();
                        services.AddScoped(_ => trafficMessageService);
                        services.AddSingleton(roleService);
                        services.AddSingleton(moderationService);
                        services.AddSingleton(CommandService);
                        services.AddSingleton(LeaderboardService);

                        services.AddSingleton<IDiscordInteractionService>(Substitute.For<IDiscordInteractionService>());
                    })
                    .Configure(app =>
                    {
                        app.UseMiddleware<ExceptionHandlingMiddleware>();
                        app.UseMiddleware<CorsMiddleware>();
                        app.UseRouting();
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.UseEndpoints(endpoints => endpoints.MapControllers());
                    })
            )
            .StartAsync();
        _server = _host.GetTestServer();

        Client = _server.CreateClient();

        await using AsyncServiceScope scope = _server.Services.CreateAsyncScope();
        LundBotDbContext dbContext = scope.ServiceProvider.GetRequiredService<LundBotDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        _host.Dispose();
        await _connection.DisposeAsync();
    }

    public HttpRequestMessage AuthorizedRequest(HttpMethod method, string path, HttpContent? content = null)
    {
        HttpRequestMessage request = new HttpRequestMessage(method, path) { Content = content };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);
        return request;
    }

    public async Task<T> WithDbContextAsync<T>(Func<LundBotDbContext, Task<T>> action)
    {
        await using AsyncServiceScope scope = _server.Services.CreateAsyncScope();
        return await action(scope.ServiceProvider.GetRequiredService<LundBotDbContext>());
    }
}
