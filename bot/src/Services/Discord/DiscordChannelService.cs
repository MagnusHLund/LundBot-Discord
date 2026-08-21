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
    }
}
