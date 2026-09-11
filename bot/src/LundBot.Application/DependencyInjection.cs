using LundBot.Application.Common.Bot;
using LundBot.Application.Common.Messaging;
using LundBot.Application.Features.InfiniteWarfare.Maps;
using LundBot.Application.Features.Invites;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Application.Features.Leaderboards.Shared;
using LundBot.Application.Features.Leaderboards.Types;
using LundBot.Application.Features.MemberJoin;
using LundBot.Application.Features.Moderation;
using LundBot.Application.Features.WebsiteTraffic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LundBot.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices();
            services.AddConfiguration(configuration);

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ICommandService, CommandService>();
            services.AddSingleton<IRandomMapService, RandomMapService>();

            services.AddScoped<IMemberJoinService, MemberJoinService>();
            services.AddScoped<IInviteService, InviteService>();
            services.AddScoped<IInviteLeaderboardService, InviteLeaderboardService>();
            services.AddScoped<IWarnLeaderboardService, WarnLeaderboardService>();
            services.AddScoped<UpvoteLeaderboardService>();
            services.AddScoped<IAbstractLeaderboardService>(serviceProvider =>
                serviceProvider.GetRequiredService<UpvoteLeaderboardService>()
            );
            services.AddScoped<IUpvoteLeaderboardService>(serviceProvider =>
                serviceProvider.GetRequiredService<UpvoteLeaderboardService>()
            );
            services.AddScoped<IWebsiteTrafficService, WebsiteTrafficService>();
            services.AddScoped<IModerationActionService, ModerationActionService>();

            services.AddScoped<LeaderboardMessageFactory>();
            services.AddScoped<MemberJoinMessageFactory>();
            services.AddScoped<WebsiteTrafficMessageFactory>();
            services.AddScoped(typeof(IMessageService<,,>), typeof(MessageService<,,>));

            return services;
        }

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
