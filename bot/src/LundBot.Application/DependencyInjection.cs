using LundBot.Application.Common.Bot;
using LundBot.Application.Common.Messaging;
using LundBot.Application.Features.InfiniteWarfare.Maps;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Application.Features.Leaderboards.Shared;
using LundBot.Application.Features.Leaderboards.Types;
using LundBot.Application.Features.MemberJoin;
using LundBot.Application.Features.Moderation;
using LundBot.Application.Features.Users;
using LundBot.Application.Features.WebsiteTraffic;
using Microsoft.Extensions.DependencyInjection;

namespace LundBot.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddServices();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ICommandService, CommandService>();
            services.AddSingleton<IRandomMapService, RandomMapService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMemberJoinService, MemberJoinService>();
            services.AddScoped<IInviteLeaderboardService, InviteLeaderboardService>();
            services.AddScoped<IWarnLeaderboardService, WarnLeaderboardService>();
            services.AddScoped<IUpvoteLeaderboardService, UpvoteLeaderboardService>();
            services.AddScoped<IWebsiteTrafficService, WebsiteTrafficService>();
            services.AddScoped<IModerationActionService, ModerationActionService>();

            services.AddScoped<LeaderboardMessageFactory>();
            services.AddScoped<WebsiteTrafficMessageFactory>();
            services.AddScoped(typeof(IMessageService<,,>), typeof(MessageService<,,>));

            return services;
        }
    }
}
