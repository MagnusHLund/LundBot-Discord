using DSharpPlus;
using DSharpPlus.EventArgs;
using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Members;
using LundBot.Application.Features.Invites;
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

            DiscordMemberDto memberDto = new DiscordMemberDto(
                userId: eventArgs.Member.Id,
                username: eventArgs.Member.Username,
                displayName: eventArgs.Member.Discriminator
            );

            DiscordGuildDto guildDto = new DiscordGuildDto(
                guildId: eventArgs.Guild.Id,
                guildName: eventArgs.Guild.Name
            );

            using var scope = _serviceProvider.CreateScope();
            var inviteService = scope.ServiceProvider.GetRequiredService<IInviteService>();
            var memberJoinService = scope.ServiceProvider.GetRequiredService<IMemberJoinService>();

            await memberJoinService.SendWelcomeMessageAsync(guildDto.GuildId, memberDto);
            await inviteService.RegisterUserJoinedWithInviteAsync(guildDto, memberDto);
        }
    }
}
