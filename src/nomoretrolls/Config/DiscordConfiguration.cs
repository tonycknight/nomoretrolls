using Newtonsoft.Json;

namespace nomoretrolls.Config
{
    public class DiscordConfiguration
    {
        [JsonProperty("clientToken")]
        public string? DiscordClientToken { get; set; }

        [JsonProperty("clientId")]
        public string? DiscordClientId { get; set; }        
    }
}
