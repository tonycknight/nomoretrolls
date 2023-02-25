namespace nomoretrolls.Blacklists
{
    internal class BlacklistProvider : IBlacklistProvider
    {
        private readonly Lazy<Task<IBlacklistProvider>> _cache;
        private readonly IBlacklistProvider _persistent;

        public BlacklistProvider(IBlacklistProvider cache, IBlacklistProvider persistent)
        {
            _cache = new Lazy<Task<IBlacklistProvider>>(async () => await HydrateCache(cache, persistent));
            _persistent = persistent;

        }

        public async Task DeleteUserEntryAsync(ulong userId)
        {
            await _persistent.DeleteUserEntryAsync(userId);
            var cache = await _cache.Value;
            cache.DeleteUserEntryAsync(userId);
        }

        public async Task<IList<UserBlacklistEntry>> GetUserEntriesAsync()
        {
            var cache = await _cache.Value;
            return await cache.GetUserEntriesAsync();
        }

        public async Task<UserBlacklistEntry?> GetUserEntryAsync(ulong userId)
        {
            var cache = await _cache.Value;
            return await cache.GetUserEntryAsync(userId);
        }

        public async Task SetUserEntryAsync(UserBlacklistEntry entry)
        {
            await _persistent.SetUserEntryAsync(entry);
            var cache = await _cache.Value;
            await cache.SetUserEntryAsync(entry);
        }

        private static async Task<IBlacklistProvider> HydrateCache(IBlacklistProvider cache, IBlacklistProvider source)
        {
            var entries = await source.GetUserEntriesAsync();

            foreach (var entry in entries)
            {
                await cache.SetUserEntryAsync(entry);
            }

            return cache;
        }
    }
}
