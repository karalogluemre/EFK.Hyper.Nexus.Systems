using Commons.Domain.Models.Companies;

namespace Commons.Domain.Models.Packages
{
    public class Package : BaseEntity
    {
        public ICollection<Company> Companies { get; set; } = new List<Company>();
        public ICollection<PackageMenu> PackageMenus { get; set; } = new List<PackageMenu>();
    }
}
