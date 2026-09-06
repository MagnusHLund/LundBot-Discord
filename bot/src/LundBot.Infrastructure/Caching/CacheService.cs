using LundBot.Application.Common.Caching;

namespace LundBot.Infrastructure.Caching
{
    public sealed class CacheService : ICacheService
    {
        public T? Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            throw new NotImplementedException();
        }

        public void Update<T>(string key, Func<T?, T> updater, TimeSpan? expiration = null)
        {
            throw new NotImplementedException();
        }

        public void Clear(string key)
        {
            throw new NotImplementedException();
        }
    }
}
