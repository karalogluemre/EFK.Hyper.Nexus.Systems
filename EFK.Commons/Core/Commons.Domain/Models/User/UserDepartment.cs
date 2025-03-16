using Commons.Domain.Models.Departments;

namespace Commons.Domain.Models.User
{
    public class UserDepartment
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; }

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
