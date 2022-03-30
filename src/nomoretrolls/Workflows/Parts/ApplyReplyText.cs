using nomoretrolls.Workflows.Reactions;

namespace nomoretrolls.Workflows.Parts
{
    internal class ApplyReplyText : IMessageWorkflowPart
    {
        private readonly ITextGenerator _textGenerator;

        public ApplyReplyText(ITextGenerator textGenerator)
        {
            _textGenerator = textGenerator;
        }

        public Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var result = context.DeepClone()
                                .ReplyText(_textGenerator.GenerateReply(context.UserMention()));
            return Task.FromResult(result);
        }
    }
}
