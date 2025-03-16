using Commons.Domain.Models.Departments;
using Commons.Domain.Models.Organizations;

namespace Commons.Domain.Models.Groups
{
    public class Group:BaseEntity
    {
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public ICollection<Department> Departments { get; set; } = new List<Department>(); 

    }
}
