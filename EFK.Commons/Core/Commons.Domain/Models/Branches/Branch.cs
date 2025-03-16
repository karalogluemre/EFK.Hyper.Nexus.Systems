using Commons.Domain.Models.Companies;
using Commons.Domain.Models.Organizations;

namespace Commons.Domain.Models.Branches
{
    public class Branch : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<BranchMenu> BranchMenus { get; set; } = new List<BranchMenu>();
        public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
    }
}
