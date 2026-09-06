using DSharpPlus;
using DSharpPlus.EventArgs;
using LundBot.Application.Features.MemberJoin;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class GuildMemberAddedHandler : IEventHandler<GuildMemberAddedEventArgs>
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger _logger = Log.ForContext<GuildMemberAddedHandler>();

        public GuildMemberAddedHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleEventAsync(DiscordClient sender, GuildMemberAddedEventArgs eventArgs)
        {
            _logger.Information(
                "Member added: {UserName} ({UserId}) to guild {GuildName} ({GuildId})",
                eventArgs.Member.Username,
                eventArgs.Member.Id,
                eventArgs.Guild.Name,
                eventArgs.Guild.Id
            );

            using var scope = _serviceProvider.CreateScope();
            var memberJoinService = scope.ServiceProvider.GetRequiredService<IMemberJoinService>();

            // await memberJoinService.SendWelcomeMessageAsync(eventArgs.Guild, eventArgs.Member);
            // await memberJoinService.RegisterWhoInvitedJoinedUser(eventArgs.Guild, eventArgs.Member);
        }
    }
}
