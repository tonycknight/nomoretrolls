using System;
using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Statistics;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class UserWarningsFilterTests
    {
        [Fact]
        public async Task ExecuteAsync_StatsQueried()
        {
            var name = "test";
            var authorId = 1234uL;
            var limit = 5;
            var timeframe = TimeSpan.FromSeconds(1);

            var stats = Substitute.For<IUserStatisticsProvider>();
            var part = new UserWarningsFilter(stats, name, PeriodRange.AtLeast(limit, timeframe));

            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new nomoretrolls.Workflows.MessageWorkflowContext(messageContext);
            var r = await part.ExecuteAsync(context);

            stats.Received(1).GetUserStatisticCountAsync(authorId, name, timeframe);
        }

        [Fact]
        public async Task ExecuteAsync_StatsQueried_AtLeast_CountLessThanLimit_ReturnsIgnore()
        {
            var name = "test";
            var authorId = 1234uL;
            var bumps = 1;
            var limit = bumps + 5;
            var timeframe = TimeSpan.FromSeconds(1);

            var stats = Substitute.For<IUserStatisticsProvider>();
            stats.GetUserStatisticCountAsync(authorId, name, timeframe).Returns(bumps);

            var part = new UserWarningsFilter(stats, name, PeriodRange.AtLeast(limit, timeframe));

            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new nomoretrolls.Workflows.MessageWorkflowContext(messageContext);
            var r = await part.ExecuteAsync(context);

            r.Should().BeNull();

            stats.Received(1).GetUserStatisticCountAsync(authorId, name, timeframe);
        }

        [Fact]
        public async Task ExecuteAsync_StatsQueried_AtLeast_CountLessThanLimit_ReturnsContinue()
        {
            var name = "test";
            var authorId = 1234uL;
            var bumps = 1;
            var limit = bumps;
            var timeframe = TimeSpan.FromSeconds(1);

            var stats = Substitute.For<IUserStatisticsProvider>();
            stats.GetUserStatisticCountAsync(authorId, name, timeframe).Returns(bumps);

            var part = new UserWarningsFilter(stats, name, PeriodRange.AtLeast(limit, timeframe));

            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new nomoretrolls.Workflows.MessageWorkflowContext(messageContext);
            var r = await part.ExecuteAsync(context);

            r.Should().NotBeNull();

            stats.Received(1).GetUserStatisticCountAsync(authorId, name, timeframe);
        }

        [Fact]
        public async Task ExecuteAsync_StatsQueried_AtMost_CountLessThanLimit_ReturnsContinue()
        {
            var name = "test";
            var authorId = 1234uL;
            var bumps = 10;
            var limit = bumps + 1;
            var timeframe = TimeSpan.FromSeconds(1);

            var stats = Substitute.For<IUserStatisticsProvider>();
            stats.GetUserStatisticCountAsync(authorId, name, timeframe).Returns(bumps);



            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new nomoretrolls.Workflows.MessageWorkflowContext(messageContext);

            var part = new UserWarningsFilter(stats, name, PeriodRange.AtMost(limit, timeframe));
            var r = await part.ExecuteAsync(context);

            r.Should().NotBeNull();

            stats.Received(1).GetUserStatisticCountAsync(authorId, name, timeframe);
        }

        [Fact]
        public async Task ExecuteAsync_StatsQueried_AtMost_CountEqualToLimit_ReturnsContinue()
        {
            var name = "test";
            var authorId = 1234uL;
            var bumps = 10;
            var limit = bumps;
            var timeframe = TimeSpan.FromSeconds(1);

            var stats = Substitute.For<IUserStatisticsProvider>();
            stats.GetUserStatisticCountAsync(authorId, name, timeframe).Returns(bumps);



            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new nomoretrolls.Workflows.MessageWorkflowContext(messageContext);

            var part = new UserWarningsFilter(stats, name, PeriodRange.AtMost(limit, timeframe));
            var r = await part.ExecuteAsync(context);

            r.Should().NotBeNull();

            stats.Received(1).GetUserStatisticCountAsync(authorId, name, timeframe);
        }

        [Fact]
        public async Task ExecuteAsync_StatsQueried_AtMost_CountGreaterThanLimit_ReturnsNull()
        {
            var name = "test";
            var authorId = 1234uL;
            var bumps = 10;
            var limit = 1;
            var timeframe = TimeSpan.FromSeconds(1);

            var stats = Substitute.For<IUserStatisticsProvider>();
            stats.GetUserStatisticCountAsync(authorId, name, timeframe).Returns(bumps);



            var author = Substitute.For<IUser>();
            author.Id.Returns(authorId);
            var msg = Substitute.For<IMessage>();
            msg.Author.Returns(author);
            var messageContext = Substitute.For<IDiscordMessageContext>();
            messageContext.Message.Returns(msg);

            var context = new nomoretrolls.Workflows.MessageWorkflowContext(messageContext);

            var part = new UserWarningsFilter(stats, name, PeriodRange.AtMost(limit, timeframe));
            var r = await part.ExecuteAsync(context);

            r.Should().BeNull();

            stats.Received(1).GetUserStatisticCountAsync(authorId, name, timeframe);
        }
    }
}
