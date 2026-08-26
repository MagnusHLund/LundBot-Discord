using DSharpPlus;
using LundBot.BackgroundServices;
using LundBot.Config;
using LundBot.Data;
using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Queues;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using LundBot.Middleware;
using LundBot.Queues;
using LundBot.Repositories;
using LundBot.Services;
using LundBot.Services.Discord;
using LundBot.Services.Discord.Events;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LundBot
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            RegisterLogger(builder);
            RegisterConfiguration(builder);

            SetupDatabase(builder);
            RegisterServices(builder.Services);
            RegisterFactories(builder.Services);
            RegisterRepositories(builder.Services);
            RegisterBackgroundServices(builder.Services);

            RegisterBotConfiguration(builder);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            RegisterMiddleware(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.UseCors();

            app.Run();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Queues
            services.AddSingleton<ILeaderboardQueue, LeaderboardQueue>();

            // Base singleton services
            services.AddSingleton<IBotService, BotService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<ICommandsService, CommandsService>();
            services.AddSingleton<IModerationActionsService, ModerationActionsService>();

            // Discord services. Always singleton.
            services.AddSingleton<IDiscordBotService, DiscordBotService>();
            services.AddSingleton<IDiscordChannelService, DiscordChannelService>();
            services.AddSingleton<IDiscordCommandService, DiscordCommandService>();
            services.AddSingleton<IDiscordGuildService, DiscordGuildService>();
            services.AddSingleton<IDiscordInteractionService, DiscordInteractionService>();
            services.AddSingleton<IDiscordMemberService, DiscordMemberService>();
            services.AddSingleton<IDiscordMessageService, DiscordMessageService>();
            services.AddSingleton<IDiscordUserService, DiscordUserService>();
            services.AddSingleton<IDiscordStickerService, DiscordStickerService>();

            // Base scoped services
            services.AddScoped<IWelcomeMessageService, WelcomeMessageService>();
            services.AddScoped<ILeaderboardService, LeaderboardService>();
            services.AddScoped<IWebsiteTrafficService, WebsiteTrafficService>();

            services.AddScoped<
                IMessageService<
                    LeaderboardMessagesEntity,
                    LeaderboardMessagesRepository,
                    LeaderboardMessageFactory
                >,
                MessageService<
                    LeaderboardMessagesEntity,
                    LeaderboardMessagesRepository,
                    LeaderboardMessageFactory
                >
            >();
            services.AddScoped<
                IMessageService<
                    WebsiteTrafficMessagesEntity,
                    WebsiteTrafficMessagesRepository,
                    WebsiteTrafficMessageFactory
                >,
                MessageService<
                    WebsiteTrafficMessagesEntity,
                    WebsiteTrafficMessagesRepository,
                    WebsiteTrafficMessageFactory
                >
            >();
            services.AddScoped<
                IMessageService<
                    WelcomeMessageEntity,
                    WelcomeMessagesRepository,
                    WelcomeMessageFactory
                >,
                MessageService<
                    WelcomeMessageEntity,
                    WelcomeMessagesRepository,
                    WelcomeMessageFactory
                >
            >();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<LeaderboardMessagesRepository>();
            services.AddScoped<WebsiteTrafficMessagesRepository>();
            services.AddScoped<WelcomeMessagesRepository>();

            services.AddScoped<IWebsiteTrafficRepository, WebsiteTrafficRepository>();
            services.AddScoped<ILeaderboardsRepository, LeaderboardsRepository>();
            services.AddScoped<
                ILeaderboardScoreSourceRepository,
                LeaderboardScoreSourceRepository
            >();
            services.AddScoped<ILeaderboardScoresRepository, LeaderboardScoresRepository>();
            services.AddScoped<ILeaderboardMessagesRepository, LeaderboardMessagesRepository>();
            services.AddScoped<
                IWebsiteTrafficMessagesRepository,
                WebsiteTrafficMessagesRepository
            >();
            services.AddScoped<IWelcomeMessagesRepository, WelcomeMessagesRepository>();
        }

        private static void RegisterFactories(IServiceCollection services)
        {
            services.AddScoped<LeaderboardMessageFactory>();
            services.AddScoped<WebsiteTrafficMessageFactory>();
            services.AddScoped<WelcomeMessageFactory>();

            services.AddScoped<
                IMessageEntityFactory<LeaderboardMessagesEntity>,
                LeaderboardMessageFactory
            >();
            services.AddScoped<
                IMessageEntityFactory<WebsiteTrafficMessagesEntity>,
                WebsiteTrafficMessageFactory
            >();
            services.AddScoped<
                IMessageEntityFactory<WelcomeMessageEntity>,
                WelcomeMessageFactory
            >();
        }

        private static void RegisterBackgroundServices(IServiceCollection services)
        {
            services.AddHostedService<DiscordBotBackgroundService>();
            services.AddHostedService<UpdateLeaderboardBackgroundService>();
        }

        private static void RegisterMiddleware(WebApplication app)
        {
            app.UseMiddleware<CorsMiddleware>();
        }

        private static void RegisterBotConfiguration(WebApplicationBuilder builder)
        {
            string discordToken = builder.Configuration["Discord:Token"] ?? "";
            DiscordIntents intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers;

            DiscordClientBuilder discordBotBuilder = DiscordClientBuilder.CreateDefault(
                discordToken,
                intents
            );

            discordBotBuilder.ConfigureEventHandlers(events =>
            {
                events.AddEventHandlers<ComponentInteractionCreatedHandler>();
                events.AddEventHandlers<GuildDownloadCompletedHandler>();
                events.AddEventHandlers<GuildMemberUpdatedHandler>();
                events.AddEventHandlers<GuildMemberAddedHandler>();
                events.AddEventHandlers<SessionCreatedHandler>();
                events.AddEventHandlers<CommandInvokedHandler>();
                events.AddEventHandlers<CommandErroredHandler>();
                events.AddEventHandlers<GuildCreatedHandler>();
            });
        }

        private static void RegisterLogger(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog();
        }

        private static void RegisterConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.Configure<DiscordConfig>(builder.Configuration.GetSection("Discord"));
            builder.Services.Configure<ServerConfig>(builder.Configuration.GetSection("Server"));
            builder.Services.Configure<DeveloperEnvironmentConfig>(
                builder.Configuration.GetSection("DeveloperEnvironment")
            );
        }

        private static void SetupDatabase(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetSection("Database")["ConnectionString"];

            builder.Services.AddDbContext<LundBotDiscordDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
        }
    }
}
