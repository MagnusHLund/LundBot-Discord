namespace LundBot.Domain.Common
{
    public abstract class AbstractEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
