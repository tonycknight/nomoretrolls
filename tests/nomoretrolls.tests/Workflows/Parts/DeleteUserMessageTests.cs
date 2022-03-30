using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class DeleteUserMessageTests
    {
        [Fact]
        public async Task ExecuteAsync_DiscordInvoked()
        {           
            var part = new DeleteUserMessage();
                       
            var msg = Substitute.For<IMessage>();
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new nomoretrolls.Workflows.MessageWorkflowContext(messageContext);
            var r = await part.ExecuteAsync(context);

            r.Should().NotBeNull();

            msg.Received(1).DeleteAsync();

        }
    }
}
