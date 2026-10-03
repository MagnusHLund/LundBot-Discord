namespace LundBot.Application.Common.Concurrency
{
    /// <summary>
    /// Serializes asynchronous work per key. Lock entries are reference counted and removed once no
    /// holder or waiter remains, so memory scales with current concurrency instead of historical keys.
    /// </summary>
    internal sealed class KeyedAsyncLock<TKey>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, LockEntry> _locks = new();
        private readonly object _syncRoot = new();

        public async Task<IDisposable> AcquireAsync(TKey key, CancellationToken cancellationToken = default)
        {
            LockEntry entry;

            lock (_syncRoot)
            {
                if (!_locks.TryGetValue(key, out LockEntry? existingEntry))
                {
                    existingEntry = new LockEntry();
                    _locks.Add(key, existingEntry);
                }

                entry = existingEntry;
                entry.ReferenceCount++;
            }

            try
            {
                await entry.Semaphore.WaitAsync(cancellationToken);
            }
            catch
            {
                ReleaseReference(key, entry);
                throw;
            }

            return new Releaser(this, key, entry);
        }

        private void Release(TKey key, LockEntry entry)
        {
            entry.Semaphore.Release();
            ReleaseReference(key, entry);
        }

        private void ReleaseReference(TKey key, LockEntry entry)
        {
            lock (_syncRoot)
            {
                entry.ReferenceCount--;

                if (entry.ReferenceCount > 0)
                {
                    return;
                }

                _locks.Remove(key);
                entry.Semaphore.Dispose();
            }
        }

        private sealed class LockEntry
        {
            public SemaphoreSlim Semaphore { get; } = new(1, 1);
            public int ReferenceCount { get; set; }
        }

        private sealed class Releaser : IDisposable
        {
            private readonly KeyedAsyncLock<TKey> _owner;
            private readonly TKey _key;
            private readonly LockEntry _entry;
            private int _disposed;

            public Releaser(KeyedAsyncLock<TKey> owner, TKey key, LockEntry entry)
            {
                _owner = owner;
                _key = key;
                _entry = entry;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 0)
                {
                    _owner.Release(_key, _entry);
                }
            }
        }
    }
}
