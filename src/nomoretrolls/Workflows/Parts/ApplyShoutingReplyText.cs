﻿using nomoretrolls.Workflows.Reactions;

namespace nomoretrolls.Workflows.Parts
{
    internal class ApplyShoutingReplyText : IMessageWorkflowPart
    {
        private readonly IShoutingReplyTextGenerator _replyTextGenerator;

        public ApplyShoutingReplyText(IShoutingReplyTextGenerator replyTextGenerator)
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
