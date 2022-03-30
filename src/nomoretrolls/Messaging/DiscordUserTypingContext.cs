using Discord;

namespace nomoretrolls.Messaging
{
    internal record DiscordUserTypingContext
    {
        public DiscordUserTypingContext(Cacheable<IUser, ulong> user, Cacheable<IMessageChannel, ulong> channel)
        {
            User = user;        
            Channel = channel;
        }

        public Cacheable<IUser, ulong> User { get; init; }

        public Cacheable<IMessageChannel, ulong> Channel { get; init; }
    }
}
