using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using LundBot.Application.Discord.Commands;
using Serilog;

namespace LundBot.Infrastructure.Discord.Commands
{
    public sealed class DiscordCommandService : IDiscordCommandService
    {
        private readonly DiscordClient _discordClient;
        private readonly CommandsExtension _commands;

        private readonly ILogger _logger = Log.ForContext<DiscordCommandService>();

        public DiscordCommandService(DiscordClient discordClient, CommandsExtension commands)
        {
            _discordClient = discordClient;
            _commands = commands;
        }

        public async Task<bool> RefreshCommandsAsync()
        {
            _logger.Information("Refreshing commands...");

            try
            {
                await _commands.RefreshAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to refresh commands");
                return false;
            }
        }

        public async Task<bool> DeleteGlobalApplicationCommandAsync(ulong commandId)
        {
            _logger.Information("Deleting global application command with ID {CommandId}...", commandId);

            try
            {
                await _discordClient.DeleteGlobalApplicationCommandAsync(commandId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete global application command with ID {CommandId}", commandId);
                return false;
            }
        }

        public async Task<bool> DeleteGuildApplicationCommandAsync(ulong guildId, ulong commandId)
        {
            _logger.Information(
                "Deleting guild application command with ID {CommandId} in guild {GuildId}...",
                commandId,
                guildId
            );

            try
            {
                await _discordClient.DeleteGuildApplicationCommandAsync(guildId, commandId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to delete guild application command with ID {CommandId} in guild {GuildId}",
                    commandId,
                    guildId
                );
                return false;
            }
        }

        public async Task<bool> DeleteAllGlobalApplicationCommandsAsync()
        {
            _logger.Information("Deleting all global application commands...");

            try
            {
                IReadOnlyList<DiscordApplicationCommand> commands =
                    await _discordClient.GetGlobalApplicationCommandsAsync();

                foreach (var command in commands)
                {
                    await _discordClient.DeleteGlobalApplicationCommandAsync(command.Id);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete all global application commands");
                return false;
            }
        }

        public async Task<bool> DeleteAllGuildApplicationCommandsAsync(ulong guildId)
        {
            _logger.Information("Deleting all guild application commands in guild {GuildId}...", guildId);

            try
            {
                IReadOnlyList<DiscordApplicationCommand> commands =
                    await _discordClient.GetGuildApplicationCommandsAsync(guildId);

                foreach (var command in commands)
                {
                    await _discordClient.DeleteGuildApplicationCommandAsync(guildId, command.Id);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete all guild application commands in guild {GuildId}", guildId);
                return false;
            }
        }
    }
}
