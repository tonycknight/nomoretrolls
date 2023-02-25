using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Telemetry;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class IfPartTests
    {
        [Fact]
        public async Task ExecuteAsync_PositiveCondition_SuccessInvoked()
        {

            var telemetry = Substitute.For<ITelemetry>();
            var exec = Substitute.For<IMessageWorkflowExecutor>();
            var condition = Substitute.For<IMessageWorkflowPart>();

            condition.ExecuteAsync(Arg.Any<MessageWorkflowContext>())
                .Returns(ci => Task.FromResult((MessageWorkflowContext)ci.Args()[0]));

            var success = Substitute.For<IMessageWorkflow>();
            var failure = Substitute.For<IMessageWorkflow>();

            var if1 = new IfPart(telemetry, exec, condition, success, failure);

            var context = new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>());

            var result = await if1.ExecuteAsync(context);

            result.Should().Be(context);

            exec.Received(0).ExecuteAsync(failure, Arg.Any<IDiscordMessageContext>());
            exec.Received(1).ExecuteAsync(success, Arg.Any<IDiscordMessageContext>());

            telemetry.Received(1).Event(Arg.Is<TelemetryEvent>(s => !string.IsNullOrEmpty(s.Message)));
            telemetry.Received(1).Event(Arg.Is<TelemetryEvent>(s => s.Message.StartsWith("[Message ")));

        }

        [Fact]
        public async Task ExecuteAsync_NegativeCondition_FailureInvoked()
        {

            var telemetry = Substitute.For<ITelemetry>();
            var exec = Substitute.For<IMessageWorkflowExecutor>();
            var condition = Substitute.For<IMessageWorkflowPart>();
            condition.ExecuteAsync(Arg.Any<MessageWorkflowContext>())
                .Returns(ci => Task.FromResult((MessageWorkflowContext)null));

            var success = Substitute.For<IMessageWorkflow>();
            var failure = Substitute.For<IMessageWorkflow>();


            var if1 = new IfPart(telemetry, exec, condition, success, failure);

            var context = new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>());

            var result = await if1.ExecuteAsync(context);

            result.Should().BeNull();

            exec.Received(0).ExecuteAsync(success, Arg.Any<IDiscordMessageContext>());
            exec.Received(1).ExecuteAsync(failure, Arg.Any<IDiscordMessageContext>());


            telemetry.Received(1).Event(Arg.Is<TelemetryEvent>(s => !string.IsNullOrEmpty(s.Message)));
            telemetry.Received(1).Event(Arg.Is<TelemetryEvent>(s => s.Message.StartsWith("[Message ")));

        }
    }
}
