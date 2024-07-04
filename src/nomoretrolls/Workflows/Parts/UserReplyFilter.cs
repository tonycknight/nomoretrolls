using nomoretrolls.Replies;

namespace nomoretrolls.Workflows.Parts
{
    internal class UserReplyFilter : IMessageWorkflowPart
    {
        private readonly IReplyProvider _replyProvider;

        public UserReplyFilter(IReplyProvider replyProvider)
        {
            _replyProvider = replyProvider;
        }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var (isMatch, reply) = await IsCharacterMatch(context);
            if (isMatch)
            {
                return context.ReplyText(reply);
            }
            return null;
        }

        private async Task<(bool, string)> IsCharacterMatch(MessageWorkflowContext context)
        {
            var userId = context.AuthorId();

            var result = await _replyProvider.GetUserEntriesAsync(userId);

            if (result == null) return (false, null);

            return (true, result.Message);
        }
    }
}
