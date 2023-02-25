namespace nomoretrolls.Workflows.Reactions
{
    internal class ShoutingReplyTextGenerator : ReplyTextGenerator, IShoutingReplyTextGenerator
    {
        private readonly string[] _formats;

        public ShoutingReplyTextGenerator() : this(PickRandom())
        {
        }

        public ShoutingReplyTextGenerator(Func<int, int> picker) : base(picker)
        {
            _formats = new[]
            {
                "{0} Stop shouting please.",
                "{0} Please - STOP SHOUTING",
                "Heard you the first time {0}.",
                "Yell if you like but if you keep it up, we'll ban you {0}."
            };
        }

        public string GenerateReply(string userMention)
        {
            var format = PickRandomFormat(_formats);

            return String.Format(format, userMention);
        }
    }
}
