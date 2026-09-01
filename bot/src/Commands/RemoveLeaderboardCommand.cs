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
    public sealed class RemoveLeaderboardCommand : BaseCommand
    {
        private readonly ILeaderboardService _leaderboardService;

        public RemoveLeaderboardCommand(
            ILeaderboardService leaderboardService,
            IDiscordInteractionService discordInteractionService
        )
            : base(discordInteractionService)
        {
            _leaderboardService = leaderboardService;
        }

        [RequirePermissions(DiscordPermission.Administrator)]
        [Command("remove-leaderboard")]
        [Description("Removes an existing leaderboard.")]
        public async Task RemoveLeaderboardAsync(
            CommandContext context,
            [Parameter("channel")]
            [SlashAutoCompleteProvider(typeof(LeaderboardChannelsAutocomplete))]
            [Description("The Channel that the leaderboard is in.")]
                string channelId,
            [Parameter("confirm")]
            [Description("Confirm the removal of the leaderboard.")]
                bool confirm
        )
        {
            if (!await IsCommandSentFromServer(context))
            {
                return;
            }

            if (!confirm)
            {
                await SendResponseAsync(
                    context,
                    "You must confirm the removal of the leaderboard by setting the 'Confirm' option to true."
                );
                return;
            }

            if (
                !ulong.TryParse(channelId, out var parsedId)
                || await context.Guild!.GetChannelAsync(parsedId) is not DiscordChannel channel
            )
            {
                await SendResponseAsync(context, "The specified channel does not exist.");
                return;
            }

            await TaskWithErrorHandlingAsync(
                context,
                () => _leaderboardService.RemoveLeaderboardAsync(channel),
                $"Leaderboard removed successfully from {channel.Mention}."
            );
        }
    }
}
