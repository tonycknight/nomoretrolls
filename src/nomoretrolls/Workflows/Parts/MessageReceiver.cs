using nomoretrolls.Messaging;

namespace nomoretrolls.Workflows.Parts
{
    internal class MessageReceiver : IMessageContextReceiver
    {        
        public Task<MessageWorkflowContext> ReceiveAsync(IDiscordMessageContext context)
        {
            context.ArgNotNull(nameof(context));

            var result = new MessageWorkflowContext(context);

            return Task.FromResult(result);
        }
    }
}
