using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using nomoretrolls.Workflows;
using nomoretrolls.Telemetry;
using nomoretrolls.Messaging;
using System;
using nomoretrolls.Statistics;

namespace nomoretrolls.tests.Workflows
{
    public class MessageWorkflowExecutorTests
    {
        [Fact]
        public async Task ExecuteAsync_NullWorkflow_Returns()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var exec = new MessageWorkflowExecutor(telemetry);

            var workflow = Substitute.For<IMessageWorkflow>();
            var context = Substitute.For<IDiscordMessageContext>();

            await exec.ExecuteAsync(workflow, context);
        }

        [Fact]
        public async Task ExecuteAsync_EmptyWorkflow_Returns()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var exec = new MessageWorkflowExecutor(telemetry);

            var context = Substitute.For<IDiscordMessageContext>();
            var msgContext = new MessageWorkflowContext(context);

            var workflow = Substitute.For<IMessageWorkflow>();
            var receiver = Substitute.For<IMessageContextReceiver>();
            receiver.ReceiveAsync(Arg.Any<IDiscordMessageContext>()).Returns(Task.FromResult(msgContext));
            workflow.Receiver.Returns(receiver);


            await exec.ExecuteAsync(workflow, context);
        }

        [Fact]
        public async Task ExecuteAsync_MultiStepWorkflow_SuccessfulReturnsFromAllSteps()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var exec = new MessageWorkflowExecutor(telemetry);

            var context = Substitute.For<IDiscordMessageContext>();
            var msgContext = new MessageWorkflowContext(context);


            var receiver = Substitute.For<IMessageContextReceiver>();
            var results = new[]
            {
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
            };
            var parts = results.Select(r =>
            {
                var part = Substitute.For<IMessageWorkflowPart>();
                part.ExecuteAsync(Arg.Any<MessageWorkflowContext>()).Returns(Task.FromResult((MessageWorkflowContext?)r));
                return part;
            }).ToArray();


            var workflow = Substitute.For<IMessageWorkflow>();
            receiver.ReceiveAsync(Arg.Any<IDiscordMessageContext>()).Returns(Task.FromResult(msgContext));
            workflow.Receiver.Returns(receiver);
            workflow.Parts.Returns(parts);

            await exec.ExecuteAsync(workflow, context);

            foreach (var part in parts)
            {
                part.Received(1);
            }
        }

        [Fact]
        public async Task ExecuteAsync_MultiStepWorkflow_NullReturnsFromAllSteps()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var exec = new MessageWorkflowExecutor(telemetry);

            var context = Substitute.For<IDiscordMessageContext>();
            var msgContext = new MessageWorkflowContext(context);


            var receiver = Substitute.For<IMessageContextReceiver>();
            var parts = new[]
            {
                Substitute.For<IMessageWorkflowPart>(),
                Substitute.For<IMessageWorkflowPart>(),
            };

            var workflow = Substitute.For<IMessageWorkflow>();
            receiver.ReceiveAsync(Arg.Any<IDiscordMessageContext>()).Returns(Task.FromResult(msgContext));
            workflow.Receiver.Returns(receiver);
            workflow.Parts.Returns(parts);

            await exec.ExecuteAsync(workflow, context);

            await parts[0].Received(1).ExecuteAsync(Arg.Any<MessageWorkflowContext>());
            await parts[1].DidNotReceive().ExecuteAsync(Arg.Any<MessageWorkflowContext>());
        }

        [Fact]
        public async Task ExecuteAsync_MultiStepWorkflow_SuccessfulReturnsFromAllSteps_TelemetrySent()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var exec = new MessageWorkflowExecutor(telemetry);

            var context = Substitute.For<IDiscordMessageContext>();
            var msgContext = new MessageWorkflowContext(context);

            var receiver = Substitute.For<IMessageContextReceiver>();
            var results = new[]
            {
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
            };
            var parts = results.Select(r =>
            {
                var part = Substitute.For<IMessageWorkflowPart>();
                part.ExecuteAsync(Arg.Any<MessageWorkflowContext>()).Returns(Task.FromResult((MessageWorkflowContext?)r));
                return part;
            }).ToArray();


            var workflow = Substitute.For<IMessageWorkflow>();
            receiver.ReceiveAsync(Arg.Any<IDiscordMessageContext>()).Returns(Task.FromResult(msgContext));
            workflow.Receiver.Returns(receiver);
            workflow.Parts.Returns(parts);

            await exec.ExecuteAsync(workflow, context);

            telemetry.Received(1).Event(Arg.Is<TelemetryTraceEvent>(s => s.Message.Contains("Starting workflow")));
            telemetry.Received(1).Event(Arg.Is<TelemetryEvent>(s => s.Message.Contains("Finished workflow")));
            telemetry.Received(parts.Length).Event(Arg.Is<TelemetryEvent>(s => s.Message.Contains("Executing part")));
            telemetry.Received(parts.Length + 2).Event(Arg.Is<TelemetryEvent>(s => s.Message.Contains("Message")));
        }

        [Fact]
        public async Task ExecuteAsync_ConditionalWorkflow_PositiveCase()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var exec = new MessageWorkflowExecutor(telemetry);

            var context = Substitute.For<IDiscordMessageContext>();
            var msgContext = new MessageWorkflowContext(context);

            var receiver = Substitute.For<IMessageContextReceiver>();
            receiver.ReceiveAsync(Arg.Any<IDiscordMessageContext>()).Returns(Task.FromResult(msgContext));

            var sp = CreateMockServiceProvider();
            var usp = Substitute.For<IUserStatisticsProvider>();
            sp.GetService(typeof(IMessageWorkflowExecutor)).Returns(exec);
            sp.GetService(typeof(IUserStatisticsProvider)).Returns(usp);
            sp.GetService(typeof(ITelemetry)).Returns(telemetry);

            var results = new[]
            {
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
            };
            var parts = results.Select(r =>
            {
                var part = Substitute.For<IMessageWorkflowPart>();
                part.ExecuteAsync(Arg.Any<MessageWorkflowContext>()).Returns(Task.FromResult((MessageWorkflowContext?)r));
                return part;
            }).ToArray();

            var wf = new MessageWorkflowBuilder(sp)
                .Receiver(receiver)
                .If(b => b.Part(parts[0]),
                    b => b.Part(parts[1]),
                    b => b.Part(parts[2]))
                .Build("");

            await exec.ExecuteAsync(wf, context);

            await parts[1].Received(1).ExecuteAsync(Arg.Any<MessageWorkflowContext>());
            await parts[2].DidNotReceive().ExecuteAsync(Arg.Any<MessageWorkflowContext>());
        }


        [Fact]
        public async Task ExecuteAsync_ConditionalWorkflow_NegativeCase()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var exec = new MessageWorkflowExecutor(telemetry);

            var context = Substitute.For<IDiscordMessageContext>();
            var msgContext = new MessageWorkflowContext(context);

            var receiver = Substitute.For<IMessageContextReceiver>();
            receiver.ReceiveAsync(Arg.Any<IDiscordMessageContext>()).Returns(Task.FromResult(msgContext));

            var sp = CreateMockServiceProvider();
            var usp = Substitute.For<IUserStatisticsProvider>();
            sp.GetService(typeof(IUserStatisticsProvider)).Returns(usp);
            sp.GetService(typeof(ITelemetry)).Returns(telemetry);
            sp.GetService(typeof(IMessageWorkflowExecutor)).Returns(exec);
            var results = new[]
            {
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
                new MessageWorkflowContext(Substitute.For<IDiscordMessageContext>()),
            };
            var parts = results.Select((r, i) =>
            {
                var part = Substitute.For<IMessageWorkflowPart>();
                if (i > 0)
                    part.ExecuteAsync(Arg.Any<MessageWorkflowContext>()).Returns(Task.FromResult((MessageWorkflowContext?)r));
                return part;
            }).ToArray();

            var wf = new MessageWorkflowBuilder(sp)
                .Receiver(receiver)
                .If(b => b.Part(parts[0]),
                    b => b.Part(parts[1]),
                    b => b.Part(parts[2]))
                .Build("");

            await exec.ExecuteAsync(wf, context);

            await parts[1].DidNotReceive().ExecuteAsync(Arg.Any<MessageWorkflowContext>());
            await parts[2].Received(1).ExecuteAsync(Arg.Any<MessageWorkflowContext>());

        }


        private IServiceProvider CreateMockServiceProvider() => Substitute.For<IServiceProvider>();
    }
}
