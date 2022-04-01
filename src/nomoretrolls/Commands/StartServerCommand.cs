using System.Diagnostics.CodeAnalysis;
using McMaster.Extensions.CommandLineUtils;
using nomoretrolls.Blacklists;
using nomoretrolls.Config;
using nomoretrolls.Statistics;
using nomoretrolls.Telemetry;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;

namespace nomoretrolls.Commands
{
    [ExcludeFromCodeCoverage]
    [Command("start", Description = "Start the bot")]
    internal class StartServerCommand
    {
        private readonly IConfigurationProvider _configProvider;
        private readonly ITelemetry _telemetry;
        private readonly IMessageWorkflowExecutor _workflowExecutor;
        private readonly IMessageWorkflow[] _clientMessageWorkflows;
        private readonly IBlacklistProvider _blacklistProvider;
        private readonly IUserStatisticsProvider _statsProvider;
        private readonly IWorkflowConfigurationRepository _workflowConfig;

        public StartServerCommand(Config.IConfigurationProvider configProvider, Telemetry.ITelemetry telemetry, 
                                  Workflows.IMessageWorkflowFactory wfFactory, Workflows.IMessageWorkflowExecutor workflowExecutor,
                                  Blacklists.IBlacklistProvider blacklistProvider, Statistics.IUserStatisticsProvider statsProvider,
                                  Config.IWorkflowConfigurationRepository workflowConfig)
        {
            _configProvider = configProvider.ArgNotNull(nameof(configProvider));
            _telemetry = telemetry.ArgNotNull(nameof(telemetry));            
            _workflowExecutor = workflowExecutor.ArgNotNull(nameof(workflowExecutor));
            _blacklistProvider = blacklistProvider;
            _statsProvider = statsProvider;
            _workflowConfig = workflowConfig;
            
            var wfProvider = new MessageWorkflowProvider(wfFactory);

            _clientMessageWorkflows = new[] { wfProvider.CreateBlacklistedUserWorkflow(),
                                              wfProvider.CreateShoutingWorkflow(),
                                              wfProvider.CreateShoutingPersonalReplyWorkflow() };
        }

        [Argument(0, Name ="configFile", Description = "The path to the configuration file.")]
        public string? ConfigurationFile { get; set; }

        public async Task<int> OnExecuteAsync()
        {            
            if(this.ConfigurationFile == null)
            {
                return false.ToReturnCode();
            }

            new StartServerCommandValidator().Validate(this);

            var config = this.ConfigurationFile.Pipe(_configProvider.SetFilePath).Pipe(c => c.GetAppConfiguration());

            _telemetry.Message("Starting services...");

            _telemetry.Message("Starting client...");
            var client = new Messaging.DiscordMessagingClient(config, _telemetry,
                                                                395338442822,
                                                                c => c.Discord?.DiscordClientToken,
                                                                c => c.Discord?.DiscordClientId);

            client.AddMessageReceivedHandler(async msg =>
            {                
                var context = new Messaging.DiscordMessageContext(msg);

                await _workflowExecutor.ExecuteAsync(_clientMessageWorkflows, context);
            });

            /*
            client.AddUserIsTypingHandler(async (user, channel) =>
            {
                var mention = user.Mention;

                var msg = $"{mention} Noone cares what you think.";

                await channel.SendMessageAsync(msg);
            });
            */

            await CreateAdminCommandHandler(client);

            await client.StartAsync();
            
            _telemetry.Message("Startup complete.");            
            _telemetry.Message($"Chat bot registration URI: {client.BotRegistrationUri}");
            _telemetry.Message("Proxy started. Hit CTRL-C to quit");

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += async (object? sender, ConsoleCancelEventArgs e) =>
            {
                _telemetry.Message("Shutting down services...");

                await client.StopAsync();

                client.Dispose();
                client = null;

                cts.Cancel();

                _telemetry.Message("Services shutdown");                
            };

            WaitHandle.WaitAll(new[] { cts.Token.WaitHandle });

            return true.ToReturnCode();
        }

        private async Task<AdminCommandsHandler> CreateAdminCommandHandler(Messaging.DiscordMessagingClient client)
        {            
            var adminHandler = new Commands.AdminCommandsHandler(client.Client, new Discord.Commands.CommandService(), _blacklistProvider, _telemetry, _statsProvider, _workflowConfig);
            
            await adminHandler.InstallCommandsAsync();

            return adminHandler;
        }
    }
}
