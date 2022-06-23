using Microsoft.Extensions.Caching.Memory;

namespace nomoretrolls.Knocking
{
    internal class CachedKnockingScheduleRepository : IKnockingScheduleRepository
    {
        private const string CacheKeyPrefix = $"{nameof(CachedKnockingScheduleRepository)}-Users";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(60);

        private readonly IMemoryCache _cache;
        private readonly IKnockingScheduleRepository _sourceRepo;

        public CachedKnockingScheduleRepository(IMemoryCache cache, IKnockingScheduleRepository sourceRepo)
        {
            _cache = cache;
            _sourceRepo = sourceRepo;
        }

        public async Task DeleteUserEntryAsync(ulong userId)
        {
            await _sourceRepo.DeleteUserEntryAsync(userId);

            _cache.Remove(CacheKeyPrefix);
        }

        public async Task<IList<KnockingScheduleEntry>> GetUserEntriesAsync()
        {
            var key = CacheKeyPrefix;

            var result = await _cache.GetOrCreateAsync(key, async e =>
            {
                var r = await _sourceRepo.GetUserEntriesAsync();
                if (r != null)
                {
                    e.AbsoluteExpiration = r.Any() 
                                            ? r.Min(e => e.Expiry) 
                                            : DateTime.UtcNow.Add(CacheExpiry);
                    return r;
                }
                return null;
            });

            return result;
        }

        public async Task SetUserEntryAsync(KnockingScheduleEntry entry)
        {
            await _sourceRepo.SetUserEntryAsync(entry);

            _cache.Remove(CacheKeyPrefix);
        }
    }
}
