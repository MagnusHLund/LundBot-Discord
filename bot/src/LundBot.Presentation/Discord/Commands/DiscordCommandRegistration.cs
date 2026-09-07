using DSharpPlus;
using DSharpPlus.Commands;
using LundBot.Infrastructure.Discord.Configuration;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.InfiniteWarfare.Maps;
using LundBot.Presentation.Discord.Leaderboards.Commands;
using Microsoft.Extensions.Options;

namespace LundBot.Presentation.Discord.Commands
{
    public sealed class DiscordCommandRegistration : IDiscordCommandRegistration
    {
        private readonly DiscordClient _discordClient;
        private readonly DiscordConfig _discordConfig;
        private readonly CommandsExtension _commands;

        private readonly ILogger _logger = Log.ForContext<DiscordCommandRegistration>();

        public DiscordCommandRegistration(
            DiscordClient discordClient,
            IOptions<DiscordConfig> discordConfig,
            CommandsExtension commands
        )
        {
            _discordClient = discordClient;
            _discordConfig = discordConfig.Value;
            _commands = commands;
        }

        public async Task<bool> RegisterCommandsAsync()
        {
            List<ulong?> guildIds = _discordConfig.FastUpdateGuildIds.Select(id => (ulong?)id).ToList();

            if (_discordConfig.ShouldRegisterGlobalCommands)
            {
                guildIds.Add(null); // Add null to register global commands
            }

            bool success = true;

            foreach (ulong? guildId in guildIds)
            {
                _logger.Information(
                    "Registering commands for guild {GuildId}…",
                    guildId == null ? "GLOBAL" : guildId.Value.ToString()
                );

                try
                {
                    AddCommands(_commands, guildId);
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        ex,
                        "Failed to register commands for guild {GuildId}",
                        guildId == null ? "GLOBAL" : guildId.Value.ToString()
                    );

                    success = false;
                }
            }

            return success;
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
    }
}
