using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using nomoretrolls.Emotes;

namespace nomoretrolls.tests.Emotes
{
    public class EmoteGeneratorTests
    {
        [Property(Verbose = true)]
        public bool PickDisapproveEmotes_ReturnsString()
        {
            var gen = new EmoteGenerator();
            var result = gen.PickDisapproveEmotes();

            return !string.IsNullOrWhiteSpace(result);
        }

        [Property(Verbose = true)]
        public bool PickDisapproveEmotes_FixedRng_ReturnsSameString(PositiveInt iterations)
        {
            var gen = new EmoteGenerator(x => 0);

            var result = Enumerable.Range(0, iterations.Get)
                .Select(i => gen.PickDisapproveEmotes())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();

            return result.Count == 1;
        }
    }
}
