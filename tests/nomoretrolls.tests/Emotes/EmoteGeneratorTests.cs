using System.Linq;
using System.Threading.Tasks;
using FsCheck;
using FsCheck.Xunit;
using nomoretrolls.Emotes;
using NSubstitute;

namespace nomoretrolls.tests.Emotes
{
    public class EmoteGeneratorTests
    {
        [Property(Verbose = true)]
        public async Task<bool> PickEmoteAsync_ReturnsString()
        {
            var gen = new EmoteGenerator(new EmoteRepository());
            var result = await gen.PickEmoteAsync("blacklist");

            return !string.IsNullOrWhiteSpace(result);
        }

        [Property(Verbose = true)]
        public async Task<bool> PickEmoteAsync_FixedRng_ReturnsSameString(PositiveInt iterations)
        {
            var gen = new EmoteGenerator(x => 0, new EmoteRepository());

            var emoteTasks = Enumerable.Range(0, iterations.Get)
                .Select(i => gen.PickEmoteAsync("blacklist"))
                .ToArray();

            var emotes = await Task.WhenAll(emoteTasks);

            var result = emotes.Where(s => !string.IsNullOrWhiteSpace(s))
                               .Distinct()
                               .ToList();

            return result.Count == 1;
        }

        [Property(Verbose = true)]
        public async Task<bool> PickEmoteAsync_EmptyEmoteInfo_ReturnsNull(PositiveInt iterations)
        {
            var emotes = new[] { new EmoteInfo(new string[0]) };
            var emoteRepo = Substitute.For<IEmoteRepository>();
            emoteRepo.GetEmotesAsync(Arg.Any<string>()).Returns(emotes);

            var gen = new EmoteGenerator(x => 0, emoteRepo);

            var emoteTasks = Enumerable.Range(0, iterations.Get)
                                    .Select(i => gen.PickEmoteAsync("blacklist"))
                                    .ToArray();

            var emoteResults = await Task.WhenAll(emoteTasks);

            var result = emoteResults
                .Where(s => s == null)
                .ToList();

            return result.Count == iterations.Get;
        }
    }
}
