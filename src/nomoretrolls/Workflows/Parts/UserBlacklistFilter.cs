using nomoretrolls.Blacklists;
using Tk.Extensions.Guards;

namespace nomoretrolls.Workflows.Parts
{
    internal class UserBlacklistFilter : IMessageWorkflowPart
    {
        private readonly IBlacklistProvider _blacklistProvider;
        
        public UserBlacklistFilter(IBlacklistProvider blacklistProvider)
        {
            _blacklistProvider = blacklistProvider.ArgNotNull(nameof(blacklistProvider));            
        }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {            
            if (await IsCharacterMatch(context))
            {
                return context;
            }

            return null;
        }

        private async Task<bool> IsCharacterMatch(MessageWorkflowContext context)
        {
            var userId = context.AuthorId();
            var result = await _blacklistProvider.GetUserEntryAsync(userId);

            return result != null;
        }
    }
}
