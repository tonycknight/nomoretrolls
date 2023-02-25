using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using FluentAssertions;
using nomoretrolls.Commands;
using NSubstitute;
using Xunit;


namespace nomoretrolls.tests.Commands
{
    public class CommandContextExtensionsTests
    {


        [Fact]
        public async Task GetUserAsync_UserName_Unknown_ReturnsNull()
        {
            var userName = "unknown#1234";
            IUser user = null;

            var ctxt = Substitute.For<ICommandContext>();
            ctxt.GetUserAsync("unknown", "1234").Returns(Task.FromResult(user));

            var result = await ctxt.GetUserAsync(userName);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserAsync_UserName_Known_ReturnsUser()
        {
            var userName = "unknown#1234";
            IUser user = Substitute.For<IUser>();

            var ctxt = Substitute.For<ICommandContext>();
            ctxt.GetUserAsync("unknown", "1234").Returns(Task.FromResult(user));


            var result = await ctxt.GetUserAsync(userName);

            result.Should().Be(user);
        }

        [Fact]
        public async Task GetUserAsync_UserName_InvalidUserName_ReturnsNull()
        {
            var userName = "unknown1234";
            IUser user = null;

            var ctxt = Substitute.For<ICommandContext>();

            ctxt.GetUserAsync((string)null, (string)null).Returns(Task.FromResult(user));

            var result = await ctxt.GetUserAsync(userName);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserAsync_UserId_Unknown_ReturnsNull()
        {
            ulong userId = 1234L;
            IUser user = null;

            var ctxt = Substitute.For<ICommandContext>();
            ctxt.GetUserAsync(userId).Returns(Task.FromResult(user));

            var result = await ctxt.GetUserAsync(userId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserAsync_UserId_Known_ReturnsUser()
        {
            ulong userId = 1234L;
            IUser user = Substitute.For<IUser>();

            var ctxt = Substitute.For<ICommandContext>();
            ctxt.GetUserAsync(userId).Returns(Task.FromResult(user));


            var result = await ctxt.GetUserAsync(userId);

            result.Should().Be(user);
        }
    }
}
