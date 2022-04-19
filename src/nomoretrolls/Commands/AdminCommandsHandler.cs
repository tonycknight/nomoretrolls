using System.Diagnostics.CodeAnalysis;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using nomoretrolls.Blacklists;
using nomoretrolls.Commands.DiscordCommands;
using nomoretrolls.Config;
using nomoretrolls.Statistics;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Commands
{
    [ExcludeFromCodeCoverage] // Excluded until Discord.Net provides complete interfaces
    internal class AdminCommandsHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IBlacklistProvider _blacklistProvider;
        private readonly ITelemetry _telemetry;
        private readonly IUserStatisticsProvider _statsProvider;
        private readonly IWorkflowConfigurationRepository _workflowRepo;

        public AdminCommandsHandler(DiscordSocketClient client, CommandService commandService, 
                                    Blacklists.IBlacklistProvider blacklistProvider, ITelemetry telemetry,
                                    Statistics.IUserStatisticsProvider statsProvider,
                                    Config.IWorkflowConfigurationRepository workflowRepo)
        {
            _client = client;
            _commandService = commandService;
            _blacklistProvider = blacklistProvider;
            _telemetry = telemetry;
            _statsProvider = statsProvider;
            _workflowRepo = workflowRepo;
        }

        public async Task InstallCommandsAsync()
        {            
            _client.MessageReceived += HandleCommandAsync;
            var sp = CreateServiceProvider();

            await _commandService.AddModuleAsync(type: typeof(HelpAdminCommands), services: sp);
            await _commandService.AddModuleAsync(type: typeof(UserAdminCommands), services: sp);
            await _commandService.AddModuleAsync(type: typeof(WorkflowAdminCommands), services: sp);
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null) { return; }

            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) ||
                    message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                    message.Author.IsBot)
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);
            var sp = CreateServiceProvider();

            await _commandService.ExecuteAsync(context: context, argPos: argPos, services: sp);
        }

        private IServiceProvider CreateServiceProvider() => new ServiceCollection()
                .AddSingleton<Blacklists.IBlacklistProvider>(_blacklistProvider)
                .AddSingleton<Statistics.IUserStatisticsProvider>(_statsProvider)
                .AddSingleton<Telemetry.ITelemetry>(_telemetry)
                .AddSingleton<Config.IWorkflowConfigurationRepository>(_workflowRepo)
                .BuildServiceProvider();
    }
}
