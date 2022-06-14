using FsCheck;
using FsCheck.Xunit;
using nomoretrolls.Emotes;
using nomoretrolls.Messaging;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using NSubstitute;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class ApplyReactionEmoteTests
    {

        [Property(Verbose = true)]
        public bool ExecuteAsync_EmoteApplied(NonEmptyString value)
        {
            var gen = Substitute.For<IEmoteGenerator>();
            gen.PickDisapproveEmotes().Returns(value.Get);

            var p = new ApplyReactionEmote(gen);

            var msg = Substitute.For<IDiscordMessageContext>();
            var context = new MessageWorkflowContext(msg);

            var r = p.ExecuteAsync(context).GetAwaiter().GetResult();

            return r.EmoteCode() == value.Get;
        }
    }
}
