using Discord.WebSocket;
using Tk.Extensions.Tasks;

namespace nomoretrolls.Workflows.Parts
{    
    internal class NonDmChannelFilter : IMessageWorkflowPart
    {
        public Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var channel = context.DiscordContext.Message.Channel;
            var isPrivate = (channel as SocketDMChannel)?.Recipient != null;
            
            var result = isPrivate ? null : context;

            return result.ToTaskResult();
        }
    }
}
