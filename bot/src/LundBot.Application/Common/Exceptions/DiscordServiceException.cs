namespace LundBot.Application.Common.Exceptions
{
    /// <summary>
    /// Represents an unexpected failure while communicating with Discord, such as a transient API outage,
    /// rate limit, or permission error. Discord adapters should throw this instead of returning a fallback
    /// value (e.g. null), since a fallback value is indistinguishable from a confirmed "not found" result
    /// and can cause callers to duplicate or orphan Discord messages.
    /// </summary>
    public sealed class DiscordServiceException : Exception
    {
        public DiscordServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
