using System.Linq;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using nomoretrolls.Workflows.Reactions;

namespace nomoretrolls.tests.Workflows.Reactions
{
    public class BlacklistReplyTextGeneratorTests
    {
        [Property(Verbose = true)]
        public bool GenerateReply_AlwaysReturnsValue()
        {
            var gen = new BlacklistReplyTextGenerator();

            var r = gen.GenerateReply("");

            return !string.IsNullOrWhiteSpace(r);
        }

        [Property(Verbose = true)]
        public bool GenerateReply_MentionAppears(NonEmptyString mention)
        {
            var gen = new BlacklistReplyTextGenerator();

            var r = gen.GenerateReply(mention.Get);

            return r.Contains(mention.Get);
        }

        [Property(Verbose=true)]
        public bool GenerateReply_InjectedRng_AlwaysReturnsValue(PositiveInt iterations)
        {
            var gen = new BlacklistReplyTextGenerator(x => 0);

            var results = Enumerable.Range(0, iterations.Get)
                                    .Select(x => gen.GenerateReply(""))
                                    .Where(s => !string.IsNullOrWhiteSpace(s))
                                    .Distinct()
                                    .ToList();

            return results.Count == 1;
        }
    }
}
