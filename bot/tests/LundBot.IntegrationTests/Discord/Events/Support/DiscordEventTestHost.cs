using DSharpPlus;
using DSharpPlus.Clients;
using LundBot.Application;
using LundBot.Application.Common.Caching;
using LundBot.Application.Common.Persistence;
using LundBot.Application.Discord.Bot;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Commands;
using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Invites;
using LundBot.Application.Discord.Members;
using LundBot.Application.Discord.Messages;
using LundBot.Application.Discord.Moderation;
using LundBot.Application.Discord.Roles;
using LundBot.Application.Discord.Stickers;
using LundBot.Application.Discord.Users;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Application.Features.MemberJoin;
using LundBot.Application.Features.Moderation;
using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Infrastructure.Caching;
using LundBot.Infrastructure.Persistence;
using LundBot.Infrastructure.Persistence.Repositories.Leaderboards;
using LundBot.Infrastructure.Persistence.Repositories.MemberJoin;
using LundBot.Infrastructure.Persistence.Repositories.Moderation;
using LundBot.Infrastructure.Persistence.Repositories.WebsiteTraffic;
using LundBot.Infrastructure.Queues;
using LundBot.Presentation.Discord.Events;
using LundBot.Presentation.Discord.Interactions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Serilog;

namespace LundBot.IntegrationTests.Discord.Events.Support;

internal sealed class DiscordEventTestHost : IDisposable
{
    private readonly ILogger _previousLogger;
    private readonly Serilog.Core.Logger _logger;
    private readonly SqliteConnection _connection;
    private readonly HttpClient _httpClient;
    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;

    public DiscordClient Client { get; }
    public OfflineInteractionTransport Transport { get; } = new();
    public EventLogSink Logs { get; } = new();
    public IDiscordChannelService Channels { get; } = Substitute.For<IDiscordChannelService>();
    public IDiscordMessageService Messages { get; } = Substitute.For<IDiscordMessageService>();
    public IDiscordGuildService Guilds { get; } = Substitute.For<IDiscordGuildService>();
    public IDiscordMemberService Members { get; } = Substitute.For<IDiscordMemberService>();
    public IDiscordRoleService Roles { get; } = Substitute.For<IDiscordRoleService>();
    public IDiscordModerationService Moderation { get; } = Substitute.For<IDiscordModerationService>();
    public IDiscordCommandService Commands { get; } = Substitute.For<IDiscordCommandService>();
    public IDiscordBotService Bot { get; } = Substitute.For<IDiscordBotService>();
    public IDiscordInteractionService Interactions { get; } = Substitute.For<IDiscordInteractionService>();
    public ICacheService Cache => _provider.GetRequiredService<ICacheService>();

    public DiscordEventTestHost()
    {
        _previousLogger = Log.Logger;
        _logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(Logs).CreateLogger();
        Log.Logger = _logger;
        _httpClient = new HttpClient(Transport) { BaseAddress = new Uri("https://discord.com/api/v10/") };
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient(Arg.Any<string>()).Returns(_httpClient);
        Client = DiscordClientBuilder
            .CreateDefault("offline-test-token", DiscordIntents.All)
            .DisableDefaultLogging()
            .ConfigureServices(services =>
            {
                services.AddLogging();
                services.Replace(ServiceDescriptor.Singleton(factory));
                services.Replace(ServiceDescriptor.Singleton(Substitute.For<IShardOrchestrator>()));
            })
            .Build();
        DiscordEventData.AddGuild(Client);
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        ServiceCollection services = new ServiceCollection();
        services.AddApplication(new ConfigurationBuilder().Build());
        services.AddSingleton<IHostEnvironment>(new EventHostEnvironment());
        services.AddSingleton(Client);
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<ILeaderboardQueue, LeaderboardQueue>();
        services.AddDbContext<LundBotDbContext>(options => options.UseSqlite(_connection));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
        services.AddScoped<ILeaderboardScoreRepository, LeaderboardScoreRepository>();
        services.AddScoped<ILeaderboardScoreSourceRepository, LeaderboardScoreSourceRepository>();
        services.AddScoped<ILeaderboardMessageRepository, LeaderboardMessageRepository>();
        services.AddScoped<IMemberJoinMessageRepository, MemberJoinMessageRepository>();
        services.AddScoped<IAutoKickRolesRepository, AutoKickRolesRepository>();
        services.AddScoped<IWebsiteTrafficRepository, WebsiteTrafficRepository>();
        services.AddScoped<IWebsiteTrafficMessageRepository, WebsiteTrafficMessageRepository>();
        services.AddScoped<IWebsiteTrafficChannelRepository, WebsiteTrafficChannelRepository>();
        services.AddSingleton(Channels);
        services.AddSingleton(Messages);
        services.AddSingleton(Guilds);
        services.AddSingleton(Members);
        services.AddSingleton(Roles);
        services.AddSingleton(Moderation);
        services.AddSingleton(Commands);
        services.AddSingleton(Bot);
        services.AddSingleton(Interactions);
        services.AddSingleton(Substitute.For<IDiscordUserService>());
        var stickers = Substitute.For<IDiscordStickerService>();
        services.AddSingleton(stickers);
        services.AddScoped<GuildMemberAddedHandler>();
        services.AddScoped<GuildMemberUpdatedHandler>();
        services.AddScoped<ComponentInteractionCreatedHandler>();
        services.AddScoped<GuildCreatedHandler>();
        services.AddScoped<GuildDownloadCompletedHandler>();
        services.AddScoped<SessionCreatedHandler>();
        services.AddScoped<CommandExecutedHandler>();
        services.AddScoped<CommandErroredHandler>();
        _provider = services.BuildServiceProvider(
            new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true }
        );
        _scope = _provider.CreateScope();
        Db.Database.EnsureCreated();

        Channels
            .GetSystemChannelAsync(DiscordEventData.GuildId)
            .Returns(new DiscordChannelDto(DiscordEventData.ChannelId, DiscordEventData.GuildId));
        Channels
            .GetChannelAsync(DiscordEventData.ChannelId)
            .Returns(new DiscordChannelDto(DiscordEventData.ChannelId, DiscordEventData.GuildId));
        Messages
            .SendMessageWithComponentsAsync(
                Arg.Any<ulong>(),
                Arg.Any<string>(),
                Arg.Any<
                    IReadOnlyCollection<LundBot.Application.Discord.Interactions.AbstractDiscordMessageComponentDto>
                >()
            )
            .Returns(new DiscordMessageDto(DiscordEventData.MessageId, DiscordEventData.ChannelId, 0, "Welcome"));
        Messages
            .SendMessageAsync(Arg.Any<ulong>(), Arg.Any<DiscordMessageBuilderDto>())
            .Returns(new DiscordMessageDto(DiscordEventData.MessageId + 1, DiscordEventData.ChannelId, 0, "Hi"));
        Guilds.GetGuildInvitesAsync(Arg.Any<ulong>()).Returns(new List<DiscordInviteDto>());
        Members
            .GetMemberAsync(DiscordEventData.UserId, DiscordEventData.GuildId)
            .Returns(new DiscordMemberDto(DiscordEventData.UserId, "Actor", "Actor"));
        Members
            .GetMemberAsync(DiscordEventData.TargetId, DiscordEventData.GuildId)
            .Returns(new DiscordMemberDto(DiscordEventData.TargetId, "Target", "Target"));
        Members.PreloadMembersAsync(Arg.Any<ulong>()).Returns(true);
        Bot.UpdateBotStatusAsync(Arg.Any<string>()).Returns(true);
        Commands.RefreshCommandsAsync().Returns(true);
    }

    public LundBotDbContext Db => _scope.ServiceProvider.GetRequiredService<LundBotDbContext>();

    public T Handler<T>()
        where T : notnull => _scope.ServiceProvider.GetRequiredService<T>();

    public async Task<T> ReadAsync<T>(Func<LundBotDbContext, Task<T>> read)
    {
        using var scope = _provider.CreateScope();
        return await read(scope.ServiceProvider.GetRequiredService<LundBotDbContext>());
    }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
        Client.Dispose();
        if (Client.ServiceProvider is IDisposable sdkProvider)
        {
            sdkProvider.Dispose();
        }
        _httpClient.Dispose();
        _connection.Dispose();
        Log.Logger = _previousLogger;
        _logger.Dispose();
    }

    private sealed class EventHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "DiscordEventTests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
