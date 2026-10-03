using LundBot.Domain.Common;

namespace LundBot.Domain.WebsiteTraffic
{
    /// <summary>
    /// Represents a channel used for website traffic analytics within a Discord guild.
    /// </summary>
    public sealed class WebsiteTrafficAnalyticsChannel : AbstractEntity
    {
        public ulong ChannelId { get; set; }
        public ulong GuildId { get; set; }
    }
}
