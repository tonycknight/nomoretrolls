using nomoretrolls.Messaging;

namespace nomoretrolls.Workflows
{
    internal interface IMessageContextReceiver
    {
        Task<MessageWorkflowContext> ReceiveAsync(IDiscordMessageContext context);
    }
}
