using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
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

        [SlashRequirePermissions(Permissions.Administrator)]
        [SlashCommand(
            "warn",
            "Register a warning for a user on the specified leaderboard. User will NOT be notified."
        )]
        public async Task RegisterWarningAsync(
            InteractionContext context,
            [Autocomplete(typeof(WarningLeaderboardChannelsAutocomplete))]
            [Option("channel", "The Channel that has the leaderboard")]
                string channelId,
            [Option("user", "The user to register warning for")] DiscordUser userTarget
        )
        {
            if (!await IsCommandSentFromServer(context))
            {
                return;
            }

            if (
                !ulong.TryParse(channelId, out var parsedId)
                || context.Guild.GetChannel(parsedId) is not DiscordChannel channel
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
