using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using nomoretrolls.Emotes;
using Xunit;

namespace nomoretrolls.tests.Emotes
{
    public class EmoteRepositoryTests
    {
        public static IEnumerable<object[]> GetEomteListNames()
        {
            var repo = new EmoteRepository();

            var names = repo.GetEmoteNamesAsync().Result;

            return names.Select(n => new[] { n });
        }

        [Theory]
        [MemberData(nameof(GetEomteListNames))]
        public void GetEmoteNamesAsync_NamesAreUnique(string name)
        {
            name.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(GetEomteListNames))]
        public async Task GetEmotesAsync_AllEmotesAreUnique(string emoteName)
        {
            var repo = new EmoteRepository();
                        
            var emotes = await repo.GetEmotesAsync(emoteName);

            emotes.Should().NotBeEmpty();                
                
            var emoteTexts = emotes.SelectMany(e => e.Emotes).ToList();
            emoteTexts.All(s => s.Length > 0).Should().BeTrue();

            var uniqueEmoteTexts = emoteTexts.Distinct().ToList();
            uniqueEmoteTexts.Count().Should().Be(emoteTexts.Count);            
        }

        [Property(Verbose = true)]
        public async Task<bool> GetEmotesAsync_UnknownNameReturnsNull(Guid name)
        {
            var listName = Guid.NewGuid().ToString();
            var repo = new EmoteRepository();

            var emotes = await repo.GetEmotesAsync(listName);

            return emotes == null;
        }

    }
}
