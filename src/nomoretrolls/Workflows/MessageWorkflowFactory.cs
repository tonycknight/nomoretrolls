namespace nomoretrolls.Workflows
{
    internal class MessageWorkflowFactory : IMessageWorkflowFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageWorkflowFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMessageWorkflowBuilder CreateBuilder() => new MessageWorkflowBuilder(_serviceProvider);
    }
}
