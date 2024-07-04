using nomoretrolls.Workflows.Reactions;

namespace nomoretrolls.Workflows.Parts
{
    internal class ApplyCallingEveryoneReplyText : IMessageWorkflowPart
    {
        private readonly ICallingEveryoneReplyTextGenerator _replyTextGenerator;

        public ApplyCallingEveryoneReplyText(ICallingEveryoneReplyTextGenerator replyTextGenerator)
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
