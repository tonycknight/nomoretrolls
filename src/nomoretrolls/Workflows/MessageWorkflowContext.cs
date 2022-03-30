using nomoretrolls.Messaging;

namespace nomoretrolls.Workflows
{
    internal record MessageWorkflowContext
    {
        public MessageWorkflowContext(IDiscordMessageContext message)
        {
            DiscordContext = message;
            AppData = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public IDiscordMessageContext DiscordContext { get; init; }
        public Dictionary<string, string> AppData { get; init; }
    }
}
