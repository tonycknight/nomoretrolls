namespace nomoretrolls.Workflows.Parts
{
    internal class SendUserReplyMessage : IMessageWorkflowPart
    {
        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var replyText = context.ReplyText();
            
            if (!string.IsNullOrWhiteSpace(replyText))
            {
                var msg = context.DiscordContext.Message;
                var msgRef = new Discord.MessageReference(msg.Id);
                
                await msg.Channel.SendMessageAsync(text: replyText, messageReference: msgRef);
            }

            return context;
        }
    }
}
