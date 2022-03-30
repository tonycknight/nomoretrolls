using Discord;
using Discord.WebSocket;

namespace nomoretrolls.Messaging
{
    internal interface IDiscordMessagingClient : IDisposable
    {
        Task StartAsync();
        Task StopAsync();

        void AddMessageReceivedHandler(Func<SocketUserMessage, Task> handler);

        void AddUserIsTypingHandler(Func<IUser, IMessageChannel, Task> handler);
    }
}
