using DSharpPlus;
using DSharpPlus.SlashCommands;
using LundBot.Commands;
using LundBot.Config;
using LundBot.Interfaces.Services;
using Microsoft.Extensions.Options;
using Serilog;
using ILogger = Serilog.ILogger;

namespace LundBot.Services
{
    public sealed class CommandsService : ICommandsService
    {
        private readonly ILogger _logger = Log.ForContext<CommandsService>();
        private readonly DiscordConfig _discordConfig;

        public CommandsService(IOptions<DiscordConfig> options)
        {
            _discordConfig = options.Value;
        }

        public async Task RegisterCommandsAsync(DiscordClient discordClient)
        {
            SlashCommandsExtension slash = discordClient.GetSlashCommands();

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

                slash.RegisterCommands<CreateUpvoteLeaderboardCommand>(guildId);
                slash.RegisterCommands<PingCommand>(guildId);
                slash.RegisterCommands<RemoveLeaderboardCommand>(guildId);
                slash.RegisterCommands<UpvoteUserOnLeaderboardCommand>(guildId);
            }
        }

        public async Task LogRegisteredCommandsForGuildsAsync(DiscordClient discordClient)
        {
            foreach (ulong guildId in GetFastUpdateGuildIds())
            {
                if (!discordClient.Guilds.ContainsKey(guildId))
                {
                    continue;
                }

                var registeredCommands = await discordClient.GetGuildApplicationCommandsAsync(
                    guildId
                );

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

        private List<ulong> GetFastUpdateGuildIds()
        {
            return _discordConfig.FastUpdateGuildIds;
        }
    }
}
