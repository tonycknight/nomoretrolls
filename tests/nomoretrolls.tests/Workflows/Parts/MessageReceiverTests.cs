using System;
using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class MessageReceiverTests
    {
        [Fact]
        public void ReceiveAsync_NullContext_ThrowsException()
        {
            var receiver = new MessageReceiver();

            var f = async () => await receiver.ReceiveAsync(null as IDiscordMessageContext);

            f.Should().ThrowAsync<ArgumentNullException>().WithMessage("?*");
        }

        [Fact]
        public async Task ReceiveAsync_ReturnsContext()
        {
            var context = Substitute.For<IDiscordMessageContext>();
            var receiver = new MessageReceiver();

            var result = await receiver.ReceiveAsync(context);

            result.Should().NotBeNull();
        }
    }
}
