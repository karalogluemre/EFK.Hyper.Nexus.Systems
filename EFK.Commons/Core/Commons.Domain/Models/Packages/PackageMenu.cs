using Commons.Domain.Models.Menus;

namespace Commons.Domain.Models.Packages
{
    public class PackageMenu: BaseEntity
    {
        public Guid PackageId { get; set; }
        public Package Package { get; set; }

        public Guid MenuId { get; set; }
        public Menu Menu { get; set; }
    }
}
