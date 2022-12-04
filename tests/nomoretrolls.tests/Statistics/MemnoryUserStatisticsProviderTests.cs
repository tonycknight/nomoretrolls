using System;
using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Statistics;
using Xunit;

namespace nomoretrolls.tests.Statistics
{
    public class MemnoryUserStatisticsProviderTests
    {
        [Theory]
        [InlineData(1, "a", 1)]
        [InlineData(2, "aaa", 10)]
        public async Task BumpUserStatisticAsync_StatsBumped(ulong userId, string key, int iterations)
        {
            var p = new MemoryUserStatisticsProvider();
            
            for (var i = 0; i < iterations; i++)
            {
                await p.BumpUserStatisticAsync(userId, key, TimeSpan.Zero);
            }

            var result = await p.GetUserStatisticCountAsync(userId, key, TimeSpan.FromHours(5));

            result.Should().Be(iterations);
        }

        [Theory]
        [InlineData(1, "a", 10)]
        [InlineData(2, "aaa", 10)]
        [InlineData(2, "aaa", 100)]
        public async Task BumpUserStatisticAsync_StatTimeFrameApplied(ulong userId, string key, int iterations)
        {
            int timeframeSecs = 2;
            var now = DateTime.UtcNow;
            var timeframe = TimeSpan.FromSeconds(0);
            var getNow = () => now.Add(timeframe);

            var p = new MemoryUserStatisticsProvider(getNow);
            

            for (var i = 0; i < iterations; i++)
            {
                await p.BumpUserStatisticAsync(userId, key, TimeSpan.Zero);
            }
            
            timeframe = TimeSpan.FromSeconds(timeframeSecs); // bump the clock

            var result = await p.GetUserStatisticCountAsync(userId, key, TimeSpan.FromSeconds(1));

            result.Should().Be(0);
        }


    }
}
