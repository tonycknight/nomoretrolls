namespace nomoretrolls.Emotes
{
    internal interface IEmoteConfigProvider
    {
        Task<UserEmoteAnnotationEntry?> GetUserEmoteAnnotationEntryAsync(ulong userId);
        Task SetUserEmoteAnnotationEntryAsync(UserEmoteAnnotationEntry entry);
    }
}
