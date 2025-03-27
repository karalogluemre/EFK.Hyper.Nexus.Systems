using Commons.Domain.Models.User;
using Microsoft.AspNetCore.Identity;

namespace Commons.Domain.Models.Role
{
    public class AppRole : IdentityRole<Guid>
    {
        public ICollection<RoleMenuPermission> RoleMenuPermissions { get; set; } = new List<RoleMenuPermission>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
