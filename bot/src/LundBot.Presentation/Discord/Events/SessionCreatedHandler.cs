using DSharpPlus;
using DSharpPlus.EventArgs;
using LundBot.Application.Discord.Bot;
using LundBot.Application.Discord.Members;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class SessionCreatedHandler : IEventHandler<SessionCreatedEventArgs>
    {
        private readonly DiscordClient _discordClient;
        private readonly IDiscordBotService _discordBotService;
        private readonly IDiscordMemberService _discordMemberService;

        private readonly ILogger _logger = Log.ForContext<SessionCreatedHandler>();

        public SessionCreatedHandler(
            DiscordClient discordClient,
            IDiscordBotService discordBotService,
            IDiscordMemberService discordMemberService
        )
        {
            _discordClient = discordClient;
            _discordBotService = discordBotService;
            _discordMemberService = discordMemberService;
        }

        public async Task HandleEventAsync(DiscordClient sender, SessionCreatedEventArgs eventArgs)
        {
            _logger.Information("Ready fired, running BotService initialization...");

            await SetBotStatusAsync();
            await PreloadMembersAsync();
        }

        private async Task SetBotStatusAsync()
        {
            string statusMessage = "Stuck in a movie theater";
            await _discordBotService.UpdateBotStatusAsync(statusMessage);
        }

        private async Task PreloadMembersAsync()
        {
            foreach (var guild in _discordClient.Guilds.Values)
            {
                // TODO: For some reason guild name is null here. Maybe look into DSharpPlus and fix this issue?
                _logger.Information("Bot is in guild: {GuildName} ({GuildId})", guild.Name, guild.Id);

                bool success = await _discordMemberService.PreloadMembersAsync(guild.Id);

                if (!success)
                {
                    _logger.Error("Failed to preload members for guild: {GuildName} ({GuildId})", guild.Name, guild.Id);
                }
            }
        }
    }
}
