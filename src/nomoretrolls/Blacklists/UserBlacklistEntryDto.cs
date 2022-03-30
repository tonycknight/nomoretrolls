using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace nomoretrolls.Blacklists
{
    internal class UserBlacklistEntryDto
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("userId")]
        public ulong UserId { get; init; }

        [BsonElement("start")]
        public DateTime Start { get; init; }

        [BsonElement("expiry")]
        public DateTime Expiry { get; init; }
    }
}
