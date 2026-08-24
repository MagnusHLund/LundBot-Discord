using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordGuildService : IDiscordGuildService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordGuildService>();

        public async Task<DiscordGuild> GetGuildAsync(ulong guildId)
        {
            _logger.Information("Fetching guild with ID {GuildId}...", guildId);

            try
            {
                return await BotService.DiscordClient.GetGuildAsync(guildId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch guild with ID {GuildId}.", guildId);
                throw;
            }
        }

        public async Task<IReadOnlyList<DiscordInvite>> GetGuildInvitesAsync(DiscordGuild guild)
        {
            _logger.Information("Fetching invites for guild {GuildId}...", guild.Id);

            try
            {
                return await guild.GetInvitesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch invites for guild {GuildId}.", guild.Id);
                throw;
            }
        }

        public bool BotIsInGuild(ulong guildId)
        {
            _logger.Information("Checking if guild with ID {GuildId} uses the bot...", guildId);

            try
            {
                return BotService.DiscordClient.Guilds.ContainsKey(guildId);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to check if guild with ID {GuildId} uses the bot.",
                    guildId
                );
                throw;
            }
        }

        public async Task<DiscordRole> GetRoleByIdAsync(DiscordGuild guild, ulong roleId)
        {
            _logger.Information(
                "Fetching role with ID {RoleId} in guild {GuildId}...",
                roleId,
                guild.Id
            );

            try
            {
                return await guild.GetRoleAsync(roleId);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to fetch role with ID {RoleId} in guild {GuildId}.",
                    roleId,
                    guild.Id
                );
                throw;
            }
        }
    }
}
