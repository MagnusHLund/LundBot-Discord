using System.ComponentModel;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using LundBot.Enums;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using LundBot.Utils;

namespace LundBot.Commands
{
    public sealed class CreateLeaderboardCommand : BaseCommand
    {
        private readonly ILeaderboardService _leaderboardService;

        public CreateLeaderboardCommand(
            ILeaderboardService leaderboardService,
            IDiscordInteractionService discordInteractionService
        )
            : base(discordInteractionService)
        {
            _leaderboardService = leaderboardService;
        }

        [Command("create-leaderboard")]
        [Description("Creates a new leaderboard.")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task CreateLeaderboardAsync(
            CommandContext context,
            [Parameter("Channel")]
            [Description("The Channel that the leaderboard will use")]
            [ChannelTypes(DiscordChannelType.Text)]
                DiscordChannel Channel,
            [Parameter("type")] [Description("The type of the leaderboard.")] LeaderboardType type,
            [Parameter("title")]
            [Description(
                "The title of the leaderboard. eg 'Top Upvoted Users'. Max 64 characters."
            )]
            [MinMaxValue(1, 64)]
                string title,
            [Parameter("message")]
            [Description(
                "Message to prepend above the leaderboard data and title. Max 256 characters."
            )]
            [MinMaxLength(0, 256)]
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
