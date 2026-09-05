using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Members;
using Serilog;

namespace LundBot.Infrastructure.Discord.Services
{
    public sealed class DiscordGuildService : IDiscordGuildService
    {
        private readonly DiscordClient _discordClient;

        private readonly ILogger _logger = Log.ForContext<DiscordGuildService>();

        public DiscordGuildService(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task<DiscordGuildDto?> GetGuildAsync(ulong guildId)
        {
            _logger.Information("Fetching guild with ID {GuildId}...", guildId);

            try
            {
                DiscordGuild guild = await _discordClient.GetGuildAsync(guildId);
                return new DiscordGuildDto(guild.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch guild with ID {GuildId}", guildId);
                return null;
            }
        }

        public async Task<IReadOnlyList<DiscordInviteDto>> GetGuildInvitesAsync(ulong guildId)
        {
            _logger.Information("Fetching invites for guild with ID {GuildId}...", guildId);

            try
            {
                DiscordGuild guild = await _discordClient.GetGuildAsync(guildId);
                IReadOnlyList<DiscordInvite> invites = await guild.GetInvitesAsync();

                return invites
                    .Select(invite => new DiscordInviteDto(
                        invite.Code,
                        (ushort)invite.Uses,
                        new DiscordMemberDto(
                            userId: invite.Inviter?.Id ?? 0,
                            username: invite.Inviter?.Username ?? "",
                            displayName: invite.Inviter?.GlobalName ?? ""
                        )
                    ))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch invites for guild with ID {GuildId}", guildId);
                return new List<DiscordInviteDto>();
            }
        }

        public bool IsBotInGuild(ulong guildId)
        {
            _logger.Information("Checking if guild with ID {GuildId} uses the bot...", guildId);

            try
            {
                return _discordClient.Guilds.ContainsKey(guildId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to check if guild with ID {GuildId} uses the bot", guildId);
                return false;
            }
        }
    }
}
