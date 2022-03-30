namespace nomoretrolls.Blacklists
{
    internal class BlacklistProvider : IBlacklistProvider
    {
        private readonly Lazy<IBlacklistProvider> _cache;
        private readonly IBlacklistProvider _persistent;

        public BlacklistProvider(IBlacklistProvider cache, IBlacklistProvider persistent)
        {
            _cache = new Lazy<IBlacklistProvider>(() => HydrateCache(cache, persistent).GetAwaiter().GetResult());
            _persistent = persistent;

        }

        public async Task DeleteUserEntryAsync(ulong userId)
        {
            await _persistent.DeleteUserEntryAsync(userId);
            await _cache.Value.DeleteUserEntryAsync(userId);            
        }

        public Task<IList<UserBlacklistEntry>> GetUserEntriesAsync()
            => _cache.Value.GetUserEntriesAsync();
        

        public Task<UserBlacklistEntry?> GetUserEntryAsync(ulong userId)
            => _cache.Value.GetUserEntryAsync(userId);

        public async Task SetUserEntryAsync(UserBlacklistEntry entry)
        {
            await _persistent.SetUserEntryAsync(entry);
            await _cache.Value.SetUserEntryAsync(entry);
        }

        private static async Task<IBlacklistProvider> HydrateCache(IBlacklistProvider cache, IBlacklistProvider source)
        {
            var entries = await source.GetUserEntriesAsync();
            
            foreach(var entry in entries)
            {
                await cache.SetUserEntryAsync(entry);
            }

            return cache;
        }
    }
}
