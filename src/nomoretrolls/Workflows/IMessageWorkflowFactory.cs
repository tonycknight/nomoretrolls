namespace nomoretrolls.Workflows
{
    internal interface IMessageWorkflowFactory
    {
        IMessageWorkflowBuilder CreateBuilder();
    }
}
