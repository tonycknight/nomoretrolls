using System.Diagnostics.CodeAnalysis;
using Discord.Commands;
using nomoretrolls.Blacklists;
using nomoretrolls.Formatting;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Commands
{
    [ExcludeFromCodeCoverage] // Excluded until Discord.Net provides complete interfaces
    [Group("user")]
    internal class UserAdminCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ITelemetry _telemetry;
        private readonly IBlacklistProvider _blacklistProvider;

        public UserAdminCommands(ITelemetry telemetry, IBlacklistProvider blacklistProvider)
        {
            _telemetry = telemetry;
            _blacklistProvider = blacklistProvider;
        }
        
        [Command("allow")]
        public async Task AllowUserAsync([Remainder][Summary("The user name")] string userName)
        {
            try
            {                
                var user = await Context.GetUserAsync(userName);
                if (user == null)
                {
                    await SendMessageAsync("User not found in this server.");
                }
                else
                {
                    await _blacklistProvider.DeleteUserEntryAsync(user.Id);
                    await SendMessageAsync("Done.");
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message);
            }
        }

        
        [Command("blacklist")]
        public async Task BlacklistUserAsync([Summary("The user name")] string userName, int duration = 5)
        {
            try
            {
                var user = await Context.GetUserAsync(userName);
                if (user == null)
                {
                    await SendMessageAsync("User not found in this server.");
                }
                else
                {
                    var entry = user.CreateBlacklistEntry(DateTime.UtcNow, duration);
                    await _blacklistProvider.SetUserEntryAsync(entry);
                    await SendMessageAsync("Done.");
                }
            }
            catch(Exception ex)
            {
                await SendMessageAsync(ex.Message);
            }
        }

        [Command("list")]
        public async Task ListtUsersAsync()
        {
            try
            {
                var entries = await _blacklistProvider.GetUserEntriesAsync();

                var userEntries = entries.Select(async e => 
                {
                    var user = await Context.GetUserAsync(e.UserId);
                    var userName = user != null ? $"{user.Username}#{user.Discriminator}" : e.UserId.ToString();

                    return new { userName = userName, entry = e };
                }).ToArray();

                var lines = (await Task.WhenAll(userEntries))
                                        .OrderBy(a => a.userName)
                                        .Select(a => $"{a.userName}: Blacklisted, expires {a.entry.Expiry}")
                                        .Join(Environment.NewLine);


                lines = lines.Length > 0 ? lines : "None found.";

                await SendMessageAsync(lines);
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message);
            }
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            var line = new[]
            {
                $"{"!user allow <user name>".ToCode()}",
                "Clears a user from the blacklist.",
                "The user name is the Discord username & discriminator, e.g. ``joebloggs#1234``",
                "",
                $"{"!user blacklist <user name> <duration>".ToCode()}",
                "Sets a user's blacklist for a given duration in minutes. Default is 5 minutes.",
                $"The user name is the Discord username & discriminator, e.g. {"joebloggs#1234".ToCode()}",
                "",
                $"{"!user list".ToCode()}",
                "Lists users' details."
            }.Join(Environment.NewLine);

            await SendMessageAsync(line);
        }

        private Task SendMessageAsync(string message)
        {
            try
            {
                return ReplyAsync(message.ToMaxLength().ToCode());
            }
            catch(Exception ex)
            {
                _telemetry.Message(ex.Message);
                return Task.CompletedTask;
            }
        }

    }
}
