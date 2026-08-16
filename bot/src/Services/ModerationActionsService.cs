using DSharpPlus.Entities;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Services
{
    public class ModerationActionsService : IModerationActionsService
    {
        private readonly IDiscordMemberService _discordMemberService;
        private readonly Serilog.ILogger _logger =
            Serilog.Log.ForContext<ModerationActionsService>();

        public ModerationActionsService(IDiscordMemberService discordMemberService)
        {
            _discordMemberService = discordMemberService;
        }

        public async Task KickUserDueToRoleAssignmentAsync(
            DiscordGuild guild,
            DiscordMember user,
            DiscordRole? roleToKick,
            string reason
        )
        {
            if (roleToKick is null)
            {
                _logger.Warning(
                    "Role to kick is null. Cannot kick user {UserId} from guild {GuildId}.",
                    user.Id,
                    guild.Id
                );
                return;
            }

            if (_discordMemberService.MemberHasRole(user, roleToKick))
            {
                await KickUserAsync(guild, user, reason);
            }
        }

        public async Task KickUserAsync(DiscordGuild guild, DiscordMember user, string reason)
        {
            if (user.IsPending == true)
            {
                _logger.Information(
                    "User {UserId} is doing onboarding in guild {GuildId}. Skipping kick.",
                    user.Id,
                    guild.Id
                );
                return;
            }

            await _discordMemberService.KickMemberAsync(user, reason);
        }
    }
}
