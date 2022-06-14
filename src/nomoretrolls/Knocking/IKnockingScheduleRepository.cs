namespace nomoretrolls.Knocking
{
    internal interface IKnockingScheduleRepository
    {        
        Task<IList<KnockingScheduleEntry>> GetUserEntriesAsync();

        Task SetUserEntryAsync(KnockingScheduleEntry entry);

        Task DeleteUserEntryAsync(ulong userId);
    }
}
