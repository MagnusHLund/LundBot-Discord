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
            CommandsExtension slash = await _discordCommandService.GetSlashCommandsAsync();

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

                // TODO: This can probably be written better
                if (guildId is null)
                {
                    slash.AddCommands<CreateLeaderboardCommand>();
                    slash.AddCommands<PingCommand>();
                    slash.AddCommands<RandomMapCommand>();
                    slash.AddCommands<WarnOnLeaderboardCommand>();
                    slash.AddCommands<RemoveLeaderboardCommand>();
                    slash.AddCommands<UpvoteUserOnLeaderboardCommand>();
                }
                else
                {
                    slash.AddCommands<CreateLeaderboardCommand>(guildId.Value);
                    slash.AddCommands<PingCommand>(guildId.Value);
                    slash.AddCommands<RandomMapCommand>(guildId.Value);
                    slash.AddCommands<WarnOnLeaderboardCommand>(guildId.Value);
                    slash.AddCommands<RemoveLeaderboardCommand>(guildId.Value);
                    slash.AddCommands<UpvoteUserOnLeaderboardCommand>(guildId.Value);
                }
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
            CommandsExtension slash = await _discordCommandService.GetSlashCommandsAsync();

            // TODO: Figure this out. Broken in the latest DSharpPlus version.
            // await slash.RefreshCommands();
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

        private List<ulong> GetFastUpdateGuildIds()
        {
            return _discordConfig.FastUpdateGuildIds;
        }
    }
}
