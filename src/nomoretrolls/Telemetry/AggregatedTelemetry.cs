using nomoretrolls.Config;

namespace nomoretrolls.Telemetry
{
    internal class AggregatedTelemetry : ITelemetry
    {
        private readonly IList<ITelemetry> _telemetries;
        private readonly TelemetryConfiguration? _telemetryConfig;

        public AggregatedTelemetry(IList<ITelemetry> telemetries, IConfigurationProvider configProvider)
        {
            _telemetries = telemetries;            
            _telemetryConfig = configProvider.GetAppConfiguration().Telemetry;
        }

        public void Event(TelemetryEvent evt)
        {
            foreach (var t in _telemetries)
            {
                t.Event(evt);
            }
        }
    }
}
