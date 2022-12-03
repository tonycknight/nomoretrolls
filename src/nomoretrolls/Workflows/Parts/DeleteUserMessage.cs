namespace nomoretrolls.Workflows.Parts
{
    internal class DeleteUserMessage : IMessageWorkflowPart
    {
        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var msg = context.DiscordContext.Message;
            // TODO: cannot delete DM messages
            
            await msg.DeleteAsync();

            return context;
        }
    }
}
