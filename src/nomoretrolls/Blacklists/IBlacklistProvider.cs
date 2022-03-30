namespace nomoretrolls.Blacklists
{
    internal interface IBlacklistProvider
    {
        Task<UserBlacklistEntry?> GetUserEntryAsync(ulong userId);

        Task SetUserEntryAsync(UserBlacklistEntry entry);

        Task DeleteUserEntryAsync(ulong userId);

        Task<IList<UserBlacklistEntry>> GetUserEntriesAsync();
    }
}
