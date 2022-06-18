using nomoretrolls.Messaging;
using nomoretrolls.Scheduling;
using nomoretrolls.Telemetry;
using Tk.Extensions.Time;

namespace nomoretrolls.Knocking
{
    internal class KnockingScheduleJob : IJob
    {
        private readonly ITimeProvider _timeProvider;
        private readonly ITelemetry _telemetry;
        private readonly IDiscordMessagingClient _discordClient;
        private readonly IKnockingScheduleRepository _scheduleRepo;

        public KnockingScheduleJob(ITimeProvider timeProvider, ITelemetry telemetry, IDiscordMessagingClientProvider discordClientProvider, IKnockingScheduleRepository scheduleRepo)
        {
            _timeProvider = timeProvider;
            _telemetry = telemetry;
            _discordClient = discordClientProvider.GetClient();
            _scheduleRepo = scheduleRepo;
        }

        public TimeSpan Frequency => TimeSpan.FromMinutes(1);

        public async Task ExecuteAsync()
        {            
            var scheds = await _scheduleRepo.GetUserEntriesAsync();

            var tasks = scheds.Where(IsKnockDue)
                              .Select(KnockAsync)
                              .ToArray();

            if (tasks.Length > 0)
            {
                _telemetry.Event(new TelemetryInfoEvent() { Message = $"Found {tasks.Length} user(s) to knock." } );
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    _telemetry.Event(new TelemetryErrorEvent() { Exception = ex } );
                }
            }
        }

        private bool IsKnockDue(KnockingScheduleEntry entry)
        {
            var cron = Cronos.CronExpression.Parse(entry.Frequency);
            
            var now = _timeProvider.UtcNow();
            var start = now - this.Frequency;
                        
            var occursNext = cron.GetNextOccurrence(start, true);

            return (occursNext >= start && occursNext <= now);
        }

        private async Task KnockAsync(KnockingScheduleEntry entry)
        {            
            try
            {                
                var user = (await _discordClient.GetUsersAsync(new[] { entry.UserId })).FirstOrDefault();
                if (user != null)
                {
                    _telemetry.Event(new TelemetryTraceEvent() { Message = $"Knocking {user.Username}#{user.Discriminator}..." });
                    var channel = await user.CreateDMChannelAsync();
                    if (channel != null)
                    {
                        var msg = await channel.SendMessageAsync($"{user.Mention} Knock knock");

                        await msg.DeleteAsync();

                        _telemetry.Event(new TelemetryTraceEvent() { Message = $"Knocked {user.Username}#{user.Discriminator}." } );
                    }
                }
            }
            catch(Exception ex)
            {
                _telemetry.Event(new TelemetryErrorEvent() { Message = $"Error occured knocking user {entry.UserId}{Environment.NewLine}{ex.Message}", Exception = ex } );
            }
        }

    }
}
