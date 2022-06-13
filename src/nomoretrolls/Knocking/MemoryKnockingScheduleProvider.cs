using System.Collections.Concurrent;
using Tk.Extensions.Tasks;

namespace nomoretrolls.Knocking
{
    internal class MemoryKnockingScheduleProvider : IKnockingScheduleProvider
    {
        private readonly ConcurrentDictionary<ulong, KnockingScheduleEntry> _entries;

        public MemoryKnockingScheduleProvider()
        {
            _entries = new ConcurrentDictionary<ulong, KnockingScheduleEntry>();
        }

        public Task DeleteUserEntryAsync(ulong userId)
        {
            _entries.TryRemove(userId, out var _);

            return Task.CompletedTask;
        }

        public Task<IList<KnockingScheduleEntry>> GetUserEntriesAsync()
        {
            return _entries.Values.ToList().ToTaskResult<IList<KnockingScheduleEntry>>();
        }

        public Task SetUserEntryAsync(KnockingScheduleEntry entry)
        {
            _entries[entry.UserId] = entry;

            return Task.CompletedTask;
        }
    }
}
