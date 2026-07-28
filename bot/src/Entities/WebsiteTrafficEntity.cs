namespace LundBot.Entities
{
    /// <summary>
    /// Represents website traffic and interaction data in the database. It tracks https://infinitewarfarecommunity.com.
    /// </summary>
    public sealed class WebsiteTrafficEntity : AbstractEntity
    {
        public byte[] HashedIp { get; set; } = null!;
        public bool ClickedInviteButton { get; set; }
    }
}
