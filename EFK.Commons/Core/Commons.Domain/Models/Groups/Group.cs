using Commons.Domain.Models.Departments;
using Commons.Domain.Models.Organizations;
using Commons.Domain.Models.User;

namespace Commons.Domain.Models.Groups
{
    public class Group:BaseEntity
    {
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public ICollection<Department> Departments { get; set; } = new List<Department>(); 
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    }
}
