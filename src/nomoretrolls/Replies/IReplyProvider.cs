namespace nomoretrolls.Replies
{
    internal interface IReplyProvider
    {
        Task SetUserEntryAsync(UserReplyEntry entry);
        Task DeleteUserEntryAsync(ulong userId);
        Task<IList<UserReplyEntry>> GetUserEntriesAsync();
    }
}
