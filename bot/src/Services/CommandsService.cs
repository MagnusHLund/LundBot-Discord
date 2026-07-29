using DSharpPlus.SlashCommands;
using LundBot.Commands;
using LundBot.Config;
using LundBot.Interfaces.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace LundBot.Services
{
    public sealed class CommandsService : ICommandsService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<CommandsService>();
        private readonly DiscordConfig _discordConfig;

        public CommandsService(IOptions<DiscordConfig> options)
        {
            _discordConfig = options.Value;
        }

        public async Task RegisterCommandsAsync()
        {
            SlashCommandsExtension slash = BotService.DiscordClient.GetSlashCommands();

            List<ulong?> guildIds = GetFastUpdateGuildIds().Select(id => (ulong?)id).ToList();

            if (_discordConfig.ShouldRegisterGlobalCommands)
            {
                guildIds.Add(null); // Add null to register global commands
            }

            foreach (ulong? guildId in guildIds)
            {
                _logger.Information(
                    "Registering commands for guild {GuildId}…",
                    guildId == null ? "GLOBAL" : guildId.Value.ToString()
                );

                slash.RegisterCommands<CreateLeaderboardCommand>(guildId);
                slash.RegisterCommands<PingCommand>(guildId);
                slash.RegisterCommands<RandomMapCommand>(guildId);
                slash.RegisterCommands<RemoveLeaderboardCommand>(guildId);
                slash.RegisterCommands<UpvoteUserOnLeaderboardCommand>(guildId);
            }
        }

        public async Task LogRegisteredCommandsForGuildsAsync()
        {
            foreach (ulong guildId in GetFastUpdateGuildIds())
            {
                if (!BotService.DiscordClient.Guilds.ContainsKey(guildId))
                {
                    continue;
                }

                var registeredCommands =
                    await BotService.DiscordClient.GetGuildApplicationCommandsAsync(guildId);

                _logger.Information(
                    "Registered commands for guild {GuildId}: {Count}",
                    guildId.ToString(),
                    registeredCommands.Count
                );

                foreach (var command in registeredCommands)
                {
                    _logger.Information(
                        "Command: {Name} - {Description}",
                        command.Name,
                        command.Description
                    );
                }
            }
        }

        public async Task RefreshCommands()
        {
            SlashCommandsExtension slash = BotService.DiscordClient.GetSlashCommands();
            await slash.RefreshCommands();
        }

        public async Task<bool> UnregisterCommand(string commandId, bool global = false)
        {
            if (global)
            {
                try
                {
                    await BotService.DiscordClient.DeleteGlobalApplicationCommandAsync(
                        ulong.Parse(commandId)
                    );
                    _logger.Information(
                        "Unregistered global command with ID {CommandId}",
                        commandId
                    );
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        ex,
                        "Failed to unregister global command with ID {CommandId}",
                        commandId
                    );

                    return false;
                }
            }
            else
            {
                foreach (ulong guildId in GetFastUpdateGuildIds())
                {
                    try
                    {
                        await BotService.DiscordClient.DeleteGuildApplicationCommandAsync(
                            guildId,
                            ulong.Parse(commandId)
                        );
                        _logger.Information(
                            "Unregistered command with ID {CommandId} for guild {GuildId}",
                            commandId,
                            guildId.ToString()
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            ex,
                            "Failed to unregister command with ID {CommandId} for guild {GuildId}",
                            commandId,
                            guildId.ToString()
                        );

                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> UnregisterAllCommands(bool global = false)
        {
            if (global)
            {
                try
                {
                    var globalCommands =
                        await BotService.DiscordClient.GetGlobalApplicationCommandsAsync();

                    foreach (var command in globalCommands)
                    {
                        await BotService.DiscordClient.DeleteGlobalApplicationCommandAsync(
                            command.Id
                        );
                        _logger.Information(
                            "Unregistered global command with ID {CommandId}",
                            command.Id.ToString()
                        );
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to unregister global commands");
                    return false;
                }
            }
            else
            {
                foreach (ulong guildId in GetFastUpdateGuildIds())
                {
                    try
                    {
                        var guildCommands =
                            await BotService.DiscordClient.GetGuildApplicationCommandsAsync(
                                guildId
                            );

                        foreach (var command in guildCommands)
                        {
                            await BotService.DiscordClient.DeleteGuildApplicationCommandAsync(
                                guildId,
                                command.Id
                            );
                            _logger.Information(
                                "Unregistered command with ID {CommandId} for guild {GuildId}",
                                command.Id.ToString(),
                                guildId.ToString()
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            ex,
                            "Failed to unregister commands for guild {GuildId}",
                            guildId.ToString()
                        );
                        return false;
                    }
                }
            }

            return true;
        }

        private List<ulong> GetFastUpdateGuildIds()
        {
            return _discordConfig.FastUpdateGuildIds;
        }
    }
}
