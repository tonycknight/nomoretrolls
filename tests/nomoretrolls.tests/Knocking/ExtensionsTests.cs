using System;
using Discord;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Knocking
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateScheduleEntry_Cron_EmptyCron_ExceptionThrown(string cron)
        {
            var f = () => nomoretrolls.Knocking.Extensions.CreateScheduleEntry(Substitute.For<IUser>(), DateTime.Now, TimeSpan.Zero, cron);

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }

        [Theory]
        [InlineData("a")]
        [InlineData("$")]
        public void CreateScheduleEntry_Cron_InvalidCron_ExceptionThrown(string cron)
        {
            var f = () => nomoretrolls.Knocking.Extensions.CreateScheduleEntry(Substitute.For<IUser>(), DateTime.Now, TimeSpan.Zero, cron);

            f.Should().Throw<ArgumentException>().WithMessage("?*");
        }


        [Theory]
        [InlineData("* * * * * ")]
        [InlineData("*/1 * * * *")]
        public void CreateScheduleEntry_Cron_ValidCron_NoExceptionThrown(string cron)
        {
            var r = nomoretrolls.Knocking.Extensions.CreateScheduleEntry(Substitute.For<IUser>(), DateTime.Now, TimeSpan.Zero, cron);

            r.Frequency.Should().Be(cron);
        }

        [Fact]
        public void CreateScheduleEntry_User_NullUser_ExceptionThrown()
        {
            var f = () => nomoretrolls.Knocking.Extensions.CreateScheduleEntry(null, DateTime.Now, TimeSpan.Zero, "* * * * *");

            f.Should().Throw<ArgumentNullException>();
        }

        [Property(Verbose = true)]
        public bool CreateScheduleEntry_User_ValidUser_UserIdMapped(ulong id)
        {
            var user = Substitute.For<IUser>();
            user.Id.Returns(id);
            var r = nomoretrolls.Knocking.Extensions.CreateScheduleEntry(user, DateTime.Now, TimeSpan.Zero, "* * * * *");

            return r.UserId == id;
        }

        [Property(Verbose = true)]
        public bool CreateScheduleEntry_Start_StartSet(DateTime start)
        {            
            var user = Substitute.For<IUser>();

            var r = nomoretrolls.Knocking.Extensions.CreateScheduleEntry(user, start, TimeSpan.Zero, "* * * * *");

            return r.Start == start;
        }

        [Property(Verbose = true)]
        public bool CreateScheduleEntry_Duration_NonZeroDuration_ExpirySet(DateTime start, PositiveInt durationDays)
        {
            var duration = TimeSpan.FromDays(durationDays.Get);
            var user = Substitute.For<IUser>();

            var r = nomoretrolls.Knocking.Extensions.CreateScheduleEntry(user, start, duration, "* * * * *");

            return r.Expiry == start.Add(duration);
        }

        [Property(Verbose = true)]
        public bool CreateScheduleEntry_Duration_ZeroDuration_ExpiryInOneYear(DateTime start)
        {
            var user = Substitute.For<IUser>();

            var r = nomoretrolls.Knocking.Extensions.CreateScheduleEntry(user, start, TimeSpan.Zero, "* * * * *");

            return r.Expiry == start.AddDays(365);
        }


    }
}
