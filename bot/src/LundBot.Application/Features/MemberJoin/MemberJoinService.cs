using LundBot.Application.Discord.Members;

namespace LundBot.Application.Features.MemberJoin
{
    public sealed class MemberJoinService : IMemberJoinService
    {
        private readonly IDiscordMemberService _discordMemberService;

        private readonly ILogger _logger = Log.ForContext<MemberJoinService>();

        public MemberJoinService(IDiscordMemberService discordMemberService)
        {
            _discordMemberService = discordMemberService;
        }

        public async Task<bool> HandleMemberJoinHiEventAsync(
            ulong senderUserId,
            ulong targetUserId,
            ulong channelId,
            ulong guildId
        )
        {
            DiscordMemberDto? senderUser = await _discordMemberService.GetMemberAsync(senderUserId, guildId);
            DiscordMemberDto? targetUser = await _discordMemberService.GetMemberAsync(targetUserId, guildId);

            if (senderUser is null || targetUser is null)
            {
                return false;
            }

            // TODO: This implementation is not done

            string message = $"**{senderUser.Username}** says hi to **{targetUser.Username}**";
            return true;
        }
    }
}
