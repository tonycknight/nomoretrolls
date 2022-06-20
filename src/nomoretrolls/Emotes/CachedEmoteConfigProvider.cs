using Microsoft.Extensions.Caching.Memory;

namespace nomoretrolls.Emotes
{
    internal class CachedEmoteConfigProvider : IEmoteConfigProvider
    {
        private const string CacheEntryKeyPrefix = $"{nameof(CachedEmoteConfigProvider)}-User";

        private readonly IMemoryCache _cache;
        private readonly MongoDbEmoteConfigProvider _sourceRepo;

        public CachedEmoteConfigProvider(IMemoryCache cache, MongoDbEmoteConfigProvider sourceRepo)
        {
            _cache = cache;
            _sourceRepo = sourceRepo;
        }

        public async Task DeleteUserEmoteAnnotationEntryAsync(ulong userId)
        {
            await _sourceRepo.DeleteUserEmoteAnnotationEntryAsync(userId);

            var key = CacheEntryKey(userId);
            _cache.Remove(key);
        }

        public Task<IList<UserEmoteAnnotationEntry>> GetUserEmoteAnnotationEntriesAsync()
            => _sourceRepo.GetUserEmoteAnnotationEntriesAsync();

        public async Task<UserEmoteAnnotationEntry?> GetUserEmoteAnnotationEntryAsync(ulong userId)
        {
            var key = CacheEntryKey(userId);
            var result = await _cache.GetOrCreateAsync(key, async e =>
            {
                var r = await _sourceRepo.GetUserEmoteAnnotationEntryAsync(userId);
                if (r != null)
                {
                    e.AbsoluteExpiration = r.Expiry;
                    return r;
                }
                return null;
            });

            return result;
        }

        public async Task SetUserEmoteAnnotationEntryAsync(UserEmoteAnnotationEntry entry)
        {
            var key = CacheEntryKey(entry.UserId);

            await _sourceRepo.SetUserEmoteAnnotationEntryAsync(entry);

            _cache.Set(key, entry, entry.Expiry);
        }

        private string CacheEntryKey(ulong userId) => $"{CacheEntryKeyPrefix}-{userId}";
    }
}
