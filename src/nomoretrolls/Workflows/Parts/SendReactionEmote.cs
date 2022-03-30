namespace nomoretrolls.Workflows.Parts
{
    internal class SendReactionEmote : IMessageWorkflowPart
    {        
        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var emoteCode = context.EmoteCode();
            if (!string.IsNullOrWhiteSpace(emoteCode))
            {                
                var emo = new Discord.Emoji(emoteCode);
                await context.DiscordContext.Message.AddReactionAsync(emo);
            }
             return context;
        }
    }
}
