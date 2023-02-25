namespace nomoretrolls.Workflows.Reactions
{
    internal class BlacklistReplyTextGenerator : ReplyTextGenerator, IBlacklistReplyTextGenerator
    {
        private readonly string[] _formats;

        public BlacklistReplyTextGenerator() : this(PickRandom())
        {
        }

        public BlacklistReplyTextGenerator(Func<int, int> picker) : base(picker)
        {
            _formats = new[]
            {
                "{0} So?",
                "{0} Wat",
                "Thanks for your contribution {0}, we'll get back to you.",
                "{0} Thank you, but no-one cares.",
                @"{0} Thank you, filed under ""don't care"".",
                "Someone somewhere will understand you {0} but no-one else will.",
                "Why are you still here {0}?"
            };
        }


        public string GenerateReply(string userMention)
        {
            var format = PickRandomFormat(_formats);

            return String.Format(format, userMention);
        }
    }
}
