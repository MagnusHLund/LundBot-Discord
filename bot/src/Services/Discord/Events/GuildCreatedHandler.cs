using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LundBot.Interfaces.Services;

namespace LundBot.Services.Discord.Events
{
    public sealed class GuildCreatedHandler : IEventHandler<GuildCreatedEventArgs>
    {
        private readonly ICommandsService _commandsService;
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<GuildCreatedHandler>();

        public GuildCreatedHandler(ICommandsService commandsService)
        {
            _commandsService = commandsService;
        }

        public Task HandleEventAsync(DiscordClient sender, GuildCreatedEventArgs eventArgs)
        {
            _logger.Information(
                "Guild created: {GuildName} ({GuildId})",
                eventArgs.Guild.Name,
                eventArgs.Guild.Id
            );

            return RefreshCommandsForGuildAsync(eventArgs.Guild);
        }

        private async Task RefreshCommandsForGuildAsync(DiscordGuild guild)
        {
            try
            {
                await _commandsService.RefreshCommandsAsync();
                _logger.Information("Registered commands for guild {GuildId}", guild.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error registering commands for guild {GuildId}", guild.Id);
            }
        }
    }
}
