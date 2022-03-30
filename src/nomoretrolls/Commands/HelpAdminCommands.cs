using System.Diagnostics.CodeAnalysis;
using Discord.Commands;
using nomoretrolls.Formatting;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Commands
{
    [ExcludeFromCodeCoverage] // Excluded until Discord.Net provides complete interfaces
    [Group("help")]
    internal class HelpAdminCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ITelemetry _telemetry;

        public HelpAdminCommands(ITelemetry telemetry)
        {
            _telemetry = telemetry;
        }

        [Command("")]
        public async Task ShowHelpAsync()
        {
            var line = new[]
            {
                $"{"!user help".ToCode()}",
                "Get help on user administration.",
                $"{"!workflow help".ToCode()}",
                "Get help on workflow administration.",

            }.Join(Environment.NewLine);

            await SendMessageAsync(line);
        }

        private Task SendMessageAsync(string message)
        {
            try
            {
                return ReplyAsync(message.ToMaxLength().ToCode());
            }
            catch (Exception ex)
            {
                _telemetry.Message(ex.Message);
                return Task.CompletedTask;
            }
        }
    }
}