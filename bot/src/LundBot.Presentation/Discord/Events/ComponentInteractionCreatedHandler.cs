using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class ComponentInteractionCreatedHandler : IEventHandler<ComponentInteractionCreatedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
