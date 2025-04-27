using Commons.Domain.Models.Menus;

namespace Commons.Domain.Models.Branches
{
    public class BranchMenu : BaseEntity
    {
        public Guid BranchId { get; set; }
        public Branch Branch { get; set; }

        public Guid MenuId { get; set; }
        public Menu Menu { get; set; }
    }
}
