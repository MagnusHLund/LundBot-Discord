namespace LundBot.Enums
{
    // Not really an enum, but its used like one. Enums do not allow strings though, and this is basically a string enum.
    public sealed record DiscordRoles(string Key)
    {
        public static readonly DiscordRoles Owner = new("Owner");
        public static readonly DiscordRoles Moderator = new("Moderator");
        public static readonly DiscordRoles NeedsHelp = new("NeedsHelp");
        public static readonly DiscordRoles Bot = new("Bot");
        public static readonly DiscordRoles OfferingHelp = new("OfferingHelp");
        public static readonly DiscordRoles ContentCreator = new("ContentCreator");
        public static readonly DiscordRoles SpeedRunner = new("SpeedRunner");
        public static readonly DiscordRoles Unlocker = new("Unlocker");
        public static readonly DiscordRoles NotPcPlayer = new("NotPcPlayer");
        public static readonly DiscordRoles Microsoft = new("Microsoft");
        public static readonly DiscordRoles Steam = new("Steam");
        public static readonly DiscordRoles ServerBooster = new("ServerBooster");
    }
}
