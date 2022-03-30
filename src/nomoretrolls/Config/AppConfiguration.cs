using Newtonsoft.Json;

namespace nomoretrolls.Config
{
    public class AppConfiguration
    {
        [JsonProperty("discord")]
        public DiscordConfiguration? Discord { get; set; }

        [JsonProperty("discordAdmin")]
        public DiscordConfiguration? DiscordAdmin { get; set; }

        [JsonProperty("mongoDb")]
        public MongoDbConfiguration? MongoDb { get; set; }

        [JsonProperty("telemetry")]
        public TelemetryConfiguration? Telemetry { get; set; }
    }
}
