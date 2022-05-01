using nomoretrolls.Statistics;
using Tk.Extensions;

namespace nomoretrolls.Workflows.Parts
{
    internal class UserWarningsFilter : IMessageWorkflowPart
    {
        private readonly IUserStatisticsProvider _statsProvider;
        private readonly string _statsName;
        private readonly PeriodRange _period;
        
        public UserWarningsFilter(Statistics.IUserStatisticsProvider statsProvider, string statsName, PeriodRange period)
        {
            _statsProvider = statsProvider.ArgNotNull(nameof(statsProvider));
            _statsName = statsName;
            _period = period;
        }

        public string StatsName => _statsName;

        public PeriodRange Period => _period;

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {            
            var userId = context.AuthorId();

            var count = await _statsProvider.GetUserStatisticCountAsync(userId, _statsName, _period.Duration);

            var pass = _period switch
            {
                AtMost atMost => count <= atMost.Count,
                AtLeast atLeast => count >= atLeast.Count,
                _ => false,
            };

            return pass ? context : null;
        }
    }
}
