using nomoretrolls.Statistics;
using Tk.Extensions.Guards;

namespace nomoretrolls.Workflows.Parts
{
    internal class BumpUserMessage : IMessageWorkflowPart
    {
        private IUserStatisticsProvider _statsProvider;
        private readonly string _statsName = "user_message";
        private readonly TimeSpan _statsExpiry = TimeSpan.FromDays(30 * 6);

        public BumpUserMessage(IUserStatisticsProvider statsProvider)
        {
            _statsProvider = statsProvider.ArgNotNull(nameof(statsProvider));
        }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var userId = context.AuthorId();

            await _statsProvider.BumpUserStatisticAsync(userId, _statsName, _statsExpiry);

            return context;
        }
    }
}
