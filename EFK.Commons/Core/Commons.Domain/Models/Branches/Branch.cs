using Commons.Domain.Models.Companies;

namespace Commons.Domain.Models.Branches
{
    public class Branch : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<BranchMenu> BranchMenus { get; set; } = new List<BranchMenu>();
    }
}
