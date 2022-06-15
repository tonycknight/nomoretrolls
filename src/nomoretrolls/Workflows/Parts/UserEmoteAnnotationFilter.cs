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
                return context.EmoteListName("farmyardanimals"); // TODO: pick up from config
            }

            return null;            
        }

        private Task<bool> IsCharacterMatch(MessageWorkflowContext context) => true.ToTaskResult(); // TODO: implement
    }
}
