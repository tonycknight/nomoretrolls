using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace nomoretrolls.Config
{
    internal class WorkflowConfiguration
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

    }

    internal class WorkflowConfigurationDto
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("enabled")]
        public bool Enabled { get; set; }

    }
}
