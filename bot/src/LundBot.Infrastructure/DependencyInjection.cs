using LundBot.Application.Common.Caching;
using LundBot.Application.Discord.Bot;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Members;
using LundBot.Application.Discord.Messages;
using LundBot.Application.Discord.Stickers;
using LundBot.Application.Discord.Users;
using LundBot.Application.Features.Leaderboards;
using LundBot.Application.Features.MemberJoin;
using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Infrastructure.Caching;
using LundBot.Infrastructure.Discord.Configuration;
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
            services.AddConfiguration(configuration);

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

        private static IServiceCollection AddConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<DiscordConfig>(configuration.GetSection("Discord"));

            return services;
        }
    }
}
