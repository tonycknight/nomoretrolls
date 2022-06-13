namespace nomoretrolls.Messaging
{
    internal class DiscordMessagingClientProvider : IDiscordMessagingClientProvider
    {
        private DiscordMessagingClient _client;
        private readonly object _lock = new object();

        public DiscordMessagingClient GetClient()
        {
            if (_client == null)
            {
                throw new InvalidOperationException("Client instance is not set.");
            }
            return _client;
        }

        public void SetClient(DiscordMessagingClient client)
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
