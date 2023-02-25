using System;
using FluentAssertions;
using nomoretrolls.Messaging;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Messaging
{
    public class DiscordMessagingClientProviderTests
    {
        [Fact]
        public void GetClient_NoValueSet_ExceptionThrown()
        {
            var f = () => new DiscordMessagingClientProvider().GetClient();

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }

        [Fact]
        public void GetClient_ValueSet_ValueSet()
        {
            var c = Substitute.For<IDiscordMessagingClient>();
            var p = new DiscordMessagingClientProvider();
            p.SetClient(c);

            var r = p.GetClient();

            r.Should().Be(c);
        }

        [Fact]
        public void SetClient_ValueRepeatedlySet_ExceptionThrown()
        {
            var p = new DiscordMessagingClientProvider();
            var c = Substitute.For<IDiscordMessagingClient>();
            p.SetClient(c);
            var f = () => p.SetClient(c);

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }
    }
}
