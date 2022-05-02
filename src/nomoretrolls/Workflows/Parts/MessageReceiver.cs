using Discord.WebSocket;
using nomoretrolls.Messaging;
using Tk.Extensions;

namespace nomoretrolls.Workflows.Parts
{
    internal class MessageReceiver : IMessageContextReceiver
    {        
        public Task<MessageWorkflowContext> ReceiveAsync(IDiscordMessageContext context)
        {
            context.ArgNotNull(nameof(context));

            var a = context.Message.Author as SocketGuildUser;
            if (a?.GuildPermissions.Administrator == true)
            {
                return Task.FromResult((MessageWorkflowContext)null);
            }
            
            var result = new MessageWorkflowContext(context);

            return Task.FromResult(result);
        }
    }
}
