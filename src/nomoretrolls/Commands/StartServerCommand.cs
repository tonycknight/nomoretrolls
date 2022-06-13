using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using nomoretrolls.Config;
using nomoretrolls.Scheduling;
using nomoretrolls.Telemetry;
using nomoretrolls.Workflows;
using Tk.Extensions;
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
                                  Config.IWorkflowConfigurationRepository workflowConfig,
                                  IServiceProvider serviceProvider, IJobScheduler jobScheduler)
        {
            _configProvider = configProvider.ArgNotNull(nameof(configProvider));
            _telemetry = telemetry.ArgNotNull(nameof(telemetry));            
            _workflowExecutor = workflowExecutor.ArgNotNull(nameof(workflowExecutor));
            _serviceProvider = serviceProvider;
            _jobScheduler = jobScheduler;
            var wfProvider = new MessageWorkflowProvider(wfFactory);

            _clientMessageWorkflows = new[] { wfProvider.CreateBlacklistedUserWorkflow(),
                                              wfProvider.CreateShoutingWorkflow(),
                                              wfProvider.CreateShoutingPersonalReplyWorkflow() };
        }

        [Option(CommandOptionType.SingleValue, Description = "The configuration file's path.", LongName = "config", ShortName = "c")]
        public string ConfigurationFile { get; set; }

        public async Task<int> OnExecuteAsync()
        {
            var config = GetConfig();

            var attrs = typeof(ProgramBootstrap).Assembly.GetCustomAttributes();
            _telemetry.Message($"{attrs.GetAttributeValue<AssemblyProductAttribute>(a => a.Product)} {attrs.GetAttributeValue<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion).Format("Version {0}")}");

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

            await CreateAdminCommandHandler(client);

            await client.StartAsync();

            _telemetry.Message("Starting job scheduler...");
            var jobs = GetJobSchedules().ToList();
            _telemetry.Message($"Found {jobs.Count} scheduled job(s).");
            _jobScheduler.Register(jobs);
            _jobScheduler.Start();
            _telemetry.Message("Finished job scheduler.");

            _telemetry.Message("Startup complete.");
            _telemetry.Message($"Bot registration URI: {client.BotRegistrationUri}");
            _telemetry.Message("Proxy started. Hit CTRL-C to quit");

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += async (object? sender, ConsoleCancelEventArgs e) =>
            {
                _telemetry.Message("Shutting down job scheduler...");
                _jobScheduler.Stop();
                _telemetry.Message("Job scheduler shutdown.");

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
