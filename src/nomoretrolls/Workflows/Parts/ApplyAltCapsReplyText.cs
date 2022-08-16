using nomoretrolls.Workflows.Reactions;

namespace nomoretrolls.Workflows.Parts
{
    internal class ApplyAltCapsReplyText : IMessageWorkflowPart
    {
        private readonly IAltCapsReplyTextGenerator _replyTextGenerator;

        public ApplyAltCapsReplyText(IAltCapsReplyTextGenerator replyTextGenerator)
        {
            _replyTextGenerator = replyTextGenerator;
        }

        public Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var result = context.DeepClone()
                                .ReplyText(_replyTextGenerator.GenerateReply(context.UserMention()));
            return Task.FromResult(result);
        }
    }
}
