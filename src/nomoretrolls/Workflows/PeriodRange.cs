namespace nomoretrolls.Workflows
{
    internal abstract class PeriodRange
    {
        protected PeriodRange(int count, TimeSpan duration)
        {
            Count = count;
            Duration = duration;
        }

        public int Count { get; set; }
        public TimeSpan Duration { get; set; }

        public static AtMost AtMost(int count, TimeSpan duration) => new AtMost(count, duration);
        public static AtLeast AtLeast(int count, TimeSpan duration) => new AtLeast(count, duration);
    }

    internal class AtMost : PeriodRange
    {
        public AtMost(int count, TimeSpan duration) : base(count, duration)
        {
        }
    }

    internal class AtLeast : PeriodRange
    {
        public AtLeast(int count, TimeSpan duration) : base(count, duration)
        {
        }
    }

}
