using Tk.Extensions.Guards;

namespace nomoretrolls.Emotes
{
    internal class EmoteGenerator : IEmoteGenerator
    {
        private readonly Func<int, int> _picker;
        private readonly IEmoteRepository _emoteRepo;
        
        public EmoteGenerator(IEmoteRepository emoteRepo) : this(PickRandom(), emoteRepo)
        {
        }

        public EmoteGenerator(Func<int, int> picker, IEmoteRepository emoteRepo)
        {
            _picker = picker.ArgNotNull(nameof(picker));
            _emoteRepo = emoteRepo.ArgNotNull(nameof(emoteRepo));
        }

        public async Task<string> PickEmoteAsync(string name)
        {
            var emotes = await _emoteRepo.GetEmotesAsync(name);
            if (emotes == null) return null;

            return emotes[_picker(emotes.Count)].Emotes.FirstOrDefault();
        }

        private static Func<int, int> PickRandom()
        {
            var rng = new Random();
            return x => rng.Next(0, x);
        }
    }
}
