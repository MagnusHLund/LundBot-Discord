namespace LundBot.Application.Discord.Interactions
{
    public abstract record AbstractDiscordMessageComponentDto
    {
        public string CustomId { get; }
        public string Label { get; }

        public AbstractDiscordMessageComponentDto(string customId, string label)
        {
            CustomId = customId;
            Label = label;
        }
    }
}
