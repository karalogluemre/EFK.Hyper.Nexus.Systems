namespace Commons.Domain.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        virtual public DateTime? UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
