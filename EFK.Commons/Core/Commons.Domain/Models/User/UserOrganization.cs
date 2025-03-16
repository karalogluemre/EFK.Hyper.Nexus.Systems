using Commons.Domain.Models.Organizations;

namespace Commons.Domain.Models.User
{
    public class UserOrganization
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
