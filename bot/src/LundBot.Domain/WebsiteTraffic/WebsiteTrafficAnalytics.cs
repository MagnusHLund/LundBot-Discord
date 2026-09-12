using LundBot.Domain.Common;

namespace LundBot.Domain.WebsiteTraffic
{
    /// <summary>
    /// Represents website traffic and interaction data in the database.
    /// It tracks https://infinitewarfarecommunity.com.
    /// </summary>
    public sealed class WebsiteTrafficAnalytics : AbstractEntity
    {
        public byte[] HashedIp { get; set; } = null!;
        public bool ClickedInviteButton { get; set; }

        public int WebsiteTrafficAnalyticsChannelId { get; set; }

        public WebsiteTrafficAnalyticsChannel Channel { get; set; } = null!;
    }
}
