using System;
using Discord;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using nomoretrolls.Emotes;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Emotes
{
    public class ExtensionsTests
    {

        [Fact]
        public void CreateUserEmoteAnnotationEntry_User_NullUser_ExceptionThrown()
        {
            IUser user = null;
            var f = () => user.CreateUserEmoteAnnotationEntry(DateTime.Now, TimeSpan.Zero, "emotes");

            f.Should().Throw<ArgumentNullException>();
        }

        [Property(Verbose = true)]
        public bool CreateUserEmoteAnnotationEntry_User_ValidUser_UserIdMapped(ulong id)
        {
            var user = Substitute.For<IUser>();
            user.Id.Returns(id);
            var r = user.CreateUserEmoteAnnotationEntry(DateTime.Now, TimeSpan.Zero, "emotes");

            return r.UserId == id;
        }

        [Property(Verbose = true)]
        public bool CreateUserEmoteAnnotationEntry_Start_StartSet(DateTime start)
        {
            var user = Substitute.For<IUser>();

            var r = user.CreateUserEmoteAnnotationEntry(start, TimeSpan.Zero, "emotes");

            return r.Start == start;
        }

        [Property(Verbose = true)]
        public bool CreateUserEmoteAnnotationEntry_Duration_NonZeroDuration_ExpirySet(DateTime start, PositiveInt durationDays)
        {
            var duration = TimeSpan.FromDays(durationDays.Get);
            var user = Substitute.For<IUser>();

            var r = user.CreateUserEmoteAnnotationEntry(start, duration, "emotes");

            return r.Expiry == start.Add(duration);
        }

        [Property(Verbose = true)]
        public bool CreateUserEmoteAnnotationEntry_Duration_ZeroDuration_ExpiryInOneYear(DateTime start)
        {
            var user = Substitute.For<IUser>();

            var r = user.CreateUserEmoteAnnotationEntry(start, TimeSpan.Zero, "emotes");

            return r.Expiry == start.AddDays(365);
        }

        [Property(Verbose = true)]
        public bool CreateUserEmoteAnnotationEntry_Emotes_NameMapped(NonEmptyString value)
        {
            var user = Substitute.For<IUser>();

            var r = user.CreateUserEmoteAnnotationEntry(DateTime.Now, TimeSpan.Zero, value.Get);

            return r.EmoteListName == value.Get;
        }
    }
}
