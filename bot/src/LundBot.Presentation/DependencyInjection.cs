using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Extensions;
using LundBot.Presentation.Api.Authentication;
using LundBot.Presentation.Api.Middleware;
using LundBot.Presentation.Config;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.Commands;
using LundBot.Presentation.Discord.Events;
using LundBot.Presentation.Discord.Interactions;
using LundBot.Presentation.Discord.Leaderboards.BackgroundServices;
using Microsoft.AspNetCore.Authentication;
using Serilog.Events;

namespace LundBot.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddConfiguration(configuration);
            services.AddDiscord(configuration);

            services.AddApiAuthentication();

            services.AddBackgroundServices();
            services.AddServices();
            services.AddEvents();

            return services;
        }

        public static WebApplication AddMiddleware(this WebApplication app)
        {
            app.UseMiddleware<CorsMiddleware>();

            return app;
        }

        public static WebApplicationBuilder AddLogger(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("DSharpPlus", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();

            return builder;
        }

        private static IServiceCollection AddDiscord(this IServiceCollection services, IConfiguration configuration)
        {
            string discordToken = configuration["Discord:Token"] ?? "";
            DiscordIntents intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers;

            services.AddDiscordClient(discordToken, intents);
            services.AddCommandsExtension((ServiceProvider, extension) => { });

            services.AddSingleton<IDiscordCommandRegistration, DiscordCommandRegistration>();

            return services;
        }

        private static IServiceCollection AddEvents(this IServiceCollection services)
        {
            services.ConfigureEventHandlers(events =>
            {
                events.AddEventHandlers<ComponentInteractionCreatedHandler>();
                events.AddEventHandlers<GuildDownloadCompletedHandler>();
                events.AddEventHandlers<GuildMemberUpdatedHandler>();
                events.AddEventHandlers<GuildMemberAddedHandler>();
                events.AddEventHandlers<SessionCreatedHandler>();
                events.AddEventHandlers<CommandExecutedHandler>();
                events.AddEventHandlers<CommandErroredHandler>();
                events.AddEventHandlers<GuildCreatedHandler>();
            });

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IDiscordInteractionService, DiscordInteractionService>();

            return services;
        }

        private static IServiceCollection AddConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<ServerConfig>(configuration.GetSection("Server"));
            services.Configure<DiscordCommandConfig>(configuration.GetSection("Discord"));
            services.Configure<DiscordKickConfig>(configuration.GetSection("Discord"));
            services.Configure<DeveloperEnvironmentConfig>(configuration.GetSection("DeveloperEnvironment"));

            return services;
        }

        private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<DiscordBotBackgroundService>();
            services.AddHostedService<UpdateLeaderboardBackgroundService>();

            return services;
        }

        private static IServiceCollection AddApiAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication("ApiKey")
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", _ => { });

            services.AddAuthorization();

            return services;
        }
    }
}
