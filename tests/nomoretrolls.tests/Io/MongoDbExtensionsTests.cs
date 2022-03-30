using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Config;
using nomoretrolls.Io;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Io
{
    public class MongoDbExtensionsTests
    {
        [Fact]
        public void GetValidateConfig_NullMongoDb_ReturnsConfig()
        {
            var c = new AppConfiguration();

            var cp = Substitute.For<IConfigurationProvider>();
            cp.GetAppConfiguration().Returns(c);


            var r = cp.GetValidateConfig();

            r.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetValidateConfig_NonNullMongoDb_BadConnection_ThrowsException(string cn)
        {
            var c = new AppConfiguration()
            {
                MongoDb = new MongoDbConfiguration()
                {
                    Connection = cn,
                }
            };

            var cp = Substitute.For<IConfigurationProvider>();
            cp.GetAppConfiguration().Returns(c);


            var f = () => cp.GetValidateConfig();

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetValidateConfig_NonNullMongoDb_BadDatabaseName_ThrowsException(string value)
        {
            var c = new AppConfiguration()
            {
                MongoDb = new MongoDbConfiguration()
                {
                    Connection = "aaa",
                    DatabaseName = value,
                }
            };

            var cp = Substitute.For<IConfigurationProvider>();
            cp.GetAppConfiguration().Returns(c);


            var f = () => cp.GetValidateConfig();

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetValidateConfig_NonNullMongoDb_BadUserStatsCollectionName_ThrowsException(string value)
        {
            var c = new AppConfiguration()
            {
                MongoDb = new MongoDbConfiguration()
                {
                    Connection = "aaa",
                    UserStatsCollectionName = value,
                }
            };

            var cp = Substitute.For<IConfigurationProvider>();
            cp.GetAppConfiguration().Returns(c);


            var f = () => cp.GetValidateConfig();

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetValidateConfig_NonNullMongoDb_BadUserBlacklistCollectionName_ThrowsException(string value)
        {
            var c = new AppConfiguration()
            {
                MongoDb = new MongoDbConfiguration()
                {
                    Connection = "aaa",
                    UserBlacklistCollectionName = value,
                }
            };

            var cp = Substitute.For<IConfigurationProvider>();
            cp.GetAppConfiguration().Returns(c);


            var f = () => cp.GetValidateConfig();

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }


        [Theory]
        [InlineData("a", "b", "c", "d")]
        public void GetValidateConfig_NonNullMongoDb_GoodValues_ReturnsConfig(string cn, string db, string userStats, string userBlacklist)
        {
            var c = new AppConfiguration()
            {
                MongoDb = new MongoDbConfiguration()
                {
                    Connection = cn,
                    DatabaseName = db,
                    UserBlacklistCollectionName = userBlacklist,
                    UserStatsCollectionName = userStats,
                }
            };

            var cp = Substitute.For<IConfigurationProvider>();
            cp.GetAppConfiguration().Returns(c);


            var r = cp.GetValidateConfig();

            r.Should().NotBeNull();
            r.Should().Be(c);
        }
    }
}
