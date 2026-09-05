using DSharpPlus.Entities;
using LundBot.Application.Discord.Users;

namespace LundBot.Infrastructure.Discord.Users.Mappings
{
    public static class DiscordUserMapper
    {
        public static DiscordUserDto Map(DiscordUser user)
        {
            return new DiscordUserDto(userId: user.Id, username: user.Username);
        }
    }
}
