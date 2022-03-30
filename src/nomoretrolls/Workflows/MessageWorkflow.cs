namespace nomoretrolls.Workflows
{
    internal class MessageWorkflow : IMessageWorkflow
    {
        private readonly IMessageContextReceiver _receiver;
        private readonly IList<IMessageWorkflowPart> _segments;

        public MessageWorkflow(IMessageContextReceiver receiver, IList<IMessageWorkflowPart> segments)
        {
            _receiver = receiver;
            _segments = segments;
        }

        public IMessageContextReceiver Receiver => _receiver;
        public IEnumerable<IMessageWorkflowPart> Parts => _segments;
    }
}
