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

        public async Task<bool> KickUserDueToRoleAssignmentAsync(
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
                return false;
            }

            if (!_discordMemberService.MemberHasRole(user, roleToKick))
            {
                return false;
            }

            return await KickUserAsync(guild, user, reason);
        }

        public async Task<bool> KickUserAsync(DiscordGuild guild, DiscordMember user, string reason)
        {
            if (user.IsPending == true)
            {
                _logger.Information(
                    "User {UserId} is doing onboarding in guild {GuildId}. Skipping kick.",
                    user.Id,
                    guild.Id
                );
                return false;
            }

            try
            {
                await _discordMemberService.KickMemberAsync(user, reason);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to kick user {UserId} from guild {GuildId}.",
                    user.Id,
                    guild.Id
                );
                return false;
            }
            return true;
        }
    }
}
