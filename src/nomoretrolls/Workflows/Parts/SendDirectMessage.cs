namespace nomoretrolls.Workflows.Parts
{
    internal class SendDirectMessage : IMessageWorkflowPart
    {
        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {            
            var replyText = context.ReplyText();

            if (!string.IsNullOrWhiteSpace(replyText))
            {                
                var replyChannel = await context.DiscordContext.Message.Author.CreateDMChannelAsync();
                await replyChannel.SendMessageAsync(replyText);
            }

            return context;
        }
    }
}
