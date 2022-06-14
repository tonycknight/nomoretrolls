using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using nomoretrolls.Knocking;
using nomoretrolls.Messaging;
using nomoretrolls.Telemetry;
using NSubstitute;
using Tk.Extensions.Tasks;
using Tk.Extensions.Time;
using Xunit;

namespace nomoretrolls.tests.Knocking
{
    public class KnockingScheduleJobTests
    {
        [Fact]
        public void Ctor_AllOK()
        {
            var job = new KnockingScheduleJob(CreateMockTimeProvider(), CreateMockTelemetry(), 
                                              CreateMockDiscordClientProvider(), CerateMockScheduleRepo());
        }

        [Fact]
        public async Task ExecuteAsync_EmptySchedules_NoEffects()
        {
            var telemetry = CreateMockTelemetry();
            var scheduleRepo = CerateMockScheduleRepo();
            var job = new KnockingScheduleJob(CreateMockTimeProvider(), telemetry,
                                              CreateMockDiscordClientProvider(), scheduleRepo);

            await job.ExecuteAsync();

            scheduleRepo.Received(1).GetUserEntriesAsync();
            telemetry.Received(0);
        }

        [Fact]
        public async Task ExecuteAsync_SingleSchedule_NoMatchingUsers_NoEffects()
        {
            var client = CreateMockDiscordClient();
            client.GetUsersAsync(Arg.Any<IEnumerable<ulong>>()).Returns(new Discord.IUser[0]);
            var clientProvider = CreateMockDiscordClientProvider(client);
            
            var telemetry = CreateMockTelemetry();
            
            var time = CreateMockTimeProvider();
            var now = DateTime.UtcNow;
            time.UtcNow().Returns(now);

            var scheduleRepo = CerateMockScheduleRepo();
            var schedules = new[] { new KnockingScheduleEntry()
            {
                Start = now,
                Expiry = now.AddYears(1),
                Frequency = "*/1 * * * *"
            } };
            
            scheduleRepo.GetUserEntriesAsync().Returns(schedules);

            var job = new KnockingScheduleJob(time, telemetry,
                                              clientProvider, scheduleRepo);

            await job.ExecuteAsync();

            scheduleRepo.Received(1).GetUserEntriesAsync();

            telemetry.Received(1).Message(Arg.Is<string>(s => s.StartsWith("Found 1 user(s) to knock.")));
            telemetry.Received(1);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(7)]
        public async Task ExecuteAsync_MultipleSchedules_MatchingUser_Effects(int userCount)
        {

            var client = CreateMockDiscordClient();
            var users = Enumerable.Range(1, userCount).Select(x => CreateMockDiscordUser((ulong)x)).ToList() as IList<Discord.IUser>;
            client.GetUsersAsync(Arg.Any<IEnumerable<ulong>>()).Returns(users.ToTaskResult());
            var clientProvider = CreateMockDiscordClientProvider(client);

            var telemetry = CreateMockTelemetry();

            var time = CreateMockTimeProvider();
            var now = DateTime.UtcNow;
            time.UtcNow().Returns(now);

            var scheduleRepo = CerateMockScheduleRepo();
            var schedules = users.Select(u => new KnockingScheduleEntry()
            {
                UserId = u.Id,
                Start = now,
                Expiry = now.AddYears(1),
                Frequency = "*/1 * * * *"
            }).ToList();

            scheduleRepo.GetUserEntriesAsync().Returns(schedules);

            var job = new KnockingScheduleJob(time, telemetry,
                                              clientProvider, scheduleRepo);

            await job.ExecuteAsync();

            scheduleRepo.Received(1).GetUserEntriesAsync();

            telemetry.Received(1).Message(Arg.Is<string>(s => s.StartsWith($"Found {userCount} user(s) to knock.")));
            telemetry.Received(userCount).Message(Arg.Is<string>(s => s.StartsWith("Knocking ")));
            telemetry.Received(userCount).Message(Arg.Is<string>(s => s.StartsWith("Knocked ")));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(7)]
        public async Task ExecuteAsync_MultipleSchedules_NoMatchingTime_NoEffects(int userCount)
        {

            var client = CreateMockDiscordClient();
            var users = Enumerable.Range(1, userCount).Select(x => CreateMockDiscordUser((ulong)x)).ToList() as IList<Discord.IUser>;
            client.GetUsersAsync(Arg.Any<IEnumerable<ulong>>()).Returns(users.ToTaskResult());
            var clientProvider = CreateMockDiscordClientProvider(client);

            var telemetry = CreateMockTelemetry();

            var time = CreateMockTimeProvider();
            var now = new DateTime(2022, 12, 1, 1, 0, 0, DateTimeKind.Utc);
            time.UtcNow().Returns(now);

            var scheduleRepo = CerateMockScheduleRepo();
            var schedules = users.Select(u => new KnockingScheduleEntry()
            {
                UserId = u.Id,
                Start = now,
                Expiry = now.AddYears(1),
                Frequency = $"*/1 * {now.Day+1} * *"
            }).ToList();

            scheduleRepo.GetUserEntriesAsync().Returns(schedules);

            var job = new KnockingScheduleJob(time, telemetry,
                                              clientProvider, scheduleRepo);

            await job.ExecuteAsync();

            scheduleRepo.Received(1).GetUserEntriesAsync();

            telemetry.Received(0).Message(Arg.Is<string>(s => s.StartsWith($"Found {userCount} user(s) to knock.")));
            telemetry.Received(0).Message(Arg.Is<string>(s => s.StartsWith("Knocking ")));
            telemetry.Received(0).Message(Arg.Is<string>(s => s.StartsWith("Knocked ")));
        }


        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(7)]
        public async Task ExecuteAsync_MultipleSchedules_MatchingUser_MessagesSent(int userCount)
        {

            var client = CreateMockDiscordClient();
            var userMessage = CreateMockUserMessage();
            var dmChannel = CreateMockDmChannel(userMessage);
            
            var users = Enumerable.Range(1, userCount).Select(x => CreateMockDiscordUser((ulong)x, dmChannel)).ToList() as IList<Discord.IUser>;
            client.GetUsersAsync(Arg.Any<IEnumerable<ulong>>()).Returns(users.ToTaskResult());
            var clientProvider = CreateMockDiscordClientProvider(client);

            var telemetry = CreateMockTelemetry();

            var time = CreateMockTimeProvider();
            var now = DateTime.UtcNow;
            time.UtcNow().Returns(now);

            var scheduleRepo = CerateMockScheduleRepo();
            var schedules = users.Select(u => new KnockingScheduleEntry()
            {
                UserId = u.Id,
                Start = now,
                Expiry = now.AddYears(1),
                Frequency = "*/1 * * * *"
            }).ToList();

            scheduleRepo.GetUserEntriesAsync().Returns(schedules);

            var job = new KnockingScheduleJob(time, telemetry,
                                              clientProvider, scheduleRepo);

            await job.ExecuteAsync();

            dmChannel.Received(userCount).SendMessageAsync(Arg.Is<string>(s => s.Contains("Knock knock")));
            userMessage.Received(userCount).DeleteAsync();
        }


        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(7)]
        public async Task ExecuteAsync_MultipleSchedules_MatchingUser_ExceptionThrown(int userCount)
        {

            var client = CreateMockDiscordClient();            
            var users = Enumerable.Range(1, userCount).Select(x => CreateMockDiscordUser((ulong)x)).ToList() as IList<Discord.IUser>;
            client.GetUsersAsync(Arg.Any<IEnumerable<ulong>>()).Returns(Task.FromException<IList<Discord.IUser>>(new Exception()));
            var clientProvider = CreateMockDiscordClientProvider(client);

            var telemetry = CreateMockTelemetry();

            var time = CreateMockTimeProvider();
            var now = DateTime.UtcNow;
            time.UtcNow().Returns(now);

            var scheduleRepo = CerateMockScheduleRepo();
            var schedules = users.Select(u => new KnockingScheduleEntry()
            {
                UserId = u.Id,
                Start = now,
                Expiry = now.AddYears(1),
                Frequency = "*/1 * * * *"
            }).ToList();

            scheduleRepo.GetUserEntriesAsync().Returns(schedules);

            var job = new KnockingScheduleJob(time, telemetry,
                                              clientProvider, scheduleRepo);

            await job.ExecuteAsync();

            telemetry.Received(userCount).Error(Arg.Is<string>(s => s.StartsWith("Error occured knocking user")));
        }



        private ITelemetry CreateMockTelemetry() => Substitute.For<ITelemetry>();
        private ITimeProvider CreateMockTimeProvider() => Substitute.For<ITimeProvider>();
        private IDiscordMessagingClientProvider CreateMockDiscordClientProvider(IDiscordMessagingClient client = null)
        {
            client ??= CreateMockDiscordClient();
            var result = Substitute.For<IDiscordMessagingClientProvider>();
            result.GetClient().Returns(client);
            return result;
        }

        private IDiscordMessagingClient CreateMockDiscordClient() => Substitute.For<IDiscordMessagingClient>();
        private IKnockingScheduleRepository CerateMockScheduleRepo() => Substitute.For<IKnockingScheduleRepository>();
        private Discord.IUser CreateMockDiscordUser(ulong id, Discord.IDMChannel dmChannel = null)
        {
            var result = Substitute.For<Discord.IUser>();
            result.Id.Returns(id);

            dmChannel ??= CreateMockDmChannel();
            result.CreateDMChannelAsync().Returns(dmChannel);

            return result;
        }

        private Discord.IDMChannel CreateMockDmChannel(Discord.IUserMessage userMessage = null)
        {
            var result = Substitute.For<Discord.IDMChannel>();

            userMessage ??= CreateMockUserMessage();
            result.SendMessageAsync(Arg.Any<string>()).Returns(userMessage);

            return result;
        }
        
        private Discord.IUserMessage CreateMockUserMessage()
        {
            var result = Substitute.For<Discord.IUserMessage>();

            return result;
        }
    }
}
