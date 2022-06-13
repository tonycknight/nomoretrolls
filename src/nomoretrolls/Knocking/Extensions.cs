using Cronos;

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

        public static KnockingScheduleEntry CreateScheduleEntry(this Discord.IUser user, DateTime start, TimeSpan duration, string frequency)
        {
            try
            {
                var cronFreq = CronExpression.Parse(frequency, CronFormat.Standard);
            }
            catch (CronFormatException)
            {
                throw new ArgumentException("Invalid Cron expression.");
            }

            if (duration <= TimeSpan.Zero)
            {
                duration = DateTime.UtcNow.AddYears(1).Subtract(DateTime.UtcNow);
            }

            return new KnockingScheduleEntry()
            {
                UserId = user.Id,
                Start = start,
                Expiry = start.Add(duration),
                Frequency = frequency
            };
        }
    }
}
