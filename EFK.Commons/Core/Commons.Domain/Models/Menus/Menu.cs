using Commons.Domain.Models.Packages;

namespace Commons.Domain.Models.Menus
{
    public class Menu : BaseEntity
    {
        public ICollection<PackageMenu> PackageMenus { get; set; } = new List<PackageMenu>();

    }
}
