using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using Microsoft.Extensions.Caching.Memory;
using nomoretrolls.Emotes;
using NSubstitute;
using Tk.Extensions.Tasks;
using Xunit;

namespace nomoretrolls.tests.Emotes
{
    public class CachedEmoteConfigProviderTests
    {
        [Fact]
        public async Task GetUserEmoteAnnotationEntriesAsync_DependneciesInvoked()
        {
            var cache = CreateMockMemoryCache();
            var sourceRepo = Substitute.For<IEmoteConfigProvider>();
            var expectedResult = new List<UserEmoteAnnotationEntry>();
            sourceRepo.GetUserEmoteAnnotationEntriesAsync().Returns(expectedResult.ToTaskResult<IList<UserEmoteAnnotationEntry>>());

            var repo = new CachedEmoteConfigProvider(cache, sourceRepo);

            var result = await repo.GetUserEmoteAnnotationEntriesAsync();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Property(Verbose = true)]
        public async Task<bool> SetUserEmoteAnnotationEntryAsync_DependenciesInvoked(ulong userId, DateTime expiry)
        {
            var e = new UserEmoteAnnotationEntry() { UserId = userId, Expiry = expiry };

            var cache = CreateMockMemoryCache();
            var sourceRepo = Substitute.For<IEmoteConfigProvider>();

            var repo = new CachedEmoteConfigProvider(cache, sourceRepo);

            await repo.SetUserEmoteAnnotationEntryAsync(e);

            sourceRepo.Received(1).SetUserEmoteAnnotationEntryAsync(e);
            cache.Received(1).Set(Arg.Any<object>(), e, new DateTimeOffset(e.Expiry));

            return true;
        }


        [Property(Verbose = true)]
        public async Task<bool> GetUserEmoteAnnotationEntryAsync_DependenciesInvoked(ulong userId, DateTime expiry)
        {
            var cache = CreateMockMemoryCache();
            
            var sourceRepo = Substitute.For<IEmoteConfigProvider>();
            var e = new UserEmoteAnnotationEntry() { UserId = userId, Expiry = expiry };
            sourceRepo.GetUserEmoteAnnotationEntryAsync(userId).Returns(e.ToTaskResult());
            var repo = new CachedEmoteConfigProvider(cache, sourceRepo);

            var result = await repo.GetUserEmoteAnnotationEntryAsync(userId);
            cache.Received(1).Set(Arg.Any<object>(), Arg.Any<Func<ICacheEntry, Task<UserEmoteAnnotationEntry>>>);

            return result == e;
        }



        [Property(Verbose = true)]
        public async Task<bool> DeleteUserEmoteAnnotationEntryAsync_DependenciesInvoked(ulong userId, DateTime expiry)
        {            
            var cache = CreateMockMemoryCache();
            var sourceRepo = Substitute.For<IEmoteConfigProvider>();

            var repo = new CachedEmoteConfigProvider(cache, sourceRepo);

            await repo.DeleteUserEmoteAnnotationEntryAsync(userId);

            sourceRepo.Received(1).DeleteUserEmoteAnnotationEntryAsync(userId);
            cache.Received(1).Remove(Arg.Any<object>());
            
            return true;
        }


        private IMemoryCache CreateMockMemoryCache() => Substitute.For<IMemoryCache>();
    }
}
