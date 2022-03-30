namespace nomoretrolls.Workflows.Reactions
{
    internal abstract class ReplyTextGenerator
    {
        private readonly Func<int, int> _picker;

        public ReplyTextGenerator() : this(PickRandom())
        {
        }

        public ReplyTextGenerator(Func<int, int> picker)
        {
            _picker = picker;
        }

        protected string PickRandomFormat(string[] formats)
        {
            var idx = _picker(formats.Length);

            return formats[idx];
        }

        protected static Func<int, int> PickRandom()
        {
            var rng = new Random();
            return x => rng.Next(0, x);
        }

    }
}
