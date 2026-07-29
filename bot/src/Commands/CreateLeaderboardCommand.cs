using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using LundBot.Enums;
using LundBot.Interfaces.Services;
using LundBot.Utils;

namespace LundBot.Commands
{
    public sealed class CreateLeaderboardCommand : BaseCommand
    {
        private readonly ILeaderboardService _leaderboardService;

        public CreateLeaderboardCommand(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [SlashRequirePermissions(Permissions.Administrator)]
        [SlashCommand("create-leaderboard", "Creates a new leaderboard.")]
        public async Task CreateLeaderboardAsync(
            InteractionContext context,
            [Option("Channel", "The Channel that the leaderboard will use")]
            [ChannelTypes(ChannelType.Text)]
                DiscordChannel Channel,
            [Option("type", "The type of the leaderboard.")] LeaderboardType type,
            [Option(
                "title",
                "The title of the leaderboard. eg 'Top Upvoted Users'. Max 64 characters."
            )]
            [MinimumLength(1)]
            [MaximumLength(64)]
                string title,
            [Option(
                "message",
                "Message to prepend above the leaderboard data and title. Max 256 characters."
            )]
            [MaximumLength(256)]
                string? message
        )
        {
            if (!await IsCommandSentFromServer(context))
            {
                return;
            }

            title = title.Trim();
            if (!ValidationUtils.IsValidLengthString(title, 64, 1))
            {
                await SendResponseAsync(
                    context,
                    "The title must be between 1 and 64 characters long."
                );
                return;
            }

            message = message != null ? message.Trim() : "";
            if (!ValidationUtils.IsValidLengthString(message, 256, 0))
            {
                await SendResponseAsync(
                    context,
                    "The message must be between 0 and 256 characters long."
                );
                return;
            }

            await TaskWithErrorHandlingAsync(
                context,
                () => _leaderboardService.CreateLeaderboardAsync(Channel, title, message, type),
                $"{type} Leaderboard created successfully, in {Channel.Mention}."
            );
        }
    }
}
