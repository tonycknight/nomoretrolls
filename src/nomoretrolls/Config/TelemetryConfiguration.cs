using Newtonsoft.Json;

namespace nomoretrolls.Config
{
    public class TelemetryConfiguration
    {
        [JsonProperty("logMessageContent")]
        public bool LogMessageContent { get; set; } = false;

        [JsonProperty("LogTelemetryTypes")]
        public IList<string> LogTelemetryTypes { get; set; }
    }
}
