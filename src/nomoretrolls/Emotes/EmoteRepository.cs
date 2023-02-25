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

            if (_emotes.TryGetValue(name, out var emotes))
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
                { "blacklist", new[] { "🍋", "👎", "🤐", "🧏‍♂️", "🧏‍♀️", "🍿", "🇸🇦", "🧇" }.ToEmotes() },
                { "shouting", new[] { "🍋", "👎", "🤐", "🧏‍♂️", "🧏‍♀️", "🧇" }.ToEmotes() },
                { "farmyardanimals", new[] { "🐐", "🐑", "🐮", "🐄", "🐷" }.ToEmotes() },
                { "gay", new[] { "🏳️‍🌈", "🏳️‍⚧️", "⚧", "👨‍❤️‍👨", "👨‍❤️‍💋‍👨" }.ToEmotes() },
                { "religion", new[] { "✝️", "☪️", "✡️", "🕉️", "☸️", "☦️", "🕎", "🔯" }.ToEmotes() },
                { "snooze", new[] { "💤", "😴" }.ToEmotes() },
                { "shrug", new[] { "🤷", "🤷‍♀️" }.ToEmotes() },
                { "altcaps", new[] { "🍋", "🤷", "🤷‍♀️" }.ToEmotes() }
            };
    }
}
