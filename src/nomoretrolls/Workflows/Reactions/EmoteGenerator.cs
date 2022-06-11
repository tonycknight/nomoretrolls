namespace nomoretrolls.Workflows.Reactions
{
    internal class EmoteGenerator : IEmoteGenerator
    {
        private readonly Func<int, int> _picker;
        private readonly string[] _disapproveEmotes = new[] { "🍋", "👎", "🤐", "🧏‍♂️", "🧏‍♀️", "🍿", "🇸🇦", ":waffle:" };

        public EmoteGenerator() : this(PickRandom())
        {
        }

        public EmoteGenerator(Func<int, int> picker)
        {
            _picker = picker;
        }

        public string PickDisapproveEmotes() => _disapproveEmotes[_picker(_disapproveEmotes.Length)];

        private static Func<int, int> PickRandom()
        {
            var rng = new Random();
            return x => rng.Next(0, x);
        }
    }
}
