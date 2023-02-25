using Microsoft.Extensions.Caching.Memory;

namespace nomoretrolls.Blacklists
{
    internal class CachedBlacklistProvider : IBlacklistProvider
    {
        private const string CacheEntryKeyPrefix = $"{nameof(CachedBlacklistProvider)}-User";

        private readonly IMemoryCache _cache;
        private readonly IBlacklistProvider _sourceProvider;

        public CachedBlacklistProvider(IMemoryCache cache, IBlacklistProvider sourceProvider)
        {
            _cache = cache;
            _sourceProvider = sourceProvider;
        }

        public async Task DeleteUserEntryAsync(ulong userId)
        {
            await _sourceProvider.DeleteUserEntryAsync(userId);

            var key = CacheEntryKey(userId);
            _cache.Remove(key);
        }

        public Task<IList<UserBlacklistEntry>> GetUserEntriesAsync() => _sourceProvider.GetUserEntriesAsync();

        public async Task<UserBlacklistEntry?> GetUserEntryAsync(ulong userId)
        {
            var key = CacheEntryKey(userId);
            var result = await _cache.GetOrCreateAsync(key, async e =>
            {
                var r = await _sourceProvider.GetUserEntryAsync(userId);
                if (r != null)
                {
                    e.AbsoluteExpiration = r.Expiry;
                    return r;
                }
                return null;
            });

            return result;
        }

        public async Task SetUserEntryAsync(UserBlacklistEntry entry)
        {
            var key = CacheEntryKey(entry.UserId);

            await _sourceProvider.SetUserEntryAsync(entry);

            _cache.Set(key, entry, entry.Expiry);
        }

        private string CacheEntryKey(ulong userId) => $"{CacheEntryKeyPrefix}-{userId}";
    }
}
