namespace nomoretrolls.Replies
{
    internal class UserReplyEntry
    {
        public ulong UserId { get; init; }
        public string Message { get; init; }
        public DateTime Start { get; init; }
        public DateTime Expiry { get; init; }
    }
}
