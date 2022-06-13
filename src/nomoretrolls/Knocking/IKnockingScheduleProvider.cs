namespace nomoretrolls.Knocking
{
    internal interface IKnockingScheduleProvider
    {        
        Task<IList<KnockingScheduleEntry>> GetUserEntriesAsync();

        Task SetUserEntryAsync(KnockingScheduleEntry entry);

        Task DeleteUserEntryAsync(ulong userId);
    }
}
