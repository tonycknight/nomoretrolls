namespace nomoretrolls.Blacklists
{
    internal static class Extensions
    {
        public static UserBlacklistEntryDto ToDto(this UserBlacklistEntry value) => new UserBlacklistEntryDto()
        {
            UserId = value.UserId,
            Start = value.Start,
            Expiry = value.Expiry,
        };

        public static UserBlacklistEntry FromDto(this UserBlacklistEntryDto value) => new UserBlacklistEntry()
        {
            UserId = value.UserId,
            Start = value.Start,
            Expiry = value.Expiry,
        };

        public static UserBlacklistEntry CreateBlacklistEntry(this Discord.IUser user, DateTime now, TimeSpan duration)
        {
            duration = duration > TimeSpan.Zero ? duration : TimeSpan.FromDays(365);

            return new UserBlacklistEntry()
            {
                UserId = user.Id,
                Start = now,
                Expiry = now.Add(duration),
            };
        }

    }
}
