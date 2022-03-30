using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class SendReactionEmoteTests
    {
        [Fact]
        public async Task ExecuteAsync_EmptyEmote_DiscordNotInvoked()
        {
            var part = new SendReactionEmote();

            var msg = Substitute.For<IMessage>();
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new MessageWorkflowContext(messageContext);

            context.EmoteCode(null);

            var r = await part.ExecuteAsync(context);

            r.Should().Be(context);

            msg.Received(0).AddReactionAsync(Arg.Any<IEmote>());

        }

        [Fact]
        public async Task ExecuteAsync_NonEmptyEmote_DiscordInvoked()
        {

            var part = new SendReactionEmote();

            var msg = Substitute.For<IMessage>();
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new MessageWorkflowContext(messageContext);

            context.EmoteCode("test");

            var r = await part.ExecuteAsync(context);

            r.Should().Be(context);

            msg.Received(1).AddReactionAsync(Arg.Any<IEmote>());

        }
    }
}
