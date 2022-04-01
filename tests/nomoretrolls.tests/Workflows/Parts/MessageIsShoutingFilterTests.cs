using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class MessageIsShoutingFilterTests
    {
        
        [Fact]
        public async Task ExecuteAsync_NullContent_ReturnsNull()
        {
            var f = new MessageIsShoutingFilter();

            var msg = Substitute.For<IDiscordMessageContext>();
            var context = new MessageWorkflowContext(msg);

            var result = await f.ExecuteAsync(context);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ExecuteAsync_EmptyContent_ReturnsNull(string content)
        {
            var f = new MessageIsShoutingFilter();

            var socketMsg = Substitute.For<Discord.IMessage>();
            socketMsg.Content.Returns(content);

            var msg = Substitute.For<IDiscordMessageContext>();
            msg.Message.Returns(socketMsg);

            var context = new MessageWorkflowContext(msg);

            var result = await f.ExecuteAsync(context);

            result.Should().BeNull();
        }


        [Theory]
        [InlineData("a")]
        [InlineData(" a ")]
        [InlineData(" B ")]
        public async Task ExecuteAsync_NonEmptyContent_ReturnsNull(string content)
        {
            var f = new MessageIsShoutingFilter();

            var socketMsg = Substitute.For<Discord.IMessage>();
            socketMsg.Content.Returns(content);

            var msg = Substitute.For<IDiscordMessageContext>();
            msg.Message.Returns(socketMsg);

            var context = new MessageWorkflowContext(msg);

            var result = await f.ExecuteAsync(context);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData("SHOUT")]
        [InlineData(" SHOUTing SHOUTing  ")]
        [InlineData(" will you STOP SHOUTING")]
        [InlineData(" TESt TeST TEsT")]
        [InlineData(" this AND this AND this OR that AND NO SHOUTING")]
        public async Task ExecuteAsync_Shouting_ReturnsNonNull(string content)
        {
            var f = new MessageIsShoutingFilter();

            var socketMsg = Substitute.For<Discord.IMessage>();
            socketMsg.Content.Returns(content);

            var msg = Substitute.For<IDiscordMessageContext>();
            msg.Message.Returns(socketMsg);

            var context = new MessageWorkflowContext(msg);

            var result = await f.ExecuteAsync(context);

            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData("shout")]
        [InlineData(" this AND this AND this OR that")]
        public async Task ExecuteAsync_NotShouting_ReturnsNull(string content)
        {
            var f = new MessageIsShoutingFilter();

            var socketMsg = Substitute.For<Discord.IMessage>();
            socketMsg.Content.Returns(content);

            var msg = Substitute.For<IDiscordMessageContext>();
            msg.Message.Returns(socketMsg);

            var context = new MessageWorkflowContext(msg);

            var result = await f.ExecuteAsync(context);

            result.Should().BeNull();
        }
    }
}
