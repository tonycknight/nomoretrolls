namespace nomoretrolls.Messaging
{
    internal interface IDiscordMessagingClientProvider
    {
        DiscordMessagingClient GetClient();
        void SetClient(DiscordMessagingClient client);
    }
}
