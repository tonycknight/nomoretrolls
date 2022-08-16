using nomoretrolls.Parsing;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Workflows.Parts
{
    internal class MessageIsAltCapsFilter : IMessageWorkflowPart
    {
        private readonly ITelemetry _telemetry;

        public MessageIsAltCapsFilter(ITelemetry telemetry)
        {
            _telemetry = telemetry;
        }

        public Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            MessageWorkflowContext? result = null;

            if (IsAltCaps(context))
            {                
                result = context;
            }

            return Task.FromResult(result);
        }

        private bool IsAltCaps(MessageWorkflowContext context)
        {            
            var content = context.Content();
            if (!string.IsNullOrWhiteSpace(content))
            {
                var letterCount = content.LetterCount();
                var capitalCount = content.CapitalCount();
                var gini = content.CapitalGiniImpurity();

                _telemetry.Event(new TelemetryTraceEvent() { Message = $"[Message {context.DiscordContext.Message.Id}] [{this.GetType().Name}] Letters {letterCount} capitals {capitalCount} gini {gini}" } );

                return letterCount >= 5 &&
                    capitalCount >= letterCount / 2 &&
                    gini <= 0.6;
            }
            
            return false;
        }
    }
}
