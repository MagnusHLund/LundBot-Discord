using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordCommandService : IDiscordCommandService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordCommandService>();

        public async Task DeleteGlobalApplicationCommandAsync(ulong commandId)
        {
            _logger.Information("Unregistering global command with ID {CommandId}", commandId);

            try
            {
                await BotService.DiscordClient.DeleteGlobalApplicationCommandAsync(commandId);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to unregister global command with ID {CommandId}",
                    commandId
                );
                throw;
            }
        }

        public async Task DeleteGuildApplicationCommandAsync(ulong guildId, ulong commandId)
        {
            _logger.Information(
                "Unregistering command with ID {CommandId} for guild {GuildId}",
                commandId.ToString(),
                guildId.ToString()
            );

            try
            {
                await BotService.DiscordClient.DeleteGuildApplicationCommandAsync(
                    guildId,
                    commandId
                );
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to unregister command with ID {CommandId} for guild {GuildId}",
                    commandId,
                    guildId
                );
                throw;
            }
        }

        public async Task<
            IReadOnlyList<DiscordApplicationCommand>
        > GetGlobalApplicationCommandsAsync()
        {
            _logger.Information("Fetching global application commands...");

            try
            {
                return await BotService.DiscordClient.GetGlobalApplicationCommandsAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch global application commands");
                return [];
            }
        }

        public async Task<
            IReadOnlyList<DiscordApplicationCommand>
        > GetGuildApplicationCommandsAsync(ulong guildId)
        {
            _logger.Information("Fetching application commands for guild {GuildId}...", guildId);

            try
            {
                return await BotService.DiscordClient.GetGuildApplicationCommandsAsync(guildId);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to fetch application commands for guild {GuildId}",
                    guildId
                );
                return [];
            }
        }

        public async Task<SlashCommandsExtension> GetSlashCommandsAsync()
        {
            _logger.Information("Fetching slash commands...");

            try
            {
                // TODO: Figure this out in latest DSharpPlus version.
                // return BotService.DiscordClient.GetSlashCommands();
                throw new NotImplementedException("Idk how to do this yet");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch slash commands");
                throw;
            }
        }

        public async Task RefreshCommandsAsync(SlashCommandsExtension slashCommands)
        {
            _logger.Information("Refreshing commands...");

            try
            {
                // TODO: Figure this out in latest DSharpPlus version.
                // await BotService.DiscordClient.RefreshCommands(slashCommands);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to refresh commands");
            }
        }
    }
}
