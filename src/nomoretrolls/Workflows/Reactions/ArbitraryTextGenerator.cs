namespace nomoretrolls.Workflows.Reactions
{
    internal class ArbitraryTextGenerator : ITextGenerator
    {
        private readonly string _format;

        public ArbitraryTextGenerator(string format)
        {
            _format = format;
        }

        public string GenerateReply(string userMention)
        {
            return String.Format(_format, userMention);
        }
    }
}
