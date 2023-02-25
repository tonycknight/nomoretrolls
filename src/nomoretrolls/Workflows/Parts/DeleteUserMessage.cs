using Discord.WebSocket;

namespace nomoretrolls.Workflows.Parts
{
    internal class DeleteUserMessage : IMessageWorkflowPart
    {
        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var msg = context.DiscordContext.Message;
            // Can't delete messages from DM channels!
            var chnl = msg.Channel as SocketDMChannel;
            if (chnl?.Recipient != null)
            {
                return context;
            }
            await msg.DeleteAsync();

            return context;
        }
    }
}
