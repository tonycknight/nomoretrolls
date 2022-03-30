using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace nomoretrolls.Statistics
{
    internal class UserStatisticsEntryDto
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("userId")]        
        public ulong UserId {  get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; init; }

        [BsonElement("count")]
        public long Count { get; set; }

        [BsonElement("expiryTime")]
        public DateTime ExpiryTime { get; init; }
    }
}
