using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LundBot.Config;
using LundBot.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace LundBot.Services.Discord.Events
{
    public sealed class GuildMemberUpdatedHandler : IEventHandler<GuildMemberUpdatedEventArgs>
    {
        private readonly IModerationActionsService _moderationActionsService;
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordConfig _discordConfig;
        private readonly Serilog.ILogger _logger =
            Serilog.Log.ForContext<GuildMemberUpdatedHandler>();

        public GuildMemberUpdatedHandler(
            IModerationActionsService moderationActionsService,
            IServiceProvider serviceProvider,
            IOptions<DiscordConfig> discordConfig
        )
        {
            _moderationActionsService = moderationActionsService;
            _serviceProvider = serviceProvider;
            _discordConfig = discordConfig.Value;
        }

        public async Task HandleEventAsync(
            DiscordClient sender,
            GuildMemberUpdatedEventArgs eventArgs
        )
        {
            _logger.Information(
                "Member updated: {UserName} ({UserId}) in guild {GuildName} ({GuildId})",
                eventArgs.Member.Username,
                eventArgs.Member.Id,
                eventArgs.Guild.Name,
                eventArgs.Guild.Id
            );

            await AutoKickUserIfRoleAssigned(eventArgs.Guild, eventArgs.Member);
        }

        private async Task AutoKickUserIfRoleAssigned(DiscordGuild guild, DiscordMember member)
        {
            ulong roleIdToAutoKick = _discordConfig.RoleIdToAutoKick;

            if (roleIdToAutoKick != 0)
            {
                DiscordRole? roleToKick = guild.Roles.Values.FirstOrDefault(r =>
                    r.Id == roleIdToAutoKick
                );

                string kickReason =
                    $"You have been automatically kicked due to picking the \"{roleToKick?.Name ?? "Unknown"}\" role. This community is PC only. Feel free to join back, if you own IW on PC or plan to purchase it on PC";

                bool kicked = await _moderationActionsService.KickUserDueToRoleAssignmentAsync(
                    guild,
                    member,
                    roleToKick,
                    kickReason
                );

                if (kicked)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var welcomeMessageService =
                        scope.ServiceProvider.GetRequiredService<IWelcomeMessageService>();

                    await welcomeMessageService.RemoveWelcomeMessageAsync(guild, member.Id);
                }
            }
        }
    }
}
