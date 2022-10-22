﻿using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Telemetry;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class MessageIsAltCapsFilterTests
    {
        
        [Fact]
        public async Task ExecuteAsync_NullContent_ReturnsNull()
        {
            var f = new MessageIsAltCapsFilter(Substitute.For<ITelemetry>());

            var msg = Substitute.For<IDiscordMessageContext>();
            var context = new MessageWorkflowContext(msg);

            var result = await f.ExecuteAsync(context);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ExecuteAsync_EmptyContent_ReturnsNull(string content)
        {
            var f = new MessageIsAltCapsFilter(Substitute.For<ITelemetry>());

            var socketMsg = Substitute.For<Discord.IMessage>();
            socketMsg.Content.Returns(content);

            var msg = Substitute.For<IDiscordMessageContext>();
            msg.Message.Returns(socketMsg);

            var context = new MessageWorkflowContext(msg);

            var result = await f.ExecuteAsync(context);

            result.Should().BeNull();
        }


        [Theory]
        [InlineData("a")]
        [InlineData(" a ")]
        [InlineData(" B ")]
        public async Task ExecuteAsync_NonEmptyContent_ReturnsNull(string content)
        {
            var f = new MessageIsAltCapsFilter(Substitute.For<ITelemetry>());

            var socketMsg = Substitute.For<Discord.IMessage>();
            socketMsg.Content.Returns(content);

            var msg = Substitute.For<IDiscordMessageContext>();
            msg.Message.Returns(socketMsg);

            var context = new MessageWorkflowContext(msg);

            var result = await f.ExecuteAsync(context);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(" will you STOP SHOUTing")]
        [InlineData(" TeSt TeSt TeSt")]
        [InlineData(" this AND this AND this OR that AND NO SHOUTING")]
        public async Task ExecuteAsync_AltCaps_ReturnsNonNull(string content)
        {
            var f = new MessageIsAltCapsFilter(Substitute.For<ITelemetry>());

            var socketMsg = Substitute.For<Discord.IMessage>();
            socketMsg.Content.Returns(content);

            var msg = Substitute.For<IDiscordMessageContext>();
            msg.Message.Returns(socketMsg);

            var context = new MessageWorkflowContext(msg);

            var result = await f.ExecuteAsync(context);

            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData("shout")]
        [InlineData(" this AND this AND this OR that")]
        [InlineData("SHOUT")]
        [InlineData(" SHOUTing SHOUTing  ")]
        [InlineData(" will you STOP SHOUTING")]
        [InlineData(" TESt TeST TEsT")]
        public async Task ExecuteAsync_NotAltCaps_ReturnsNull(string content)
        {
            var f = new MessageIsAltCapsFilter(Substitute.For<ITelemetry>());

            var socketMsg = Substitute.For<Discord.IMessage>();
            socketMsg.Content.Returns(content);

            var msg = Substitute.For<IDiscordMessageContext>();
            msg.Message.Returns(socketMsg);

            var context = new MessageWorkflowContext(msg);

            var result = await f.ExecuteAsync(context);

            result.Should().BeNull();
        }
    }
}