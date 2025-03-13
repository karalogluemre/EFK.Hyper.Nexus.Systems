using Commons.Domain.Models.Branches;

namespace Commons.Domain.Models.Organizations
{
    public class Organization : BaseEntity
    {
        public Guid BranchId { get; set; }
        public Branch Branch { get; set; }

        public ICollection<OrganizationMenu> OrganizationMenus { get; set; } = new List<OrganizationMenu>();

    }
}
