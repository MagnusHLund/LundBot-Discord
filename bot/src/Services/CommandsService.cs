using DSharpPlus.Commands;
using LundBot.Commands;
using LundBot.Config;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using Microsoft.Extensions.Options;
using Serilog;

namespace LundBot.Services
{
    public sealed class CommandsService : ICommandsService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<CommandsService>();
        private readonly IDiscordCommandService _discordCommandService;
        private readonly IDiscordGuildService _discordGuildService;
        private readonly DiscordConfig _discordConfig;

        public CommandsService(
            IDiscordCommandService discordCommandService,
            IDiscordGuildService discordGuildService,
            IOptions<DiscordConfig> options
        )
        {
            _discordCommandService = discordCommandService;
            _discordGuildService = discordGuildService;
            _discordConfig = options.Value;
        }

        public async Task RegisterCommandsAsync()
        {
            CommandsExtension commands = await _discordCommandService.GetCommandsAsync();

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

                AddCommands(commands, guildId);
            }
        }

        public async Task LogRegisteredCommandsForGuildsAsync()
        {
            foreach (ulong guildId in GetFastUpdateGuildIds())
            {
                if (!_discordGuildService.BotIsInGuild(guildId))
                {
                    continue;
                }

                var registeredCommands =
                    await _discordCommandService.GetGuildApplicationCommandsAsync(guildId);

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

        public async Task RefreshCommandsAsync()
        {
            await _discordCommandService.RefreshCommandsAsync();
        }

        public async Task<bool> UnregisterCommand(string commandId, bool global = false)
        {
            if (global)
            {
                try
                {
                    await _discordCommandService.DeleteGlobalApplicationCommandAsync(
                        ulong.Parse(commandId)
                    );
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                foreach (ulong guildId in GetFastUpdateGuildIds())
                {
                    try
                    {
                        await _discordCommandService.DeleteGuildApplicationCommandAsync(
                            guildId,
                            ulong.Parse(commandId)
                        );
                    }
                    catch
                    {
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
                        await _discordCommandService.GetGlobalApplicationCommandsAsync();

                    foreach (var command in globalCommands)
                    {
                        await _discordCommandService.DeleteGlobalApplicationCommandAsync(
                            command.Id
                        );
                    }
                }
                catch
                {
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
                            await _discordCommandService.GetGuildApplicationCommandsAsync(guildId);

                        foreach (var command in guildCommands)
                        {
                            await _discordCommandService.DeleteGuildApplicationCommandAsync(
                                guildId,
                                command.Id
                            );
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void AddCommands(CommandsExtension commands, ulong? guildId = null)
        {
            if (guildId is null)
            {
                commands.AddCommands<CreateLeaderboardCommand>();
                commands.AddCommands<PingCommand>();
                commands.AddCommands<RandomMapCommand>();
                commands.AddCommands<WarnOnLeaderboardCommand>();
                commands.AddCommands<RemoveLeaderboardCommand>();
                commands.AddCommands<UpvoteUserOnLeaderboardCommand>();

                return;
            }

            commands.AddCommands<CreateLeaderboardCommand>(guildId.Value);
            commands.AddCommands<PingCommand>(guildId.Value);
            commands.AddCommands<RandomMapCommand>(guildId.Value);
            commands.AddCommands<WarnOnLeaderboardCommand>(guildId.Value);
            commands.AddCommands<RemoveLeaderboardCommand>(guildId.Value);
            commands.AddCommands<UpvoteUserOnLeaderboardCommand>(guildId.Value);
        }

        private List<ulong> GetFastUpdateGuildIds()
        {
            return _discordConfig.FastUpdateGuildIds;
        }
    }
}
