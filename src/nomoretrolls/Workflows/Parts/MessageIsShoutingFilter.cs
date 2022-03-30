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
                var (wordCount, capitalsCount, maxWordLength) = content.SplitWords().WordCapitalsSpread();

                return wordCount > 1 && maxWordLength > 5 && capitalsCount > 0;
            }
            
            return false;
        }
    }
}
