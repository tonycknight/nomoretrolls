using System.Collections.Concurrent;

namespace nomoretrolls.Statistics
{

    internal class UserStatistics
    {
        public UserStatistics()
        {
            Entries = new ConcurrentBag<UserStatisticsEntry>();
        }

        public ConcurrentBag<UserStatisticsEntry> Entries { get; init; } 
        
        public UserStatistics Add(UserStatisticsEntry entry)
        {
            Entries.Add(entry);
            return this;
        }
    }

    internal class UserStatisticsEntry
    {
        public DateTime Time { get; init; }
        public string Key { get; init; }
        public long Count { get; init; }
    }
}
