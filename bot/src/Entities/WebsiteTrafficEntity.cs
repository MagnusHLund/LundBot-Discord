namespace LundBot.Entities
{
    public sealed class WebsiteTrafficEntity
    {
        public int WebsiteTrafficId { get; set; }
        public byte[] HashedIp { get; set; } = null!;
        public bool ClickedInviteButton { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
