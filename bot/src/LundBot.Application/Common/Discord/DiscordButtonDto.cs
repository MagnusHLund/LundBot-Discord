namespace LundBot.Application.Common.Discord
{
    public sealed record DiscordButtonDto : DiscordMessageComponentDto
    {
        public DiscordButtonStyleEnum ButtonStyle { get; }

        public DiscordButtonDto(string customId, string label, DiscordButtonStyleEnum buttonStyle)
            : base(customId, label)
        {
            ButtonStyle = buttonStyle;
        }
    }
}
