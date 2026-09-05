using DSharpPlus.Entities;
using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Users;
using LundBot.Infrastructure.Discord.Users.Mappings;

namespace LundBot.Infrastructure.Discord.Guilds.Mappings
{
    public static class DiscordInviteMapper
    {
        public static DiscordInviteDto Map(DiscordInvite invite)
        {
            DiscordUserDto inviter = DiscordUserMapper.Map(invite.Inviter);
            return new DiscordInviteDto(inviteCode: invite.Code, uses: (ushort)invite.Uses, inviter: inviter);
        }

        public static IReadOnlyList<DiscordInviteDto> Map(IReadOnlyList<DiscordInvite> invites)
        {
            return invites.Select(i => Map(i)).ToList();
        }
    }
}
