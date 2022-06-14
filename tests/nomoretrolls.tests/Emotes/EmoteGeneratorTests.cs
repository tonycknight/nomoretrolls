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
        public bool PickDisapproveEmotes_ReturnsString()
        {
            var gen = new EmoteGenerator(new EmoteRepository());
            var result = gen.PickDisapproveEmotes();

            return !string.IsNullOrWhiteSpace(result);
        }

        [Property(Verbose = true)]
        public bool PickDisapproveEmotes_FixedRng_ReturnsSameString(PositiveInt iterations)
        {
            var gen = new EmoteGenerator(x => 0, new EmoteRepository());

            var result = Enumerable.Range(0, iterations.Get)
                .Select(i => gen.PickDisapproveEmotes())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();

            return result.Count == 1;
        }

        [Property(Verbose = true)]
        public bool PickDisapproveEmotes_EmptyEmoteInfo_ReturnsNull(PositiveInt iterations)
        {
            var emotes = new[] { new EmoteInfo(new string[0]) };
            var emoteRepo = Substitute.For<IEmoteRepository>();
            emoteRepo.GetEmotesAsync(Arg.Any<string>()).Returns(emotes);

            var gen = new EmoteGenerator(x => 0, emoteRepo);

            var result = Enumerable.Range(0, iterations.Get)
                .Select(i => gen.PickDisapproveEmotes())
                .Where(s => s == null)
                .ToList();

            return result.Count == iterations.Get;
        }
    }
}
