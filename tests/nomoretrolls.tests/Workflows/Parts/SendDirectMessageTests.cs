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
    public class SendDirectMessageTests
    {
        [Fact]
        public async Task ExecuteAsync_EmptyText_DiscordNotInvoked()
        {
            var msgId = 1234uL;
            var authorId = 4321uL;

            var part = new SendDirectMessage();

            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            
            var channel = Substitute.For<IMessageChannel>();
            var replyChannel = Substitute.For<IDMChannel>();
            replyChannel.Recipient.Returns(author);
            author.CreateDMChannelAsync().Returns(replyChannel);

            var msg = Substitute.For<IMessage>();
            msg.Channel.Returns(channel);
            msg.Id.Returns(msgId);
            msg.Author.Returns(author);
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
            var authorId = 4321uL;

            var part = new SendDirectMessage();

            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            author.Mention.Returns("test");

            var channel = Substitute.For<IMessageChannel>();
            var replyChannel = Substitute.For<IDMChannel>();
            replyChannel.Recipient.Returns(author);                        
            author.CreateDMChannelAsync().Returns(replyChannel);
                
            var msg = Substitute.For<IMessage>();
            msg.Channel.Returns(channel);
            msg.Id.Returns(msgId);
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new MessageWorkflowContext(messageContext);
            context.ReplyText("hello world");
            
            var r = await part.ExecuteAsync(context);

            r.Should().Be(context);

            replyChannel.Received(1).SendMessageAsync(Arg.Is<string>(s => !string.IsNullOrWhiteSpace(s)));            
        }
    }
}
