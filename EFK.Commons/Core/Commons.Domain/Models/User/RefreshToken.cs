namespace Commons.Domain.Models.User
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public Guid AppUserId { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public AppUser AppUser { get; set; }
    }
}
