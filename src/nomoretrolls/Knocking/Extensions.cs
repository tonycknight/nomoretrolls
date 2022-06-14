using Cronos;
using Tk.Extensions.Guards;

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
            user.ArgNotNull(nameof(user));
            frequency.InvalidOpArg(string.IsNullOrWhiteSpace, $"{nameof(frequency)} is invalid.");
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
                duration = start.AddYears(1).Subtract(start);
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
