using System.Collections.Concurrent;
using LundBot.Application.Common.Caching;

namespace LundBot.Infrastructure.Caching
{
    public sealed class CacheService : ICacheService
    {
        private record CacheEntry
        {
            public object? Value { get; set; }
            public DateTime? Expiration { get; set; }
        }

        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

        public T? Get<T>(string key)
        {
            if (!_cache.TryGetValue(key, out var entry))
                return default;

            if (entry.Expiration is not null && entry.Expiration < DateTime.UtcNow)
            {
                _cache.TryRemove(key, out _);
                return default;
            }

            return (T?)entry.Value;
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            CacheEntry entry = new CacheEntry()
            {
                Value = value,
                Expiration = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null,
            };

            _cache[key] = entry;
        }

        public void Update<T>(string key, Func<T?, T> updater, TimeSpan? expiration = null)
        {
            _cache.AddOrUpdate(
                key,
                _ => new CacheEntry()
                {
                    Value = updater(default),
                    Expiration = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null,
                },
                (_, existing) =>
                {
                    var newValue = updater((T?)existing.Value);

                    return new CacheEntry()
                    {
                        Value = newValue,
                        Expiration = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : existing.Expiration,
                    };
                }
            );
        }

        public void Clear(string key)
        {
            _cache.TryRemove(key, out _);
        }
    }
}
