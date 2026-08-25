using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Services.Discord.Events
{
    public class ComponentInteractionCreatedHandler
        : IEventHandler<ComponentInteractionCreatedEventArgs>
    {
        public Task HandleEventAsync(
            DiscordClient sender,
            ComponentInteractionCreatedEventArgs eventArgs
        )
        {
            throw new NotImplementedException();
        }
    }
}
