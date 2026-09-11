namespace LundBot.Presentation.Api.Server.Dtos
{
    public sealed record ServerHealthResponse
    {
        public required string Status { get; init; }
        public required string Version { get; init; }
    }
}
