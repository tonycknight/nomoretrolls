using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using nomoretrolls.Emotes;
using NSubstitute;

namespace nomoretrolls.tests.Emotes
{
    public class EmoteGeneratorTests
    {
        [Property(Verbose = true)]
        public bool PickEmoteAsync_ReturnsString()
        {
            var gen = new EmoteGenerator(new EmoteRepository());
            var result = gen.PickEmoteAsync("blacklist").GetAwaiter().GetResult();

            return !string.IsNullOrWhiteSpace(result);
        }

        [Property(Verbose = true)]
        public bool PickEmoteAsync_FixedRng_ReturnsSameString(PositiveInt iterations)
        {
            var gen = new EmoteGenerator(x => 0, new EmoteRepository());

            var result = Enumerable.Range(0, iterations.Get)
                .Select(i => gen.PickEmoteAsync("blacklist").GetAwaiter().GetResult())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();

            return result.Count == 1;
        }

        [Property(Verbose = true)]
        public bool PickEmoteAsync_EmptyEmoteInfo_ReturnsNull(PositiveInt iterations)
        {
            var emotes = new[] { new EmoteInfo(new string[0]) };
            var emoteRepo = Substitute.For<IEmoteRepository>();
            emoteRepo.GetEmotesAsync(Arg.Any<string>()).Returns(emotes);

            var gen = new EmoteGenerator(x => 0, emoteRepo);

            var result = Enumerable.Range(0, iterations.Get)
                .Select(i => gen.PickEmoteAsync("blacklist").GetAwaiter().GetResult())
                .Where(s => s == null)
                .ToList();

            return result.Count == iterations.Get;
        }
    }
}
