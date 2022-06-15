using System.Collections.Concurrent;
using Tk.Extensions.Tasks;
using Tk.Extensions.Time;

namespace nomoretrolls.Emotes
{
    internal class MemoryEmoteConfigProvider : IEmoteConfigProvider
    {
        private readonly ConcurrentDictionary<ulong, UserEmoteAnnotationEntry> _entries;
        private readonly ITimeProvider _time;

        public MemoryEmoteConfigProvider(ITimeProvider time) 
        {
            _time = time;
            _entries = new ConcurrentDictionary<ulong,UserEmoteAnnotationEntry>();            
        }

        public Task DeleteUserEmoteAnnotationEntryAsync(ulong userId)
        {
            _entries.TryRemove(userId, out var _);
            return Task.CompletedTask;
        }

        public Task<IList<UserEmoteAnnotationEntry>> GetUserEmoteAnnotationEntriesAsync()
        {
            var result = _entries.Values.ToList();

            return Task.FromResult((IList<UserEmoteAnnotationEntry>)result);
        }

        public Task<UserEmoteAnnotationEntry?> GetUserEmoteAnnotationEntryAsync(ulong userId)
        {
            UserEmoteAnnotationEntry result = null;

            if(_entries.TryGetValue(userId, out var value))
            {
                if (value.Expiry <= _time.UtcNow())
                {
                    _entries.TryRemove(userId, out var _);
                }
                else
                {
                    result = value;
                }
            }

            return result.ToTaskResult();
        }

        public Task SetUserEmoteAnnotationEntryAsync(UserEmoteAnnotationEntry entry)
        {
            _entries[entry.UserId] = entry;
            return Task.CompletedTask;
        }
    }
}
