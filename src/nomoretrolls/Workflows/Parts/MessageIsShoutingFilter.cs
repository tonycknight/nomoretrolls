using nomoretrolls.Parsing;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Workflows.Parts
{
    internal class MessageIsShoutingFilter : IMessageWorkflowPart
    {
        private readonly ITelemetry _telemetry;

        public MessageIsShoutingFilter(ITelemetry telemetry)
        {
            _telemetry = telemetry;
        }

        public Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            MessageWorkflowContext? result = null;

            if (IsCapitals(context))
            {                
                result = context;
            }

            return Task.FromResult(result);
        }

        private bool IsCapitals(MessageWorkflowContext context)
        {            
            var content = context.Content();
            if (!string.IsNullOrWhiteSpace(content))
            {
                var analysis = content.AnalyseCapitals();

                _telemetry.Event(new TelemetryTraceEvent() { Message = $"[Message {context.DiscordContext.Message.Id}] [{this.GetType().Name}] Letters {analysis.LetterCount} capitals {analysis.CapitalCount} gini {analysis.CapitalGini}" });

                return analysis.LetterCount >= 5 &&
                    analysis.CapitalRatio > 0.6 &&
                    analysis.CapitalGini < 0.5;
            }
            
            return false;
        }
    }
}
