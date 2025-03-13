using Commons.Domain.Models.Menus;

namespace Commons.Domain.Models.Role
{
    public class RoleMenuPermission
    {
        public Guid RoleId { get; set; }
        public AppRole AppRole { get; set; }

        public Guid MenuId { get; set; }
        public Menu Menu { get; set; }

        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}
