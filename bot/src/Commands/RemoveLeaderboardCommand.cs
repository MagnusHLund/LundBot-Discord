using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
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

        [SlashRequirePermissions(true, DiscordPermission.Administrator)]
        [SlashCommand("remove-leaderboard", "Removes an existing leaderboard.")]
        public async Task RemoveLeaderboardAsync(
            InteractionContext context,
            [Option("channel", "The Channel that the leaderboard is in")] DiscordChannel channel,
            [Option("confirm", "Confirm the removal of the leaderboard")] bool confirm
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

            await TaskWithErrorHandlingAsync(
                context,
                () => _leaderboardService.RemoveLeaderboardAsync(channel),
                $"Leaderboard removed successfully from {channel.Mention}."
            );
        }
    }
}
