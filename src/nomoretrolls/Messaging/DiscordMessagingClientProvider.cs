namespace nomoretrolls.Messaging
{
    internal class DiscordMessagingClientProvider : IDiscordMessagingClientProvider
    {
        private IDiscordMessagingClient _client;
        private readonly object _lock = new object();

        public IDiscordMessagingClient GetClient()
        {
            if (_client == null)
            {
                throw new InvalidOperationException("Client instance is not set.");
            }
            return _client;
        }

        public void SetClient(IDiscordMessagingClient client)
        {
            lock (_lock)
            {
                if (_client != null)
                {
                    throw new InvalidOperationException("Client instance already set.");
                }

                _client = client;
            }
        }
    }
}
