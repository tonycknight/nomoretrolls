using System;
using System.Linq;
using FluentAssertions;
using nomoretrolls.Blacklists;
using nomoretrolls.Statistics;
using nomoretrolls.Telemetry;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using nomoretrolls.Workflows.Reactions;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows
{
    public class MessageWorkflowBuilderTests
    {
        [Fact]
        public void Build_ReceiverApplied_PartApplied_ReturnsWorkflow()
        {
            var sp = CreateMockServiceProvider();
            var receiver = Substitute.For<IMessageContextReceiver>();
            var part = Substitute.For<IMessageWorkflowPart>();

            var builder = new MessageWorkflowBuilder(sp)
                                    .Receiver(receiver)
                                    .Part(part);
                        
            var result = builder.Build("");

            result.Should().NotBeNull();            
            result.Receiver.Should().Be(receiver);
            result.Parts.Should().BeEquivalentTo(new[] { part });
        }

        [Fact]
        public void Build_ReceiverNotApplied_PartApplied_ReturnsWorkflow()
        {
            var sp = CreateMockServiceProvider();
            var part = Substitute.For<IMessageWorkflowPart>();

            var builder = new MessageWorkflowBuilder(sp)
                                    .Part(part);

            var f = () => builder.Build("");

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }

        [Fact]
        public void Build_ReceiverApplied_PartNotApplied_ReturnsWorkflow()
        {
            var sp = CreateMockServiceProvider();
            var receiver = Substitute.For<IMessageContextReceiver>();
            
            var builder = new MessageWorkflowBuilder(sp)
                                    .Receiver(receiver);

            var f = () => builder.Build("");

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }

        [Fact]
        public void MessageIsShoutingFilter()
        {
            var sp = CreateMockServiceProvider();
            sp.GetService(typeof(IEmoteGenerator)).Returns(Substitute.For<IEmoteGenerator>());
            sp.GetService(typeof(IShoutingReplyTextGenerator)).Returns(Substitute.For<IShoutingReplyTextGenerator>());

            var builder = new MessageWorkflowBuilder(sp)
                                    .Receiver(Substitute.For<IMessageContextReceiver>())
                                    .MessageIsShouting();


            var result = builder.Build("").Parts.ToList();

            result[0].Should().BeOfType<MessageIsShoutingFilter>();
        }

        [Fact]
        public void SendReactionEmote()
        {
            var sp = CreateMockServiceProvider();

            var builder = new MessageWorkflowBuilder(sp)
                                    .Receiver(Substitute.For<IMessageContextReceiver>())
                                    .SendReactionEmote();


            var result = builder.Build("").Parts.ToList();

            result[0].Should().BeOfType<SendReactionEmote>();
        }


        [Fact]
        public void SendUserMentionMessage()
        {
            var sp = CreateMockServiceProvider();

            var builder = new MessageWorkflowBuilder(sp)
                                    .Receiver(Substitute.For<IMessageContextReceiver>())
                                    .SendUserReplyMessage();


            var result = builder.Build("").Parts.ToList();

            result[0].Should().BeOfType<SendUserReplyMessage>();
        }


        [Theory]
        [InlineData("", 1, 100)]
        [InlineData(" ", 1, 100)]
        [InlineData("a", 1, 100)]
        [InlineData(" b ", 10, 1000)]
        public void UserWarningsFilter(string statName, int limit, int timeframeSecs)
        {
            var timeframe = TimeSpan.FromSeconds(timeframeSecs);

            var usp = Substitute.For<IUserStatisticsProvider>();
            var sp = CreateMockServiceProvider();
            sp.GetService(typeof(IUserStatisticsProvider)).Returns(usp);
            
            var builder = new MessageWorkflowBuilder(sp)
                                    .Receiver(Substitute.For<IMessageContextReceiver>())
                                    .UserWarningsFilter(statName, PeriodRange.AtLeast(limit, timeframe));


            var result = (UserWarningsFilter)builder.Build("").Parts.Single();

            result.StatsName.Should().Be(statName);
            result.Period.Count.Should().Be(limit);
            result.Period.Duration.Should().Be(timeframe);            
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("a")]
        [InlineData(" a ")]
        public void BumpUserWarnings(string statName)
        {
            var usp = Substitute.For<IUserStatisticsProvider>();
            var sp = CreateMockServiceProvider();
            sp.GetService(typeof(IUserStatisticsProvider)).Returns(usp);

            var builder = new MessageWorkflowBuilder(sp)
                                    .Receiver(Substitute.For<IMessageContextReceiver>())
                                    .BumpUserWarnings(statName);


            var result = (BumpUserWarnings)builder.Build("").Parts.Single();

            result.StatsName.Should().Be(statName);
        }


        [Fact]
        public void UserBlacklistFilter()
        {            
            var sp = CreateMockServiceProvider();
            sp.GetService(typeof(IBlacklistProvider)).Returns(Substitute.For<IBlacklistProvider>());
            sp.GetService(typeof(IShoutingReplyTextGenerator)).Returns(Substitute.For<IShoutingReplyTextGenerator>());
            sp.GetService(typeof(IEmoteGenerator)).Returns(Substitute.For<IEmoteGenerator>());

            var builder = new MessageWorkflowBuilder(sp)
                                    .Receiver(Substitute.For<IMessageContextReceiver>())
                                    .UserIsBlacklisted();


            var result = builder.Build("").Parts.ToList();

            result[0].Should().BeOfType<UserBlacklistFilter>();
        }

        [Fact]
        public void If_SingleTier()
        {
            var sp = CreateMockServiceProvider();
            var usp = Substitute.For<IUserStatisticsProvider>();
            sp.GetService(typeof(ITelemetry)).Returns(Substitute.For<ITelemetry>());
            sp.GetService(typeof(IMessageWorkflowExecutor)).Returns(Substitute.For<IMessageWorkflowExecutor>());
            sp.GetService(typeof(IUserStatisticsProvider)).Returns(usp);
            
            var receiver = Substitute.For<IMessageContextReceiver>();

            var builder = new MessageWorkflowBuilder(sp)
                                .Receiver(receiver)
                                .If(b => b.UserWarningsFilter("blacklisted_user", PeriodRange.AtLeast(3, TimeSpan.FromMinutes(5))),
                                    b => b.SendUserReplyMessage(),
                                    b => b.DeleteUserMessage());

            var wf = builder.Build("");
            var parts = wf.Parts.ToList();

            var if1 = (IfPart)parts.Single();
            if1.Condition.Should().BeOfType<UserWarningsFilter>();
            if1.OnSuccess.Should().BeOfType<MessageWorkflow>();
            if1.OnFailure.Should().BeOfType<MessageWorkflow>();            
        }

        [Fact]
        public void If_DoubleTierIfs()
        {            
            var sp = CreateMockServiceProvider();
            sp.GetService(typeof(ITelemetry)).Returns(Substitute.For<ITelemetry>());
            sp.GetService(typeof(IMessageWorkflowExecutor)).Returns(Substitute.For<IMessageWorkflowExecutor>());
            sp.GetService(typeof(IUserStatisticsProvider)).Returns(Substitute.For<IUserStatisticsProvider>());
            sp.GetService(typeof(IBlacklistProvider)).Returns(Substitute.For<IBlacklistProvider>());
            sp.GetService(typeof(IShoutingReplyTextGenerator)).Returns(Substitute.For<IShoutingReplyTextGenerator>());
            sp.GetService(typeof(IEmoteGenerator)).Returns(Substitute.For<IEmoteGenerator>());

            var receiver = Substitute.For<IMessageContextReceiver>();

            var builder = new MessageWorkflowBuilder(sp)
                                .Receiver(receiver)
                                .If(b => b.UserWarningsFilter("blacklisted_user", PeriodRange.AtLeast(3, TimeSpan.FromMinutes(5))),
                                    b => b.If(b1 => b1.UserWarningsFilter("test", PeriodRange.AtLeast(5, TimeSpan.FromMinutes(1))),
                                              b1 => b1.Noop(),
                                              b1 => b1.DeleteUserMessage()),    
                                    b => b.If(b2 => b2.UserIsBlacklisted(),
                                              b2 => b2.DeleteUserMessage(),
                                              b2 => b2.Noop()));

            var wf = builder.Build("");
            var parts = wf.Parts.ToList();

            var if1 = (IfPart)parts.Single();
            if1.Condition.Should().BeOfType<UserWarningsFilter>();
            
            var if21 = (MessageWorkflow)if1.OnSuccess;            
            var if22 = (MessageWorkflow)if1.OnFailure;            
        }

        [Fact]
        public void If_NestedIfs()
        {
            var sp = CreateMockServiceProvider();

            sp.GetService(typeof(ITelemetry)).Returns(Substitute.For<ITelemetry>());
            sp.GetService(typeof(IMessageWorkflowExecutor)).Returns(Substitute.For<IMessageWorkflowExecutor>());
            sp.GetService(typeof(IUserStatisticsProvider)).Returns(Substitute.For<IUserStatisticsProvider>());
            sp.GetService(typeof(IBlacklistProvider)).Returns(Substitute.For<IBlacklistProvider>());
            sp.GetService(typeof(IShoutingReplyTextGenerator)).Returns(Substitute.For<IShoutingReplyTextGenerator>());
            sp.GetService(typeof(IEmoteGenerator)).Returns(Substitute.For<IEmoteGenerator>());

            var receiver = Substitute.For<IMessageContextReceiver>();

            var builder = new MessageWorkflowBuilder(sp)
                                .Receiver(receiver)
                                .If(b => b.UserWarningsFilter("blacklisted_user", PeriodRange.AtLeast(3, TimeSpan.FromMinutes(5))),
                                    b => b.SendUserReplyMessage(),
                                    b => b.If(b2 => b2.UserIsBlacklisted(),
                                              b2 => b2.DeleteUserMessage(),
                                              b2 => b2.Noop()));

            var wf = builder.Build("");
            var parts = wf.Parts.ToList();

            var if1 = (IfPart)parts.Single();
            if1.Condition.Should().BeOfType<UserWarningsFilter>();
            if1.OnSuccess.Should().BeOfType<MessageWorkflow>();
            if1.OnFailure.Should().BeOfType<MessageWorkflow>();            
        }

        private IServiceProvider CreateMockServiceProvider() => Substitute.For<IServiceProvider>();
    }
}
