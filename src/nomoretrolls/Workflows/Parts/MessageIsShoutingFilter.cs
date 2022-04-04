﻿using nomoretrolls.Parsing;
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
                var letterCount = content.LetterCount();
                var capitalCount = content.CapitalCount();
                var gini = content.CapitalGiniImpurity();

                _telemetry.Message($"[Message {context.DiscordContext.Message.Id}] [{this.GetType().Name}] Letters {letterCount} capitals {capitalCount} gini {gini}");

                return letterCount >= 5 &&
                    capitalCount > letterCount / 2 && 
                    gini < 0.5;
            }
            
            return false;
        }
    }
}
