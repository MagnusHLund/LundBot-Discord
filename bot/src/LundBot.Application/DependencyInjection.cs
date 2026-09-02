using LundBot.Application.Common.Bot;
using LundBot.Application.Features.Leaderboards;
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

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMemberJoinService, MemberJoinService>();
            services.AddScoped<ILeaderboardService, LeaderboardService>();
            services.AddScoped<IWebsiteTrafficService, WebsiteTrafficService>();
            services.AddScoped<IModerationActionService, ModerationActionService>();

            // TODO: MessageService. It is a little more advanced than the others.

            return services;
        }
    }
}
