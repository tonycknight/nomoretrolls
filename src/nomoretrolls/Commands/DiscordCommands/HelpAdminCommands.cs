using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Discord.Commands;
using nomoretrolls.Formatting;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Commands.DiscordCommands
{
    [ExcludeFromCodeCoverage] // Excluded until Discord.Net provides complete interfaces    
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    internal class HelpAdminCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ITelemetry _telemetry;

        public HelpAdminCommands(ITelemetry telemetry)
        {
            _telemetry = telemetry;
        }

        [Command("help", RunMode = RunMode.Async)]
        public async Task ShowHelpAsync()
        {
            var line = new[]
            {
                $"{"!allow <user name>".ToCode()}",
                "Clears a user from the blacklist.",
                "The user name is the Discord username & discriminator, e.g. ``joebloggs#1234``",
                "",
                $"{"!blacklist <user name> <duration>".ToCode()}",
                "Sets a user's blacklist for a given duration in minutes. The default is 60 minutes.",
                $"The user name is the Discord username & discriminator, e.g. {"joebloggs#1234".ToCode()}",
                $"If the user has spaces in it, use double quotes e.g. {"\"joe bloggs#1234\"".ToCode()}",
                "",
                $"{"!users".ToCode()}",
                "Lists all configured users' details.",
                "",
                $"{"!enable <feature name>".ToCode()}",
                "Enables a feature.",
                "",
                $"{"!disable <feature name>".ToCode()}",
                "Disables a feature.",
                "",
                $"{"!features".ToCode()}",
                "Lists all features.",
                "",
                $"{"!servers".ToCode()}",
                "Show the servers & channels the bot is watching."

            }.Join(Environment.NewLine);

            await SendMessageAsync(line);
        }


        [Command("servers", RunMode = RunMode.Async)]
        public async Task ShowServersAsync()
        {
            try
            {
                var gs = Context.Client.Guilds;
                var line = "No servers or channels found.";

                var guildChannels = gs.Select(g => new
                {
                    Guild = g.Name,
                    Channels = g.Channels.Where(c => c.Users.Any(u => u.Id == Context.Client.CurrentUser.Id))
                                         .Select(c => ("#" + c.Name).ToCode())
                                         .OrderBy(n => n)
                                         .ToList(),
                }).ToList();

                if (guildChannels.Count > 0)
                {
                    var header = new[] { "Servers the bot is watching:", "" };
                    var lines = guildChannels.Select(gc => $"{gc.Guild.ToBold()}: {gc.Channels.Join(" ")}{Environment.NewLine}");
                    line = header.Concat(lines).Join(Environment.NewLine);
                }

                await SendMessageAsync(line);
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message);
            }
        }

        [Command("about", RunMode = RunMode.Async)]
        public Task ShowAboutAsync()
        {
            try
            {
                var line = ProgramBootstrap.GetDescription();

                return SendMessageAsync(line);                
            }
            catch(Exception ex)
            {
                return SendMessageAsync(ex.Message);
            }
        }

        private Task SendMessageAsync(string message)
        {
            try
            {
                return ReplyAsync(message.ToMaxLength());
            }
            catch (Exception ex)
            {
                _telemetry.Message(ex.Message);
                return Task.CompletedTask;
            }
        }
    }
}