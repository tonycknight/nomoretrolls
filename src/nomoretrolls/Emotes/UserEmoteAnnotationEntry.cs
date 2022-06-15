namespace nomoretrolls.Emotes
{
    internal class UserEmoteAnnotationEntry
    {
        public ulong UserId { get; init; }
        public DateTime Start { get; init; }
        public DateTime Expiry { get; init; }
        public string EmoteListName { get; init; }
    }
}
