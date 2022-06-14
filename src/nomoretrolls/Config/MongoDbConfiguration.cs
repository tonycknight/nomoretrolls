using Newtonsoft.Json;

namespace nomoretrolls.Config
{
    public class MongoDbConfiguration
    {
        [JsonProperty("connection")]
        public string? Connection { get; set; }

        [JsonProperty("databaseName")]
        public string? DatabaseName { get; set; } = "nomoretrolls";

        [JsonProperty("userStatsCollectionName")]
        public string UserStatsCollectionName { get; set; } = "userstats";

        [JsonProperty("userBlacklistCollectionName")]
        public string UserBlacklistCollectionName { get; set; } = "userblacklist";

        [JsonProperty("workflowConfigCollectionName")]
        public string WorkflowConfigCollectionName { get; set; } = "workflowconfigs";

        [JsonProperty("userKnockingScheduleCollectionName")]
        public string UserKnockingScheduleCollectionName { get; set; } = "userknockschedule";
    }
}
