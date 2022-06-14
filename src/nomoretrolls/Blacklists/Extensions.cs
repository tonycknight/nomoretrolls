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

        public static UserBlacklistEntry CreateBlacklistEntry(this Discord.IUser user, DateTime now, int duration)
        {
            duration = duration > 0 ? duration : (int)TimeSpan.FromDays(365).TotalMinutes;

            return new UserBlacklistEntry()
            {
                UserId = user.Id,
                Start = now,
                Expiry = now.AddMinutes(duration),
            };
        }

    }
}
