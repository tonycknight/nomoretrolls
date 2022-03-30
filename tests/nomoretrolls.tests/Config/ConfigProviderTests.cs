using System;
using System.IO;
using FluentAssertions;
using nomoretrolls.Config;
using nomoretrolls.Io;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Config
{
    public class ConfigProviderTests
    {
        [Fact]
        public void ConfigurationProvider_NullFilePath_ExceptionThrown()
        {
            var io = Substitute.For<IoProvider>();
            var cp = new ConfigurationProvider(io);

            var f = () => cp.GetAppConfiguration();

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }

        [Theory]
        [InlineData("test_token")]
        public void ConfigurationProvider_ConfigReturned(string token)
        {
            var json = $"{{  clientToken: \"{token}\" }}";
            var jsonbuff = System.Text.Encoding.UTF8.GetBytes(json);
            using var s = new MemoryStream(jsonbuff);
            using var srdr = new StreamReader(s);

            var io = Substitute.For<IIoProvider>();
            io.OpenFileReader(Arg.Any<string>()).Returns(srdr);


            var provider = new ConfigurationProvider(io);
            provider.SetFilePath("dummyfilepath");

            var config = provider.GetAppConfiguration();
            config.Discord?.DiscordClientToken.Should().Be(token);
        }
    }
}
