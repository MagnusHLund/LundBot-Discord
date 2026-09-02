using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Extensions;
using LundBot.Presentation.Api.Bot.Middleware;
using LundBot.Presentation.Config;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.Leaderboards;
using Serilog;
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
            services.AddBackgroundServices();

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
                .Enrich.FromLogContext()
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

            return services;
        }

        private static IServiceCollection AddConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<ServerConfig>(configuration.GetSection("Server"));
            services.Configure<DiscordConfig>(configuration.GetSection("Discord"));
            services.Configure<DeveloperEnvironmentConfig>(configuration.GetSection("DeveloperEnvironment"));

            return services;
        }

        private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<DiscordBotBackgroundService>();
            services.AddHostedService<UpdateLeaderboardBackgroundService>();

            return services;
        }
    }
}
