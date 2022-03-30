namespace nomoretrolls.Workflows.Parts
{
    internal class Noop : IMessageWorkflowPart
    {
        public Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
            => Task.FromResult(context);

    }
}
