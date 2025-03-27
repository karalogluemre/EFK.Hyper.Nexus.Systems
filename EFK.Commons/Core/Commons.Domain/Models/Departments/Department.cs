using Commons.Domain.Models.Groups;
using Commons.Domain.Models.Units;
using Commons.Domain.Models.User;

namespace Commons.Domain.Models.Departments
{
    public class Department : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Group Group { get; set; }

        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();

    }
}
