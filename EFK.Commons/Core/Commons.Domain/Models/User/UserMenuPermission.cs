using Commons.Domain.Models.Menus;

namespace Commons.Domain.Models.User
{
    public class UserMenuPermission
    {
        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public Guid MenuId { get; set; }
        public Menu Menu { get; set; }

        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}
