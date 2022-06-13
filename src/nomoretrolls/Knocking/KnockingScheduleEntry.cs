namespace nomoretrolls.Knocking
{
    internal class KnockingScheduleEntry
    {
        public ulong UserId { get; init; }
        public DateTime Start { get; init; }
        public DateTime Expiry { get; init; }
        public string Frequency { get; set; }
    }
}
