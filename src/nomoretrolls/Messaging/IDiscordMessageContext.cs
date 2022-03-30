using Discord;

namespace nomoretrolls.Messaging
{
    public interface IDiscordMessageContext
    {
        IMessage Message { get; }
    }
}
