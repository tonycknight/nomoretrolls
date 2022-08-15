using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using Microsoft.Extensions.Caching.Memory;
using nomoretrolls.Blacklists;
using NSubstitute;
using Tk.Extensions.Tasks;
using Xunit;

namespace nomoretrolls.tests.Blacklists
{
    public class CachedBlacklistProviderTests
    {
        [Fact]
        public async Task GetUserEntriesAsync_DependneciesInvoked()
        {
            var cache = CreateMockMemoryCache();
            var sourceRepo = Substitute.For<IBlacklistProvider>();
            var expectedResult = new List<UserBlacklistEntry>();
            sourceRepo.GetUserEntriesAsync().Returns(expectedResult.ToTaskResult<IList<UserBlacklistEntry>>());

            var repo = new CachedBlacklistProvider(cache, sourceRepo);

            var result = await repo.GetUserEntriesAsync();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Property(Verbose = true)]
        public bool SetUserEntryAsync_DependenciesInvoked(ulong userId, DateTime expiry)
        {
            var e = new UserBlacklistEntry() { UserId = userId, Expiry = expiry };

            var cache = CreateMockMemoryCache();
            var sourceRepo = Substitute.For<IBlacklistProvider>();

            var repo = new CachedBlacklistProvider(cache, sourceRepo);

            repo.SetUserEntryAsync(e).GetAwaiter().GetResult();

            sourceRepo.Received(1).SetUserEntryAsync(e);
            cache.Received(1).Set(Arg.Any<object>(), e, new DateTimeOffset(e.Expiry));

            return true;
        }


        [Property(Verbose = true)]
        public bool GetUserEntryAsync_DependenciesInvoked(ulong userId, DateTime expiry)
        {
            var cache = CreateMockMemoryCache();

            var sourceRepo = Substitute.For<IBlacklistProvider>();
            var e = new UserBlacklistEntry() { UserId = userId, Expiry = expiry };
            sourceRepo.GetUserEntryAsync(userId).Returns(e.ToTaskResult());
            var repo = new CachedBlacklistProvider(cache, sourceRepo);

            var result = repo.GetUserEntryAsync(userId).GetAwaiter().GetResult();
            cache.Received(1).Set(Arg.Any<object>(), Arg.Any<Func<ICacheEntry, Task<UserBlacklistEntry>>>);

            return result == e;
        }



        [Property(Verbose = true)]
        public bool DeleteUserEntryAsync_DependenciesInvoked(ulong userId, DateTime expiry)
        {
            var cache = CreateMockMemoryCache();
            var sourceRepo = Substitute.For<IBlacklistProvider>();

            var repo = new CachedBlacklistProvider(cache, sourceRepo);

            repo.DeleteUserEntryAsync(userId).GetAwaiter().GetResult();

            sourceRepo.Received(1).DeleteUserEntryAsync(userId);
            cache.Received(1).Remove(Arg.Any<object>());

            return true;
        }


        private IMemoryCache CreateMockMemoryCache() => Substitute.For<IMemoryCache>();
    }
}
