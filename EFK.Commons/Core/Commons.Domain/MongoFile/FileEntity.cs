using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Commons.Domain.MongoFile
{
    public class FileEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }
}
