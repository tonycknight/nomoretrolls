namespace nomoretrolls.Workflows
{
    internal interface IMessageWorkflow
    {
        string Name { get; }
        IMessageContextReceiver Receiver { get; }
        IEnumerable<IMessageWorkflowPart> Parts { get; }
    }
}
