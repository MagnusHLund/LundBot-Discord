using LundBot.Application.Common.Bot;
using LundBot.Application.Common.Caching;
using LundBot.Application.Common.Messaging;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Members;
using LundBot.Application.Discord.Roles;
using LundBot.Application.Discord.Users;
using LundBot.Application.Features.InfiniteWarfare.Maps;
using LundBot.Application.Features.Invites;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Application.Features.Leaderboards.Shared;
using LundBot.Application.Features.Leaderboards.Types;
using LundBot.Application.Features.MemberJoin;
using LundBot.Application.Features.Moderation;
using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Domain.Leaderboards;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LundBot.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices();
            services.AddFactories();
            services.AddConfiguration(configuration);

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ICommandService, CommandService>();
            services.AddSingleton<IRandomMapService, RandomMapService>();

            services.AddScoped<IMemberJoinService, MemberJoinService>();
            services.AddScoped<IInviteService, InviteService>();

            services.AddLeaderboardServices();

            services.AddScoped<IWebsiteTrafficService, WebsiteTrafficService>();
            services.AddScoped<IModerationActionService, ModerationActionService>();

            services.AddScoped(typeof(IMessageService<,,>), typeof(MessageService<,,>));

            return services;
        }

        private static IServiceCollection AddFactories(this IServiceCollection services)
        {
            services.AddScoped<LeaderboardMessageFactory>();
            services.AddScoped<MemberJoinMessageFactory>();
            services.AddScoped<WebsiteTrafficMessageFactory>();

            return services;
        }

        private static IServiceCollection AddLeaderboardServices(this IServiceCollection services)
        {
            const int DefaultTopScoreLimit = 100;
            const int WarnTopScoreLimit = 1000;

            // The generic ILeaderboardService is exposed for consumers that operate on leaderboards
            // regardless of type (e.g. the background refresh job, the API controller, autocompletes).
            services.AddScoped<ILeaderboardService>(serviceProvider =>
                CreateLeaderboardService(serviceProvider, DefaultTopScoreLimit)
            );

            services.AddScoped<InviteLeaderboardService>(serviceProvider => new InviteLeaderboardService(
                serviceProvider.GetRequiredService<IHostEnvironment>(),
                serviceProvider.GetRequiredService<IDiscordRoleService>(),
                serviceProvider.GetRequiredService<ILeaderboardRepository>(),
                serviceProvider.GetRequiredService<ILeaderboardScoreSourceRepository>(),
                serviceProvider.GetRequiredService<ILeaderboardScoreRepository>(),
                serviceProvider.GetRequiredService<IDiscordChannelService>(),
                serviceProvider.GetRequiredService<ILeaderboardQueue>(),
                CreateLeaderboardService(serviceProvider, DefaultTopScoreLimit)
            ));
            services.AddScoped<IInviteLeaderboardService>(serviceProvider =>
                serviceProvider.GetRequiredService<InviteLeaderboardService>()
            );

            services.AddScoped<UpvoteLeaderboardService>(serviceProvider => new UpvoteLeaderboardService(
                serviceProvider.GetRequiredService<IDiscordChannelService>(),
                serviceProvider.GetRequiredService<ILeaderboardQueue>(),
                serviceProvider.GetRequiredService<ILeaderboardScoreRepository>(),
                serviceProvider.GetRequiredService<ILeaderboardScoreSourceRepository>(),
                CreateLeaderboardService(serviceProvider, DefaultTopScoreLimit)
            ));
            services.AddScoped<IUpvoteLeaderboardService>(serviceProvider =>
                serviceProvider.GetRequiredService<UpvoteLeaderboardService>()
            );

            services.AddScoped<WarnLeaderboardService>(serviceProvider => new WarnLeaderboardService(
                serviceProvider.GetRequiredService<IDiscordChannelService>(),
                serviceProvider.GetRequiredService<ILeaderboardQueue>(),
                serviceProvider.GetRequiredService<ILeaderboardScoreRepository>(),
                serviceProvider.GetRequiredService<ILeaderboardScoreSourceRepository>(),
                CreateLeaderboardService(serviceProvider, WarnTopScoreLimit)
            ));
            services.AddScoped<IWarnLeaderboardService>(serviceProvider =>
                serviceProvider.GetRequiredService<WarnLeaderboardService>()
            );

            return services;
        }

        private static LeaderboardService CreateLeaderboardService(
            IServiceProvider serviceProvider,
            int topScoreLimit
        ) =>
            new LeaderboardService(
                serviceProvider.GetRequiredService<IDiscordUserService>(),
                serviceProvider.GetRequiredService<IDiscordMemberService>(),
                serviceProvider.GetRequiredService<IDiscordChannelService>(),
                serviceProvider.GetRequiredService<ILeaderboardRepository>(),
                serviceProvider.GetRequiredService<ILeaderboardScoreRepository>(),
                serviceProvider.GetRequiredService<ILeaderboardMessageRepository>(),
                serviceProvider.GetRequiredService<
                    IMessageService<LeaderboardMessage, ILeaderboardMessageRepository, LeaderboardMessageFactory>
                >(),
                serviceProvider.GetRequiredService<ICacheService>(),
                topScoreLimit
            );

        private static IServiceCollection AddConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<WebsiteTrafficConfig>(configuration.GetSection("Discord"));

            return services;
        }
    }
}
