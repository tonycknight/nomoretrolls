using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.WebSocket;
using nomoretrolls.Config;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Messaging
{
    [ExcludeFromCodeCoverage]
    internal class DiscordMessagingClient : IDiscordMessagingClient
    {
        private readonly AppConfiguration _config;
        private readonly ITelemetry _telemetry;
        private readonly Func<AppConfiguration, string?> _getClientToken;
        private DiscordSocketClient _client;

        private readonly System.Collections.Concurrent.ConcurrentBag<Func<SocketUserMessage, Task>> _messageReceivedHandlers;
        private readonly System.Collections.Concurrent.ConcurrentBag<Func<IUser, IMessageChannel, Task>> _userTypingHandlers;

        public DiscordMessagingClient(AppConfiguration config, ITelemetry telemetry, Func<AppConfiguration, string?> getClientToken)
        {
            _config = config;
            _telemetry = telemetry;
            _getClientToken = getClientToken;
            _userTypingHandlers = new System.Collections.Concurrent.ConcurrentBag<Func<IUser, IMessageChannel, Task>>();
            _messageReceivedHandlers = new System.Collections.Concurrent.ConcurrentBag<Func<SocketUserMessage, Task>>();

            var clientConfig = new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true,                
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            };
            
            _client = new DiscordSocketClient(clientConfig);

            _client.Log += client_Log;
            _client.Ready += client_Ready;
            _client.Disconnected += client_Disconnected;
            _client.MessageReceived += client_MessageReceived;
            _client.UserIsTyping += client_UserIsTyping;
        }


        ~DiscordMessagingClient()
        {
            Dispose(false);
        }

        public DiscordSocketClient Client => _client;

        public async Task StartAsync()
        {
            await _client.LoginAsync(Discord.TokenType.Bot, _getClientToken(_config));
            await _client.StartAsync();            
        }

        public void AddMessageReceivedHandler(Func<SocketUserMessage, Task> handler)
        {
            _messageReceivedHandlers.Add(handler);
        }

        public void AddUserIsTypingHandler(Func<IUser, IMessageChannel, Task> handler)
        {
            _userTypingHandlers.Add(handler);
        }

        public async Task StopAsync()
        {
            await _client.StopAsync();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            _client?.Dispose();
            _client = null;
        }

        private Task client_MessageReceived(SocketMessage arg)
        {            
            var msg = arg as SocketUserMessage;
            if(msg != null && !msg.Author.IsBot)
            {
                Task.Run(() => HandleMessageAsync(msg));
            }
            return Task.CompletedTask;
        }


        private Task client_UserIsTyping(Cacheable<IUser, ulong> arg1, Discord.Cacheable<Discord.IMessageChannel, ulong> arg2)
        {
            var user = arg1.Value;
            var channel = arg2.Value;

            if (user != null && channel != null && (!user.IsBot))
            {
                Task.Run(() => HandleUserTypingAsync(user, channel));
            }
            return Task.CompletedTask;
        }

        private Task client_Log(LogMessage arg) 
        {            
            _telemetry.Message(arg.Message);

            return Task.CompletedTask;
        }

        private Task client_Ready()
        {
            return Task.CompletedTask;
        }

        private Task client_Disconnected(Exception arg)
        {
            return Task.CompletedTask;
        }

        private Task HandleUserTypingAsync(IUser user, IMessageChannel channel)
        {
            foreach(var h in _userTypingHandlers)
            {
                _ = Task.Run(() => h(user, channel));
            }

            return Task.CompletedTask;
        }

        private Task HandleMessageAsync(SocketUserMessage msg)
        {
            LogMessageContent(msg);

            foreach(var h in _messageReceivedHandlers)
            {
                _ = Task.Run(() => h(msg));
            }

            return Task.CompletedTask;
        }

        private string UserLogPrefix(IUser user) => $"{user.Id} {user.Username}#{user.Discriminator}";


        private void LogMessageContent(SocketMessage msg)
        {
            if (_config.Telemetry?.LogMessageContent == true)
            {
                var line = $"[{msg.Channel.Name}] [Message {msg.Id}] [{UserLogPrefix(msg.Author)}] {msg.Content}";
                _telemetry.Message(line);
            }
        }
    }
}
