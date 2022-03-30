using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class NoopTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsContext()
        {
            var p = new Noop();

            var context = new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>());

            var r = await p.ExecuteAsync(context);

            r.Should().Be(context);

        }
    }
}
