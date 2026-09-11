using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using LundBot.Application.Discord.Users;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.Interactions;
using LundBot.Presentation.Discord.Leaderboards.AutoCompletes;

namespace LundBot.Presentation.Discord.Leaderboards.Commands
{
    public sealed class WarnOnLeaderboardCommand : AbstractBaseCommand
    {
        private readonly IWarnLeaderboardService _warnLeaderboardService;

        public WarnOnLeaderboardCommand(
            IWarnLeaderboardService leaderboardService,
            IDiscordInteractionService discordInteractionService
        )
            : base(discordInteractionService)
        {
            _warnLeaderboardService = leaderboardService;
        }

        [RequirePermissions(DiscordPermission.Administrator)]
        [Command("warn")]
        [Description("Register a warning for a user on the specified leaderboard. User will NOT be notified.")]
        public async Task RegisterWarningAsync(
            CommandContext context,
            [SlashAutoCompleteProvider(typeof(WarningLeaderboardChannelAutocomplete))]
            [Parameter("channel")]
            [Description("The Channel that has the leaderboard")]
                ulong channelId,
            [Parameter("user")] [Description("The user to register warning for")] DiscordUser user
        )
        {
            if (!await IsCommandSentFromGuild(context))
            {
                return;
            }

            DiscordUserDto targetUser = new DiscordUserDto(user.Id, user.Username, user.GlobalName);

            await TaskWithErrorHandlingAsync(
                context,
                () => _warnLeaderboardService.RegisterWarningAsync(channelId, context.User.Id, user.Id),
                $"Registered a warning for {targetUser.Username} on the leaderboard in <#{channelId}>."
            );
        }
    }
}
