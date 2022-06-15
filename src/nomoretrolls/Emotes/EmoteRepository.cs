using Tk.Extensions.Tasks;

namespace nomoretrolls.Emotes
{
    internal class EmoteRepository : IEmoteRepository
    {
        private readonly Dictionary<string, IList<EmoteInfo>> _emotes;

        public EmoteRepository()
        {
            _emotes = CreateEmotes();
        }

        public Task<IList<EmoteInfo>> GetEmotesAsync(string name)
        {
            IList<EmoteInfo> result = null;

            if(_emotes.TryGetValue(name, out var emotes))
            {
                result = emotes;
            }
            return result.ToTaskResult();
        }

        public Task<IList<string>> GetEmoteNamesAsync() 
            => _emotes.Keys.ToList().ToTaskResult<IList<string>>();

        private Dictionary<string, IList<EmoteInfo>> CreateEmotes() =>
            new Dictionary<string, IList<EmoteInfo>>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "blacklist", CreateBlacklistDefaults()  },
                { "shouting", CreateShoutingDefaults() },
                { "farmyardanimals", CreateFarmyardAnimalsDefaults() },
                { "gay", CreateGayDefaults() },
            };

        private IList<EmoteInfo> CreateBlacklistDefaults()
        {
            var emotes = new[] { "🍋", "👎", "🤐", "🧏‍♂️", "🧏‍♀️", "🍿", "🇸🇦", "🧇" };

            return ToEmotes(emotes.Select(s => new[] { s }));            
        }

        private IList<EmoteInfo> CreateShoutingDefaults()
        {
            var emotes = new[] { "🍋", "👎", "🤐", "🧏‍♂️", "🧏‍♀️", "🍿", "🧇" };

            return ToEmotes(emotes.Select(s => new[] { s }));
        }

        private IList<EmoteInfo> CreateGayDefaults()
        {
            var emotes = new[] { "🏳️‍🌈", "🏳️‍⚧️", "⚧", "👨‍❤️‍👨", "👨‍❤️‍💋‍👨" };

            return ToEmotes(emotes.Select(s => new[] { s }));
        }

        private IList<EmoteInfo> CreateFarmyardAnimalsDefaults()
        {
            var emotes = new[] { "🐐", "🐑", "🐮", "🐄", "🐷" };

            return ToEmotes(emotes.Select(s => new[] { s }));
        }

        private IList<EmoteInfo> ToEmotes(IEnumerable<string[]> emotes)
            => emotes.Select(e => new EmoteInfo(e)).ToArray();
    }
}
