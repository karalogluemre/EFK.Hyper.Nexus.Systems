using Commons.Domain.Models;
using MongoDB.Bson.Serialization.Attributes;
using System.Reflection.Metadata.Ecma335;

namespace Commons.Domain.MongoFile
{
    public class FileMetaData : BaseEntity
    {
        [BsonId]
        public string ObjectId { get; set; }
        public Guid ReferenceId { get; set; }
  
        public string? FileName { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        public string Path { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
        public string? Guid { get; set; }
        public byte[]? FileData { get; set; }

        public string? ReferenceType { get; set; }
        public string? ContentType { get; set; }
        public DateTime? UploadedAt{ get; set; }
        public string? Tag{ get; set; }
        public string? Description{ get; set; }
    }
}
