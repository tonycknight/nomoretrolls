using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Statistics;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class BumpUserWarningsTests
    {
        [Fact]
        public async Task ExecuteAsync_StatsBumped()
        {
            var name = "test";
            var authorId = 1234uL;
            var stats = Substitute.For<IUserStatisticsProvider>();
            var part = new BumpUserWarnings(stats, name);

            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new nomoretrolls.Workflows.MessageWorkflowContext(messageContext);
            var r = await part.ExecuteAsync(context);

            r.Should().NotBeNull();

            stats.Received(1).BumpUserStatisticAsync(authorId, name);

        }
    }
}
