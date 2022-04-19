using System.Diagnostics.CodeAnalysis;
using Discord.Commands;
using nomoretrolls.Blacklists;
using nomoretrolls.Formatting;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Commands.DiscordCommands
{
    [ExcludeFromCodeCoverage] // Excluded until Discord.Net provides complete interfaces
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    internal class UserAdminCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ITelemetry _telemetry;
        private readonly IBlacklistProvider _blacklistProvider;

        private const string DateTimeFormat = "dd MMM yyyy HH:mm:ss UTC";

        public UserAdminCommands(ITelemetry telemetry, IBlacklistProvider blacklistProvider)
        {
            _telemetry = telemetry;
            _blacklistProvider = blacklistProvider;
        }

        [Command("allow", RunMode = RunMode.Async)]
        public async Task AllowUserAsync([Remainder][Summary("The user name")] string userName)
        {
            try
            {
                var user = await Context.GetUserAsync(userName);
                if (user == null)
                {
                    await SendMessageAsync("The user was not found on any allowed servers.".ToCode());
                }
                else
                {
                    await _blacklistProvider.DeleteUserEntryAsync(user.Id);
                    await SendMessageAsync("Done.".ToCode());
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message.ToCode());
            }
        }


        [Command("blacklist", RunMode = RunMode.Async)]
        public async Task BlacklistUserAsync([Summary("The user name")] string userName, int duration = 60)
        {
            try
            {
                userName = userName.Trim('"');
                var user = await Context.GetUserAsync(userName);
                if (user == null)
                {
                    await SendMessageAsync("The user was not found on any allowed servers.".ToCode());
                }
                else
                {
                    var entry = user.CreateBlacklistEntry(DateTime.UtcNow, duration);
                    await _blacklistProvider.SetUserEntryAsync(entry);
                    await SendMessageAsync($"Done. {userName.ToCode()} has been blacklisted until {entry.Expiry.ToString(DateTimeFormat).ToBold()}");
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message.ToCode());
            }
        }

        [Command("users", RunMode = RunMode.Async)]
        public async Task ListtUsersAsync()
        {
            try
            {
                var entries = await _blacklistProvider.GetUserEntriesAsync();

                var userEntries = entries.Select(async e =>
                {
                    var user = await Context.GetUserAsync(e.UserId);
                    var userName = user != null ? $"{user.Username}#{user.Discriminator}" : e.UserId.ToString();

                    return new { userName, entry = e };
                }).ToArray();

                var lines = (await Task.WhenAll(userEntries))
                                        .OrderBy(a => a.userName)
                                        .SelectMany(a => new[] { a.userName.ToCode().ToBold(),
                                                                 $"Blacklisted - expires {a.entry.Expiry.ToString(DateTimeFormat).ToBold()}" })
                                        .Join(Environment.NewLine);


                lines = lines.Length > 0 ? lines : "No blacklisted users found.".ToCode();

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
