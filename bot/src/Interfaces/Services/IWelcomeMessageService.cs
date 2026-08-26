using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services
{
    public interface IWelcomeMessageService
    {
        Task SendWelcomeMessageAsync(DiscordGuild guild, DiscordMember member);
        Task RemoveWelcomeMessageAsync(DiscordGuild guild, ulong discordMemberId);
        Task HandleWelcomeInteractionAsync(
            DiscordUser senderUser,
            DiscordMember targetUser,
            DiscordChannel channel
        );
    }
}
