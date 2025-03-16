using Commons.Domain.Models.Branches;
using Commons.Domain.Models.Groups;

namespace Commons.Domain.Models.Organizations
{
    public class Organization : BaseEntity
    {
        public Guid BranchId { get; set; }
        public Branch Branch { get; set; }

        public ICollection<OrganizationMenu> OrganizationMenus { get; set; } = new List<OrganizationMenu>();
        public ICollection<Group> Groups { get; set; } = new List<Group>();

    }
}
