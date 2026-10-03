using LundBot.Presentation.Api.Common.Validation;

namespace LundBot.Presentation.Api.Traffic.Dtos
{
    public sealed class TrafficRequestDto
    {
        [DiscordId]
        public required ulong GuildId { get; init; }
    }
}
