using Commons.Domain.Models.Branches;
using Commons.Domain.Models.Organizations;
using Commons.Domain.Models.Packages;
using Commons.Domain.Models.User;

namespace Commons.Domain.Models.Menus
{
    public class Menu : BaseEntity
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string? Icon { get; set; }
        public string? RouterLink { get; set; }
        public string? Url { get; set; }
        public string? Target { get; set; }

        public Guid? MenuId { get; set; }  
        public Menu ParentMenu { get; set; }  
        public List<Menu> Items { get; set; } = new List<Menu>();

        public ICollection<PackageMenu> PackageMenus { get; set; } = new List<PackageMenu>();
        public ICollection<BranchMenu> BranchMenus { get; set; } = new List<BranchMenu>();
        public ICollection<OrganizationMenu> OrganizationMenus { get; set; } = new List<OrganizationMenu>();
        public ICollection<UserMenuPermission> UserMenuPermissions { get; set; } = new List<UserMenuPermission>();
    }
}