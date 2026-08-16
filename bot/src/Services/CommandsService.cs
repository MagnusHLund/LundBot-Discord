using DSharpPlus.SlashCommands;
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
            SlashCommandsExtension slash = await _discordCommandService.GetSlashCommandsAsync();

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

        public async Task RefreshCommands()
        {
            SlashCommandsExtension slash = await _discordCommandService.GetSlashCommandsAsync();
            await slash.RefreshCommands();
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
