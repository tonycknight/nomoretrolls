using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Telemetry;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class SendUserReplyMessageTests
    {
        [Fact]
        public async Task ExecuteAsync_EmptyText_DiscordNotInvoked()
        {
            var part = new SendUserReplyMessage(Substitute.For<ITelemetry>());

            var channel = Substitute.For<IMessageChannel>();
            var msg = Substitute.For<IMessage>();       
            msg.Channel.Returns(channel);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new MessageWorkflowContext(messageContext);

            context.ReplyText(" ");

            var r = await part.ExecuteAsync(context);

            r.Should().Be(context);

            channel.Received(0).SendMessageAsync(Arg.Is<string>(s => !string.IsNullOrWhiteSpace(s)));
        }

        [Fact]
        public async Task ExecuteAsync_NonEmptyText_DiscordInvoked()
        {
            var msgId = 1234uL;

            var part = new SendUserReplyMessage(Substitute.For<ITelemetry>());

            var channel = Substitute.For<IMessageChannel>();
            var msg = Substitute.For<IMessage>();
            msg.Channel.Returns(channel);
            msg.Id.Returns(msgId);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new MessageWorkflowContext(messageContext);
            context.ReplyText("hello world");
            
            var r = await part.ExecuteAsync(context);

            r.Should().Be(context);


            channel.Received(1).SendMessageAsync(Arg.Is<string>(s => !string.IsNullOrWhiteSpace(s)), Arg.Any<bool>(),
                Arg.Any<Embed>(), Arg.Any<RequestOptions>(), Arg.Any<AllowedMentions>(), 
                Arg.Is<MessageReference>(mr => mr.MessageId.Value == msgId));
        }
    }
}
