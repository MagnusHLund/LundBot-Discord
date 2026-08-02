namespace LundBot.Interfaces.Services
{
    public interface ICacheService
    {
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        T? Get<T>(string key);
        void Clear(string key);
        void Update<T>(string key, Func<T?, T> updater, TimeSpan? expiration = null);
    }
}
