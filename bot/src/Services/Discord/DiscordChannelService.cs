using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordChannelService : IDiscordChannelService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordChannelService>();

        public async Task<DiscordChannel> GetChannelAsync(ulong channelId)
        {
            _logger.Information("Fetching channel with ID {ChannelId}...", channelId);

            try
            {
                return await BotService.DiscordClient.GetChannelAsync(channelId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch channel with ID {ChannelId}.", channelId);
                throw;
            }
        }

        public async Task<DiscordChannel> GetSystemChannelAsync(DiscordGuild guild)
        {
            _logger.Information("Fetching system channel for guild {GuildId}...", guild.Id);

            try
            {
                var channel = await guild.GetSystemChannelAsync();

                if (channel is null)
                {
                    _logger.Warning("No system channel found for guild {GuildId}.", guild.Id);
                }

                return channel;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch system channel for guild {GuildId}.", guild.Id);
                throw;
            }
        }
    }
}
