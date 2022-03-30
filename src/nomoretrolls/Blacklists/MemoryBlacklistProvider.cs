using System.Collections.Concurrent;

namespace nomoretrolls.Blacklists
{
    internal class MemoryBlacklistProvider : IBlacklistProvider
    {
        private readonly ConcurrentDictionary<ulong, UserBlacklistEntry> _blacklists;
        private readonly Func<DateTime> _getTime;

        public MemoryBlacklistProvider()
            : this(() => DateTime.UtcNow)
        {
        }

        internal MemoryBlacklistProvider(Func<DateTime> getTime)
        {
            _blacklists = new ConcurrentDictionary<ulong, UserBlacklistEntry>();
            _getTime = getTime;
        }

        public Task DeleteUserEntryAsync(ulong userId)
        {
            _blacklists.TryRemove(userId, out var _);

            return Task.CompletedTask;
        }

        public Task<IList<UserBlacklistEntry>> GetUserEntriesAsync()
        {
            var result = _blacklists.Values.ToList();

            return Task.FromResult((IList<UserBlacklistEntry>)result);
        }

        public Task<UserBlacklistEntry?> GetUserEntryAsync(ulong userId)
        {
            UserBlacklistEntry? result = null;

            if (_blacklists.TryGetValue(userId, out var value))
            {                
                if (value.Expiry <= _getTime())
                {
                    _blacklists.TryRemove(userId, out var _);
                }
                else
                {
                    result = value;
                }
            }

            return Task.FromResult(result);
        }

        public Task SetUserEntryAsync(UserBlacklistEntry entry)
        {
            _blacklists[entry.UserId] = entry.ArgNotNull(nameof(entry));

            return Task.CompletedTask;            
        }
    }
}
