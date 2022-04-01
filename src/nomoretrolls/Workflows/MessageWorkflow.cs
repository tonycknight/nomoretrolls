namespace nomoretrolls.Workflows
{
    internal class MessageWorkflow : IMessageWorkflow
    {
        private readonly IMessageContextReceiver _receiver;
        private readonly IList<IMessageWorkflowPart> _segments;
        private readonly string _name;

        public MessageWorkflow(IMessageContextReceiver receiver, IList<IMessageWorkflowPart> segments, string name)
        {
            _receiver = receiver;
            _segments = segments;
            _name = name;
        }

        public IMessageContextReceiver Receiver => _receiver;
        public IEnumerable<IMessageWorkflowPart> Parts => _segments;
        public string Name => _name;
    }
}
