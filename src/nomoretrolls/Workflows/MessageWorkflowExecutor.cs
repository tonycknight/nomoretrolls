﻿using System.Diagnostics.CodeAnalysis;
using nomoretrolls.Messaging;
using nomoretrolls.Telemetry;
using Tk.Extensions.Guards;

namespace nomoretrolls.Workflows
{
    internal class MessageWorkflowExecutor : IMessageWorkflowExecutor
    {
        private readonly ITelemetry _telemetry;

        public MessageWorkflowExecutor(ITelemetry telemetry)
        {
            _telemetry = telemetry.ArgNotNull(nameof(telemetry));
        }

        [ExcludeFromCodeCoverage]
        public Task ExecuteAsync(IEnumerable<IMessageWorkflow> workflows, IDiscordMessageContext context)
        {
            foreach (var wf in workflows)
            {
                Task.Run(() => ExecuteAsync(wf, context));
            }

            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(IMessageWorkflow workflow, IDiscordMessageContext context)
        {
            var logPrefix = $"Message {context.Message?.Id}";

            _telemetry.Event(new TelemetryTraceEvent() { Message = $"[{logPrefix}] Starting workflow '{workflow.Name}'..." });

            var msgContext = await workflow.Receiver.ReceiveAsync(context);

            if (msgContext != null)
            {
                foreach (var part in workflow.Parts)
                {
                    _telemetry.Event(new TelemetryTraceEvent() { Message = $"[{logPrefix}] Executing part '{workflow.Name}.{part.GetType().Name}'..." });

                    MessageWorkflowContext? segmentResult = null;
                    try
                    {
                        segmentResult = await part.ExecuteAsync(msgContext);
                    }
                    catch (Exception ex)
                    {
                        _telemetry.Event(new TelemetryErrorEvent() { Exception = ex });
                    }
                    if (segmentResult == null)
                    {
                        break;
                    }
                    msgContext = segmentResult;
                }
            }
            _telemetry.Event(new TelemetryTraceEvent() { Message = $"[{logPrefix}] Finished workflow '{workflow.Name}'." });
        }
    }
}
