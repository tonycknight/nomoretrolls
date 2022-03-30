namespace nomoretrolls.Blacklists
{
    internal class UserBlacklistEntry
    {
        public ulong UserId { get; init; }
        public DateTime Start { get;init; }
        public DateTime Expiry { get; init; }
    }
}
