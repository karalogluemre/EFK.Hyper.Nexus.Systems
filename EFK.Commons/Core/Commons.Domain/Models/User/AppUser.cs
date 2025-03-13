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
        public Guid RoleId { get; set; }
        public AppRole Role { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
