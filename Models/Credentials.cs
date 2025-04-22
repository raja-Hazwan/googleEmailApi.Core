using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleEmailApi.Core.Models
{
    public class Credential
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("key")]
        public string Key { get; set; }

        [BsonElement("clientId")]
        public string ClientId { get; set; }

        [BsonElement("clientSecret")]
        public string ClientSecret { get; set; }

        [BsonElement("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
