namespace LundBot.Presentation.Config
{
    public sealed class ForwardedHeadersConfig
    {
        public List<string> KnownProxies { get; set; } = new();
        public List<string> KnownNetworks { get; set; } = new();
    }
}
