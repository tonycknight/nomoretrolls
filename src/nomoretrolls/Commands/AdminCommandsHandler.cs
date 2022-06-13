using System.Diagnostics.CodeAnalysis;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using nomoretrolls.Blacklists;
using nomoretrolls.Commands.DiscordCommands;
using nomoretrolls.Config;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Commands
{
    [ExcludeFromCodeCoverage] // Excluded until Discord.Net provides complete interfaces
    internal class AdminCommandsHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly ITelemetry _telemetry;
        private readonly IServiceProvider _serviceProvider;

        public AdminCommandsHandler(DiscordSocketClient client, CommandService commandService, 
                                    ITelemetry telemetry,
                                    IServiceProvider serviceProvider)
        {
            _client = client;
            _commandService = commandService;
            _telemetry = telemetry;
            _serviceProvider = serviceProvider;
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

            if (!(message.HasCharPrefix(HelpExtensions.CommandPrefix, ref argPos) ||
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
                .AddSingleton(_telemetry)
                .AddSingleton(_serviceProvider.GetService(typeof(IBlacklistProvider)) as IBlacklistProvider)                
                .AddSingleton(_serviceProvider.GetService(typeof(Knocking.IKnockingScheduleRepository)) as Knocking.IKnockingScheduleRepository)
                .AddSingleton(_serviceProvider.GetService(typeof(IWorkflowConfigurationRepository)) as IWorkflowConfigurationRepository)
                .BuildServiceProvider();
    }
}
