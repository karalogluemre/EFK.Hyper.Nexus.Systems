using Commons.Domain.Models.Role;

namespace Commons.Domain.Models.User
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public AppUser AppUser { get; set; }

        public Guid RoleId { get; set; }
        public AppRole AppRole { get; set; }
    }
}
