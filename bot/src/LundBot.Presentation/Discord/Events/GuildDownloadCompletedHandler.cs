using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LundBot.Application.Common.Caching;
using LundBot.Application.Discord.Guilds;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class GuildDownloadCompletedHandler : IEventHandler<GuildDownloadCompletedEventArgs>
    {
        private readonly ICacheService _cacheService;
        private readonly IDiscordGuildService _discordGuildService;

        private readonly ILogger _logger = Log.ForContext<GuildDownloadCompletedHandler>();

        public GuildDownloadCompletedHandler(IDiscordGuildService discordGuildService, ICacheService cacheService)
        {
            _discordGuildService = discordGuildService;
            _cacheService = cacheService;
        }

        public async Task HandleEventAsync(DiscordClient sender, GuildDownloadCompletedEventArgs eventArgs)
        {
            List<DiscordGuild> guildList = sender.Guilds.Values.ToList();

            await DownloadGuildInvitesAsync(guildList);
        }

        private async Task DownloadGuildInvitesAsync(IReadOnlyCollection<DiscordGuild> guilds)
        {
            foreach (var guild in guilds)
            {
                var invites = await _discordGuildService.GetGuildInvitesAsync(guild.Id);
                _cacheService.Set(CacheKeys.GuildInvites(guild.Id), invites.ToList());
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
