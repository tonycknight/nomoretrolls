using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Tk.Extensions.Tasks;

namespace nomoretrolls.Knocking
{
    [ExcludeFromCodeCoverage] // Tech debt added to the backlog
    internal class MemoryKnockingScheduleRepository : IKnockingScheduleRepository
    {
        private readonly ConcurrentDictionary<ulong, KnockingScheduleEntry> _entries;

        public MemoryKnockingScheduleRepository()
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
