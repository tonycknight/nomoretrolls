namespace nomoretrolls.Workflows
{
    internal interface IMessageWorkflowPart
    {
        Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context);
    }
}
