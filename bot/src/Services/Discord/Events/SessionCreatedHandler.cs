using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Services.Discord.Events
{
    public sealed class SessionCreatedHandler : IEventHandler<SessionCreatedEventArgs>
    {
        private readonly IDiscordMemberService _discordMemberService;
        private readonly IDiscordBotService _discordBotService;

        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<SessionCreatedHandler>();

        public SessionCreatedHandler(
            IDiscordMemberService discordMemberService,
            IDiscordBotService discordBotService
        )
        {
            _discordMemberService = discordMemberService;
            _discordBotService = discordBotService;
        }

        public async Task HandleEventAsync(DiscordClient sender, SessionCreatedEventArgs eventArgs)
        {
            _logger.Information("Ready fired, running BotService initialization...");
            await SetBotStatusAsync();
            await PreloadMembersAsync();
        }

        private async Task SetBotStatusAsync()
        {
            var activity = new DiscordActivity(
                "Stuck in a movie theater",
                DiscordActivityType.Playing
            );
            await _discordBotService.UpdateBotStatusAsync(activity);
        }

        private async Task PreloadMembersAsync()
        {
            foreach (var g in BotService.DiscordClient.Guilds.Values)
            {
                _logger.Information("Bot is in guild: {GuildName} ({GuildId})", g.Name, g.Id);

                await _discordMemberService.PreloadMembersAsync(g);
            }
        }
    }
}
