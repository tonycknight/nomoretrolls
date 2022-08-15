using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using nomoretrolls.Knocking;
using nomoretrolls.Messaging;
using nomoretrolls.Telemetry;
using NSubstitute;
using Tk.Extensions.Tasks;
using Tk.Extensions.Time;
using Xunit;

namespace nomoretrolls.tests.Knocking
{
    public class CachedKnockingScheduleRepositoryTests
    {
        [Fact]
        public async Task DeleteUserEntryAsync_DependenciesInvoked()
        {
            ulong userId = 1234;

            var cache = CreateMockMemoryCache();
            var sourceRepo = Substitute.For<IKnockingScheduleRepository>();
            var tp = Substitute.For<ITimeProvider>();

            var repo = new CachedKnockingScheduleRepository(cache, sourceRepo, tp);

            await repo.DeleteUserEntryAsync(userId);

            sourceRepo.Received(1).DeleteUserEntryAsync(userId);
            cache.Received(1).Remove(Arg.Any<string>());
        }

        [Fact]
        public async Task SetUserEntryAsync_DependenciesInvoked()
        {
            var e = new KnockingScheduleEntry();

            var cache = CreateMockMemoryCache();
            var sourceRepo = Substitute.For<IKnockingScheduleRepository>();
            var tp = Substitute.For<ITimeProvider>();

            var repo = new CachedKnockingScheduleRepository(cache, sourceRepo, tp);

            await repo.SetUserEntryAsync(e);

            sourceRepo.Received(1).SetUserEntryAsync(e);
            cache.Received(1).Remove(Arg.Any<string>());
        }

        [Fact]
        public async Task GetUserEntriesAsync_EmptyResultsFlowFromSource()
        {
            var entries = new KnockingScheduleEntry[0];

            var cache = CreateMockMemoryCache();
            var sourceRepo = Substitute.For<IKnockingScheduleRepository>();
            sourceRepo.GetUserEntriesAsync().Returns(entries.ToTaskResult<IList<KnockingScheduleEntry>>());
            var tp = Substitute.For<ITimeProvider>();

            var repo = new CachedKnockingScheduleRepository(cache, sourceRepo, tp);

            var result = await repo.GetUserEntriesAsync();

            result.Should().BeEquivalentTo(entries);            
        }

        [Fact]
        public async Task GetUserEntriesAsync_ResultsFlowFromSource()
        {
            var now = DateTime.UtcNow;
            var exp = now.AddMinutes(1);
            var entries = new[] { new KnockingScheduleEntry() { Expiry = exp } };

            var cache = CreateMockMemoryCache();
            var sourceRepo = Substitute.For<IKnockingScheduleRepository>();
            sourceRepo.GetUserEntriesAsync().Returns(entries.ToTaskResult<IList<KnockingScheduleEntry>>());
            var tp = Substitute.For<ITimeProvider>();

            var repo = new CachedKnockingScheduleRepository(cache, sourceRepo, tp);

            var result = await repo.GetUserEntriesAsync();

            result.Should().BeEquivalentTo(entries);            
        }

        private IMemoryCache CreateMockMemoryCache() => Substitute.For<IMemoryCache>();
    }
}
