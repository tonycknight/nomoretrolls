using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Blacklists;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Blacklists
{
    public class BlacklistProviderTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        public async Task GetUserEntryAsync_CacheHydrated(int count)
        {
            IList<UserBlacklistEntry> entries = Enumerable.Range(0, count)
                .Select(i => new UserBlacklistEntry())
                .ToList();

            var cache = CreateCache();
            var persist = CreatePersistent();
            persist.GetUserEntriesAsync().Returns(Task.FromResult(entries));

            var p = new BlacklistProvider(cache, persist);

            var r = await p.GetUserEntryAsync(1);


            cache.Received(count).SetUserEntryAsync(Arg.Any<UserBlacklistEntry>());
            persist.Received(1).GetUserEntriesAsync();
        }

        [Fact]
        public async Task GetUserEntryAsync_CacheHitPersistNotHit()
        {
            var entry = new UserBlacklistEntry();

            var cache = CreateCache();
            cache.GetUserEntryAsync(Arg.Any<ulong>()).Returns(Task.FromResult(entry));
            var persist = CreatePersistent();

            var p = new BlacklistProvider(cache, persist);

            var r = await p.GetUserEntryAsync(1);

            r.Should().Be(entry);
            cache.Received(1).GetUserEntryAsync(Arg.Any<ulong>());
            persist.Received(0).GetUserEntryAsync(Arg.Any<ulong>());
        }

        [Fact]
        public async Task GetUserEntryAsync_CacheMissPersistNotHit()
        {
            UserBlacklistEntry entry = null;

            var cache = CreateCache();
            cache.GetUserEntryAsync(Arg.Any<ulong>()).Returns(Task.FromResult(entry));
            var persist = CreatePersistent();

            var p = new BlacklistProvider(cache, persist);

            var r = await p.GetUserEntryAsync(1);

            r.Should().Be(entry);
            cache.Received(1).GetUserEntryAsync(Arg.Any<ulong>());
            persist.Received(0).GetUserEntryAsync(Arg.Any<ulong>());
        }

        [Fact]
        public async Task GetUserEntriesAsync_CacheHitPersistNotHit()
        {
            IList<UserBlacklistEntry> entries = new[] { new UserBlacklistEntry(), new UserBlacklistEntry(), new UserBlacklistEntry() };
            var cache = CreateCache();
            cache.GetUserEntriesAsync().Returns(Task.FromResult(entries));
            var persist = CreatePersistent();

            var p = new BlacklistProvider(cache, persist);

            cache.Received(0).GetUserEntriesAsync();
            persist.Received(0).GetUserEntriesAsync();

            var iterations = 3;
            for (var x = 1; x <= iterations; x++)
            {
                var r = await p.GetUserEntriesAsync();

                r.Should().BeEquivalentTo(entries);

                cache.Received(x).GetUserEntriesAsync();
                persist.Received(1).GetUserEntriesAsync();
            }
        }

        [Fact]
        public async Task SetUserEntryAsync_PersistHitThenCache()
        {
            var entry = new UserBlacklistEntry();

            var cache = CreateCache();
            var persist = CreatePersistent();

            var p = new BlacklistProvider(cache, persist);

            await p.SetUserEntryAsync(entry);

            persist.Received(1).SetUserEntryAsync(entry);
            cache.Received(1).SetUserEntryAsync(entry);
        }

        [Fact]
        public void SetUserEntryAsync_PersistThrowsExceptionThenNoCacheAttempt()
        {
            var entry = new UserBlacklistEntry();

            var cache = CreateCache();
            var persist = CreatePersistent();
            persist.SetUserEntryAsync(entry).Returns(x => { throw new Exception(); });

            var p = new BlacklistProvider(cache, persist);

            var a = async () =>
            {
                await p.SetUserEntryAsync(entry);
                return true;
            };

            a.Should().ThrowAsync<Exception>();
            persist.Received(1).SetUserEntryAsync(entry);
            cache.Received(0).SetUserEntryAsync(entry);
        }


        [Fact]
        public async Task DeleteUserEntryAsync_PersistHitThenCache()
        {
            ulong entryId = 1;

            var cache = CreateCache();
            var persist = CreatePersistent();

            var p = new BlacklistProvider(cache, persist);

            await p.DeleteUserEntryAsync(entryId);

            persist.Received(1).DeleteUserEntryAsync(entryId);
            cache.Received(1).DeleteUserEntryAsync(entryId);
        }

        [Fact]
        public void DeleteUserEntryAsync_PersistThrowsExceptionCacheNotInvoked()
        {
            ulong entryId = 1;

            var cache = CreateCache();
            var persist = CreatePersistent();
            persist.DeleteUserEntryAsync(entryId).Returns(x => { throw new Exception(); });

            var p = new BlacklistProvider(cache, persist);

            var a = async () =>
            {
                await p.DeleteUserEntryAsync(entryId);
                return true;
            };

            a.Should().ThrowAsync<Exception>();

            persist.Received(1).DeleteUserEntryAsync(entryId);
            cache.Received(0).DeleteUserEntryAsync(entryId);
        }

        private IBlacklistProvider CreateCache() => Substitute.For<IBlacklistProvider>();

        private IBlacklistProvider CreatePersistent() => Substitute.For<IBlacklistProvider>();
    }
}
