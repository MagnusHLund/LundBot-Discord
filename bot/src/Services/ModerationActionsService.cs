using DSharpPlus.Entities;
using LundBot.Interfaces.Services;

namespace LundBot.Services
{
    public class ModerationActionsService : IModerationActionsService
    {
        private readonly Serilog.ILogger _logger =
            Serilog.Log.ForContext<ModerationActionsService>();

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

            if (user.Roles.Contains(roleToKick))
            {
                await KickUserAsync(guild, user, reason);
            }
        }

        public async Task KickUserAsync(DiscordGuild guild, DiscordMember user, string reason)
        {
            if (user?.IsPending == true)
            {
                _logger.Information(
                    "User {UserId} is doing onboarding in guild {GuildId}. Skipping kick. If the role is still assigned after the onboarding, the user will be kicked after the onboarding is complete.",
                    user.Id,
                    guild.Id
                );
                return;
            }

            try
            {
                await user.RemoveAsync(reason);
                _logger.Information(
                    "Kicked user {UserId} from guild {GuildId} for reason: {Reason}",
                    user.Id,
                    guild.Id,
                    reason
                );
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error kicking user {UserId} from guild {GuildId} for reason: {Reason}",
                    user.Id,
                    guild.Id,
                    reason
                );
            }
        }
    }
}
