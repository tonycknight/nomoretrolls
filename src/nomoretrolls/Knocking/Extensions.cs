namespace nomoretrolls.Knocking
{
    internal static class Extensions
    {
        public static KnockingScheduleEntry FromDto(this KnockingScheduleEntryDto value) => new KnockingScheduleEntry()
        {
            UserId = value.UserId,
            Start = value.Start,
            Expiry = value.Expiry,
            Frequency = value.Frequency,
        };

        public static KnockingScheduleEntryDto ToDto(this KnockingScheduleEntry value) => new KnockingScheduleEntryDto()
        {
            UserId = value.UserId,
            Start = value.Start,
            Expiry = value.Expiry,
            Frequency = value.Frequency,
        };

        public static KnockingScheduleEntry CreateScheduleEntry(this Discord.IUser user, DateTime now, TimeSpan duration, TimeSpan frequency)
            => new KnockingScheduleEntry()
            {
                UserId = user.Id,
                Start = now,
                Expiry = now.Add(duration),
                Frequency = frequency
            };
    }
}
