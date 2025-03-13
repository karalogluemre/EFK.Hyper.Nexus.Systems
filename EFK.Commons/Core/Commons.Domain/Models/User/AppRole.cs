using Microsoft.AspNetCore.Identity;

namespace Commons.Domain.Models.User
{
    public class AppRole : IdentityRole<Guid>
    {
        public ICollection<AppUser> Users { get; set; }
    }
}
