using nomoretrolls.Messaging;
using nomoretrolls.Scheduling;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Knocking
{
    internal class KnockingScheduleJob : IJob
    {
        private readonly ITelemetry _telemetry;
        private readonly IDiscordMessagingClient _discordClient;
        private readonly IKnockingScheduleRepository _scheduleRepo;

        public KnockingScheduleJob(ITelemetry telemetry, IDiscordMessagingClientProvider discordClientProvider, IKnockingScheduleRepository scheduleRepo)
        {
            _telemetry = telemetry;
            _discordClient = discordClientProvider.GetClient();
            _scheduleRepo = scheduleRepo;
        }

        public TimeSpan Frequency => TimeSpan.FromMinutes(1);

        public async Task ExecuteAsync()
        {            
            var scheds = await _scheduleRepo.GetUserEntriesAsync();

            var tasks = scheds.Select(KnockAsync).ToArray();
            if (tasks.Length > 0)
            {
                _telemetry.Message($"Knocking {tasks.Length} user(s).");
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    _telemetry.Error(ex.Message);
                }
            }
        }

        private async Task KnockAsync(KnockingScheduleEntry entry)
        {
            try
            {
                var user = (await _discordClient.GetUsersAsync(new[] { entry.UserId })).FirstOrDefault();
                if (user != null)
                {
                    _telemetry.Message($"Knocking {user.Username}#{user.Discriminator}...");
                    var channel = await user.CreateDMChannelAsync();
                    if (channel != null)
                    {
                        var msgText = $"{user.Mention} Knock knock";
                        var msg = await channel.SendMessageAsync(msgText);

                        await msg.DeleteAsync();

                        _telemetry.Message($"Knocked {user.Username}#{user.Discriminator}.");
                    }
                }
            }
            catch(Exception ex)
            {
                _telemetry.Error($"Error occured knocking user {entry.UserId}{Environment.NewLine}{ex.Message}");
            }
        }

    }
}
