using nomoretrolls.Statistics;
using Tk.Extensions.Guards;

namespace nomoretrolls.Workflows.Parts
{
    internal class BumpUserWarnings : IMessageWorkflowPart
    {
        private readonly IUserStatisticsProvider _statsProvider;
        private readonly string _statsName;
        private readonly TimeSpan _statsExpiry = TimeSpan.FromDays(2);

        public BumpUserWarnings(IUserStatisticsProvider statsProvider, string statsName)
        {
            _statsProvider = statsProvider.ArgNotNull(nameof(statsProvider));
            _statsName = statsName;
        }

        public string StatsName => _statsName;

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var userId = context.AuthorId();

            await _statsProvider.BumpUserStatisticAsync(userId, _statsName, _statsExpiry);

            return context;
        }
    }
}
