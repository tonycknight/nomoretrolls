using nomoretrolls.Statistics;
using Tk.Extensions;

namespace nomoretrolls.Workflows.Parts
{
    internal class BumpUserWarnings : IMessageWorkflowPart
    {
        private readonly IUserStatisticsProvider _statsProvider;
        private readonly string _statsName;

        public BumpUserWarnings(IUserStatisticsProvider statsProvider, string statsName)
        {
            _statsProvider = statsProvider.ArgNotNull(nameof(statsProvider));
            _statsName = statsName;
        }

        public string StatsName => _statsName;

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var userId = context.AuthorId();
            
            await _statsProvider.BumpUserStatisticAsync(userId, _statsName);

            return context;
        }
    }
}
