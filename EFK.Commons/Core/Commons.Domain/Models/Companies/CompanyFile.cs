namespace Commons.Domain.Models.Companies
{
    public class CompanyFile
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string FileId { get; set; } // Mongo ObjectId
        public string FileType { get; set; } // örn: "Logo", "Contract"
        public Company Company { get; set; }

    }
}
