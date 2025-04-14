using Commons.Domain.Models;

namespace Commons.Domain.MongoFile
{
    public class FileMetaData : BaseEntity
    {
        public string ObjectId { get; set; } // Aynı zamanda GridFS id
        public Guid ReferenceId { get; set; } // entity.Id
        public string ReferenceType { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedAt { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; } // örn: "logo", "contract", "tax-document"
    }
}
