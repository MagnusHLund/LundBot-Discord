using DSharpPlus.Entities;
using LundBot.Application.Discord.Invites;
using LundBot.Application.Discord.Users;
using LundBot.Infrastructure.Discord.Users.Mappings;

namespace LundBot.Infrastructure.Discord.Invites.Mappings
{
    public static class DiscordInviteMapper
    {
        public static DiscordInviteDto Map(this DiscordInvite invite)
        {
            DiscordUserDto? inviter = invite.Inviter is null ? null : DiscordUserMapper.Map(invite.Inviter);
            return new DiscordInviteDto(inviteCode: invite.Code, uses: (ushort)invite.Uses, inviter: inviter);
        }

        public static IReadOnlyList<DiscordInviteDto> Map(this IReadOnlyList<DiscordInvite> invites)
        {
            return invites.Select(i => i.Map()).ToList();
        }
    }
}
