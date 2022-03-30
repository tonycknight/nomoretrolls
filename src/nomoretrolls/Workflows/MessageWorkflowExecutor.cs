using System.Diagnostics.CodeAnalysis;
using nomoretrolls.Messaging;
using nomoretrolls.Telemetry;

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

            _telemetry.Message($"[{logPrefix}] Starting workflow...");
            
            var msgContext = await workflow.Receiver.ReceiveAsync(context);
            
            if (msgContext != null)
            {
                foreach (var part in workflow.Parts)
                {
                    _telemetry.Message($"[{logPrefix}] Executing part {part.GetType().Name}...");

                    MessageWorkflowContext? segmentResult = null;
                    try
                    {
                        segmentResult = await part.ExecuteAsync(msgContext);                        
                    }
                    catch(Exception ex)
                    {
                        _telemetry.Error(ex.Message);                     
                    }
                    if (segmentResult == null)
                    {
                        break;
                    }
                    msgContext = segmentResult;
                }
            }
            _telemetry.Message($"[{logPrefix}] Finished workflow.");
        }
    }
}
