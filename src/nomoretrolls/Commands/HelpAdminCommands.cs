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
                $"{"!allow <user name>".ToCode()}",
                "Clears a user from the blacklist.",
                "The user name is the Discord username & discriminator, e.g. ``joebloggs#1234``",
                "",
                $"{"!blacklist <user name> <duration>".ToCode()}",
                "Sets a user's blacklist for a given duration in minutes. The default is 5 minutes.",
                $"The user name is the Discord username & discriminator, e.g. {"joebloggs#1234".ToCode()}",
                $"If the user has spaces in it, use double quotes e.g. {"\"joe bloggs#1234\"".ToCode()}",
                "",
                $"{"!users".ToCode()}",
                "Lists all configured users' details.",
                "",
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