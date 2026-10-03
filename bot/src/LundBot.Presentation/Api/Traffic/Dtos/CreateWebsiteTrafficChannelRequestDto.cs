using LundBot.Presentation.Api.Common.Validation;

namespace LundBot.Presentation.Api.Traffic.Dtos
{
    public sealed class CreateWebsiteTrafficChannelRequestDto
    {
        [DiscordId]
        public required ulong GuildId { get; set; }

        [DiscordId]
        public required ulong ChannelId { get; set; }
    }
}
