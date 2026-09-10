using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.Interactions;

namespace LundBot.Presentation.Discord.Leaderboards.Commands
{
    public sealed class CreateLeaderboardCommand : AbstractBaseCommand
    {
        private readonly IAbstractLeaderboardService _leaderboardService;

        public CreateLeaderboardCommand(
            IDiscordInteractionService discordInteractionService,
            IAbstractLeaderboardService leaderboardService
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
            [Parameter("type")] [Description("The type of the leaderboard.")] LeaderboardTypeEnum type,
            [Parameter("title")]
            [Description("The title of the leaderboard. eg 'Top Upvoted Users'. Max 64 characters.")]
            [MinMaxLength(1, 64)]
                string title,
            [Parameter("message")]
            [Description("Message to prepend above the leaderboard data and title. Max 256 characters.")]
            [MinMaxLength(0, 256)]
                string? message
        )
        {
            if (!await IsCommandSentFromGuild(context))
            {
                return;
            }

            title = title.Trim();
            message = message != null ? message.Trim() : string.Empty;

            await TaskWithErrorHandlingAsync(
                context,
                () => _leaderboardService.CreateLeaderboardAsync(Channel.Id, title, message, type),
                $"{type} Leaderboard created successfully, in {Channel.Mention}."
            );
        }
    }
}
