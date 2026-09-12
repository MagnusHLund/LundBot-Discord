using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.Interactions;
using LundBot.Presentation.Discord.Leaderboards.AutoCompletes;

namespace LundBot.Presentation.Discord.Leaderboards.Commands
{
    public sealed class RemoveLeaderboardCommand : AbstractBaseCommand
    {
        private readonly ILeaderboardService _leaderboardService;

        public RemoveLeaderboardCommand(
            IDiscordInteractionService discordInteractionService,
            ILeaderboardService leaderboardService
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
            [SlashAutoCompleteProvider(typeof(LeaderboardChannelAutocomplete))]
            [Description("The Channel that the leaderboard is in.")]
                ulong channelId,
            [Parameter("confirm")] [Description("Confirm the removal of the leaderboard.")] bool confirm
        )
        {
            if (!await IsCommandSentFromGuild(context))
            {
                return;
            }

            if (!await IsValidDiscordIdAsync(context, channelId, "channel"))
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

            await TaskWithErrorHandlingAsync(
                context,
                () => _leaderboardService.RemoveLeaderboardAsync(channelId),
                $"Leaderboard removed successfully from <#{channelId}>."
            );
        }
    }
}
