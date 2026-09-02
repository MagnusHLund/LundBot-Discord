using DSharpPlus.Extensions;
using LundBot.Application.Common.Caching;
using LundBot.Application.Common.Discord;
using LundBot.Application.Features.Leaderboards;
using LundBot.Application.Features.MemberJoin;
using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Infrastructure.Caching;
using LundBot.Infrastructure.Discord.Events;
using LundBot.Infrastructure.Discord.Services;
using LundBot.Infrastructure.Persistence;
using LundBot.Infrastructure.Persistence.Repositories.Leaderboards;
using LundBot.Infrastructure.Persistence.Repositories.MemberJoin;
using LundBot.Infrastructure.Persistence.Repositories.WebsiteTraffic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LundBot.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDatabase(configuration);

            services.AddEvents();
            services.AddServices();
            services.AddRepositories();

            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetSection("Database")["ConnectionString"] ?? "";

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            services.AddDbContext<LundBotDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

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
            services.AddSingleton<ICacheService, CacheService>();

            services.AddSingleton<IDiscordBotService, DiscordBotService>();
            services.AddSingleton<IDiscordUserService, DiscordUserService>();
            services.AddSingleton<IDiscordGuildService, DiscordGuildService>();
            services.AddSingleton<IDiscordMemberService, DiscordMemberService>();
            services.AddSingleton<IDiscordChannelService, DiscordChannelService>();
            services.AddSingleton<IDiscordMessageService, DiscordMessageService>();
            services.AddSingleton<IDiscordStickerService, DiscordStickerService>();
            services.AddSingleton<IDiscordInteractionService, DiscordInteractionService>();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ILeaderboardScoreSourceRepository, LeaderboardScoreSourceRepository>();
            services.AddScoped<IWebsiteTrafficMessageRepository, WebsiteTrafficMessageRepository>();
            services.AddScoped<ILeaderboardMessageRepository, LeaderboardMessageRepository>();
            services.AddScoped<IMemberJoinMessageRepository, MemberJoinMessageRepository>();
            services.AddScoped<ILeaderboardScoreRepository, LeaderboardScoreRepository>();
            services.AddScoped<IWebsiteTrafficRepository, WebsiteTrafficRepository>();
            services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();

            services.AddScoped<WebsiteTrafficMessageRepository>();
            services.AddScoped<LeaderboardMessageRepository>();
            services.AddScoped<MemberJoinMessageRepository>();

            return services;
        }
    }
}
