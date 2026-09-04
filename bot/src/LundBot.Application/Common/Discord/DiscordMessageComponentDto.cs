namespace LundBot.Application.Common.Discord
{
    public abstract record DiscordMessageComponentDto
    {
        public string CustomId { get; }
        public string Label { get; }

        public DiscordMessageComponentDto(string customId, string label)
        {
            CustomId = customId;
            Label = label;
        }
    }
}
