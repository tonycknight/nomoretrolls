using Tk.Extensions.Tasks;

namespace nomoretrolls.Workflows.Parts
{
    internal class CallingEveryoneFilter : IMessageWorkflowPart
    {
        public Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var msg = context.DiscordContext.Message?.Content ?? "";
            
            if (msg!.IndexOf("@everyone", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return context.ToTaskResult();
            }
            
            return ((MessageWorkflowContext?)null).ToTaskResult();
        }
    }
}
