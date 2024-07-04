using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace nomoretrolls.Replies
{
    internal class UserReplyEntryDto
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; }
        [BsonElement("userId")]
        public ulong UserId { get; init; }
        [BsonElement("message")]
        public string Message { get; init; }
        [BsonElement("start")]
        public DateTime Start { get; init; }
        [BsonElement("expiry")]
        public DateTime Expiry { get; init; }
    }
}
