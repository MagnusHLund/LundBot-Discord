namespace LundBot.Presentation.Discord.Commands
{
    public interface IDiscordCommandRegistration
    {
        Task<bool> RegisterCommandsAsync();
    }
}
