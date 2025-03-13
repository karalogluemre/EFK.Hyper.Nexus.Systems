using Commons.Domain.Models.Menus;

namespace Commons.Domain.Models.Organizations
{
    public class OrganizationMenu
    {
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public Guid MenuId { get; set; }
        public Menu Menu { get; set; }
    }
}
