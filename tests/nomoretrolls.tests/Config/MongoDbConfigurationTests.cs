using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Config;
using Xunit;

namespace nomoretrolls.tests.Config
{
    public class MongoDbConfigurationTests
    {
        [Fact]
        public void DatabaseName_HasWorkableDefault()
        {
            var config = new MongoDbConfiguration();
            config.DatabaseName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void UserBlacklistCollectionName_HasWorkableDefault()
        {
            var config = new MongoDbConfiguration();
            config.UserBlacklistCollectionName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void UserStatsCollectionName_HasWorkableDefault()
        {
            var config = new MongoDbConfiguration();
            config.UserStatsCollectionName.Should().NotBeNullOrWhiteSpace();
        }
    }
}
