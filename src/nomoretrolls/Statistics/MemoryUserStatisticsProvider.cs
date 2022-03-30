using System.Collections.Concurrent;

namespace nomoretrolls.Statistics
{
    internal class MemoryUserStatisticsProvider : IUserStatisticsProvider
    {
        private readonly ConcurrentDictionary<(ulong, string), UserStatistics> _userStats;
        private readonly Func<DateTime> _getTime;

        public MemoryUserStatisticsProvider()
            : this(() => DateTime.UtcNow)
        {            
        }

        internal MemoryUserStatisticsProvider(Func<DateTime> getTime)            
        {
            _getTime = getTime;
            _userStats = new ConcurrentDictionary<(ulong, string), UserStatistics>();
        }

        public Task BumpUserStatisticAsync(ulong userId, string statName)
        {
            var key = GetMapKey(userId, statName);
            var now = _getTime();

            var entry = new UserStatisticsEntry()
            {
                Count = 1,
                Key = statName,
                Time = now,
            };

            var stats = new UserStatistics().Add(entry);

            _userStats.AddOrUpdate(key, stats, (key, stats) => stats.Add(entry));

            return Task.CompletedTask;
        }

        public Task<long> GetUserStatisticCountAsync(ulong userId, string statName, TimeSpan timeFrame)
        {
            var key = GetMapKey(userId, statName);
            long result = 0;

            if(_userStats.TryGetValue(key, out var userStats))
            {
                var earliest = _getTime() - timeFrame;

                result = userStats.Entries.Where(e => e.Time >= earliest).Sum(e => e.Count);
            }

            return Task.FromResult(result);
        }

        private static (ulong, string) GetMapKey(ulong userId, string statName) => (userId, statName.ToLower());
    }
}
