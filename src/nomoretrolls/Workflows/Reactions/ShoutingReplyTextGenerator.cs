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
                "Heard you the first time {0}.",
                "The entire server heard you {0}.",
                "Calm down {0}, calm down", 
                "Have you gone deaf {0}?",
                "Yell if you like {0} but if you keep it up, we'll ban you."
            };
        }

        public string GenerateReply(string userMention)
        {
            var format = PickRandomFormat(_formats);

            return String.Format(format, userMention);
        }
    }
}
