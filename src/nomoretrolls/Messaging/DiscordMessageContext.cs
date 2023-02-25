using Discord;

namespace nomoretrolls.Messaging
{
    internal record DiscordMessageContext : IDiscordMessageContext
    {
        public DiscordMessageContext(IMessage message)
        {
            Message = message;
        }

        public IMessage Message { get; init; }

    }
}
