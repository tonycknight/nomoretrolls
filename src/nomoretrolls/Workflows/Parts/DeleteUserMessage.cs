namespace nomoretrolls.Workflows.Parts
{
    internal class DeleteUserMessage : IMessageWorkflowPart
    {
        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var msg = context.DiscordContext.Message;

            await msg.DeleteAsync();

            return context;
        }
    }
}
