namespace nomoretrolls.Emotes
{
    internal interface IEmoteConfigProvider
    {
        Task<IList<UserEmoteAnnotationEntry>> GetUserEmoteAnnotationEntriesAsync();
        Task<UserEmoteAnnotationEntry?> GetUserEmoteAnnotationEntryAsync(ulong userId);
        Task SetUserEmoteAnnotationEntryAsync(UserEmoteAnnotationEntry entry);
        Task DeleteUserEmoteAnnotationEntryAsync(ulong userId);
    }
}
