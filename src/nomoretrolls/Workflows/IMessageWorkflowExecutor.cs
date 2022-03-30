using nomoretrolls.Messaging;

namespace nomoretrolls.Workflows
{
    internal interface IMessageWorkflowExecutor
    {
        Task ExecuteAsync(IMessageWorkflow workflow, IDiscordMessageContext context);

        Task ExecuteAsync(IEnumerable<IMessageWorkflow> workflows, IDiscordMessageContext context);
    }
}
