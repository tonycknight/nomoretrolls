namespace nomoretrolls.Workflows.Reactions
{
    internal class AltCapsReplyTextGenerator : ReplyTextGenerator, IAltCapsReplyTextGenerator
    {
        private readonly string[] _formats;

        public AltCapsReplyTextGenerator() : this(PickRandom())
        {
        }

        public AltCapsReplyTextGenerator(Func<int, int> picker) : base(picker)
        {
            _formats = new[]
            {
                "{0} Alternating caps is for muppets. Please stop.",
                "{0} OnLy MuPpEtS uSe AlTeRnAtInG cApS. Please stop.",
                "{0} Alternating caps is for stupid people. Please stop."
            };
        }

        public string GenerateReply(string userMention)
        {
            var format = PickRandomFormat(_formats);

            return String.Format(format, userMention);
        }
    }
}
