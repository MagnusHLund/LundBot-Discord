using DSharpPlus;
using DSharpPlus.EventArgs;
using LundBot.Helpers;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord.Events
{
    public sealed class GuildDownloadCompletedHandler
        : IEventHandler<GuildDownloadCompletedEventArgs>
    {
        private readonly IDiscordGuildService _discordGuildService;
        private readonly ICacheService _cacheService;
        private readonly Serilog.ILogger _logger = Log.ForContext<GuildDownloadCompletedHandler>();

        public GuildDownloadCompletedHandler(
            IDiscordGuildService discordGuildService,
            ICacheService cacheService
        )
        {
            _discordGuildService = discordGuildService;
            _cacheService = cacheService;
        }

        public async Task HandleEventAsync(
            DiscordClient sender,
            GuildDownloadCompletedEventArgs eventArgs
        )
        {
            await DownloadGuildInvitesAsync(sender);
        }

        private async Task DownloadGuildInvitesAsync(DiscordClient sender)
        {
            foreach (var guild in sender.Guilds.Values)
            {
                var invites = await _discordGuildService.GetGuildInvitesAsync(guild);
                _cacheService.Set(
                    CacheKeyHelper.GuildInvites(guild.Id.ToString()),
                    invites.ToList()
                );
                _logger.Information(
                    "Cached {InviteCount} invites for guild {GuildName} ({GuildId})",
                    invites.Count,
                    guild.Name,
                    guild.Id
                );
            }
        }
    }
}
