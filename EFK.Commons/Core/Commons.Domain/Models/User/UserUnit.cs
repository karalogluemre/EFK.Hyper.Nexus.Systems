using Commons.Domain.Models.Units;

namespace Commons.Domain.Models.User
{
    public class UserUnit
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; }

        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }
    }
}
