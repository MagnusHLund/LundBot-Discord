namespace LundBot.Application.Common.Exceptions
{
    /// <summary>
    /// Represents an unexpected failure while reading from or writing to a persistence store.
    /// Repositories should throw this instead of swallowing the underlying exception and returning
    /// a fallback value (e.g. false, null, or an empty collection), since a fallback value is
    /// indistinguishable from a legitimate result and can cause callers to make incorrect decisions
    /// (e.g. creating duplicate records, caching incomplete data, or overwriting valid content).
    /// </summary>
    public sealed class RepositoryException : Exception
    {
        public RepositoryException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
