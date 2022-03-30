using FsCheck;
using FsCheck.Xunit;
using nomoretrolls.Messaging;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using nomoretrolls.Workflows.Reactions;
using NSubstitute;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class ApplyBlacklistReplyTextTests
    {

        [Property(Verbose = true)]
        public bool ExecuteAsync_EmoteApplied(NonEmptyString value)
        {
            var gen = Substitute.For<IBlacklistReplyTextGenerator>();
            gen.GenerateReply(Arg.Any<string>()).Returns(value.Get);

            var p = new ApplyBlacklistReplyText(gen);

            var msg = Substitute.For<IDiscordMessageContext>();
            var context = new MessageWorkflowContext(msg);

            var r = p.ExecuteAsync(context).GetAwaiter().GetResult();

            return r.ReplyText().Contains(value.Get);
        }
    }
}
