using LundBot.Application.Discord.Interactions;

namespace LundBot.Application.Discord.Messages
{
    public sealed record DiscordButtonDto : AbstractDiscordMessageComponentDto
    {
        public DiscordButtonStyleEnum ButtonStyle { get; }

        public DiscordButtonDto(string customId, string label, DiscordButtonStyleEnum buttonStyle)
            : base(customId, label)
        {
            ButtonStyle = buttonStyle;
        }
    }
}
