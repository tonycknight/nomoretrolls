namespace nomoretrolls.Workflows.Reactions
{
    internal interface ICallingEveryoneReplyTextGenerator : ITextGenerator
    {

    }

    internal class CallingEveryoneReplyTextGenerator : ReplyTextGenerator, ICallingEveryoneReplyTextGenerator
    {
        private readonly string[] _formats;

        public CallingEveryoneReplyTextGenerator() : this(PickRandom())
        {
        }

        public CallingEveryoneReplyTextGenerator(Func<int, int> picker) : base(picker)
        {
            _formats = new[]
            {
                "{0} you rang?",
                "{0} no-one's home",
                "{0} no-one cares",
                "{0} I'm sure someone wants to get this, they're just not on this server.",
                "{0} 🍋",
                "{0} 🍋 🍋 🍋"
            };
        }

        public string GenerateReply(string userMention)
        {
            var format = PickRandomFormat(_formats);

            return String.Format(format, userMention);
        }
    }
}
