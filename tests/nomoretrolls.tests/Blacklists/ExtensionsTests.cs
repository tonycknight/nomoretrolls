using System;
using FluentAssertions;
using nomoretrolls.Blacklists;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Blacklists
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData(1L, 10)]
        [InlineData(10L, 100)]
        public void CreateBlacklistEntry(ulong id, int duration)
        {
            var user = Substitute.For<Discord.IUser>();
            user.Id.Returns(id);

            var now = DateTime.UtcNow;
            var result = user.CreateBlacklistEntry(now, duration);

            result.UserId.Should().Be(id);
            result.Start.Should().Be(now);
            result.Expiry.Should().Be(now.AddMinutes(duration));
        }
    }
}
