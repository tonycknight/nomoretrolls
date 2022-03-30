using nomoretrolls.Messaging;
using nomoretrolls.Workflows;
using NSubstitute;
using FluentAssertions;
using Xunit;
using Discord;

namespace nomoretrolls.tests.Workflows
{
    public class MessageWorkflowContextExtensionsTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("key", "value")]
        public void DeepClone_ConstructsNewObjectInstance(string key, string value)
        {
            var msgContext = Substitute.For<IDiscordMessageContext>();
            var context = new MessageWorkflowContext(msgContext);
            context.AppData.Add(key, value);

            var result = context.DeepClone();

            result.Should().NotBe(context);
            result.DiscordContext.Should().Be(context.DiscordContext);
            result.AppData[key].Should().Be(context.AppData[key]);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("aaa")]
        public void EmoteCode_Applied(string value)
        {
            var msgContext = Substitute.For<IDiscordMessageContext>();
            var context = new MessageWorkflowContext(msgContext);


            var result = context.EmoteCode(value).EmoteCode();

            result.Should().Be(value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("aaa")]
        public void ReplyText_Applied(string value)
        {
            var msgContext = Substitute.For<IDiscordMessageContext>();
            var context = new MessageWorkflowContext(msgContext);


            var result = context.ReplyText(value).ReplyText();

            result.Should().Be(value);
        }

        [Fact]
        public void UserMention_MockUserMessage_ReturnsEmpty()
        {
            var msgContext = Substitute.For<IDiscordMessageContext>();
            var userMsg = Substitute.For<IMessage>();
            msgContext.Message.Returns(userMsg);
            
            var context = new MessageWorkflowContext(msgContext);

            var mention = context.UserMention();
            
            mention.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("a")]
        [InlineData(" a ")]
        public void UserMention_UserMessage_ReturnsValue(string value)
        {
            var msgContext = Substitute.For<IDiscordMessageContext>();
            var author = Substitute.For<IUser>();
            author.Mention.Returns(value);
            var userMsg = Substitute.For<IMessage>();
            userMsg.Author.Returns(author);
            msgContext.Message.Returns(userMsg);

            var context = new MessageWorkflowContext(msgContext);

            var mention = context.UserMention();

            mention.Should().Be(value);
        }

        [Fact]
        public void Content_MockUserMessage_ReturnsEmpty()
        {
            var msgContext = Substitute.For<IDiscordMessageContext>();
            var userMsg = Substitute.For<IMessage>();

            userMsg.Content.Returns((string)null);

            msgContext.Message.Returns(userMsg);

            var context = new MessageWorkflowContext(msgContext);

            var content = context.Content();

            content.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("a")]
        [InlineData(" a ")]
        public void Content_UserMessage_ReturnsValue(string value)
        {
            var msgContext = Substitute.For<IDiscordMessageContext>();
            var userMsg = Substitute.For<IMessage>();
            userMsg.Content.Returns(value);
            msgContext.Message.Returns(userMsg);

            var context = new MessageWorkflowContext(msgContext);

            var content = context.Content();

            content.Should().Be(value);
        }
    }
}
