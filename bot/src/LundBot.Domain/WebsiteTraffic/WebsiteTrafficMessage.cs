using LundBot.Domain.Common;

namespace LundBot.Domain.WebsiteTraffic
{
    /// <summary>
    /// Represents the messages sent to discord for website traffic and interaction data.
    /// This is used to be able to update the messages when a new website visitor loads the page or when they click the discord server invite link on the website.
    /// </summary>
    public sealed class WebsiteTrafficMessage : AbstractMessageEntity { }
}
