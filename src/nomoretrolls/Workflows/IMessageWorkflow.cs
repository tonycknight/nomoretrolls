namespace nomoretrolls.Workflows
{
    internal interface IMessageWorkflow
    {
        IMessageContextReceiver Receiver { get; }
        IEnumerable<IMessageWorkflowPart> Parts { get; }
    }
}
