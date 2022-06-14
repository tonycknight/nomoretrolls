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

        private Dictionary<string, IList<EmoteInfo>> CreateEmotes() =>
            new Dictionary<string, IList<EmoteInfo>>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "blacklist", CreateBlacklistDefaults()  },
            };

        private IList<EmoteInfo> CreateBlacklistDefaults()
        {
            var emotes = new[] { "🍋", "👎", "🤐", "🧏‍♂️", "🧏‍♀️", "🍿", "🇸🇦", "🧇" };

            return emotes.Select(e => new EmoteInfo(new[] { e })).ToArray();
        }

    }
}
