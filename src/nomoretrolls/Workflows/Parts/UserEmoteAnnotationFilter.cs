using Tk.Extensions.Tasks;

namespace nomoretrolls.Workflows.Parts
{
    internal class UserEmoteAnnotationFilter : IMessageWorkflowPart
    {        
        public UserEmoteAnnotationFilter()
        {            
        }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            if (await IsCharacterMatch(context))
            {
                return context;
            }

            return null;            
        }

        private Task<bool> IsCharacterMatch(MessageWorkflowContext context) => true.ToTaskResult(); // TODO: implement
    }
}
