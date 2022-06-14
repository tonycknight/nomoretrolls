namespace nomoretrolls.Messaging
{
    internal interface IDiscordMessagingClientProvider
    {
        IDiscordMessagingClient GetClient();
        void SetClient(IDiscordMessagingClient client);
    }
}
