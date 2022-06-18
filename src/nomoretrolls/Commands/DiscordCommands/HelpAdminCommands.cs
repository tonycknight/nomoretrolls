using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Discord.Commands;
using nomoretrolls.Formatting;
using nomoretrolls.Telemetry;
using Tk.Extensions;

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
        [Description("Show help.")]
        public Task ShowHelpAsync()
        {
            try
            {
                var msg = this.GetType().GetDiscordCommandTypes()
                                .GetCommandHelpInfos()
                                .FormatCommandHelp()
                                .Join(Environment.NewLine);

                return ReplyAsync(msg);
            }
            catch (Exception ex)
            {
                _telemetry.Event(new TelemetryErrorEvent() { Exception = ex });
                return ReplyAsync(ex.Message);
            }
        }
              

        [Command("servers", RunMode = RunMode.Async)]
        [Description("List all servers that the bot watches.")]
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
        [Description("About this bot.")]
        public Task ShowAboutAsync()
        {
            try
            {
                var line = ProgramBootstrap.GetVersionDescription().Join(Environment.NewLine).ToCode().ToBold();

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
                _telemetry.Event(new TelemetryErrorEvent() { Exception = ex } );
                return Task.CompletedTask;
            }
        }
    }
}