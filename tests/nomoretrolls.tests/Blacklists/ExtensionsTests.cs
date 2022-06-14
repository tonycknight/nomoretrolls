using System;
using FsCheck;
using FsCheck.Xunit;
using nomoretrolls.Blacklists;
using NSubstitute;

namespace nomoretrolls.tests.Blacklists
{
    public class ExtensionsTests
    {
        [Property(Verbose = true)]
        public bool CreateBlacklistEntry_UserId_Mapped(ulong id)
        {
            var user = Substitute.For<Discord.IUser>();
            user.Id.Returns(id);

            var now = DateTime.UtcNow;
            var result = user.CreateBlacklistEntry(now, TimeSpan.Zero);

            return result.UserId == id;
        }

        [Property(Verbose = true)]
        public bool CreateBlacklistEntry_StartTime_Mapped(DateTime start)
        {
            var user = Substitute.For<Discord.IUser>();
            
            var result = user.CreateBlacklistEntry(start, TimeSpan.Zero);

            return result.Start == start;
        }

        [Property(Verbose = true)]
        public bool CreateBlacklistEntry_Duration_Mapped(DateTime start, PositiveInt duration)
        {
            var user = Substitute.For<Discord.IUser>();
            
            var result = user.CreateBlacklistEntry(start, TimeSpan.FromMinutes(duration.Get));

            return result.Expiry == start.AddMinutes(duration.Get);
        }

        [Property(Verbose = true)]
        public Property CreateBlacklistEntry_Duration_NegativeInt_GivesDefault(DateTime start, int duration)
        {
            Func<bool> rule = () =>
            {
                var user = Substitute.For<Discord.IUser>();

                var result = user.CreateBlacklistEntry(start, TimeSpan.FromMinutes(duration));

                return result.Expiry == start.AddDays(365);
            };

            return rule.When(duration <= 0);
        }
    }
}
