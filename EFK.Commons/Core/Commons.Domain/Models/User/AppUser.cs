using Commons.Domain.Models.Role;
using Microsoft.AspNetCore.Identity;

namespace Commons.Domain.Models.User
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityType { get; set; }
        public string IdentityNumber { get; set; }
        public string PlaceOfBirth { get; set; } //Doğum Yeri
        public string BloodGroup { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserMenuPermission> UserMenuPermissions { get; set; } = new List<UserMenuPermission>();
        public ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
        public ICollection<UserUnit> UserUnits { get; set; } = new List<UserUnit>();


    }
}
