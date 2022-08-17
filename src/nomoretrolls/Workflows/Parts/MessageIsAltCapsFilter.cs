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
                var analysis = content.AnalyseCapitals();

                _telemetry.Event(new TelemetryTraceEvent() { Message = $"[Message {context.DiscordContext.Message.Id}] [{this.GetType().Name}] Letters {analysis.LetterCount} capitals {analysis.CapitalCount} gini {analysis.CapitalGini}" } );
                                
                return analysis.LetterCount >= 5 &&
                    analysis.CapitalRatio >= 0.4 && analysis.CapitalRatio <= 0.6 &&
                    analysis.CapitalGini <= 0.6;
            }
            
            return false;
        }
    }
}
