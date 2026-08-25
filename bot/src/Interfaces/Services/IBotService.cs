using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.EventArgs;

namespace LundBot.Interfaces.Services
{
    public interface IBotService
    {
        static DiscordClient DiscordClient { get; set; } = null!;
        Task InitializeAsync(DiscordClient discordClient);
    }
}
