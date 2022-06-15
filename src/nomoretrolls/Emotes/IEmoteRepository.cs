namespace nomoretrolls.Emotes
{
    internal interface IEmoteRepository
    {
        Task<IList<EmoteInfo>> GetEmotesAsync(string name);

        Task<IList<string>> GetEmoteNamesAsync();
    }
}
