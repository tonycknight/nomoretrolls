using nomoretrolls.Config;
using Tk.Extensions.Linq;

namespace nomoretrolls.Telemetry
{
    internal class AggregatedTelemetry : ITelemetry
    {
        private readonly IList<ITelemetry> _telemetries;
        private readonly IConfigurationProvider _configProvider;
        
        private readonly Lazy<HashSet<string>> _telemetryTypesLogged;

        public AggregatedTelemetry(IList<ITelemetry> telemetries, IConfigurationProvider configProvider)
        {
            _telemetries = telemetries;
            _configProvider = configProvider;

            _telemetryTypesLogged = new Lazy<HashSet<string>>(GetTelemetryTypeConfig);
        }

        public void Event(TelemetryEvent evt)
        {
            if (IsLogged(evt))
            {
                foreach (var t in _telemetries)
                {
                    t.Event(evt);
                }
            }
        }

        private HashSet<string> GetTelemetryTypeConfig()
        {
            var telemetryConfig = _configProvider.GetAppConfiguration().Telemetry;
            var telemetryTypesLogged = (telemetryConfig?.LogTelemetryTypes).NullToEmpty()
                                        .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            return telemetryTypesLogged;
        }

        private bool IsLogged(TelemetryEvent evt) => IsTelemetryTypeLogged(evt);

        private bool IsTelemetryTypeLogged(TelemetryEvent evt)
        {
            if (_telemetryTypesLogged.Value.Count == 0) return true;

            var t = evt.GetType().Name;
            return _telemetryTypesLogged.Value.Contains(t);
        }
    }
}
