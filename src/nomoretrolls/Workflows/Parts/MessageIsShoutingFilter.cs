using nomoretrolls.Parsing;

namespace nomoretrolls.Workflows.Parts
{
    internal class MessageIsShoutingFilter : IMessageWorkflowPart
    {
        
        public MessageIsShoutingFilter()
        {
        }

        public Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            MessageWorkflowContext? result = null;

            if (IsCapitals(context.Content()))
            {                
                result = context;
            }

            return Task.FromResult(result);
        }

        private bool IsCapitals(string content)
        {            
            if (!string.IsNullOrWhiteSpace(content))
            {
                var letterCount = content.LetterCount();
                
                return letterCount >= 5 && 
                    (content.CapitalCount() > letterCount / 2)
                    && content.CapitalGiniImpurity() < 0.5;
            }
            
            return false;
        }
    }
}
