using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Application.Discord.Channels;
using Serilog;

namespace LundBot.Infrastructure.Discord.Channels
{
    public sealed class DiscordChannelService : IDiscordChannelService
    {
        private readonly DiscordClient _discordClient;

        private readonly ILogger _logger = Log.ForContext<DiscordChannelService>();

        public DiscordChannelService(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task<DiscordChannelDto?> GetChannelAsync(ulong channelId, ulong guildId)
        {
            _logger.Information("Getting channel with ID {ChannelId} in guild {GuildId}...", channelId, guildId);

            try
            {
                DiscordChannel channel = await _discordClient.GetChannelAsync(channelId);
                return new DiscordChannelDto(channel.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get channel with ID {ChannelId} in guild {GuildId}.", channelId, guildId);
                return null;
            }
        }

        public async Task<DiscordChannelDto?> GetSystemChannelAsync(ulong guildId)
        {
            _logger.Information("Fetching system channel for guild {GuildId}...", guildId);

            try
            {
                DiscordGuild guild = await _discordClient.GetGuildAsync(guildId);
                DiscordChannel? systemChannel = await guild.GetSystemChannelAsync();

                if (systemChannel is null)
                {
                    return null;
                }

                return new DiscordChannelDto(systemChannel.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch system channel for guild {GuildId}.", guildId);
                return null;
            }
        }
    }
}
