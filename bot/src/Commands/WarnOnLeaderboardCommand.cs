using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using LundBot.AutocompleteProviders;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Commands
{
    public sealed class WarnOnLeaderboardCommand : BaseCommand
    {
        private readonly ILeaderboardService _leaderboardService;

        public WarnOnLeaderboardCommand(
            ILeaderboardService leaderboardService,
            IDiscordInteractionService discordInteractionService
        )
            : base(discordInteractionService)
        {
            _leaderboardService = leaderboardService;
        }

        [RequirePermissions(DiscordPermission.Administrator)]
        [Command("warn")]
        [Description(
            "Register a warning for a user on the specified leaderboard. User will NOT be notified."
        )]
        public async Task RegisterWarningAsync(
            CommandContext context,
            [SlashAutoCompleteProvider(typeof(WarningLeaderboardChannelsAutocomplete))]
            [Parameter("channel")]
            [Description("The Channel that has the leaderboard")]
                string channelId,
            [Parameter("user")]
            [Description("The user to register warning for")]
                DiscordUser userTarget
        )
        {
            if (!await IsCommandSentFromServer(context))
            {
                return;
            }

            if (
                !ulong.TryParse(channelId, out var parsedId)
                || await context.Guild.GetChannelAsync(parsedId) is not DiscordChannel channel
            )
            {
                await SendResponseAsync(context, "The specified channel does not exist.");
                return;
            }

            await TaskWithErrorHandlingAsync(
                context,
                () => _leaderboardService.RegisterWarningOnLeaderboardAsync(channel, userTarget),
                $"Registered a warning for {userTarget.Username} on the leaderboard in {channel.Mention}."
            );
        }
    }
}
