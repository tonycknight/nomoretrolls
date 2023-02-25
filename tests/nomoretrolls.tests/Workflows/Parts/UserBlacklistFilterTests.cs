using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using nomoretrolls.Blacklists;
using nomoretrolls.Workflows.Reactions;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class UserBlacklistFilterTests
    {
        [Fact]
        public async Task ExecuteAsync_StatsQueried()
        {
            var authorId = 1234uL;

            var blacklists = Substitute.For<IBlacklistProvider>();
            var part = new UserBlacklistFilter(blacklists);

            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new MessageWorkflowContext(messageContext);
            var r = await part.ExecuteAsync(context);


            blacklists.Received(1).GetUserEntryAsync(authorId);

        }

        [Fact]
        public async Task ExecuteAsync_UserBlacklisted_ReturnsContinue()
        {

            var authorId = 1234uL;

            var blacklists = Substitute.For<IBlacklistProvider>();

            blacklists.GetUserEntryAsync(authorId).Returns(Task.FromResult(new UserBlacklistEntry()));

            var part = new UserBlacklistFilter(blacklists);

            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new MessageWorkflowContext(messageContext);
            var r = await part.ExecuteAsync(context);

            r.Should().NotBeNull();

            blacklists.Received(1).GetUserEntryAsync(authorId);

        }


        [Fact]
        public async Task ExecuteAsync_UserBlacklisted_ReturnsMessage()
        {
            var authorId = 1234uL;
            var blacklists = Substitute.For<IBlacklistProvider>();

            blacklists.GetUserEntryAsync(authorId).Returns(Task.FromResult(new UserBlacklistEntry()));

            var part = new UserBlacklistFilter(blacklists);

            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new MessageWorkflowContext(messageContext);
            var r = await part.ExecuteAsync(context);

            r.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_UserNotBlacklisted_ReturnsIgnore()
        {
            var authorId = 1234uL;

            var blacklists = Substitute.For<IBlacklistProvider>();

            var part = new UserBlacklistFilter(blacklists);

            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new MessageWorkflowContext(messageContext);
            var r = await part.ExecuteAsync(context);

            r.Should().BeNull();

            blacklists.Received(1).GetUserEntryAsync(authorId);
        }
    }
}
