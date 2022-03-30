using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Blacklists;
using Xunit;

namespace nomoretrolls.tests.Blacklists
{
    public class MemoryBlacklistProviderTests
    {

        [Fact]
        public async Task Set_Get_IsSymmetric()
        {
            var repo = new MemoryBlacklistProvider();

            var entry = new UserBlacklistEntry()
            {
                UserId = 1234,
                Expiry = DateTime.UtcNow + TimeSpan.FromHours(1),
            };

            await repo.SetUserEntryAsync(entry);

            var result = await repo.GetUserEntryAsync(entry.UserId);

            result.Should().Be(entry);
        }

        [Fact]
        public async Task Get_DoesNotReturnUnknownEntry()
        {
            var repo = new MemoryBlacklistProvider();

            var entry = new UserBlacklistEntry()
            {
                UserId = 1234,
                Expiry = DateTime.UtcNow + TimeSpan.FromHours(1),
            };

            await repo.SetUserEntryAsync(entry);

            var result = await repo.GetUserEntryAsync(entry.UserId * 10);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Get_DoesNotReturnEexpiredEntries()
        {
            var repo = new MemoryBlacklistProvider();
            
            var entry = new UserBlacklistEntry()
            {
                UserId = 1234,
                Expiry = DateTime.UtcNow + TimeSpan.FromHours(-1),
            };

            await repo.SetUserEntryAsync(entry);

            var result = await repo.GetUserEntryAsync(entry.UserId);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public async Task GetUserEntriesAsync_EntriesFetched(int count)
        {
            var entries = Enumerable.Range(1, count)
                .Select(i => (ulong)i)
                .Select(i => new UserBlacklistEntry() { UserId = i })
                .ToList();

            var repo = new MemoryBlacklistProvider();
            foreach(var e in entries)
            {
                await repo.SetUserEntryAsync(e);
            }

            var result = await repo.GetUserEntriesAsync();

            result = result.OrderBy(e => e.UserId).ToList();

            result.Should().BeEquivalentTo(entries);
        }
    }
}
