namespace nomoretrolls.Emotes
{
    internal interface IEmoteGenerator
    {
        Task<string> PickEmoteAsync(string name);
    }
}
