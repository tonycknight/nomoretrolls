using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Cronos;
using Discord.Commands;
using nomoretrolls.Blacklists;
using nomoretrolls.Emotes;
using nomoretrolls.Formatting;
using nomoretrolls.Knocking;
using nomoretrolls.Telemetry;
using Tk.Extensions;

namespace nomoretrolls.Commands.DiscordCommands
{
    [ExcludeFromCodeCoverage] // Excluded until Discord.Net provides complete interfaces
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    internal class UserAdminCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ITelemetry _telemetry;
        private readonly IBlacklistProvider _blacklistProvider;
        private readonly IKnockingScheduleRepository _knockingProvider;
        private readonly IEmoteConfigProvider _emoteConfig;
        private const string DateTimeFormat = "dd MMM yyyy HH:mm:ss UTC";

        public UserAdminCommands(ITelemetry telemetry, IBlacklistProvider blacklistProvider, IKnockingScheduleRepository knockingProvider,
            IEmoteConfigProvider emoteConfig)
        {
            _telemetry = telemetry;
            _blacklistProvider = blacklistProvider;
            _knockingProvider = knockingProvider;
            _emoteConfig = emoteConfig;
        }

        [Command("deleteblacklist", RunMode = RunMode.Async)]        
        [Description("Delete a user's blacklisting.")]
        [CommandForm("<user name>",
                     example: @"""JoeUser#1234""",
                     guidelines: "Use double quotes for the user name as Discord may get confused.")]
        public async Task DeleteUserBlacklistAsync([Remainder][Summary("The user name")] string userName)
        {
            try
            {
                userName = userName.Trim('"');
                var user = await Context.GetUserAsync(userName);
                if (user == null)
                {
                    await SendMessageAsync("The user was not found on any attached servers.".ToCode());
                }
                else
                {
                    await _blacklistProvider.DeleteUserEntryAsync(user.Id);
                    await SendMessageAsync($"Done. {userName.ToCode()} has been cleared.");
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message.ToCode());
            }
        }


        [Command("blacklist", RunMode = RunMode.Async)]
        [Alias("bl")]
        [Description("Blacklist a user.")]
        [CommandForm("<user name> <duration in minutes>",
            example: @"""JoeUser#1234"" 60",
            exampleExplanation: "to blacklist Joe for 60 minutes",
            guidelines: "Use double quotes for the user name as Discord may get confused.")]
        public async Task SetUserBlacklistAsync([Summary("The user name")] string userName, int duration = 60)
        {
            try
            {
                userName = userName.Trim('"');
                var user = await Context.GetUserAsync(userName);
                if (user == null)
                {
                    await SendMessageAsync("The user was not found on any attached servers.".ToCode());
                }
                else
                {
                    var entry = user.CreateBlacklistEntry(DateTime.UtcNow, TimeSpan.FromMinutes(duration));
                    await _blacklistProvider.SetUserEntryAsync(entry);
                    await SendMessageAsync($"Done. {userName.ToCode()} has been blacklisted until {entry.Expiry.ToString(DateTimeFormat).ToCode()}");
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message.ToCode());
            }
        }

        [Command("deleteemote", RunMode = RunMode.Async)]
        [Description("Delete a user's emotes.")]
        [CommandForm("<user name>",
                     example: @"""JoeUser#1234""",
                     guidelines: "Use double quotes for the user name as Discord may get confused.")]
        public async Task DeleteUserEmoteAnnotationAsync([Remainder][Summary("The user name")] string userName)
        {
            try
            {
                userName = userName.Trim('"');
                var user = await Context.GetUserAsync(userName);
                if (user == null)
                {
                    await SendMessageAsync("The user was not found on any attached servers.".ToCode());
                }
                else
                {
                    await _emoteConfig.DeleteUserEmoteAnnotationEntryAsync(user.Id);                    
                    await SendMessageAsync($"Done. {userName.ToCode()} has been cleared.");
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message.ToCode());
            }
        }

        [Command("emote", RunMode = RunMode.Async)]
        [Alias("e")]
        [Description("Annotate a user's messages with emotes.")]
        [CommandForm("<user name> <duration in minutes> <emote list name>",
            example: @"""JoeUser#1234"" 60 shouting",
            exampleExplanation: @"to annotate Joe's messages for 60 minutes with ""shouting"" emotes",
            guidelines: "Use double quotes for the user name as Discord may get confused.")]
        public async Task SetUserEmoteAnnotationAsync([Summary("The user name")] string userName, int duration = 60, string emoteList = "farmyardanimals")
        {
            try
            {
                userName = userName.Trim('"');
                emoteList = emoteList.Trim('"');
                var user = await Context.GetUserAsync(userName);
                if (user == null)
                {
                    await SendMessageAsync("The user was not found on any attached servers.".ToCode());
                }
                else
                {
                    var entry = user.CreateUserEmoteAnnotationEntry(DateTime.UtcNow, TimeSpan.FromMinutes(duration), emoteList);
                    await _emoteConfig.SetUserEmoteAnnotationEntryAsync(entry);
                    await SendMessageAsync($"Done. {userName.ToCode()} will have emotes until {entry.Expiry.ToString(DateTimeFormat).ToCode()}");
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message.ToCode());
            }
        }


        [Command("knock", RunMode = RunMode.Async)]
        [Alias("k")]
        [Description("Set a schedule to knock a user.")]
        [CommandForm("<user name> <duration in minutes> <cron frequency for UTC>", 
                     guidelines: "Use double quotes for the user name & CRON expression as Discord may get confused.",
                     example: @"""JoeUser#1234"" 60 ""*/1 * * * 1""",
                     exampleExplanation: "to knock Joe every minute on Mondays for an hour")]
        public async Task SetKnockScheduleAsync([Summary("The user name")] string userName, int duration = 60, [Remainder]string frequency = "*/3 * * * *")
        {
            try
            {
                userName = userName.Trim('"');
                frequency = frequency.Trim('"');

                var user = await Context.GetUserAsync(userName);
                if (user == null)
                {
                    await SendMessageAsync("The user was not found on any attached servers.".ToCode());
                }
                else
                {                    
                    var entry = user.CreateScheduleEntry(DateTime.UtcNow, TimeSpan.FromMinutes(duration), frequency);

                    _knockingProvider.SetUserEntryAsync(entry);
                    await SendMessageAsync($"Done.{userName.ToCode()} has been set to knock {CronExpressionDescriptor.ExpressionDescriptor.GetDescription(entry.Frequency).ToCode()}.");
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message.ToCode());
            }
        }

        [Command("deleteknock", RunMode = RunMode.Async)]
        [Description("Delete a user's knock schedule.")]
        [CommandForm("<user name>",
            example: @"""JoeUser#1234""",
            guidelines: "Use double quotes for the user name as Discord may get confused.")]
        public async Task RemoveKnockScheduleAsync([Summary("The user name")] string userName)
        {
            try
            {
                userName = userName.Trim('"');
                var user = await Context.GetUserAsync(userName);
                if (user == null)
                {
                    await SendMessageAsync("The user was not found on any attached servers.".ToCode());
                }
                else
                {                    
                    _knockingProvider.DeleteUserEntryAsync(user.Id);
                    await SendMessageAsync($"Done. {userName.ToCode()} has been cleared.");
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(ex.Message.ToCode());
            }
        }

        [Command("users", RunMode = RunMode.Async)]
        [Description("Show all configured users.")]
        public async Task ListtUsersAsync()
        {
            try
            {
                var blacklistEntries = (await _blacklistProvider.GetUserEntriesAsync()).ToDictionary(e => e.UserId);
                var knockEntries = (await _knockingProvider.GetUserEntriesAsync()).ToDictionary(e => e.UserId);
                var emoteEntries = (await _emoteConfig.GetUserEmoteAnnotationEntriesAsync()).ToDictionary(e => e.UserId);

                var users1 = blacklistEntries.Keys
                    .Concat(knockEntries.Keys)
                    .Concat(emoteEntries.Keys)
                    .Distinct()
                    .Select(userId => Context.GetUserAsync(userId))
                    .ToArray();

                var users = (await Task.WhenAll(users1))
                            .Select(user =>
                                    {
                                        var userName = user != null ? $"{user.Username}#{user.Discriminator}" : user.ToString();
                                        return new { user = user, userName = userName };
                                    }).ToArray();

                var userEntries = users.Select(u =>
                {
                    var ble = blacklistEntries.GetValueOrDefault(u.user.Id);
                    var ke = knockEntries.GetValueOrDefault(u.user.Id);
                    var em = emoteEntries.GetValueOrDefault(u.user.Id);

                    return new { userName = u.userName, blacklist = ble, knock = ke, emote = em };
                });

                var lines = userEntries.Where(a => a.blacklist != null || a.knock != null) 
                                        .OrderBy(a => a.userName)                                        
                                        .SelectMany(a => new[] { a.userName.ToCode().ToBold(),
                                                                 a.blacklist != null ? $"Blacklisted. Expires {a.blacklist.Expiry.ToString(DateTimeFormat).ToCode()}"  : null,
                                                                 a.emote != null ? $"Emotes with {a.emote.EmoteListName.ToCode()}. Expires {a.emote.Expiry.ToString(DateTimeFormat).ToCode()}" : null,
                                                                 a.knock != null ? $"Knocking - with frequency {a.knock.Frequency.ToCode()} - {CronExpressionDescriptor.ExpressionDescriptor.GetDescription(a.knock.Frequency).ToCode()}. Expires {a.knock.Expiry.ToString(DateTimeFormat).ToCode()} " : null,
                                                               })
                                        .Where(l => l != null)
                                        .Join(Environment.NewLine);


                lines = lines.Length > 0 ? lines : "No configured users found.".ToCode();

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
