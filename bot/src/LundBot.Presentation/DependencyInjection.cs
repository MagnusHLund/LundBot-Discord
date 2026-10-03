using System.Net;
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
using Microsoft.AspNetCore.HttpOverrides;
using Serilog.Events;
using IPNetwork = System.Net.IPNetwork;

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
            app.UseForwardedHeaders();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
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
            services.AddCommandsExtension((_, extension) => { });

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
            if (string.IsNullOrWhiteSpace(configuration["Server:ApiKey"]))
            {
                throw new InvalidOperationException("Server API key is not configured.");
            }

            services.Configure<ServerConfig>(configuration.GetSection("Server"));
            services.Configure<DiscordCommandConfig>(configuration.GetSection("Discord"));
            services.Configure<DiscordKickConfig>(configuration.GetSection("Discord"));
            services.Configure<DeveloperEnvironmentConfig>(configuration.GetSection("DeveloperEnvironment"));
            services.AddForwardedHeadersConfiguration(configuration);

            return services;
        }

        private static IServiceCollection AddForwardedHeadersConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            ForwardedHeadersConfig forwardedHeadersConfig =
                configuration.GetSection("ForwardedHeaders").Get<ForwardedHeadersConfig>() ?? new();

            List<IPAddress> knownProxies = forwardedHeadersConfig
                .KnownProxies.Select(proxy =>
                    IPAddress.TryParse(proxy, out IPAddress? address)
                        ? address
                        : throw new InvalidOperationException($"Invalid trusted proxy IP address '{proxy}'.")
                )
                .ToList();

            List<IPNetwork> knownNetworks = forwardedHeadersConfig
                .KnownNetworks.Select(network =>
                    IPNetwork.TryParse(network, out IPNetwork parsedNetwork)
                        ? parsedNetwork
                        : throw new InvalidOperationException($"Invalid trusted proxy network '{network}'.")
                )
                .ToList();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
                options.KnownProxies.Clear();
                options.KnownIPNetworks.Clear();

                foreach (IPAddress proxy in knownProxies)
                {
                    options.KnownProxies.Add(proxy);
                }

                foreach (IPNetwork network in knownNetworks)
                {
                    options.KnownIPNetworks.Add(network);
                }
            });

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
