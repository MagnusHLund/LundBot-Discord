using DSharpPlus;
using DSharpPlus.SlashCommands;
using LundBot.Commands;
using LundBot.Config;
using LundBot.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace LundBot.Services
{
    public sealed class CommandsService : ICommandsService
    {
        private readonly DiscordConfig _discordConfig;

        public CommandsService(IOptions<DiscordConfig> options)
        {
            _discordConfig = options.Value;
        }

        public async Task ClearCommandsAsync(
            DiscordClient discordClient,
            bool shouldClearGlobalCommands
        )
        {
            var tasks = new List<Task>();

            if (shouldClearGlobalCommands)
            {
                tasks.Add(discordClient.BulkOverwriteGlobalApplicationCommandsAsync([]));
            }

            foreach (ulong guildId in GetFastUpdateGuildIds())
            {
                // tasks.Add(discordClient.BulkOverwriteGuildApplicationCommandsAsync(guildId, []));
            }

            await Task.WhenAll(tasks);
        }

        public async Task RegisterCommandsAsync(DiscordClient discordClient)
        {
            SlashCommandsExtension slash = discordClient.GetSlashCommands();

            List<ulong?> guildIds = GetFastUpdateGuildIds().Select(id => (ulong?)id).ToList();
            guildIds.Add(null); // Add null to register global commands

            foreach (ulong? guildId in guildIds)
            {
                slash.RegisterCommands<CreateUpvoteLeaderboardCommand>(guildId);
                slash.RegisterCommands<PingCommand>(guildId);
                slash.RegisterCommands<RemoveLeaderboardCommand>(guildId);
                slash.RegisterCommands<UpvoteUserOnLeaderboardCommand>(guildId);
            }
        }

        private List<ulong> GetFastUpdateGuildIds()
        {
            return _discordConfig.FastUpdateGuildIds;
        }
    }
}
