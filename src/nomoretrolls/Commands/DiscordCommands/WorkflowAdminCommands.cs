using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Discord.Commands;
using nomoretrolls.Config;
using nomoretrolls.Formatting;
using nomoretrolls.Telemetry;
using Tk.Extensions;

namespace nomoretrolls.Commands.DiscordCommands
{
    [ExcludeFromCodeCoverage] // Excluded until Discord.Net provides complete interfaces
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    internal class WorkflowAdminCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ITelemetry _telemetry;
        private readonly IWorkflowConfigurationRepository _workflowConfig;

        public WorkflowAdminCommands(ITelemetry telemetry, IWorkflowConfigurationRepository workflowConfig)
        {
            _telemetry = telemetry;
            _workflowConfig = workflowConfig;
        }

        [Command("enable", RunMode = RunMode.Async)]
        [Description("Enable a bot feature.")]
        [CommandForm("<feature name>")]
        public async Task EnableWorkflowAsync([Remainder][Summary("The workflow's name")] string workflowName)
        {
            try
            {
                var config = await _workflowConfig.GetWorkflowConfigAsync(workflowName);

                config.Enabled = true;

                await _workflowConfig.SetWorkflowConfigAsync(config);

                await SendMessageAsync($"Done.");
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message);
            }
        }

        [Command("disable", RunMode = RunMode.Async)]
        [Description("Disable a bot feature.")]
        [CommandForm("<feature name>")]
        public async Task DisableWorkflowAsync([Remainder][Summary("The workflow's name")] string workflowName)
        {
            try
            {
                var config = await _workflowConfig.GetWorkflowConfigAsync(workflowName);

                config.Enabled = false;

                await _workflowConfig.SetWorkflowConfigAsync(config);

                await SendMessageAsync($"Done.");
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message);
            }
        }

        [Command("features", RunMode = RunMode.Async)]
        [Description("List all bot features and their status.")]
        public async Task ListWorkflowsAsync()
        {
            try
            {

                var configs = await _workflowConfig.GetWorkflowConfigsAsync();

                var lines = configs
                    .OrderBy(c => c.Name)
                    .Select(c => $"Feature '{c.Name}': enabled = {c.Enabled}")
                    .Join(Environment.NewLine);

                lines = lines.Length > 0 ? lines : "None found.";

                await SendMessageAsync(lines);
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message);
            }
        }
        
        private Task SendMessageAsync(string message)
        {
            try
            {
                return ReplyAsync(message.ToMaxLength().ToCode());
            }
            catch (Exception ex)
            {
                _telemetry.Event(new TelemetryErrorEvent() { Message = ex.Message } );
                return Task.CompletedTask;
            }
        }
    }
}
