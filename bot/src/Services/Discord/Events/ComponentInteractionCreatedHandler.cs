using DSharpPlus;
using DSharpPlus.EventArgs;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Services.Discord.Events
{
    public sealed class ComponentInteractionCreatedHandler
        : IEventHandler<ComponentInteractionCreatedEventArgs>
    {
        private readonly IDiscordInteractionService _discordInteractionService;

        private readonly Serilog.ILogger _logger =
            Serilog.Log.ForContext<ComponentInteractionCreatedHandler>();

        public ComponentInteractionCreatedHandler(
            IDiscordInteractionService discordInteractionService
        )
        {
            _discordInteractionService = discordInteractionService;
        }

        public async Task HandleEventAsync(
            DiscordClient sender,
            ComponentInteractionCreatedEventArgs eventArgs
        )
        {
            _logger.Information(
                "Component interaction created: {CustomId} by {User} in Guild={Guild}",
                eventArgs.Id,
                eventArgs.User?.Username,
                eventArgs.Guild?.Id ?? 0
            );

            try
            {
                await _discordInteractionService.HandleComponentInteractionAsync(eventArgs);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error handling component interaction: {CustomId} by {User} in Guild={Guild}",
                    eventArgs.Id,
                    eventArgs.User?.Username,
                    eventArgs.Guild?.Id ?? 0
                );
            }
        }
    }
}
