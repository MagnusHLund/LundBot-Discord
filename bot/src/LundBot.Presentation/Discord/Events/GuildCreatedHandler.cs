using DSharpPlus;
using DSharpPlus.EventArgs;
using LundBot.Application.Common.Bot;
using LundBot.Application.Discord.Guilds;
using LundBot.Infrastructure.Discord.Guilds.Mappings;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class GuildCreatedHandler : IEventHandler<GuildCreatedEventArgs>
    {
        private readonly ICommandService _commandService;

        private readonly ILogger _logger = Log.ForContext<GuildCreatedHandler>();

        public GuildCreatedHandler(ICommandService commandService)
        {
            _commandService = commandService;
        }

        public Task HandleEventAsync(DiscordClient sender, GuildCreatedEventArgs eventArgs)
        {
            _logger.Information("Guild created: {GuildName} ({GuildId})", eventArgs.Guild.Name, eventArgs.Guild.Id);

            DiscordGuildDto guild = eventArgs.Guild.Map();
            return RefreshCommandsForGuildAsync(guild);
        }

        private async Task RefreshCommandsForGuildAsync(DiscordGuildDto guild)
        {
            try
            {
                await _commandService.RefreshCommandsAsync();
                _logger.Information(
                    "Registered commands for guild {GuildName} ({GuildId})",
                    guild.GuildName,
                    guild.GuildId
                );
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error registering commands for guild {GuildName} ({GuildId})",
                    guild.GuildName,
                    guild.GuildId
                );
            }
        }
    }
}
