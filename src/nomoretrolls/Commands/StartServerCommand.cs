using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using nomoretrolls.Config;
using nomoretrolls.Scheduling;
using nomoretrolls.Telemetry;
using nomoretrolls.Workflows;
using Tk.Extensions.Funcs;
using Tk.Extensions.Guards;
using Tk.Extensions.Reflection;

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
        private readonly IServiceProvider _serviceProvider;
        private readonly IJobScheduler _jobScheduler;

        public StartServerCommand(Config.IConfigurationProvider configProvider, Telemetry.ITelemetry telemetry,
                                  Workflows.IMessageWorkflowFactory wfFactory, Workflows.IMessageWorkflowExecutor workflowExecutor,
                                  IServiceProvider serviceProvider, IJobScheduler jobScheduler)
        {
            _configProvider = configProvider.ArgNotNull(nameof(configProvider));
            _telemetry = telemetry.ArgNotNull(nameof(telemetry));
            _workflowExecutor = workflowExecutor.ArgNotNull(nameof(workflowExecutor));
            _serviceProvider = serviceProvider.ArgNotNull(nameof(serviceProvider));
            _jobScheduler = jobScheduler;
            _clientMessageWorkflows = CreateMessageWorkflows(wfFactory.ArgNotNull(nameof(wfFactory)));
        }

        [Option(CommandOptionType.SingleValue, Description = "The configuration file's path.", LongName = "config", ShortName = "c")]
        public string ConfigurationFile { get; set; }

        public async Task<int> OnExecuteAsync()
        {
            var config = GetConfig();
            EchoServiceMetadata();
            var client = CreateDiscordClient(config);
            await CreateAdminCommandHandler(client);
            await client.StartAsync();

            CreateJobScheduler();

            _telemetry.Event(new TelemetryHeadlineEvent() { Message = "Startup complete." });
            _telemetry.Event(new TelemetryHeadlineEvent() { Message = $"Bot registration URI: {client.BotRegistrationUri}" });
            _telemetry.Event(new TelemetryHeadlineEvent() { Message = "Proxy started. Hit CTRL-C to quit" });

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += async (object? sender, ConsoleCancelEventArgs e) =>
            {
                _telemetry.Event(new TelemetryInfoEvent() { Message = "Shutting down job scheduler..." });
                _jobScheduler.Stop();
                if (_jobScheduler is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _telemetry.Event(new TelemetryInfoEvent() { Message = "Job scheduler shutdown." });

                _telemetry.Event(new TelemetryInfoEvent() { Message = "Shutting down services..." });
                await client.StopAsync();
                client.Dispose();
                client = null;
                cts.Cancel();
                _telemetry.Event(new TelemetryInfoEvent() { Message = "Services shutdown" });
                e.Cancel = true;
            };

            WaitHandle.WaitAll(new[] { cts.Token.WaitHandle });

            return true.ToReturnCode();
        }

        private IMessageWorkflow[] CreateMessageWorkflows(Workflows.IMessageWorkflowFactory wfFactory)
        {
            var wfProvider = new MessageWorkflowProvider(wfFactory);

            return new[] {  wfProvider.CreateBlacklistedUserWorkflow(),
                            wfProvider.CreateShoutingWorkflow(),
                            wfProvider.CreateShoutingPersonalReplyWorkflow(),
                            wfProvider.CreateAutoEmoteWorkflow(),
                            wfProvider.CreateAltCapsWorkflow(),
                            wfProvider.CreateAltCapsPersonalReplyWorkflow(),
                            wfProvider.CreateUserReplyWorkflow(),
                            wfProvider.CreateStatCountWorkflow()};
        }

        private void EchoServiceMetadata()
        {
            var attrs = this.GetAssemblyCustomAttributes();

            var product = attrs.GetAttributeValue<AssemblyProductAttribute, string>(a => a.Product);
            var version = attrs.GetAttributeValue<AssemblyInformationalVersionAttribute, string>(a => a.InformationalVersion);

            _telemetry.Event(new TelemetryHeadlineEvent() { Message = $"{product} Version {version}" });
        }

        private void CreateJobScheduler()
        {
            _telemetry.Event(new TelemetryInfoEvent() { Message = "Starting job scheduler..." });
            var jobs = GetJobSchedules().ToList();
            _telemetry.Event(new TelemetryInfoEvent() { Message = $"Found {jobs.Count} scheduled job(s)." });
            _jobScheduler.Register(jobs);
            _jobScheduler.Start();
            _telemetry.Event(new TelemetryInfoEvent() { Message = "Finished creating job scheduler." });
        }

        private Messaging.DiscordMessagingClient CreateDiscordClient(AppConfiguration config)
        {
            _telemetry.Event(new TelemetryInfoEvent() { Message = "Starting client..." });
            const ulong requiredPermissions = 395338442822;
            var client = new Messaging.DiscordMessagingClient(config, _telemetry,
                                                                requiredPermissions,
                                                                c => c.Discord?.DiscordClientToken,
                                                                c => c.Discord?.DiscordClientId);

            var clientProvider = _serviceProvider.GetService(typeof(Messaging.IDiscordMessagingClientProvider)) as Messaging.IDiscordMessagingClientProvider;
            clientProvider.SetClient(client);

            client.AddMessageReceivedHandler(async msg =>
            {
                var context = new Messaging.DiscordMessageContext(msg);

                await _workflowExecutor.ExecuteAsync(_clientMessageWorkflows, context);
            });

            return client;
        }

        private AppConfiguration GetConfig()
        {
            new StartServerCommandValidator().Validate(this);

            var config = this.ConfigurationFile.Pipe(_configProvider.SetFilePath)
                                               .Pipe(c => c.GetAppConfiguration());
            new StartServerCommandValidator().Validate(this, config);

            return config;
        }

        private async Task<AdminCommandsHandler> CreateAdminCommandHandler(Messaging.DiscordMessagingClient client)
        {
            var adminHandler = new Commands.AdminCommandsHandler(client.Client, new Discord.Commands.CommandService(), _telemetry, _serviceProvider);

            await adminHandler.InstallCommandsAsync();

            return adminHandler;
        }

        private IEnumerable<JobScheduleInfo> GetJobSchedules()
        {
            var jobTypes = this.GetType().Assembly.GetTypes()
                               .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IJob)));

            return jobTypes.Select(_serviceProvider.GetService)
                           .Where(o => o != null)
                           .OfType<IJob>()
                           .Select(j => new JobScheduleInfo(j, j.Frequency))
                           .ToList();
        }
    }
}
